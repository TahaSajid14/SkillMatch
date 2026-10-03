import { useState } from 'react'
import { login, register } from '../services/authApi.js'

const emptyForm = { fullName: '', email: '', password: '' }

function AuthPanel({ initialMode, onAuthenticated, onClose }) {
  const [mode, setMode] = useState(initialMode)
  const [form, setForm] = useState(emptyForm)
  const [error, setError] = useState('')
  const [submitting, setSubmitting] = useState(false)

  function updateField(event) {
    setForm((current) => ({ ...current, [event.target.name]: event.target.value }))
  }

  function switchMode(nextMode) {
    setMode(nextMode)
    setError('')
  }

  async function handleSubmit(event) {
    event.preventDefault()
    setSubmitting(true)
    setError('')

    try {
      const response = mode === 'register'
        ? await register(form)
        : await login({ email: form.email, password: form.password })
      onAuthenticated(response.user)
    } catch (requestError) {
      setError(requestError.message)
    } finally {
      setSubmitting(false)
    }
  }

  const isRegister = mode === 'register'

  return (
    <aside className="auth-card" aria-label={isRegister ? 'Create account' : 'Log in'}>
      <button className="close-button" onClick={onClose} aria-label="Close authentication form">×</button>
      <p className="eyebrow">{isRegister ? 'START YOUR PROFILE' : 'WELCOME BACK'}</p>
      <h2>{isRegister ? 'Create your account' : 'Log in to SkillMatch'}</h2>
      <p className="auth-intro">
        {isRegister ? 'Your career matching workspace starts here.' : 'Continue building your career profile.'}
      </p>

      <form onSubmit={handleSubmit}>
        {isRegister && (
          <label>
            Full name
            <input name="fullName" value={form.fullName} onChange={updateField} minLength="2" maxLength="150" autoComplete="name" required />
          </label>
        )}
        <label>
          Email address
          <input name="email" type="email" value={form.email} onChange={updateField} autoComplete="email" required />
        </label>
        <label>
          Password
          <input name="password" type="password" value={form.password} onChange={updateField} minLength="8" maxLength="100" autoComplete={isRegister ? 'new-password' : 'current-password'} required />
          {isRegister && <small>Use at least 8 characters.</small>}
        </label>

        {error && <div className="form-error" role="alert">{error}</div>}

        <button className="submit-button" type="submit" disabled={submitting}>
          {submitting ? 'Please wait…' : isRegister ? 'Create account' : 'Log in'}
        </button>
      </form>

      <p className="auth-switch">
        {isRegister ? 'Already have an account?' : 'New to SkillMatch?'}{' '}
        <button onClick={() => switchMode(isRegister ? 'login' : 'register')}>
          {isRegister ? 'Log in' : 'Create one'}
        </button>
      </p>
    </aside>
  )
}

export default AuthPanel
