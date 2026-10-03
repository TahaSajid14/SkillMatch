import { useEffect, useState } from 'react'
import { createJob, deleteJob, getJob, getJobs, updateJob } from '../services/jobApi.js'

const EMPTY_FORM = { title: '', company: '', description: '' }

function JobWorkspace({ onCountChange, onMessage }) {
  const [jobs, setJobs] = useState([])
  const [selectedJob, setSelectedJob] = useState(null)
  const [editingId, setEditingId] = useState(null)
  const [form, setForm] = useState(EMPTY_FORM)
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)

  useEffect(() => {
    let active = true
    getJobs()
      .then((data) => {
        if (!active) return
        setJobs(data)
        onCountChange(data.length)
      })
      .catch((error) => { if (active) onMessage({ type: 'error', text: error.message }) })
      .finally(() => { if (active) setLoading(false) })
    return () => { active = false }
  }, [onCountChange, onMessage])

  function updateField(event) {
    setForm((current) => ({ ...current, [event.target.name]: event.target.value }))
  }

  function resetForm() {
    setForm(EMPTY_FORM)
    setEditingId(null)
  }

  async function refreshJobs() {
    const updatedJobs = await getJobs()
    setJobs(updatedJobs)
    onCountChange(updatedJobs.length)
  }

  async function handleSubmit(event) {
    event.preventDefault()
    setSaving(true)
    onMessage(null)

    try {
      const saved = editingId
        ? await updateJob(editingId, form)
        : await createJob(form)
      await refreshJobs()
      setSelectedJob(saved)
      resetForm()
      onMessage({
        type: 'success',
        text: `${saved.title} ${editingId ? 'updated' : 'saved'} with ${saved.detectedSkills.length} detected skills.`,
      })
    } catch (error) {
      onMessage({ type: 'error', text: error.message })
    } finally {
      setSaving(false)
    }
  }

  async function handleView(jobId) {
    try {
      setSelectedJob(await getJob(jobId))
    } catch (error) {
      onMessage({ type: 'error', text: error.message })
    }
  }

  async function handleEdit(jobId) {
    try {
      const job = selectedJob?.id === jobId ? selectedJob : await getJob(jobId)
      setSelectedJob(job)
      setEditingId(job.id)
      setForm({ title: job.title, company: job.company, description: job.description })
    } catch (error) {
      onMessage({ type: 'error', text: error.message })
    }
  }

  async function handleDelete(job) {
    if (!window.confirm(`Delete ${job.title} at ${job.company}?`)) return

    try {
      await deleteJob(job.id)
      const remaining = jobs.filter((item) => item.id !== job.id)
      setJobs(remaining)
      onCountChange(remaining.length)
      if (selectedJob?.id === job.id) setSelectedJob(null)
      if (editingId === job.id) resetForm()
      onMessage({ type: 'success', text: 'Job deleted.' })
    } catch (error) {
      onMessage({ type: 'error', text: error.message })
    }
  }

  return (
    <section className="jobs-workspace" id="jobs">
      <div className="section-heading jobs-heading">
        <div><p className="eyebrow">TARGET OPPORTUNITIES</p><h2>Saved jobs</h2></div>
        <span>{jobs.length} saved</span>
      </div>

      <div className="jobs-layout">
        <div className="job-list-panel">
          {loading ? <p className="empty-state">Loading saved jobs…</p> : jobs.length === 0 ? (
            <div className="empty-state"><strong>No saved jobs yet</strong><span>Add a job description to extract its required skills.</span></div>
          ) : (
            <div className="job-list">
              {jobs.map((job) => (
                <article className={selectedJob?.id === job.id ? 'selected' : ''} key={job.id}>
                  <div className="company-mark">{job.company.charAt(0).toUpperCase()}</div>
                  <div className="job-card-copy"><strong>{job.title}</strong><span>{job.company} · {job.detectedSkillCount} skills · {new Date(job.createdAt).toLocaleDateString()}</span></div>
                  <div className="resume-actions"><button onClick={() => handleView(job.id)}>View</button><button onClick={() => handleEdit(job.id)}>Edit</button><button className="danger-button" onClick={() => handleDelete(job)}>Delete</button></div>
                </article>
              ))}
            </div>
          )}
        </div>

        <form className="job-form" onSubmit={handleSubmit}>
          <div><p className="eyebrow">{editingId ? 'EDIT OPPORTUNITY' : 'ADD OPPORTUNITY'}</p><h3>{editingId ? 'Update job' : 'Save a new job'}</h3></div>
          <label>Job title<input name="title" value={form.title} onChange={updateField} minLength="2" maxLength="200" required /></label>
          <label>Company<input name="company" value={form.company} onChange={updateField} minLength="2" maxLength="200" required /></label>
          <label>Job description<textarea name="description" value={form.description} onChange={updateField} minLength="10" maxLength="50000" placeholder="Paste the complete job description…" required /></label>
          <div className="job-form-actions">
            <button className="submit-button" disabled={saving}>{saving ? 'Analyzing skills…' : editingId ? 'Update job' : 'Save and extract skills'}</button>
            {editingId && <button className="cancel-button" type="button" onClick={resetForm}>Cancel</button>}
          </div>
        </form>
      </div>

      {selectedJob && (
        <article className="job-detail">
          <div className="section-heading"><div><p className="eyebrow">JOB DETAILS</p><h2>{selectedJob.title}</h2><span>{selectedJob.company}</span></div><button className="close-button inline-close" onClick={() => setSelectedJob(null)} aria-label="Close job details">×</button></div>
          <div className="detected-skills"><p>Required skills <strong>{selectedJob.detectedSkills.length}</strong></p><div className="skill-badges">{selectedJob.detectedSkills.length > 0 ? selectedJob.detectedSkills.map((skill) => <span key={skill.id} title={skill.category}>{skill.name}</span>) : <small>No catalog skills detected.</small>}</div></div>
          <p className="job-description">{selectedJob.description}</p>
        </article>
      )}
    </section>
  )
}

export default JobWorkspace
