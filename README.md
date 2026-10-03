# SkillMatch

**Match Your Skills. Find Your Gaps. Build Your Career.**

SkillMatch is a full-stack career analysis platform that parses PDF and Word
resumes, extracts known technical skills, compares them with saved job descriptions,
and produces explainable match scores and learning plans. It uses deterministic
rules rather than presenting keyword matching as artificial intelligence.

The application includes JWT authentication, private file storage, 35 catalog
skills, directional skill compatibility, immutable match history, responsive
analytics, and curated recommendations. Phase 10 completes the ten-phase build with
an animated 3D career workspace, accessible reduced-motion behavior, automated
regression tests, CI, and container deployment configuration.

## Technology

- React 19 and Vite
- ASP.NET Core 10 Web API
- Entity Framework Core 10 and SQL Server
- JWT bearer authentication and ASP.NET Core password hashing
- PdfPig and the Open XML SDK for local document extraction
- CSS 3D transforms and responsive native charts with no animation dependency
- xUnit, GitHub Actions, Docker, Nginx, and Docker Compose

## Highlights

- Private PDF and DOCX upload with drag and drop
- Boundary-aware skill and alias extraction
- Transparent scoring with matched and missing skill snapshots
- Compatibility rules such as `ASP.NET Core` satisfying `.NET`
- Match history, category coverage, career analytics, and recurring-gap charts
- Rule-based, priority-ordered learning recommendations
- Responsive 3D career orbit with a reduced-motion fallback
- User isolation across every resume, job, match, dashboard, and recommendation API

## Run locally

1. Ensure the local SQL Server default instance (`.`) is running.
2. From `Asp.net/asp.net/asp.net`, run `dotnet ef database update`, then
   `dotnet run --launch-profile http`.
3. Open `http://localhost:5095/swagger` for the API documentation.
4. From `React/my-app`, run `npm run dev`.
5. Open `http://localhost:5173` for the web application.

The development connection string uses Windows authentication and the
`skillmatchdb` database. Change `ConnectionStrings:DefaultConnection` in
`appsettings.json` if your SQL Server instance is different.

The local JWT signing key is intentionally stored only in
`appsettings.Development.json`. Use an environment variable or secret store for a
production value (`Jwt__Key`) and never commit a production signing key.

## Test

Run backend regression tests from `Asp.net/asp.net`:

```powershell
dotnet test -c Release
```

Run frontend validation from `React/my-app`:

```powershell
npm run lint
npm run build
```

The compatibility test suite protects the directed `.NET` and CSS-family matching
rules. GitHub Actions runs the backend tests plus frontend lint and build on pushes
and pull requests to `main`.

## Run with containers

1. Copy `.env.example` to `.env` and replace both example secrets.
2. Run `docker compose up --build` from the repository root.
3. Open `http://localhost:8080`.

The container stack runs Nginx, the API, and SQL Server. Named volumes preserve the
database and private resume files, while the API applies EF Core migrations during
container startup. Do not commit the populated `.env` file.

## Deployment

The production layout keeps the existing technology choices: the React client is
hosted by Vercel, while the ASP.NET Core API and SQL Server database are hosted by
MonsterASP.NET. No production credentials belong in this repository.

### Frontend — Vercel

Import this GitHub repository into Vercel with these project settings:

