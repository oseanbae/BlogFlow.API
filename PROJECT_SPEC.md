# BlogFlow API
*Project Specification — REST API · ASP.NET Core · Portfolio Project*

---

## 1. Project Overview

| Field | Details |
|---|---|
| **Project Name** | BlogFlow API |
| **Problem Statement** | Content creators need a structured platform to publish posts, manage comments, and control access — BlogFlow is a REST API that handles all of it. |
| **Target Users** | Developers building blog platforms, personal portfolio backends, or CMS-style applications. |
| **Why Portfolio-Worthy** | Every advanced skill has a natural reason to exist — auth because users need to log in, pagination because posts grow, logging because you need to track errors, Docker because you need to ship it. |

---

## 2. Scope

**In Scope**
- User registration and login with JWT + refresh tokens
- Role-based access (Admin, Author, Reader)
- Post CRUD with categories and tags
- Comment system
- Pagination, filtering, and full-text search on posts
- Input validation with FluentValidation
- Soft deletes with audit trails
- Rate limiting and security hardening
- Logging with Serilog
- Global exception middleware
- API versioning
- Admin dashboard / reporting endpoints
- Dockerized deployment

**Out of Scope**
- Frontend
- File / image uploads
- Email notifications
- Cloud services

---

## 3. System Design

**Core User Flow**
```
Register → Login → Get JWT → Create Post →
Reader comments → Admin moderates
```

**Entities**

| Entity | Fields |
|---|---|
| User | Id, Username, Email, PasswordHash, Role, IsDeleted, DeletedAt |
| Post | Id, Title, Body, AuthorId (FK), CategoryId (FK), CreatedAt, UpdatedAt, IsDeleted, DeletedAt |
| Category | Id, Name |
| Tag | Id, Name |
| PostTag | PostId (FK), TagId (FK) ← bridge table |
| Comment | Id, Body, PostId (FK), AuthorId (FK), CreatedAt, IsDeleted, DeletedAt |
| RefreshToken | Id, Token, UserId (FK), Expires, IsRevoked, CreatedAt |

**API Endpoints**

| Method | Route | Purpose | Auth |
|---|---|---|---|
| POST | /api/v1/auth/register | Register user | None |
| POST | /api/v1/auth/login | Login, get JWT + refresh token | None |
| POST | /api/v1/auth/refresh | Exchange refresh token for new JWT | None |
| POST | /api/v1/auth/revoke | Revoke refresh token | Authenticated |
| GET | /api/v1/posts | Get all posts (paginated + filtered) | None |
| GET | /api/v1/posts/{id} | Get one post | None |
| POST | /api/v1/posts | Create post | Author |
| PUT | /api/v1/posts/{id} | Update post | Author |
| DELETE | /api/v1/posts/{id} | Soft delete post | Admin |
| GET | /api/v1/posts/{id}/comments | Get post comments | None |
| POST | /api/v1/posts/{id}/comments | Add comment | Reader |
| DELETE | /api/v1/comments/{id} | Soft delete comment | Admin |
| GET | /api/v1/categories | Get all categories | None |
| POST | /api/v1/categories | Create category | Admin |
| GET | /api/v1/tags | Get all tags | None |
| POST | /api/v1/tags | Create tag | Admin |
| GET | /api/v1/users/{id}/posts | Get posts by author | None |
| GET | /api/v1/posts/search | Full-text search posts | None |
| GET | /api/v1/admin/stats | Get platform stats | Admin |

