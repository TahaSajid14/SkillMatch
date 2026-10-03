using SkillMatch.API.Models;

namespace SkillMatch.API.Data;

public static class RecommendationSeedData
{
    public static readonly SkillRecommendation[] All =
    [
        Create(1, 1, "Build a React feature", "Practice components, hooks, state, forms, and API integration by shipping one small production-style feature.", LearningPriority.Medium),
        Create(2, 2, "Strengthen JavaScript fundamentals", "Review modern syntax, closures, promises, array methods, modules, and browser debugging through short exercises.", LearningPriority.Medium),
        Create(3, 3, "Use TypeScript in a real feature", "Add strict types to components and API contracts, then remove unsafe any values and handle nullable data.", LearningPriority.Medium),
        Create(4, 4, "Practice semantic HTML", "Build an accessible page using meaningful landmarks, forms, headings, labels, and keyboard-friendly controls.", LearningPriority.Low),
        Create(5, 5, "Improve responsive CSS", "Recreate a responsive interface using Grid, Flexbox, custom properties, and mobile-first breakpoints.", LearningPriority.Low),
        Create(6, 6, "Deepen practical C#", "Practice collections, LINQ, async/await, records, nullable reference types, and dependency injection in a small service.", LearningPriority.Medium),
        Create(7, 7, "Build an ASP.NET Core API", "Create authenticated CRUD endpoints with DTO validation, dependency injection, logging, and consistent error responses.", LearningPriority.High),
        Create(8, 8, "Model data with EF Core", "Build relationships, migrations, projections, and tracked versus no-tracking queries against a relational database.", LearningPriority.Medium),
        Create(9, 9, "Design a REST API", "Practice resource-oriented routes, status codes, validation, pagination, and clear request and response contracts.", LearningPriority.Medium),
        Create(10, 10, "Practice SQL Server querying", "Write joins, grouping queries, indexes, and execution-plan checks using a realistic application schema.", LearningPriority.High),
        Create(11, 11, "Learn MySQL essentials", "Practice schema design, joins, aggregation, indexing, and connecting MySQL to an application.", LearningPriority.Medium),
        Create(12, 12, "Learn PostgreSQL essentials", "Practice data types, joins, indexes, query plans, and application integration with PostgreSQL.", LearningPriority.Medium),
        Create(13, 13, "Model data in MongoDB", "Practice document design, indexes, aggregation pipelines, and choosing when embedding or referencing fits best.", LearningPriority.Medium),
        Create(14, 14, "Use Git with confidence", "Practice feature branches, focused commits, rebasing, conflict resolution, and reviewing diffs before merging.", LearningPriority.Medium),
        Create(15, 15, "Create a strong GitHub workflow", "Publish a documented project with issues, pull requests, code review, and an automated validation workflow.", LearningPriority.Low),
        Create(16, 16, "Containerize an application", "Write a multi-stage Dockerfile, configure environment variables, and run the frontend, API, and database locally.", LearningPriority.High),
        Create(17, 17, "Learn Kubernetes fundamentals", "Deploy a container using Deployments, Services, ConfigMaps, Secrets, health probes, and resource limits.", LearningPriority.High),
        Create(18, 18, "Deploy a project to Azure", "Deploy an ASP.NET Core application and database, configure secrets, logs, health checks, and a repeatable release process.", LearningPriority.High),
        Create(19, 19, "Deploy a project to AWS", "Practice hosting an application with managed compute and database services, secure configuration, logging, and cost limits.", LearningPriority.High),
        Create(20, 20, "Adopt a unit-testing workflow", "Write focused tests for business rules, edge cases, and failures, then run them automatically before every merge.", LearningPriority.High),
        Create(21, 21, "Implement JWT authentication safely", "Practice token validation, expiration, authorization policies, secure key configuration, and frontend session handling.", LearningPriority.High),
        Create(22, 22, "Build an Angular feature", "Practice standalone components, services, reactive forms, routing, and typed HTTP calls in a small application.", LearningPriority.Medium),
        Create(23, 23, "Build a Vue feature", "Practice components, Composition API, reactive state, routing, forms, and API integration.", LearningPriority.Medium),
        Create(24, 24, "Build a Node.js service", "Create a validated REST service with async error handling, authentication, persistence, logging, and tests.", LearningPriority.Medium),
        Create(25, 25, "Strengthen practical Python", "Practice data structures, functions, type hints, virtual environments, testing, and a small API or automation project.", LearningPriority.Medium),
        Create(26, 26, "Strengthen practical Java", "Practice collections, streams, exceptions, testing, and dependency injection in a small backend service.", LearningPriority.Medium),
        Create(27, 27, "Test .NET code with xUnit", "Write unit and integration tests using fixtures, assertions, mocks where appropriate, and WebApplicationFactory.", LearningPriority.High),
        Create(28, 28, "Build a CI/CD pipeline", "Automate restore, lint, build, test, artifact creation, and controlled deployment for a portfolio project.", LearningPriority.High),
        Create(29, 29, "Add Redis caching", "Cache an expensive read path, choose an expiration strategy, handle invalidation, and measure the performance change.", LearningPriority.Medium),
        Create(30, 30, "Build an event-driven workflow", "Use RabbitMQ to publish and consume durable messages with acknowledgements, retries, and idempotent handlers.", LearningPriority.Medium),
        Create(31, 31, "Strengthen the .NET platform basics", "Practice the CLI, project structure, configuration, dependency injection, async code, testing, and deployment.", LearningPriority.High),
        Create(32, 32, "Build a Next.js page", "Practice routing, server and client components, data fetching, metadata, loading states, and deployment.", LearningPriority.Medium),
        Create(33, 33, "Build a Tailwind design system", "Create reusable responsive components with consistent spacing, typography, colors, states, and accessibility.", LearningPriority.Low),
        Create(34, 34, "Design a GraphQL API", "Practice schemas, queries, mutations, validation, resolver performance, authorization, and avoiding N+1 queries.", LearningPriority.Medium),
        Create(35, 35, "Practice Linux for development", "Use the shell, permissions, processes, services, logs, networking tools, and environment variables on a small server.", LearningPriority.Medium)
    ];

    private static SkillRecommendation Create(
        int id,
        int skillId,
        string title,
        string description,
        LearningPriority priority) => new()
        {
            Id = id,
            SkillId = skillId,
            Title = title,
            Description = description,
            LearningPriority = priority
        };
}
