import { useEffect, useState } from 'react'
import { getJobs } from '../services/jobApi.js'
import { analyzeMatch, getMatch, getMatchHistory, getMatchRecommendations } from '../services/matchApi.js'
import { getResumes } from '../services/resumeApi.js'
import MatchResult from './MatchResult.jsx'

function MatchAnalyzer({ refreshKey, onAnalysisSaved, onMessage }) {
  const [resumes, setResumes] = useState([])
  const [jobs, setJobs] = useState([])
  const [history, setHistory] = useState([])
  const [resumeId, setResumeId] = useState('')
  const [jobId, setJobId] = useState('')
  const [result, setResult] = useState(null)
  const [recommendationPlan, setRecommendationPlan] = useState(null)
  const [recommendationsLoading, setRecommendationsLoading] = useState(false)
  const [loading, setLoading] = useState(true)
  const [analyzing, setAnalyzing] = useState(false)

  useEffect(() => {
    let active = true
    Promise.all([getResumes(), getJobs(), getMatchHistory()])
      .then(([resumeData, jobData, historyData]) => {
        if (!active) return
        setResumes(resumeData)
        setJobs(jobData)
        setHistory(historyData)
      })
      .catch((error) => { if (active) onMessage({ type: 'error', text: error.message }) })
      .finally(() => { if (active) setLoading(false) })
    return () => { active = false }
  }, [refreshKey, onMessage])

  async function handleAnalyze(event) {
    event.preventDefault()
    setAnalyzing(true)
    onMessage(null)

    try {
      const analysis = await analyzeMatch(Number(resumeId), Number(jobId))
      setResult(analysis)
      setHistory(await getMatchHistory())
      onAnalysisSaved()
      onMessage({ type: 'success', text: `Match analysis saved with a ${analysis.matchScore}% score.` })
      await loadRecommendations(analysis.id)
    } catch (error) {
      onMessage({ type: 'error', text: error.message })
    } finally {
      setAnalyzing(false)
    }
  }

  async function loadRecommendations(matchId) {
    setRecommendationPlan(null)
    setRecommendationsLoading(true)
    try {
      setRecommendationPlan(await getMatchRecommendations(matchId))
    } catch (error) {
      onMessage({ type: 'error', text: error.message })
    } finally {
      setRecommendationsLoading(false)
    }
  }

  async function handleHistorySelection(matchId) {
    try {
      setResult(await getMatch(matchId))
      await loadRecommendations(matchId)
    } catch (error) {
      onMessage({ type: 'error', text: error.message })
    }
  }

  return (
    <section className="match-workspace" id="matching">
      <div className="section-heading match-heading">
        <div><p className="eyebrow">SKILL MATCHING</p><h2>Analyze a job match</h2></div>
        <span>Required skills only</span>
      </div>

      {loading ? <p className="empty-state">Loading matching data…</p> : resumes.length === 0 || jobs.length === 0 ? (
        <div className="empty-state"><strong>Resume and job required</strong><span>Upload a resume and save a job before running an analysis.</span></div>
      ) : (
        <form className="match-form" onSubmit={handleAnalyze}>
          <label>Resume<select value={resumeId} onChange={(event) => setResumeId(event.target.value)} required><option value="">Choose a resume</option>{resumes.map((resume) => <option value={resume.id} key={resume.id}>{resume.fileName} · {resume.detectedSkillCount} skills</option>)}</select></label>
          <label>Job<select value={jobId} onChange={(event) => setJobId(event.target.value)} required><option value="">Choose a saved job</option>{jobs.map((job) => <option value={job.id} key={job.id}>{job.title} · {job.company}</option>)}</select></label>
          <button className="submit-button" disabled={analyzing}>{analyzing ? 'Calculating…' : 'Analyze match'}</button>
        </form>
      )}

      {result && <MatchResult result={result} recommendationPlan={recommendationPlan} recommendationsLoading={recommendationsLoading} />}

      <div className="match-history">
        <div className="section-heading"><div><p className="eyebrow">RECENT ANALYSES</p><h3>Match history</h3></div><span>{history.length} saved</span></div>
        {history.length === 0 ? <p className="history-empty">No match analyses yet.</p> : (
          <div className="history-list">{history.slice(0, 6).map((match) => <button className={result?.id === match.id ? 'active' : ''} type="button" aria-pressed={result?.id === match.id} key={match.id} onClick={() => handleHistorySelection(match.id)}><span><strong>{match.jobTitle}</strong><small>{match.resumeFileName} · {new Date(match.createdAt).toLocaleDateString()}</small></span><b>{match.matchScore}%</b></button>)}</div>
        )}
      </div>
    </section>
  )
}

export default MatchAnalyzer
