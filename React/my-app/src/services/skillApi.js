import { getToken } from './authApi.js'

const API_BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5095/api'

export async function extractSkills(text) {
  let response

  try {
    response = await fetch(`${API_BASE_URL}/skills/extract`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${getToken()}` },
      body: JSON.stringify({ text }),
    })
  } catch {
    throw new Error('The SkillMatch API is unavailable. Confirm that it is running on port 5095.')
  }

  const payload = await response.json().catch(() => ({}))
  if (!response.ok) throw new Error(payload.detail || 'Skills could not be extracted.')
  return payload
}
