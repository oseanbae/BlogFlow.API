# BlogFlow API
![License](https://img.shields.io/badge/license-MIT-blue.svg)
## About

BlogFlow is a RESTful blog platform API built with ASP.NET Core 10 and PostgreSQL.

It demonstrates backend engineering practices beyond basic tutorials, including authentication, role-based access control, domain modeling, content lifecycle management, and API design patterns used in production systems.

The project is implemented as a layered monolith with rich domain models inspired by Domain-Driven Design.

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
- State transitions: Draft → Published → Archived
  with explicit Unpublish and MoveToDraft paths
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
- Uncategorized is a protected fallback — cannot be deleted

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

**Rich Domain Models** — entities own their own validation and state transitions. `Post.Publish()`, `User.SoftDelete()`, `RefreshToken.Revoke()` are domain methods, not service-level property assignments.

**UserContext over IHttpContextAccessor in services** — controllers extract identity from JWT claims via `ICurrentUserService` and pass a `UserContext` object to services. Services never touch `HttpContext` directly.

**Soft delete via global query filters** — deleted records are invisible by default. Admins bypass filters via `IgnoreQueryFilters()`.

**Repository pattern with IQueryable** — repositories return `IQueryable<T>` so services can compose filters and projections before hitting the database. No N+1 queries.

**Defense in depth** — role checks exist at both controller (`[Authorize(Roles)]`) and service layer (`ValidateOwnership()`). The controller blocks at middleware level; the service guards against direct invocation.

**Post visibility** — `ApplyReadVisibility()` runs before any client filters, ensuring readers never see draft or archived content regardless of query params.

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




## API Documentation

Interactive docs available at `/scalar/v1` when running in Development.

### Auth
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | /api/v1/auth/register | None | Register new account |
| POST | /api/v1/auth/login | None | Login, returns tokens |
| POST | /api/v1/auth/refresh | None | Rotate refresh token |
| POST | /api/v1/auth/revoke | Bearer | Revoke refresh token |

### Posts
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | /api/v1/posts | None | Get paginated posts |
| GET | /api/v1/posts/{id} | None | Get post by ID |
| POST | /api/v1/posts | Author, Admin | Create post |
| PUT | /api/v1/posts/{id} | Author, Admin | Update post (draft only) |
| DELETE | /api/v1/posts/{id} | Author, Admin | Soft delete post |
| PATCH | /api/v1/posts/{id}/publish | Author, Admin | Publish post |
| PATCH | /api/v1/posts/{id}/unpublish | Author, Admin | Unpublish post |
| PATCH | /api/v1/posts/{id}/archive | Author, Admin | Archive post |
| PATCH | /api/v1/posts/{id}/draft | Author, Admin | Move to draft |
| PATCH | /api/v1/posts/{id}/restore | Admin | Restore soft deleted |
| DELETE | /api/v1/posts/{id}/hard | Admin | Permanent delete |

### Comments
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | /api/v1/posts/{id}/comments | None | Get comments for post |
| GET | /api/v1/posts/{id}/comments/{cid} | None | Get comment by ID |
| POST | /api/v1/posts/{id}/comments | Bearer | Create comment |
| PATCH | /api/v1/posts/{id}/comments/{cid} | Bearer | Update comment |
| DELETE | /api/v1/posts/{id}/comments/{cid} | Bearer | Soft delete comment |

### Categories
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | /api/v1/categories | None | Get all categories |
| GET | /api/v1/categories/{id} | None | Get category by ID |
| POST | /api/v1/categories | Admin | Create category |
| PATCH | /api/v1/categories/{id} | Admin | Rename category |
| DELETE | /api/v1/categories/{id} | Admin | Delete, reassign posts |

### Tags
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | /api/v1/tags | None | Get all tags |
| GET | /api/v1/tags/{id} | None | Get tag by ID |
| POST | /api/v1/tags | Author, Admin | Create tag |
| DELETE | /api/v1/tags/{id} | Admin | Delete tag |

### Users
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | /api/v1/users/{id} | None | Get public profile |
| GET | /api/v1/users/me | Bearer | Get own profile |
| PUT | /api/v1/users/me | Bearer | Update profile |
| PATCH | /api/v1/users/me/password | Bearer | Change password |
| DELETE | /api/v1/users/me | Bearer | Delete own account |

### Admin
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | /api/v1/admin/users | Admin | List all users |
| PATCH | /api/v1/admin/users/{id}/role | Admin | Change user role |
| DELETE | /api/v1/admin/users/{id} | Admin | Soft delete user |
| POST | /api/v1/admin/users/{id}/restore | Admin | Restore user |
| GET | /api/v1/admin/users/stats | Admin | User statistics |


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
See the [LICENSE](LICENSE) file for details.