- Root Directory: `React/my-app`
- Framework Preset: Vite
- Build Command: `npm run build`
- Output Directory: `dist`
- Install Command: `npm install` (Vercel's default is also suitable)

Set this Vercel environment variable for Production and any Preview environment
that should call the hosted API:

```env
VITE_API_URL=https://skillmatch-api.runasp.net
```

Use the real MonsterASP.NET HTTPS hostname. The frontend normalizes this origin and
adds `/api`, so the variable may also contain a URL that already ends in `/api`.
Local development continues to use `http://localhost:5095` through the committed
non-secret `.env.development`; copy `React/my-app/.env.example` to `.env.local` only
when an explicit local override is needed. The current UI does not use path-based
client-side routing, so no Vercel SPA rewrite is required.

### Backend — MonsterASP.NET

Create a .NET 10 website and an MSSQL database in the MonsterASP.NET control panel.
Publish the API from the repository root with:

```powershell
dotnet publish Asp.net/asp.net/asp.net/asp.net.csproj -c Release -o artifacts/monsterasp --no-self-contained
```

Deploy the contents of `artifacts/monsterasp` to the website root using the
MonsterASP.NET WebDeploy profile (recommended) or SFTP. The generated `web.config`
starts the framework-dependent application as `dotnet SkillMatch.API.dll` under
IIS. Enable the free HTTPS certificate before connecting the Vercel frontend.

Configure these values in the hosting control panel or the deployed application's
environment; `Asp.net/asp.net/asp.net/.env.example` is a reference and is not loaded
automatically by ASP.NET Core:

```env
ConnectionStrings__DefaultConnection=<MonsterASP.NET MSSQL connection string>
Jwt__Key=<at least 32 characters of random secret material>
Jwt__Issuer=SkillMatch.API
Jwt__Audience=SkillMatch.Web
Jwt__ExpirationMinutes=60
Cors__AllowedOrigins__0=https://skillmatch.vercel.app
ResumeStorage__Directory=Storage/Resumes
Database__MigrateOnStartup=false
```

Replace both example domains with the assigned production hostnames. Add further
allowed origins using `Cors__AllowedOrigins__1`, `__2`, and so on. Origins must be
exact HTTPS origins without paths or a trailing slash. Keep automatic migrations
disabled on the shared host after initial setup; Docker Compose deliberately enables
them for its single API instance.

Production returns generic problem responses for unhandled exceptions, enables
HSTS, and keeps Swagger limited to Development. Check the deployed API and database
with `GET https://<api-host>/api/system/status`.

### Database — MonsterASP.NET MSSQL

Copy the SQL-authentication connection string from the MonsterASP.NET control panel
into `ConnectionStrings__DefaultConnection`. Do not replace the committed local
development connection string with this hosted credential or place it in any
tracked `.env` file. Apply the existing migrations once from
`Asp.net/asp.net/asp.net`:

```powershell
dotnet ef database update --connection "<MonsterASP.NET MSSQL connection string>"
```

The connection string can remain in shell history, so prefer Visual Studio's
WebDeploy database/migration settings when handling a real credential on a shared
machine. The migration files remain committed and should not be regenerated.

### CORS and resume-file persistence

The API accepts only configured origins. Local development allows
`http://localhost:5173`; the production environment variable replaces that entry
with the exact Vercel origin.

Resume files remain private under the configured `Storage/Resumes/{userId}` path,
and the directory is created automatically. This is local website storage rather
than object storage: do not enable a WebDeploy option that removes additional files
at the destination, and back up the folder before redeployments. A hosting reset,
account suspension, quota limit, or destructive deployment can otherwise remove
uploaded resumes even while their database records remain. Confirm the free-plan
storage and file-retention behavior in the MonsterASP.NET control panel before
sharing the application publicly.

## Authentication API

- `POST /api/auth/register` creates an account and returns a JWT.
- `POST /api/auth/login` verifies the password and returns a JWT.
- `GET /api/users/me` returns the current user and requires a Bearer token.
- Logout is client-side: the React app removes the stored JWT.

## Resume API

- `GET /api/resumes` lists only the signed-in user's resumes.
- `POST /api/resumes` accepts one PDF or DOCX as multipart form data (maximum 5 MB).
- `GET /api/resumes/{id}` returns extracted text for an owned resume.
- `GET /api/resumes/{id}/file` securely downloads an owned document.
- `POST /api/resumes/{id}/extract-skills` re-runs skill detection for an owned resume.
- `DELETE /api/resumes/{id}` deletes the database record and stored file.

Resumes with match history are protected from deletion so saved analyses remain
valid.

Uploaded documents are stored below `Storage/Resumes/{userId}` and are not exposed
as public static files. PdfPig extracts PDF text and the Open XML SDK extracts
modern Word (`.docx`) text locally. Legacy `.doc` files must first be saved as
`.docx`. Image-only/scanned PDFs can be stored, but OCR is outside the current phase.

## Skills API

- `GET /api/skills` returns the seeded 35-skill catalog.
- `POST /api/skills/extract` extracts known skills from supplied text, including a
  job description preview before Phase 5 adds saved jobs.

Detection is case-insensitive, boundary-aware, and understands selected aliases
such as `MSSQL`, `K8s`, `EF Core`, `RESTful API`, and `Amazon Web Services`.
Matching also applies directional compatibility rules where a more specific skill
demonstrates a broader requirement. For example, `ASP.NET Core` or `Entity
Framework Core` on a resume satisfies a job's `.NET` requirement. The reverse is
not assumed: `.NET` alone does not satisfy an `ASP.NET Core` requirement. Likewise,
`Tailwind CSS` demonstrates generic `CSS`, while a Tailwind phrase is not counted
twice as two separate requirements unless plain CSS is mentioned independently.

## Jobs API

- `GET /api/jobs` lists the signed-in user's saved jobs.
- `POST /api/jobs` saves a job and extracts its required skills.
- `GET /api/jobs/{id}` returns the description and detected skills.
- `PUT /api/jobs/{id}` updates the job and refreshes its skills.
- `DELETE /api/jobs/{id}` deletes an owned job without match history.

All job queries are scoped to the authenticated user. Jobs with future match
history are protected from deletion so analysis records cannot become orphaned.

## Matching API

- `POST /api/match/analyze` compares an owned resume with an owned job and saves
  the result.
- `GET /api/match/history` lists the signed-in user's saved analyses.
- `GET /api/match/{id}` returns one saved analysis with its skill breakdown.

The score is `(matched required skills / total required skills) × 100`, rounded to
two decimal places. A job with no detected required skills scores `0`. Each result
stores a required-skill snapshot, so later job edits do not rewrite its matched
and missing skill history. Matching endpoints and records are user-scoped.

The React result view classifies the score into strong, promising, partial, or
early alignment. It shows matched and missing skills with their categories,
category-level coverage, the original analysis time, and a selectable recent
history. These labels explain the deterministic score; they do not use an external
AI service or generate recommendations.

Saved match results are historical snapshots. Run the analysis again after a
matching-rule change to create a result using the latest compatibility rules.

## Dashboard API

- `GET /api/dashboard` returns the signed-in user's resume and job totals, distinct
  jobs analyzed, analysis count, average and highest score, five most common
  missing skills, unique resume-skill distribution, and five recent analyses.

The Phase 8 React dashboard renders these aggregates as responsive statistic cards,
category bars, a recurring-gaps chart, and a recent-activity table. Empty accounts
receive zero totals and empty collections rather than fabricated data.

## Recommendation API

- `GET /api/match/{id}/recommendations` returns a learning plan for the missing
  skills in one owned, saved analysis.

The recommendation catalog contains one curated action for each of the 35 seeded
skills. Plans are generated from the immutable matched/missing skill snapshot,
exclude skills that were already matched, and sort High priority before Medium and
Low. A complete match returns an empty recommendation list. The frontend presents
the plan directly beneath the match result.

## Data model

The initial schema includes Users, Resumes, Skills, ResumeSkills, Jobs, JobSkills,
JobMatches, and SkillRecommendations. Authentication and feature endpoints are
implemented incrementally by project phase. `JobMatchSkills` stores the Phase 6
matched/missing skill snapshot for each analysis.

## Project status

All 10 planned development phases are complete. The remaining work is operational:
choose a hosting provider, configure production secrets and TLS, run the container
stack in that target environment, and add real product screenshots to the portfolio.
