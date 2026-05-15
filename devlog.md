# Development Log
<details>
  <summary>May 13, 2026</summary>

  ### Goal
  - Improve backend architecture and optimize queries

  ### What I did
  - Added admin user management system
  - Refactored category service layer
  - Applied AsNoTracking to queries

  ### Notes / Learnings
  - Service layer should contain business logic
  - Repository should only handle persistence
  - AsNoTracking improves read performance

  ### Next step
  - Standardize service pattern across modules
  - 
</details>

<details>
  <summary>May 14, 2026</summary>

### Goal

Build a robust backend error-handling and observability system for BlogFlow API using:

* Custom exception hierarchy
* Centralized global exception handling
* Structured logging with Serilog
* Consistent API error responses


### What I did

#### 1. Refactored domain/service layer (foundation step)

* Moved business logic into service layer
* Introduced DTO projection optimization
* Centralized validation logic in `CommentService`
* Replaced generic exceptions with domain-specific exceptions

---

#### 2. Implemented custom exception system

* Created `AppException` base abstraction
* Added domain exceptions:

  * `NotFoundException`
  * `BadRequestException`
  * `ConflictException`
  * `UnauthorizedException`
  * `ForbiddenException`
* Standardized:

  * Status codes
  * Error codes
  * Trace IDs
  * Validation error support
* Refactored exception messages to be context-aware

---

#### 3. Built global exception handling layer

* Implemented `GlobalExceptionHandler` using `IExceptionHandler`
* Centralized all unhandled exception processing
* Added fallback handling for unexpected errors (500)
* Standardized JSON error response DTO
* Ensured all API errors follow consistent structure:

  * statusCode
  * message
  * errorCode
  * timestamp
  * traceId
  * validation errors (if present)
* Improved API consistency and separation of concerns

---

#### 4. Integrated structured logging with Serilog

* Configured Serilog with:

  * Console sink (development visibility)
  * File sink (JSON format for persistence)
* Enabled Serilog as primary logging provider
* Added request logging via middleware (`UseSerilogRequestLogging`)
* Integrated `ILogger` into `GlobalExceptionHandler`
* Implemented log level mapping:

  * NotFound → Information
  * Unauthorized → Warning
  * Forbidden → Warning
  * BadRequest → Warning
  * Unexpected → Error
* Added structured logging with:

  * StatusCode
  * ErrorCode
  * TraceId
  * Exception stack traces
* Improved system observability for debugging and monitoring

---

### Notes / Learnings

* Exception handling should be centralized, not scattered across controllers/services
* Custom exceptions are most useful when paired with a global handler
* Logging is most valuable when structured (not plain strings)
* Logging responsibility should be separated:

  * Middleware → request lifecycle
  * Exception handler → errors
  * Services → business events
* TraceId is critical for debugging real request flows
* Default-safe responses prevent leaking internal system details

---

### Next step

* Add **business-level logging inside service layer** (audit events like post creation, deletion, and updates)
* Enhance logs with user context (UserId, IP address)
* Improve request observability with enriched Serilog context
* Optional: introduce correlation ID middleware for deeper tracing across services

---

</details>
