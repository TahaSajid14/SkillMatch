const scoreBands = [
  { minimum: 80, label: 'Strong match', tone: 'strong', summary: 'Your resume covers most of this role’s required skills.' },
  { minimum: 60, label: 'Promising match', tone: 'promising', summary: 'You meet many requirements, with a few clear skill gaps.' },
  { minimum: 40, label: 'Partial match', tone: 'partial', summary: 'There is some alignment, but several required skills are missing.' },
  { minimum: 0, label: 'Early match', tone: 'early', summary: 'This role currently has limited overlap with your detected skills.' },
]

function getScoreBand(score) {
  return scoreBands.find((band) => score >= band.minimum) ?? scoreBands.at(-1)
}

function buildCategoryCoverage(result) {
  const categories = new Map()

  for (const skill of result.matchedSkills) {
    const current = categories.get(skill.category) ?? { name: skill.category, matched: 0, total: 0 }
    current.matched += 1
    current.total += 1
    categories.set(skill.category, current)
  }

  for (const skill of result.missingSkills) {
    const current = categories.get(skill.category) ?? { name: skill.category, matched: 0, total: 0 }
    current.total += 1
    categories.set(skill.category, current)
  }

  return [...categories.values()].sort((left, right) => {
    const coverageDifference = right.matched / right.total - left.matched / left.total
    return coverageDifference || left.name.localeCompare(right.name)
  })
}

function SkillCollection({ title, count, skills, type }) {
  const matched = type === 'matched'

  return (
    <section className={`result-skill-group skill-status-${type}`}>
      <div className="result-section-title">
        <span className="result-section-icon" aria-hidden="true">{matched ? '✓' : '!'}</span>
        <div><h4>{title}</h4><p>{matched ? 'Found in your resume' : 'Not detected in your resume'}</p></div>
        <strong>{count}</strong>
      </div>
      <div className="result-skill-list">
        {skills.length > 0 ? skills.map((skill) => (
          <span key={skill.id} title={skill.category}>
            <i aria-hidden="true">{matched ? '✓' : '×'}</i>{skill.name}<small>{skill.category}</small>
          </span>
        )) : <p>{matched ? 'No required skills matched yet.' : 'No skill gaps detected — excellent coverage.'}</p>}
      </div>
    </section>
  )
}

function RecommendationPlan({ plan, loading }) {
  if (loading) {
    return <section className="recommendation-panel loading">Building your learning plan…</section>
  }

  if (!plan) return null

  if (plan.recommendations.length === 0) {
    return (
      <section className="recommendation-panel complete">
        <span aria-hidden="true">✓</span>
        <div><p className="eyebrow">LEARNING PLAN</p><h4>No required skill gaps</h4><p>Your resume covers every catalog skill detected for this role.</p></div>
      </section>
    )
  }

  return (
    <section className="recommendation-panel">
      <div className="recommendation-heading">
        <div><p className="eyebrow">RULE-BASED LEARNING PLAN</p><h4>Turn your gaps into next steps</h4><p>Prioritized from the missing skills in this saved analysis.</p></div>
        <span>{plan.recommendations.length} actions</span>
      </div>
      <ol className="recommendation-list">
        {plan.recommendations.map((recommendation, index) => (
          <li key={recommendation.id}>
            <span className="recommendation-number">{String(index + 1).padStart(2, '0')}</span>
            <div className="recommendation-copy">
              <div><span className={`priority-badge ${recommendation.learningPriority.toLowerCase()}`}>{recommendation.learningPriority} priority</span><small>{recommendation.skillName} · {recommendation.category}</small></div>
              <h5>{recommendation.title}</h5>
              <p>{recommendation.description}</p>
            </div>
          </li>
        ))}
      </ol>
    </section>
  )
}

function MatchResult({ result, recommendationPlan, recommendationsLoading }) {
  const score = Number(result.matchScore)
  const band = getScoreBand(score)
  const categories = buildCategoryCoverage(result)
  const requiredCount = result.matchedSkillsCount + result.missingSkillsCount
  const analyzedDate = new Intl.DateTimeFormat(undefined, {
    dateStyle: 'medium',
    timeStyle: 'short',
  }).format(new Date(result.createdAt))

  return (
    <article className="match-result" aria-live="polite">
      <header className="result-header">
        <div>
          <p className="eyebrow">MATCH RESULT</p>
          <h3>{result.jobTitle}</h3>
          <p>{result.company} <span>•</span> {result.resumeFileName}</p>
        </div>
        <time dateTime={result.createdAt}>Analyzed {analyzedDate}</time>
      </header>

      <div className="result-overview">
        <div
          className={`result-score-ring ${band.tone}`}
          style={{ '--score-angle': `${Math.max(0, Math.min(score, 100)) * 3.6}deg` }}
          role="img"
          aria-label={`${score}% match score, ${band.label}`}
        >
          <div><strong>{score.toLocaleString(undefined, { maximumFractionDigits: 2 })}</strong><span>%</span></div>
        </div>

        <div className="result-verdict">
          <span className={`verdict-pill ${band.tone}`}>{band.label}</span>
          <h4>{result.matchedSkillsCount} of {requiredCount} required skills matched</h4>
          <p>{requiredCount === 0 ? 'No catalog skills were detected in this job description, so a meaningful score cannot be calculated yet.' : band.summary}</p>
          <div className="coverage-track" aria-hidden="true"><span style={{ width: `${Math.max(0, Math.min(score, 100))}%` }} /></div>
          <small>Transparent score: matched required skills ÷ total required skills</small>
        </div>

        <dl className="result-metrics">
          <div><dt>Required</dt><dd>{requiredCount}</dd></div>
          <div className="positive"><dt>Matched</dt><dd>{result.matchedSkillsCount}</dd></div>
          <div className="negative"><dt>Missing</dt><dd>{result.missingSkillsCount}</dd></div>
        </dl>
      </div>

      <div className="result-skill-grid">
        <SkillCollection title="Matched skills" count={result.matchedSkillsCount} skills={result.matchedSkills} type="matched" />
        <SkillCollection title="Missing skills" count={result.missingSkillsCount} skills={result.missingSkills} type="missing" />
      </div>

      {categories.length > 0 && (
        <section className="category-coverage">
          <div className="result-section-title">
            <span className="result-section-icon neutral" aria-hidden="true">≡</span>
            <div><h4>Coverage by category</h4><p>Where your current strengths align with this role</p></div>
          </div>
          <div className="category-grid">
            {categories.map((category) => {
              const percentage = Math.round(category.matched / category.total * 100)
              return (
                <div className="category-row" key={category.name}>
                  <div><strong>{category.name}</strong><span>{category.matched}/{category.total}</span></div>
                  <div className="category-track"><span style={{ width: `${percentage}%` }} /></div>
                  <small>{percentage}%</small>
                </div>
              )
            })}
          </div>
        </section>
      )}

      <RecommendationPlan plan={recommendationPlan} loading={recommendationsLoading} />
    </article>
  )
}

export default MatchResult
