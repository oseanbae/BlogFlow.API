# BlogFlow API
![License](https://img.shields.io/badge/license-MIT-blue.svg)
## About

BlogFlow is a RESTful blog platform API built with ASP.NET Core 10 and PostgreSQL.

It demonstrates authentication, authorization, domain modeling, and API design patterns using a layered monolith architecture with domain-focused design.

---

## Features

### Authentication & Security
- Register, login, token refresh, and logout (token revocation)
- JWT access tokens (15 min) with refresh token rotation (7 days)
- Fixed-window rate limiting on all auth endpoints
- Passwords hashed with BCrypt
> Security: if a revoked refresh token is reused, all sessions for that user are immediately invalidated.

### Posts
- Full CRUD with draft/publish/archive state machine
- State transitions: Draft → Published → Archived, with support for Unpublish (Published → Draft) and Archive operations.
- Visibility rules enforced server-side:
  - Anonymous / Reader → Published posts only
  - Author → Published + their own Drafts and Archived
  - Admin → all posts including soft-deleted
- Filter by category, tag, author, keyword, and state
- Soft delete, restore (admin), and hard delete (admin, 
  requires soft delete first)

### Comments
- Full CRUD with soft delete
- Authors can delete comments on their own posts (moderation)
- Comment visibility gated by parent post visibility —
  cannot read comments on posts you cannot see
- Filter by author, date range (createdAfter, createdBefore)

### Categories
- Full CRUD with admin-only write access
- Safe delete with automatic post reassignment to Uncategorized
- Uncategorized is a protected system category used as a fallback for reassignment and cannot be deleted.

### Tags
- Full CRUD with admin-only delete
- Authors and admins can create tags

### Users
- Separate public profile (`GET /users/{id}`) — username, role, joined date
- Private profile (`GET /users/me`) — includes email and updatedAt
- Update profile, change password, delete own account
- Duplicate username/email checked before update

### Admin
- Paginated user list with filters (role, deleted status, search, date)
- Role management, soft delete, and restore
- User statistics: total, active, deleted, by role, 
  new this week and this month

### Infrastructure
- Global exception handling with structured error responses
- Soft delete via EF Core global query filters
- Pagination on posts, comments, and users
- Structured logging via Serilog (console + JSON file sink)
- FluentValidation on all input DTOs
- CORS policy for local development

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 10 |
| Language | C# 13 |
| ORM | Entity Framework Core 10 |
| Database | PostgreSQL |
| Authentication | JWT Bearer + Refresh Tokens |
| Validation | FluentValidation |
| Logging | Serilog |
| Password Hashing | BCrypt.Net |
| API Documentation & Testing | Scalar (OpenAPI), Postman |
| Naming Convention | EFCore.NamingConventions (snake_case) |

---

## Architecture

BlogFlow follows a **Layered Monolith** architecture with **Rich Domain Models** inspired by Domain-Driven Design (DDD).


```

HTTP Request  
→ Controllers (auth + HTTP boundary)  
→ Services (business logic layer)  
→ Repositories (data access layer)  
→ AppDbContext (EF Core mapping)  
→ PostgreSQL

```

### Key Design Decisions

- **Rich domain models** — entities encapsulate state changes and validation instead of relying on service-level property mutation.
- **UserContext abstraction** — authentication data is extracted at the controller layer and passed explicitly to services, avoiding direct HttpContext dependency.
- **Soft delete via global query filters** — deleted entities are excluded by default and only visible to administrators using explicit override queries.
- **Repository pattern with IQueryable** — repositories expose queryable data to allow composition of filters and projections before execution.
- **Defense in depth authorization** — critical actions are protected at both controller and service layers to prevent bypass through internal calls.
- **Server-side visibility enforcement** — post visibility rules are applied in query logic to ensure unauthorized data is never returned, regardless of client filters.

---

## Project Structure


```

BlogFlow.API/
├── Constants/          — CategoryConstants (protected category IDs)
├── Controllers/        — HTTP layer, route definitions
├── Data/
│   ├── AppDbContext.cs
│   └── Seeding/        — ISeeder, DatabaseSeeder, seed data
├── Domain/
│   ├── Entities/       — Post, User, Comment, Category, Tag, etc.
│   └── QueryParams/    — PostQueryParams, CommentQueryParams, etc.
├── DTOs/               — request and response objects per feature
├── Exceptions/         — AppException hierarchy, custom exceptions
├── Extensions/         — JWT, rate limiting, service registration
├── Helper/             — SlugHelper
├── Middleware/         — GlobalExceptionHandler
├── Migrations/         — EF Core migrations
├── QueryExtensions/    — AsDTO(), AsAdminDTO(), ToPaginatedResultAsync()
├── Repositories/       — data access layer + interfaces
├── Services/           — business logic layer + interfaces
├── Settings/           — JwtSettings, RateLimitOptions
└── Validators/         — FluentValidation validators per DTO
```
---

## Core Workflows

### 1. Authentication & Session Management
This workflow handles user identity, session lifecycle, and security enforcement.

#### Flow
* User registers an account
* User logs in with credentials
* System issues:
  * JWT access token (short-lived)
  * Refresh token (long-lived)
* Client uses access token for API requests
* When expired, refresh token is used to rotate session
* Logout revokes refresh token
* Reuse of revoked refresh token triggers full session invalidation

#### Covers Endpoints
* `/auth/register`
* `/auth/login`
* `/auth/refresh`
* `/auth/revoke`

#### Key Behavior
* **Stateless JWT authentication** paired with refresh token rotation
* **Reuse detection** for refresh token reuse detection for session invalidation
* **Role claims** embedded directly within the JWT for authorization

---

### 2. Post Lifecycle Management
This workflow manages the full content lifecycle from creation to archival.

#### Flow
* Author creates post in **Draft** state
* Post can be updated while in **Draft** state
* Author publishes post $\rightarrow$ becomes publicly visible (**Published** state)
* Post can be:
  * **Unpublished** (moved back to Draft)
  * **Archived** (hidden from public but preserved for author)
* Admin can restore or hard delete soft-deleted posts

#### Covers Endpoints
* `/posts` (CRUD)
* `/posts/{id}/publish`
* `/posts/{id}/unpublish`
* `/posts/{id}/archive`
* `/posts/{id}/draft`
* `/posts/{id}/restore`
* `/posts/{id}/hard`

#### Key Behavior
* **Strict state machine enforcement** (Draft $\rightarrow$ Published $\rightarrow$ Archived)
* **Server-side visibility enforcement** instead of basic client-side filtering
* **Soft delete safety net** with a dedicated admin recovery path
* **Role-based write access** control separation (Author vs. Admin)

---

### 3. Content Interaction (Comments & Visibility Rules)
This workflow handles how users interact with published content.

#### Flow
* User views a post (succeeds only if it matches their role visibility)
* User retrieves comments for that specific post
* User can:
  * Create comments (authenticated users only)
  * Edit their own comments
  * Delete their own comments
* Post author can moderate and delete comments on their own posts
* Comments are automatically hidden if the parent post visibility is restricted

#### Covers Endpoints
* `/posts/{id}`
* `/posts/{id}/comments`
* `/posts/{id}/comments/{cid}`

#### Key Behavior
* **Cascading visibility rules:** Comment visibility depends entirely on parent post visibility
* **Ownership-based authorization layers** (User vs. Post Owner vs. Admin)
* **Soft delete implementations** for community moderation safety
* **Prevention of orphaned content** exposure across the platform

---

### 4. Administration & System Management

This workflow handles platform-level control and monitoring.

#### Flow
- Admin accesses system management endpoints
- Admin manages users, roles, and system data integrity
- System enforces constraints like protected categories and safe deletions

#### Covers Endpoints
- `/admin/users`
- `/admin/users/{id}/role`
- `/admin/users/{id}/restore`
- `/admin/users/stats`
- `/categories` (admin write operations)
- `/tags` (admin delete operations)

#### Key Behavior
- Role-based access control enforced at controller and service level
- System protects core entities like “Uncategorized”
- Soft-deleted data can be restored via admin endpoints

## Getting Started

### Prerequisites

* .NET 10 SDK
* PostgreSQL
* dotnet-ef CLI
* dotnet user-secrets CLI

---

### Configuration

This application uses `dotnet user-secrets` for local development and environment variables in production.

Required values:
- Database connection string
- JWT secret

### 1. Clone the repository

```bash
git clone https://github.com/oseanbae/BlogFlow.API.git
cd BlogFlow.API
```

---

### 2. Install dependencies

```bash
dotnet restore
dotnet build
```

---

### 3. Configure secrets

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=blogflow_db;Username=postgres;Password=YOUR_PASSWORD"
dotnet user-secrets set "JwtSettings:Secret" "YOUR_RANDOM_32+_CHAR_SECRET"
```

---

### 4. Run database migrations

```bash
dotnet ef database update
```

---

### 5. Run the API

```bash
dotnet run --launch-profile http
```

---

### Application URLs

* API: http://localhost:5065
* Scalar Docs: http://localhost:5065/scalar/v1

---

### Notes

* Seed data is automatically applied on first run in Development
* Ensure PostgreSQL is running before starting the application

---
## Environment Variables

Copy `appsettings.template.json` as a reference. All secrets are managed via `dotnet user-secrets` (for development) and environment variables (for production). They are never committed to git.

| Key | Description | Example |
| --- | --- | --- |
| `ConnectionStrings:DefaultConnection` | PostgreSQL connection string | `Host=localhost;Port=5432;Database=blogflow_db;Username=postgres;Password=your_password` |
| `JwtSettings:Secret` | HMAC-SHA256 signing key (32+ chars, random) | `REPLACE_WITH_LONG_RANDOM_SECRET_32+_CHARS` |
| `JwtSettings:Issuer` | JWT issuer claim | `BlogFlowAPI` |
| `JwtSettings:Audience` | JWT audience claim | `BlogFlowClient` |
| `JwtSettings:AccessTokenExpiryMinutes` | Access token lifetime in minutes | `15` |
| `JwtSettings:RefreshTokenExpiryDays` | Refresh token lifetime in days | `7` |
| `RateLimiting:Register:MaxRequest` | Max register attempts per window | `10` |
| `RateLimiting:Register:WindowSecond` | Register rate limit window (seconds) | `300` |
| `RateLimiting:Login:MaxRequest` | Max login attempts per window | `10` |
| `RateLimiting:Login:WindowSecond` | Login rate limit window (seconds) | `60` |
| `RateLimiting:Refresh:MaxRequest` | Max refresh attempts per window | `10` |
| `RateLimiting:Refresh:WindowSecond` | Refresh rate limit window (seconds) | `300` |
| `RateLimiting:Revoke:MaxRequest` | Max revoke attempts per window | `10` |
| `RateLimiting:Revoke:WindowSecond` | Revoke rate limit window (seconds) | `300` |

## Database Seeding

Seed data is applied automatically on startup in Development. Seeders run in order:

| Order | Seeder | Data |
| --- | --- | --- |
| 1 | UserSeed | admin, author1, author2, reader1, reader2 |
| 2 | CategorySeed | Technology, Lifestyle, Sports, Travel, Food, Finance, Gaming, Education, Health + Uncategorized |
| 3 | TagSeed | AI, .NET, Cloud Computing, Minimalism, Productivity, and more |
| 4 | PostSeed | 9 posts across categories with mixed states (Published, Archived, Draft) |
| 5 | CommentSeed | Sample comments on posts |

> Seeders are idempotent — they skip if data already exists.

---

## Authentication Flow

BlogFlow uses a stateless JWT + refresh token rotation strategy:

```
1. POST /auth/register or /auth/login
   → returns accessToken (15min) + refreshToken (7 days)

2. Include accessToken in Authorization header
   → Authorization: Bearer <accessToken>

3. When accessToken expires, POST /auth/refresh
   → returns new accessToken + new refreshToken (rotation)
   → old refreshToken is revoked

4. POST /auth/revoke to logout
   → invalidates the refreshToken

Security: if a revoked refreshToken is reused,
all sessions for that user are immediately wiped.

```

---

## Known Limitations

* No email verification on register
* No password reset via email
* CORS configured for local development only (`localhost:3000`, `localhost:5173`)
* Rate limiting applies to auth endpoints only — write endpoints (posts, comments) are unprotected
* Hard delete requires soft delete first (intentional two-step safety)
* No caching — all queries hit the database directly
* Migrations run on startup (not suitable for multi-instance production)

---

## Roadmap

See [IMPROVEMENT.md](Docs/IMPROVEMENT.md) for planned V2 features and future improvements.

**Highlights:**

* Like / reaction system
* Full-text post search
* Email verification + password reset
* Admin reporting endpoints
* Docker + docker-compose
* Unit and integration tests
* Rate limiting on write endpoints
* Production CORS and HTTPS config

---

## License
This project is licensed under the MIT License.