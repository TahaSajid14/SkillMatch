const API_BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5095/api'
const TOKEN_KEY = 'skillmatch_token'

export function getToken() {
  return localStorage.getItem(TOKEN_KEY)
}

export function clearSession() {
  localStorage.removeItem(TOKEN_KEY)
}

export async function register(form) {
  return authenticate('/auth/register', form)
}

export async function login(credentials) {
  return authenticate('/auth/login', credentials)
}

export async function getCurrentUser() {
  const response = await fetch(`${API_BASE_URL}/users/me`, {
    headers: { Authorization: `Bearer ${getToken()}` },
  })

  if (!response.ok) {
    throw new Error('Your session has expired. Please log in again.')
  }

  return response.json()
}

async function authenticate(path, body) {
  let response

  try {
    response = await fetch(`${API_BASE_URL}${path}`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(body),
    })
  } catch {
    throw new Error('The SkillMatch API is unavailable. Start the API and try again.')
  }

  const payload = await response.json().catch(() => ({}))

  if (!response.ok) {
    const validationMessage = payload.errors
      ? Object.values(payload.errors).flat().join(' ')
      : null
    throw new Error(validationMessage || payload.detail || 'Authentication failed.')
  }

  localStorage.setItem(TOKEN_KEY, payload.token)
  return payload
}
