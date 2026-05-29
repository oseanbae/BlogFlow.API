# Future Improvements

## V2 Improvements

### Features
- [ ] Like / reaction system for posts
- [ ] Full-text search — search by post body content, not just title
- [ ] Email verification on register
- [ ] Password reset via email
- [ ] `/auth/logout` alias for `/auth/revoke` for frontend convenience

---

### Admin Reporting
- [ ] GET /admin/reports/posts — posts created per day/week/month
- [ ] GET /admin/reports/users — user growth over time
- [ ] GET /admin/reports/comments — comment activity per period
- [ ] GET /admin/reports/top-posts — most commented posts
- [ ] GET /admin/reports/top-authors — authors ranked by post count

---

### Enhancements
- [ ] Dynamic sorting (sortBy, sortOrder) on posts, comments, and users
      with default fallback to CreatedAt DESC
- [ ] Date range filters (from / to) on post and comment query params
- [ ] Caching for expensive queries (stats, top posts)
- [ ] Export to CSV / JSON for admin reports
- [ ] Swagger/OpenAPI — XML comments and request/response examples
      on all controllers and DTOs

---

### Security
- [ ] Rate limiting on non-auth endpoints
      (currently only register, login, refresh, revoke are rate limited —
      post creation, comment creation, and other write endpoints are unprotected)
- [ ] CORS production policy — read allowed origins from
      appsettings.Production.json via Cors:AllowedOrigins config section
- [ ] HTTPS enforcement in production
- [ ] appsettings.Production.json with environment variable binding
      for deployment without user-secrets

---

### Infrastructure
- [ ] Docker + docker-compose (API + PostgreSQL)
- [ ] API versioning (/api/v2 support, backward compatibility)
- [ ] Move database migrations out of startup —
      use a dedicated migration job or CLI command for production deployments

---

### Testing
- [ ] Unit tests for service layer
      (PostService, CommentService, AuthService, UserService)
- [ ] Integration tests for all controllers
- [ ] Seeder tests to verify seed data integrity on fresh DB

### Documentation
- [ ] Add API_EXAMPLES.md to demonstrate system behavior across core workflows
     (authentication, post lifecycle, role-based access control, and visibility rules)