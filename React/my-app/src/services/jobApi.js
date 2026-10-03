import { getToken } from './authApi.js'

const API_BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5095/api'

export function getJobs() {
  return request('/jobs')
}

export function getJob(id) {
  return request(`/jobs/${id}`)
}

export function createJob(job) {
  return request('/jobs', { method: 'POST', body: JSON.stringify(job) })
}

export function updateJob(id, job) {
  return request(`/jobs/${id}`, { method: 'PUT', body: JSON.stringify(job) })
}

export function deleteJob(id) {
  return request(`/jobs/${id}`, { method: 'DELETE' })
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
    throw new Error('The SkillMatch API is unavailable. Confirm that it is running on port 5095.')
  }

  const payload = response.status === 204 ? null : await response.json().catch(() => ({}))
  if (!response.ok) {
    const validationMessage = payload?.errors
      ? Object.values(payload.errors).flat().join(' ')
      : null
    throw new Error(validationMessage || payload?.detail || 'The job request failed.')
  }

  return payload
}
