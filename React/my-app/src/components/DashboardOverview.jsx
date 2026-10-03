import { useEffect, useState } from 'react'
import { getDashboard } from '../services/dashboardApi.js'

function formatScore(value) {
  return Number(value).toLocaleString(undefined, { maximumFractionDigits: 1 })
}

function DashboardOverview({ refreshKey, onMessage }) {
  const [dashboard, setDashboard] = useState(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    let active = true
    getDashboard()
      .then((data) => { if (active) setDashboard(data) })
      .catch((error) => { if (active) onMessage({ type: 'error', text: error.message }) })
      .finally(() => { if (active) setLoading(false) })
    return () => { active = false }
  }, [refreshKey, onMessage])

  if (loading && !dashboard) {
    return <section className="analytics-loading">Preparing your career analytics…</section>
  }

  if (!dashboard) return null

  const hasAnalyses = dashboard.totalAnalyses > 0

  return (
    <section className="dashboard-overview" id="analytics" aria-labelledby="analytics-title">
      <div className="analytics-heading">
        <div><p className="eyebrow">CAREER ANALYTICS</p><h2 id="analytics-title">Your progress at a glance</h2></div>
        <span>{dashboard.totalAnalyses} {dashboard.totalAnalyses === 1 ? 'analysis' : 'analyses'}</span>
      </div>

      <div className="analytics-stat-grid">
        <article><span className="stat-symbol document" aria-hidden="true">▤</span><div><p>Resumes</p><strong>{dashboard.totalResumes}</strong><small>Documents analyzed</small></div></article>
        <article><span className="stat-symbol target" aria-hidden="true">◎</span><div><p>Jobs analyzed</p><strong>{dashboard.totalJobsAnalyzed}</strong><small>of {dashboard.totalJobs} saved</small></div></article>
        <article><span className="stat-symbol average" aria-hidden="true">↗</span><div><p>Average match</p><strong>{formatScore(dashboard.averageMatchScore)}<i>%</i></strong><small>Across all analyses</small></div></article>
        <article><span className="stat-symbol trophy" aria-hidden="true">★</span><div><p>Highest match</p><strong>{formatScore(dashboard.highestMatchScore)}<i>%</i></strong><small>Your best result</small></div></article>
      </div>

      <div className="analytics-grid">
        <article className="analytics-card skill-profile-card">
          <div className="analytics-card-heading"><div><p className="eyebrow">SKILL PROFILE</p><h3>Distribution by category</h3></div><span>{dashboard.skillDistribution.reduce((sum, item) => sum + item.skillCount, 0)} unique</span></div>
          {dashboard.skillDistribution.length === 0 ? (
            <div className="analytics-empty"><strong>No skills to chart yet</strong><span>Upload a resume to build your profile.</span></div>
          ) : (
            <div className="profile-chart">
              {dashboard.skillDistribution.map((item) => (
                <div className="profile-row" key={item.category}>
                  <div><strong>{item.category}</strong><span>{item.skillCount} {item.skillCount === 1 ? 'skill' : 'skills'}</span></div>
                  <div className="profile-track"><span style={{ width: `${item.percentage}%` }} /></div>
                  <b>{formatScore(item.percentage)}%</b>
                </div>
              ))}
            </div>
          )}
        </article>

        <article className="analytics-card missing-chart-card">
          <div className="analytics-card-heading"><div><p className="eyebrow">RECURRING GAPS</p><h3>Most commonly missing</h3></div><span>Top 5</span></div>
          {dashboard.mostCommonMissingSkills.length === 0 ? (
            <div className="analytics-empty"><strong>{hasAnalyses ? 'No recurring gaps' : 'No match data yet'}</strong><span>{hasAnalyses ? 'Your analyzed roles have no missing catalog skills.' : 'Run a match to reveal recurring gaps.'}</span></div>
          ) : (
            <ol className="missing-chart">
              {dashboard.mostCommonMissingSkills.map((skill, index) => {
                const maximum = dashboard.mostCommonMissingSkills[0].missingCount
                return (
                  <li key={skill.skillId}>
                    <span>{index + 1}</span>
                    <div><div><strong>{skill.name}</strong><small>{skill.category}</small></div><div className="gap-track"><span style={{ width: `${skill.missingCount / maximum * 100}%` }} /></div></div>
                    <b>{skill.missingCount}×</b>
                  </li>
                )
              })}
            </ol>
          )}
        </article>

        <article className="analytics-card recent-analytics-card">
          <div className="analytics-card-heading"><div><p className="eyebrow">RECENT ACTIVITY</p><h3>Latest analyses</h3></div><span>Newest first</span></div>
          {dashboard.recentAnalyses.length === 0 ? (
            <div className="analytics-empty"><strong>No analyses yet</strong><span>Your latest match results will appear here.</span></div>
          ) : (
            <div className="analytics-table" role="table" aria-label="Recent match analyses">
              <div className="analytics-table-head" role="row"><span role="columnheader">Opportunity</span><span role="columnheader">Coverage</span><span role="columnheader">Score</span></div>
              {dashboard.recentAnalyses.map((analysis) => (
                <div className="analytics-table-row" role="row" key={analysis.id}>
                  <div role="cell"><strong>{analysis.jobTitle}</strong><span>{analysis.company} · {new Date(analysis.createdAt).toLocaleDateString()}</span></div>
                  <span role="cell">{analysis.matchedSkillsCount} matched · {analysis.missingSkillsCount} missing</span>
                  <b role="cell">{formatScore(analysis.matchScore)}%</b>
                </div>
              ))}
            </div>
          )}
        </article>
      </div>
    </section>
  )
}

export default DashboardOverview