**Folder Structure**
```
BlogFlow.API/
├── Controllers/
│   ├── AuthController.cs
│   ├── PostsController.cs
│   ├── CommentsController.cs
│   ├── CategoriesController.cs
│   ├── TagsController.cs
│   ├── UsersController.cs
│   └── AdminController.cs
├── DTOs/
│   ├── Auth/
│   │   ├── RegisterRequestDTO.cs
│   │   ├── LoginRequestDTO.cs
│   │   ├── AuthResponseDTO.cs
│   │   └── RefreshTokenRequestDTO.cs
│   ├── Posts/
│   │   ├── PostCreateDTO.cs
│   │   ├── PostUpdateDTO.cs
│   │   ├── PostReadDTO.cs
│   │   └── PaginatedPostResultDTO.cs
│   ├── Comments/
│   │   ├── CommentCreateDTO.cs
│   │   └── CommentReadDTO.cs
│   ├── Categories/
│   │   ├── CategoryCreateDTO.cs
│   │   └── CategoryReadDTO.cs
│   ├── Tags/
│   │   ├── TagCreateDTO.cs
│   │   └── TagReadDTO.cs
│   ├── Users/
│   │   └── UserReadDTO.cs
│   ├── Admin/
│   │   └── AdminStatsDTO.cs
│   └── Common/
│       ├── PaginatedResultDTO.cs
│       └── ErrorResponseDTO.cs
├── Models/
│   ├── User.cs
│   ├── Post.cs
│   ├── Category.cs
│   ├── Tag.cs
│   ├── PostTag.cs
│   ├── Comment.cs
│   └── RefreshToken.cs
├── Data/
│   └── AppDbContext.cs
├── Repositories/
│   ├── Interfaces/
│   │   ├── IPostRepository.cs
│   │   ├── IUserRepository.cs
│   │   ├── ICommentRepository.cs
│   │   ├── ICategoryRepository.cs
│   │   ├── ITagRepository.cs
│   │   └── IRefreshTokenRepository.cs
│   ├── PostRepository.cs
│   ├── UserRepository.cs
│   ├── CommentRepository.cs
│   ├── CategoryRepository.cs
│   ├── TagRepository.cs
│   └── RefreshTokenRepository.cs
├── Services/
│   ├── Interfaces/
│   │   ├── IPostService.cs
│   │   ├── IAuthService.cs
│   │   ├── ICommentService.cs
│   │   ├── ICategoryService.cs
│   │   ├── ITagService.cs
│   │   ├── IAdminService.cs
│   │   └── ISearchService.cs
│   ├── PostService.cs
│   ├── AuthService.cs
│   ├── CommentService.cs
│   ├── CategoryService.cs
│   ├── TagService.cs
│   ├── AdminService.cs
│   └── SearchService.cs
├── Validators/
│   ├── RegisterRequestValidator.cs
│   ├── LoginRequestValidator.cs
│   ├── PostCreateValidator.cs
│   ├── PostUpdateValidator.cs
│   ├── CommentCreateValidator.cs
│   ├── CategoryCreateValidator.cs
│   └── TagCreateValidator.cs
├── Helpers/
│   ├── MappingHelper.cs
│   ├── PaginationHelper.cs
│   └── SoftDeleteHelper.cs
├── Middleware/
│   ├── ExceptionMiddleware.cs
│   └── RateLimitingMiddleware.cs
├── appsettings.json
├── Dockerfile
└── Program.cs

BlogFlow.Tests/
├── Services/
└── Repositories/
```
---

## 4. NuGet Packages

| Package                                       | Purpose                        | Install In |
| --------------------------------------------- | ------------------------------ | ---------- |
| Microsoft.AspNetCore.Authentication.JwtBearer | JWT validation middleware      | Phase 1    |
| BCrypt.Net-Next                               | Password hashing               | Phase 1    |
| Microsoft.EntityFrameworkCore.SqlServer       | EF Core SQL Server provider    | Phase 1    |
| Microsoft.EntityFrameworkCore.Tools           | Migrations CLI (dotnet ef)     | Phase 1    |
| AspNetCoreRateLimit                           | Rate limiting middleware       | Phase 2    |
| FluentValidation.AspNetCore                   | Input validation               | Phase 3    |
| Serilog.AspNetCore                            | Structured logging             | Phase 5    |
| Serilog.Sinks.File                            | Log to file                    | Phase 5    |
| Microsoft.AspNetCore.Mvc.Versioning           | API versioning                 | Phase 7    |
| xunit                                         | Unit test framework            | Phase 8    |
| Moq                                           | Mocking library for unit tests | Phase 8    |
| Microsoft.EntityFrameworkCore.InMemory        | In-memory DB for testing       | Phase 8    |

---

## 5. Environment Config

