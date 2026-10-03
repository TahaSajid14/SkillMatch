const configuredApiUrl = import.meta.env.VITE_API_URL?.trim().replace(/\/+$/, '')

if (!configuredApiUrl) {
  throw new Error('VITE_API_URL must be configured before SkillMatch can start.')
}

export const API_BASE_URL = configuredApiUrl.endsWith('/api')
  ? configuredApiUrl
  : `${configuredApiUrl}/api`
