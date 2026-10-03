import { getToken } from './authApi.js'
import { API_BASE_URL } from './apiConfig.js'

export async function extractSkills(text) {
  let response

  try {
    response = await fetch(`${API_BASE_URL}/skills/extract`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${getToken()}` },
      body: JSON.stringify({ text }),
    })
  } catch {
    throw new Error('The SkillMatch API is unavailable. Please try again shortly.')
  }

  const payload = await response.json().catch(() => ({}))
  if (!response.ok) throw new Error(payload.detail || 'Skills could not be extracted.')
  return payload
}
