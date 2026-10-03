using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SkillMatch.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedRecommendations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "SkillRecommendations",
                columns: new[] { "Id", "Description", "LearningPriority", "SkillId", "Title" },
                values: new object[,]
                {
                    { 1, "Practice components, hooks, state, forms, and API integration by shipping one small production-style feature.", "Medium", 1, "Build a React feature" },
                    { 2, "Review modern syntax, closures, promises, array methods, modules, and browser debugging through short exercises.", "Medium", 2, "Strengthen JavaScript fundamentals" },
                    { 3, "Add strict types to components and API contracts, then remove unsafe any values and handle nullable data.", "Medium", 3, "Use TypeScript in a real feature" },
                    { 4, "Build an accessible page using meaningful landmarks, forms, headings, labels, and keyboard-friendly controls.", "Low", 4, "Practice semantic HTML" },
                    { 5, "Recreate a responsive interface using Grid, Flexbox, custom properties, and mobile-first breakpoints.", "Low", 5, "Improve responsive CSS" },
                    { 6, "Practice collections, LINQ, async/await, records, nullable reference types, and dependency injection in a small service.", "Medium", 6, "Deepen practical C#" },
                    { 7, "Create authenticated CRUD endpoints with DTO validation, dependency injection, logging, and consistent error responses.", "High", 7, "Build an ASP.NET Core API" },
                    { 8, "Build relationships, migrations, projections, and tracked versus no-tracking queries against a relational database.", "Medium", 8, "Model data with EF Core" },
                    { 9, "Practice resource-oriented routes, status codes, validation, pagination, and clear request and response contracts.", "Medium", 9, "Design a REST API" },
                    { 10, "Write joins, grouping queries, indexes, and execution-plan checks using a realistic application schema.", "High", 10, "Practice SQL Server querying" },
                    { 11, "Practice schema design, joins, aggregation, indexing, and connecting MySQL to an application.", "Medium", 11, "Learn MySQL essentials" },
                    { 12, "Practice data types, joins, indexes, query plans, and application integration with PostgreSQL.", "Medium", 12, "Learn PostgreSQL essentials" },
                    { 13, "Practice document design, indexes, aggregation pipelines, and choosing when embedding or referencing fits best.", "Medium", 13, "Model data in MongoDB" },
                    { 14, "Practice feature branches, focused commits, rebasing, conflict resolution, and reviewing diffs before merging.", "Medium", 14, "Use Git with confidence" },
                    { 15, "Publish a documented project with issues, pull requests, code review, and an automated validation workflow.", "Low", 15, "Create a strong GitHub workflow" },
                    { 16, "Write a multi-stage Dockerfile, configure environment variables, and run the frontend, API, and database locally.", "High", 16, "Containerize an application" },
                    { 17, "Deploy a container using Deployments, Services, ConfigMaps, Secrets, health probes, and resource limits.", "High", 17, "Learn Kubernetes fundamentals" },
                    { 18, "Deploy an ASP.NET Core application and database, configure secrets, logs, health checks, and a repeatable release process.", "High", 18, "Deploy a project to Azure" },
                    { 19, "Practice hosting an application with managed compute and database services, secure configuration, logging, and cost limits.", "High", 19, "Deploy a project to AWS" },
                    { 20, "Write focused tests for business rules, edge cases, and failures, then run them automatically before every merge.", "High", 20, "Adopt a unit-testing workflow" },
                    { 21, "Practice token validation, expiration, authorization policies, secure key configuration, and frontend session handling.", "High", 21, "Implement JWT authentication safely" },
                    { 22, "Practice standalone components, services, reactive forms, routing, and typed HTTP calls in a small application.", "Medium", 22, "Build an Angular feature" },
                    { 23, "Practice components, Composition API, reactive state, routing, forms, and API integration.", "Medium", 23, "Build a Vue feature" },
                    { 24, "Create a validated REST service with async error handling, authentication, persistence, logging, and tests.", "Medium", 24, "Build a Node.js service" },
                    { 25, "Practice data structures, functions, type hints, virtual environments, testing, and a small API or automation project.", "Medium", 25, "Strengthen practical Python" },
                    { 26, "Practice collections, streams, exceptions, testing, and dependency injection in a small backend service.", "Medium", 26, "Strengthen practical Java" },
                    { 27, "Write unit and integration tests using fixtures, assertions, mocks where appropriate, and WebApplicationFactory.", "High", 27, "Test .NET code with xUnit" },
                    { 28, "Automate restore, lint, build, test, artifact creation, and controlled deployment for a portfolio project.", "High", 28, "Build a CI/CD pipeline" },
                    { 29, "Cache an expensive read path, choose an expiration strategy, handle invalidation, and measure the performance change.", "Medium", 29, "Add Redis caching" },
                    { 30, "Use RabbitMQ to publish and consume durable messages with acknowledgements, retries, and idempotent handlers.", "Medium", 30, "Build an event-driven workflow" },
                    { 31, "Practice the CLI, project structure, configuration, dependency injection, async code, testing, and deployment.", "High", 31, "Strengthen the .NET platform basics" },
                    { 32, "Practice routing, server and client components, data fetching, metadata, loading states, and deployment.", "Medium", 32, "Build a Next.js page" },
                    { 33, "Create reusable responsive components with consistent spacing, typography, colors, states, and accessibility.", "Low", 33, "Build a Tailwind design system" },
                    { 34, "Practice schemas, queries, mutations, validation, resolver performance, authorization, and avoiding N+1 queries.", "Medium", 34, "Design a GraphQL API" },
                    { 35, "Use the shell, permissions, processes, services, logs, networking tools, and environment variables on a small server.", "Medium", 35, "Practice Linux for development" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SkillRecommendations",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SkillRecommendations",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "SkillRecommendations",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "SkillRecommendations",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "SkillRecommendations",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "SkillRecommendations",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "SkillRecommendations",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "SkillRecommendations",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "SkillRecommendations",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "SkillRecommendations",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "SkillRecommendations",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "SkillRecommendations",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "SkillRecommendations",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "SkillRecommendations",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "SkillRecommendations",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "SkillRecommendations",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "SkillRecommendations",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "SkillRecommendations",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "SkillRecommendations",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "SkillRecommendations",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "SkillRecommendations",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "SkillRecommendations",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "SkillRecommendations",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "SkillRecommendations",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "SkillRecommendations",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "SkillRecommendations",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "SkillRecommendations",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "SkillRecommendations",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "SkillRecommendations",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "SkillRecommendations",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "SkillRecommendations",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "SkillRecommendations",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "SkillRecommendations",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "SkillRecommendations",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "SkillRecommendations",
                keyColumn: "Id",
                keyValue: 35);
        }
    }
}
