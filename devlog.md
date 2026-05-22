<details>
<summary>📅 May 21, 2025</summary>

## 🐛 Bug Fixes

### 🔴 Resolved: Refresh token always invalid after login
* **The Bug:** Every `POST /api/v1/auth/refresh` request returned a `401 INVALID_REFRESH_TOKEN` error, even right after logging in.
* **The Cause:** `LoginAsync` fired `SaveChangesAsync` *before* `IssueAuthResponseAsync` ran. The new token was tracked in memory but never actually flushed to the database, leaving the client with an un-lookupable token.
* **The Fix:** Moved `SaveChangesAsync` to the very end of both `LoginAsync` and `RegisterAsync`. Token rotation, database persistence, and token replacements are now fully working end-to-end.

---

### 🔴 Resolved: PATCH /api/v1/users/me/password returns 204 with whitespace-only password
* **The Bug:** `PATCH /api/v1/users/me/password` returned `204 No Content` even when `newPassword` was a whitespace-only string like `" "`, bypassing all validation entirely.
* **The Cause:** Two problems combined. First, `AddValidatorsFromAssemblyContaining<Program>()` only registers validators in DI but never invokes them — `AddFluentValidationAutoValidation()` was missing, so validators never ran during model binding. Second, `.When(x => !string.IsNullOrWhiteSpace(x.NewPassword))` was accidentally guarding the entire rule chain, meaning validation was skipped precisely when the password was whitespace — the exact case it needed to catch.
* **The Fix:** Added `AddFluentValidationAutoValidation()` in `Program.cs` to hook FluentValidation into the model binding pipeline. Fixed `UserChangePasswordValidator` by replacing the incorrect `.When()` with `Must(p => !string.IsNullOrWhiteSpace(p))` and `Cascade(CascadeMode.Stop)` to correctly reject whitespace-only input and stop at the first failure. Added `[JsonRequired]` to `UserChangePasswordDTO` to reject missing fields at deserialization.

---

## 🏗️ Infrastructure

### ✅ Postman API Testing DAY 1
* Set up a Postman collection covering all existing endpoints for manual and automated API testing.
* Organized requests by resource with environment variables for base URL and auth tokens, making it easy to test across different environments without hardcoding values.

</details>

<details>
<summary>📅 May 22, 2025</summary>

### 🔴 Resolved: GET /api/v1/admin/users pagination, UTC, sorting, and routing issues
* **The Bug:** The user listing endpoint had unstable pagination behavior, failed on `CreatedAfter` date filtering, used hardcoded sorting inside pagination, and returned 404 due to route mismatch.
* **The Cause:** Pagination lacked strict input normalization, date filters were passed to PostgreSQL without UTC handling, sorting was embedded in `ExecutePagedQueryAsync` instead of being separated, and controller routing didn’t match `/api/v1` requests.
* **The Fix:** Added pagination safety (`page < 1 ? 1 : value`), introduced UTC normalization for date filters, extracted sorting into `ApplySorting()` to enforce SRP, and corrected API routing to match the expected endpoint structure.
### 🟢 Refactor: Support soft-deleted lookup for restore flow
* **The Change:** Updated GetByIdAsync to optionally include soft-deleted records using a parameter-controlled IgnoreQueryFilters() behavior.
* **The Reason:** Restore operations require access to entities marked as deleted (DeletedAt != null), which were previously excluded by default query filters.
* **The Result:** The repository now supports both normal lookups (active users only) and restore scenarios without duplicating methods or breaking existing query behavior.
</details>