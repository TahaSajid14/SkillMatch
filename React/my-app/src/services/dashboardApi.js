import { getToken } from './authApi.js'
import { API_BASE_URL } from './apiConfig.js'

export async function getDashboard() {
  let response
  try {
    response = await fetch(`${API_BASE_URL}/dashboard`, {
      headers: { Authorization: `Bearer ${getToken()}` },
    })
  } catch {
    throw new Error('The SkillMatch API is unavailable. Please try again shortly.')
  }

  const payload = await response.json().catch(() => ({}))
  if (!response.ok) throw new Error(payload.detail || 'The dashboard could not be loaded.')
  return payload
}