All sensitive and environment-specific values live in `appsettings.Development.json`, never hardcoded. Set this up in Phase 1 and reference it throughout.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BlogFlow;Trusted_Connection=True;"
  },
  "JwtSettings": {
    "Secret": "your-super-secret-key-min-32-chars",
    "Issuer": "BlogFlowAPI",
    "Audience": "BlogFlowClient",
    "AccessTokenExpiryMinutes": 15,
    "RefreshTokenExpiryDays": 7
  },
  "RateLimiting": {
    "LoginMaxRequests": 5,
    "LoginWindowSeconds": 60
  }
}
```

> ⚠️ Never commit `appsettings.Development.json` to source control if it contains real secrets. Add it to `.gitignore`.

---

## 6. Error Response Shape

`ErrorResponseDTO` is referenced in the folder structure and wired up in Phase 5's `ExceptionMiddleware`. Define the shape now so every error across all phases is consistent.

```json
{
  "statusCode": 400,
  "message": "Validation failed",
  "errors": [
    "Title is required",
    "Body must be at least 10 characters"
  ],
  "traceId": "0HN2D5L2V1234:00000001"
}
```

| Field      | Type     | Purpose                                  |
| ---------- | -------- | ---------------------------------------- |
| statusCode | int      | HTTP status code                         |
| message    | string   | Human-readable summary                   |
| errors     | string[] | Field-level errors (empty array if none) |
| traceId    | string   | Request trace ID for log correlation     |

---

## 7. Migration Strategy

Each phase that introduces new entities gets its own named migration. Never batch migrations across phases — if something breaks you'll know exactly which phase caused it.

| Migration Name        | Created In |
| --------------------- | ---------- |
| InitialCreate_Users   | Phase 1    |
| AddRefreshTokens      | Phase 2    |
| AddPostsAndCategories | Phase 3    |
| AddTagsAndComments    | Phase 4    |
| AddSoftDeleteFields   | Phase 6    |

**Command to create a migration:**

```bash
dotnet ef migrations add MigrationName --project BlogFlow.API
```

**Command to apply migrations:**

```bash
dotnet ef database update --project BlogFlow.API
```

---

## 8. Build Phases

---

### Phase 1 — Project Setup and Auth

*What you're building: User registration, login, JWT generation, and role-based access.*

**What You'll Learn**
- What JWTs are, how they're structured (header, payload, signature), and why they're stateless
- How password hashing works and why you never store plain-text passwords (BCrypt)
- What role-based authorization means and how ASP.NET Core enforces it with `[Authorize(Roles)]`
- How SQL Server connection strings work and how EF Core uses them to connect

**How You'll Apply It**
- Create the `User` entity and run your first migration
- Build `POST /api/v1/auth/register` with password hashing on input
- Build `POST /api/v1/auth/login` that validates credentials and returns a signed JWT
- Protect a test endpoint so it returns 401 without a token and 403 with the wrong role

**Definition of Done**
```
✓ Register returns 201 with user data
✓ Login returns JWT token
✓ Protected endpoint returns 401 without token
✓ Protected endpoint returns 403 with wrong role
```

---

### Phase 2 — Security Hardening and Refresh Tokens

*What you're building: Refresh token flow, token revocation, and rate limiting on auth endpoints.*

**What You'll Learn**
- Why short-lived JWTs alone aren't enough and what problem refresh tokens solve
- How refresh token rotation works — issuing, storing, and invalidating tokens in the DB
- What rate limiting is, why brute-force on `/auth/login` is a real threat, and how middleware stops it
- What security headers do (CORS, HTTPS enforcement) and why they matter even in development

**How You'll Apply It**
- Add the `RefreshToken` entity, repository, and migration
- Build `POST /api/v1/auth/refresh` and `POST /api/v1/auth/revoke` through `AuthService`
- Wire rate limiting middleware to auth routes with a request threshold
- Enforce HTTPS and configure security headers in `Program.cs`

**Definition of Done**
```
✓ Login returns both access token and refresh token
✓ Refresh endpoint returns new access token when given valid refresh token
✓ Revoked refresh token returns 401
✓ Auth endpoints return 429 after exceeding request threshold
```

---

### Phase 3 — Service Layer, Repository Pattern, and Validation

*What you're building: Refactor data access into repositories, business logic into services, and add input validation.*

**What You'll Learn**
- Why controllers shouldn't contain business logic and what the service layer's job actually is
- What the repository pattern is and why it decouples your data access from your business logic
- How dependency injection works with interfaces — and why you inject `IPostService` not `PostService`
- What FluentValidation is, how it works, and why it's better than Data Annotations for complex rules

**How You'll Apply It**
- Create `IPostRepository` / `PostRepository` and `IPostService` / `PostService`
- Wire all Post CRUD through the service → repository → DB chain
- Refactor controllers so they only handle HTTP concerns — no EF Core, no business rules
- Add validators for all create/update DTOs and register them in the DI pipeline

**Definition of Done**
```
✓ Controller calls service, not DbContext directly
✓ Service calls repository, not DbContext directly
✓ All Post endpoints work correctly
✓ Invalid request body returns 400 with field-level error messages
✓ Swapping repository implementation does not break service
```

---

### Phase 4 — Relationships, Tags, and Comments

*What you're building: Categories, tags (many-to-many), and comments.*

**What You'll Learn**
- How many-to-many relationships work in EF Core via a bridge table (`PostTag`)
- What `ThenInclude()` does and when you need it for nested navigation properties
- How to shape DTOs to return enriched responses (e.g. `CategoryName` and `Tags[]` inside a post)
- How to enforce ownership rules — only the post's author or an admin can modify it

**How You'll Apply It**
- Add `Category`, `Tag`, `PostTag`, and `Comment` entities with migrations
- Build `GET /api/v1/posts/{id}/comments` and `POST /api/v1/posts/{id}/comments`
- Build `GET /api/v1/users/{id}/posts`
- Return `CategoryName` and `Tags[]` in post responses using `ThenInclude()` and DTO mapping

**Definition of Done**
```
✓ Post response includes CategoryName and Tags[]
✓ Comment creation requires valid PostId
✓ Non-existent post returns 404
✓ Duplicate tag on post returns 409
```

---

### Phase 5 — Pagination, Filtering, Search, and Logging

*What you're building: Paginated post listing, filter by category/tag, full-text search, Serilog logging, and global exception middleware.*

**What You'll Learn**
- How `Skip()` and `Take()` implement pagination at the database level
- How query parameters map to filter logic in EF Core (`WHERE category = X AND tag = Y`)
- What full-text search means in a relational DB and the tradeoffs between `LIKE`, `CONTAINS`, and a search index
- What structured logging is, why Serilog is preferred over `Console.WriteLine`, and how global exception middleware gives you consistent error responses

**How You'll Apply It**
- Add `?page`, `?pageSize`, `?category`, and `?tag` query params to `GET /api/v1/posts`
- Return a `PaginatedResult<PostReadDTO>` with `totalCount` and `totalPages`
- Build `GET /api/v1/posts/search?q=` through `SearchService`
- Set up Serilog to log to console and file, and write `ExceptionMiddleware` to catch all unhandled errors and return a consistent JSON shape

**Definition of Done**
```
✓ Pagination returns correct page and page size
✓ Filter by category returns only matching posts
✓ Search returns posts matching query term in title or body
✓ All errors return consistent JSON error response
✓ Logs appear in console and file
```

---

### Phase 6 — Soft Deletes, Audit Trails, and Admin Dashboard

*What you're building: Soft delete pattern across entities, audit fields, and admin reporting endpoints.*

**What You'll Learn**
- What soft deletes are, why production systems rarely hard-delete data, and how `IsDeleted` + `DeletedAt` implement it
- How EF Core global query filters work and why they're the cleanest way to exclude soft-deleted rows everywhere automatically
- What aggregation queries are (`COUNT`, `GROUP BY`) and how EF Core translates LINQ into them

**How You'll Apply It**
- Add `IsDeleted` and `DeletedAt` to `User`, `Post`, and `Comment` with a migration
- Register a global query filter in `AppDbContext` so deleted records are invisible by default
- Change all `DELETE` endpoints to set `IsDeleted = true` instead of removing the row
- Build `GET /api/v1/admin/stats` in `AdminService` returning post count, user count, and comment count

**Definition of Done**
```
✓ Deleted posts do not appear in GET /api/v1/posts
✓ Soft-deleted record still exists in database with DeletedAt timestamp
✓ Admin stats endpoint returns correct aggregated counts
✓ Admin endpoints return 403 for non-admin roles
```

---

### Phase 7 — API Versioning

*What you're building: API versioning so future breaking changes don't affect existing consumers.*

**What You'll Learn**
- Why API versioning exists — what happens to existing clients when you ship a breaking change
- How URL-based versioning works (`/api/v1/` vs `/api/v2/`) vs header-based versioning and when to use each
- How the `Microsoft.AspNetCore.Mvc.Versioning` package handles default versions and deprecation headers

**How You'll Apply It**
- Prefix all existing routes under `/api/v1/`
- Register versioning middleware in `Program.cs` with a configured default version
- Create one v2 example endpoint (e.g. `PostsV2Controller`) with an extended response shape to prove the pattern works

**Definition of Done**
```
✓ /api/v1/posts works as before
✓ /api/v2/posts returns extended response shape
✓ Unversioned request returns configured default version
✓ Deprecated version returns Deprecation response header
```

---

### Phase 8 — Unit Testing and Docker

*What you're building: Unit tests for services and repositories, Dockerized deployment.*

**What You'll Learn**
- What unit testing is and why you test services in isolation — not controllers, not the database
- What mocking is, what Moq does, and how it lets you fake a repository so your service tests don't touch a real DB
- How Docker works conceptually — images, containers, and why `docker-compose` lets you run the API and SQL Server together with one command

**How You'll Apply It**
- Set up the `BlogFlow.Tests` xUnit project and reference the API project
- Write unit tests for `PostService` (create, update, delete, get) using Moq to mock `IPostRepository`
- Write unit tests for `AuthService` (register, login, refresh) and `SearchService`
- Write a `Dockerfile` for the API and a `docker-compose.yml` that spins up the API + SQL Server together

**Definition of Done**
```
✓ All unit tests pass
✓ Services tested without hitting a real database
✓ docker compose up starts API and database
✓ Swagger accessible at localhost after docker compose up
```

---

## 9. Skills Checklist

**Foundational — carried over from Student Enrollment**

| Skill | Status |
|---|---|
| Controller-based API architecture | ✅ Already know |
| DTO separation | ✅ Already know |
| EF Core, DbContext, migrations | ✅ Already know |
| Navigation properties and Include() | ✅ Already know |
| Many-to-many relationships | ✅ Already know |
| Business logic before insert | ✅ Already know |
| async/await | ✅ Already know |

**Advanced — new in this project**

| Skill | Introduced In |
|---|---|
| JWT authentication | Phase 1 |
| Role-based authorization | Phase 1 |
| Refresh tokens and token revocation | Phase 2 |
| Rate limiting and brute-force protection | Phase 2 |
| Security headers and HTTPS enforcement | Phase 2 |
| Service layer pattern | Phase 3 |
| Repository pattern | Phase 3 |
| Dependency injection via interfaces | Phase 3 |
| Input validation with FluentValidation | Phase 3 |
| Many-to-many via bridge table (PostTag) | Phase 4 |
| ThenInclude() for nested navigation | Phase 4 |
| Comment ownership checks | Phase 4 |
| Pagination with Skip() and Take() | Phase 5 |
| Query parameter filtering | Phase 5 |
| Full-text search with EF Core | Phase 5 |
| Serilog structured logging | Phase 5 |
| Global exception middleware | Phase 5 |
| Soft deletes with IsDeleted flag | Phase 6 |
| EF Core global query filters | Phase 6 |
| Audit trail fields (DeletedAt) | Phase 6 |
| Aggregation queries for admin stats | Phase 6 |
| API versioning with URL-based routing | Phase 7 |
| ApiVersion attribute and deprecation headers | Phase 7 |
| Unit testing with xUnit | Phase 8 |
| Mocking with Moq | Phase 8 |
| Docker and docker-compose | Phase 8 |