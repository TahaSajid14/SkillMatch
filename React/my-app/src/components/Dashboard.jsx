import { useEffect, useRef, useState } from 'react'
import JobWorkspace from './JobWorkspace.jsx'
import MatchAnalyzer from './MatchAnalyzer.jsx'
import DashboardOverview from './DashboardOverview.jsx'
import CareerOrbit from './CareerOrbit.jsx'
import { deleteResume, downloadResume, getResume, getResumes, reanalyzeResume, uploadResume } from '../services/resumeApi.js'

const MAX_FILE_SIZE = 5 * 1024 * 1024
const ALLOWED_EXTENSIONS = ['.pdf', '.docx']

function Dashboard({ user, onLogout }) {
  const firstName = user.fullName.split(' ')[0]
  const fileInputRef = useRef(null)
  const [resumes, setResumes] = useState([])
  const [selectedResume, setSelectedResume] = useState(null)
  const [selectedFile, setSelectedFile] = useState(null)
  const [dragActive, setDragActive] = useState(false)
  const [loading, setLoading] = useState(true)
  const [uploading, setUploading] = useState(false)
  const [message, setMessage] = useState(null)
  const [jobsCount, setJobsCount] = useState(0)
  const [analyticsVersion, setAnalyticsVersion] = useState(0)

  useEffect(() => {
    let active = true
    getResumes()
      .then((data) => { if (active) setResumes(data) })
      .catch((error) => { if (active) setMessage({ type: 'error', text: error.message }) })
      .finally(() => { if (active) setLoading(false) })
    return () => { active = false }
  }, [])

  function chooseFile(file) {
    if (!file) return
    const extension = file.name.slice(file.name.lastIndexOf('.')).toLowerCase()

    if (!ALLOWED_EXTENSIONS.includes(extension)) {
      setSelectedFile(null)
      setMessage({ type: 'error', text: 'Choose a PDF or DOCX resume.' })
      return
    }

    if (file.size > MAX_FILE_SIZE) {
      setSelectedFile(null)
      setMessage({ type: 'error', text: 'The resume must be smaller than 5 MB.' })
      return
    }

    setSelectedFile(file)
    setMessage(null)
  }

  function handleDrop(event) {
    event.preventDefault()
    setDragActive(false)
    chooseFile(event.dataTransfer.files[0])
  }

  async function handleUpload(event) {
    event.preventDefault()
    if (!selectedFile) {
      setMessage({ type: 'error', text: 'Choose or drop a PDF or DOCX resume first.' })
      return
    }

    setUploading(true)
    setMessage(null)

    try {
      const result = await uploadResume(selectedFile)
      setResumes(await getResumes())
      setSelectedResume(await getResume(result.id))
      setMessage({ type: 'success', text: result.message })
      setSelectedFile(null)
      if (fileInputRef.current) fileInputRef.current.value = ''
    } catch (error) {
      setMessage({ type: 'error', text: error.message })
    } finally {
      setUploading(false)
    }
  }

  async function handleView(resumeId) {
    try {
      setSelectedResume(await getResume(resumeId))
      setMessage(null)
    } catch (error) {
      setMessage({ type: 'error', text: error.message })
    }
  }

  async function handleReanalyze(resumeId) {
    try {
      const updated = await reanalyzeResume(resumeId)
      setSelectedResume(updated)
      setResumes(await getResumes())
      setMessage({ type: 'success', text: `${updated.detectedSkills.length} skills detected.` })
    } catch (error) {
      setMessage({ type: 'error', text: error.message })
    }
  }

  async function handleDelete(resume) {
    if (!window.confirm(`Delete ${resume.fileName}? This cannot be undone.`)) return
    try {
      await deleteResume(resume.id)
      setResumes((current) => current.filter((item) => item.id !== resume.id))
      if (selectedResume?.id === resume.id) setSelectedResume(null)
      setMessage({ type: 'success', text: 'Resume deleted.' })
    } catch (error) {
      setMessage({ type: 'error', text: error.message })
    }
  }

  async function handleDownload(resume) {
    try {
      await downloadResume(resume.id, resume.fileName)
    } catch (error) {
      setMessage({ type: 'error', text: error.message })
    }
  }

  const totalDetectedSkills = resumes.reduce((total, resume) => total + resume.detectedSkillCount, 0)

  return (
    <main className="dashboard-shell">
      <nav className="nav dashboard-nav">
        <a className="brand" href="#dashboard" aria-label="SkillMatch dashboard"><span className="brand-mark">S</span><span>SkillMatch</span></a>
        <div className="user-menu">
          <div className="avatar">{user.fullName.charAt(0).toUpperCase()}</div>
          <div><strong>{user.fullName}</strong><span>{user.email}</span></div>
          <button className="text-button" onClick={onLogout}>Log out</button>
        </div>
      </nav>

      <section className="dashboard-content">
        <section className="dashboard-hero" id="dashboard">
          <div className="dashboard-hero-copy">
            <div className="release-chip"><span /> SkillMatch intelligence workspace</div>
            <p className="eyebrow">YOUR CAREER COMMAND CENTER</p>
            <h1>Welcome, {firstName}.<br /><span>See where you fit.</span></h1>
            <p>Turn every resume and job description into a clear skills map, an explainable match, and a practical path forward.</p>
            <div className="hero-quick-stats">
              <div><strong>{resumes.length}</strong><span>Resumes</span></div>
              <div><strong>{jobsCount}</strong><span>Targets</span></div>
              <div><strong>{totalDetectedSkills}</strong><span>Skills found</span></div>
            </div>
          </div>
          <CareerOrbit skillCount={totalDetectedSkills} jobCount={jobsCount} />
        </section>

        <nav className="workspace-jump" aria-label="Workspace sections">
          <a href="#analytics"><span>01</span> Analytics</a>
          <a href="#resumes"><span>02</span> Resumes</a>
          <a href="#jobs"><span>03</span> Jobs</a>
          <a href="#matching"><span>04</span> Match lab</a>
        </nav>

        <DashboardOverview refreshKey={`${resumes.length}:${jobsCount}:${analyticsVersion}:${totalDetectedSkills}`} onMessage={setMessage} />

        {message && <div className={`dashboard-message ${message.type}`} role="status">{message.text}</div>}

        <div className="resume-layout">
          <section className="resume-library" id="resumes">
            <div className="section-heading"><div><p className="eyebrow">RESUME LIBRARY</p><h2>Your resumes</h2></div><span>{resumes.length} of your files</span></div>
            {loading ? <p className="empty-state">Loading your resumes…</p> : resumes.length === 0 ? (
              <div className="empty-state"><strong>No resumes yet</strong><span>Drop your first PDF or Word document here.</span></div>
            ) : (
              <div className="resume-list">
                {resumes.map((resume) => {
                  const isWord = resume.fileName.toLowerCase().endsWith('.docx')
                  return (
                    <article className={selectedResume?.id === resume.id ? 'selected' : ''} key={resume.id}>
                      <div className={`file-icon ${isWord ? 'word' : ''}`}>{isWord ? 'DOCX' : 'PDF'}</div>
                      <div className="resume-meta"><strong>{resume.fileName}</strong><span>{resume.detectedSkillCount} skills · {resume.extractedCharacterCount.toLocaleString()} characters · {new Date(resume.uploadedAt).toLocaleDateString()}</span></div>
                      <div className="resume-actions"><button onClick={() => handleView(resume.id)}>View</button><button onClick={() => handleDownload(resume)}>Download</button><button className="danger-button" onClick={() => handleDelete(resume)}>Delete</button></div>
                    </article>
                  )
                })}
              </div>
            )}
          </section>

          <aside className="upload-card">
            <p className="eyebrow">ADD A RESUME</p><h2>Upload document</h2>
            <p>Drop a file below. Text and known skills are extracted locally.</p>
            <form onSubmit={handleUpload}>
              <div className={`drop-zone ${dragActive ? 'drag-active' : ''}`} role="button" tabIndex="0"
                onClick={() => fileInputRef.current?.click()}
                onKeyDown={(event) => { if (event.key === 'Enter' || event.key === ' ') fileInputRef.current?.click() }}
                onDragEnter={(event) => { event.preventDefault(); setDragActive(true) }}
                onDragOver={(event) => event.preventDefault()}
                onDragLeave={(event) => { if (!event.currentTarget.contains(event.relatedTarget)) setDragActive(false) }}
                onDrop={handleDrop}>
                <span className="upload-icon">{dragActive ? '↓' : '↑'}</span>
                <strong>{dragActive ? 'Drop to add your resume' : 'Drag and drop your resume'}</strong>
                <small>or click to browse · PDF or DOCX · maximum 5 MB</small>
                <input ref={fileInputRef} className="file-input" type="file" accept="application/pdf,.pdf,application/vnd.openxmlformats-officedocument.wordprocessingml.document,.docx" onChange={(event) => chooseFile(event.target.files[0])} />
                {selectedFile && <span className="selected-file">✓ {selectedFile.name}</span>}
              </div>
              <button className="submit-button" disabled={uploading || !selectedFile} type="submit">{uploading ? 'Extracting text and skills…' : 'Upload and analyze'}</button>
            </form>
          </aside>
        </div>

        {selectedResume && (
          <section className="text-preview">
            <div className="section-heading">
              <div><p className="eyebrow">RESUME ANALYSIS</p><h2>{selectedResume.fileName}</h2></div>
              <div className="preview-actions"><button onClick={() => handleReanalyze(selectedResume.id)}>Re-analyze skills</button><button className="close-button inline-close" onClick={() => setSelectedResume(null)} aria-label="Close resume preview">×</button></div>
            </div>
            <div className="detected-skills"><p>Detected skills <strong>{selectedResume.detectedSkills.length}</strong></p><div className="skill-badges">{selectedResume.detectedSkills.length > 0 ? selectedResume.detectedSkills.map((skill) => <span key={skill.id} title={skill.category}>{skill.name}</span>) : <small>No catalog skills detected yet.</small>}</div></div>
            {selectedResume.extractedText ? <pre>{selectedResume.extractedText}</pre> : <p className="empty-state">No selectable text was found. Scanned PDFs require OCR.</p>}
          </section>
        )}

        <JobWorkspace onCountChange={setJobsCount} onMessage={setMessage} />
        <MatchAnalyzer refreshKey={`${resumes.length}:${jobsCount}`} onAnalysisSaved={() => setAnalyticsVersion((current) => current + 1)} onMessage={setMessage} />
      </section>
    </main>
  )
}

export default Dashboard
