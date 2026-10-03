import { getToken } from './authApi.js'
import { API_BASE_URL } from './apiConfig.js'

export function analyzeMatch(resumeId, jobId) {
  return request('/match/analyze', {
    method: 'POST',
    body: JSON.stringify({ resumeId, jobId }),
  })
}

export function getMatchHistory() {
  return request('/match/history')
}

export function getMatch(id) {
  return request(`/match/${id}`)
}

export function getMatchRecommendations(id) {
  return request(`/match/${id}/recommendations`)
}

async function request(path, options = {}) {
  let response
  try {
    response = await fetch(`${API_BASE_URL}${path}`, {
      ...options,
      headers: {
        'Content-Type': 'application/json',
        ...options.headers,
        Authorization: `Bearer ${getToken()}`,
      },
    })
  } catch {
    throw new Error('The SkillMatch API is unavailable. Please try again shortly.')
  }

  const payload = await response.json().catch(() => ({}))
  if (!response.ok) throw new Error(payload.detail || 'The match request failed.')
  return payload
}
