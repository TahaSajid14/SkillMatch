import { getToken } from './authApi.js'

const API_BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5095/api'

export async function getDashboard() {
  let response
  try {
    response = await fetch(`${API_BASE_URL}/dashboard`, {
      headers: { Authorization: `Bearer ${getToken()}` },
    })
  } catch {
    throw new Error('The SkillMatch API is unavailable. Confirm that it is running on port 5095.')
  }

  const payload = await response.json().catch(() => ({}))
  if (!response.ok) throw new Error(payload.detail || 'The dashboard could not be loaded.')
  return payload
}
