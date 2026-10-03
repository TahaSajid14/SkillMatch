import { getToken } from './authApi.js'

const API_BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5095/api'

export async function getResumes() {
  return request('/resumes')
}

export async function getResume(id) {
  return request(`/resumes/${id}`)
}

export async function uploadResume(file) {
  const body = new FormData()
  body.append('file', file)
  return request('/resumes', { method: 'POST', body })
}

export async function deleteResume(id) {
  return request(`/resumes/${id}`, { method: 'DELETE' })
}

export async function reanalyzeResume(id) {
  return request(`/resumes/${id}/extract-skills`, { method: 'POST' })
}

export async function downloadResume(id, fileName) {
  const response = await fetch(`${API_BASE_URL}/resumes/${id}/file`, {
    headers: { Authorization: `Bearer ${getToken()}` },
  })

  if (!response.ok) {
    throw new Error(await getErrorMessage(response, 'The resume could not be downloaded.'))
  }

  const objectUrl = URL.createObjectURL(await response.blob())
  const link = document.createElement('a')
  link.href = objectUrl
  link.download = fileName
  link.click()
  URL.revokeObjectURL(objectUrl)
}

async function request(path, options = {}) {
  let response

  try {
    response = await fetch(`${API_BASE_URL}${path}`, {
      ...options,
      headers: {
        ...options.headers,
        Authorization: `Bearer ${getToken()}`,
      },
    })
  } catch {
    throw new Error('The SkillMatch API is unavailable. Confirm that it is running on port 5095.')
  }

  if (!response.ok) {
    throw new Error(await getErrorMessage(response, 'The resume request failed.'))
  }

  return response.status === 204 ? null : response.json()
}

async function getErrorMessage(response, fallback) {
  const payload = await response.json().catch(() => ({}))
  return payload.detail || fallback
}
