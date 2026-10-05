import { useEffect, useRef, useState } from 'react'
import AuthPanel from './components/AuthPanel.jsx'
import Dashboard from './components/Dashboard.jsx'
import { clearSession, getCurrentUser, getToken } from './services/authApi.js'
import './App.css'

function App() {
  const [user, setUser] = useState(null)
  const [authMode, setAuthMode] = useState(null)
  const [checkingSession, setCheckingSession] = useState(true)
  const authPanelRef = useRef(null)

  useEffect(() => {
    async function restoreSession() {
      if (!getToken()) {
        setCheckingSession(false)
        return
      }

      try {
        setUser(await getCurrentUser())
      } catch {
        clearSession()
      } finally {
        setCheckingSession(false)
      }
    }

    restoreSession()
  }, [])

  useEffect(() => {
    if (!authMode || !authPanelRef.current) return

    const panel = authPanelRef.current
    const panelBounds = panel.getBoundingClientRect()
    const isFullyVisible = panelBounds.top >= 0 && panelBounds.bottom <= window.innerHeight

    if (!isFullyVisible) {
      panel.scrollIntoView({ behavior: 'smooth', block: 'center' })
    }

    panel.querySelector('input')?.focus({ preventScroll: true })
  }, [authMode])

  function openAuth(mode) {
    setAuthMode(mode)
  }

  function handleLogout() {
    clearSession()
    setUser(null)
    setAuthMode('login')
  }

  if (checkingSession) {
    return <div className="session-loading">Restoring your SkillMatch session…</div>
  }

  if (user) {
    return <Dashboard user={user} onLogout={handleLogout} />
  }

  return (
    <main className="app-shell">
      <nav className="nav" aria-label="Main navigation">
        <a className="brand" href="#top" aria-label="SkillMatch home">
          <span className="brand-mark">S</span>
          <span>SkillMatch</span>
        </a>
        <div className="nav-actions">
          <button className="text-button" type="button" onClick={() => openAuth('login')}>Log in</button>
          <button className="nav-button" type="button" onClick={() => openAuth('register')}>Create account</button>
        </div>
      </nav>

      <section className="hero" id="top">
        <div className="hero-copy">
          <p className="eyebrow">INTELLIGENT CAREER MATCHING</p>
          <h1>Match your skills.<br /><span>Build your career.</span></h1>
          <p className="intro">
            SkillMatch turns resumes and job descriptions into clear skill insights,
            transparent match scores, and practical learning recommendations.
          </p>
          <div className="actions">
            <button className="primary-button" type="button" onClick={() => openAuth('register')}>
              Create your account <span aria-hidden="true">→</span>
            </button>
            <button className="secondary-button" type="button" onClick={() => openAuth('login')}>I already have an account</button>
          </div>
        </div>

        {authMode ? (
          <AuthPanel
            mode={authMode}
            onModeChange={openAuth}
            onAuthenticated={setUser}
            onClose={() => setAuthMode(null)}
            panelRef={authPanelRef}
          />
        ) : (
          <aside className="preview-card" aria-label="Match result preview">
            <div className="card-header">
              <div><p>Match preview</p><h2>Full-Stack Developer</h2></div>
              <span className="live-dot">Live workflow</span>
            </div>
            <div className="score-row">
              <div className="score-ring"><strong>82</strong><span>%</span></div>
              <div><p className="score-label">Strong match</p><p className="muted">8 of 10 required skills found</p></div>
            </div>
            <div className="skill-block">
              <p>Matched skills</p>
              <div className="badges"><span>React</span><span>ASP.NET Core</span><span>SQL Server</span><span>Git</span></div>
            </div>
            <div className="missing"><span>Next gap</span><strong>Docker fundamentals</strong></div>
          </aside>
        )}
      </section>

      <footer className="landing-footer">
        <div className="footer-intro">
          <a className="brand footer-brand" href="#top"><span className="brand-mark">S</span><span>SkillMatch</span></a>
          <p>Private resume analysis, explainable job matching, and practical learning recommendations in one workspace.</p>
        </div>
        <div className="footer-features" aria-label="Product capabilities">
          <span>PDF + Word parsing</span>
          <span>Transparent scoring</span>
          <span>Private by design</span>
        </div>
        <p className="footer-note">Built with React, ASP.NET Core, and SQL Server.</p>
      </footer>
    </main>
  )
}

export default App
