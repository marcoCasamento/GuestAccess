# Changelog

All notable changes to the GuestAccess project will be documented in this file.

## [1.0.0] - 2026-03-07

### Initial Release

#### Added
- **Authentication System**
  - ASP.NET Core Identity integration
  - Email/password registration and login
  - Password reset functionality
  - Google OAuth 2.0 support
  - Microsoft OAuth 2.0 support
  - Secure password requirements (min 8 chars, uppercase, lowercase, digit, special char)
  - Session management

- **Authorization System**
  - Role-based access control (Admin role)
  - Custom permission model (per-user, per-button)
  - Default deny approach (no permissions on registration)
  - Admin-only access to user management panel

- **Access Control Features**
  - Three gate/door control buttons:
    - Apri Cancello (Open Main Gate)
    - Apri Cancelletto (Open Small Gate)
    - Apri Portone (Open Main Door)
  - Console logging for all actions
  - User identification in logs
  - Timestamp for each action

- **Admin Panel**
  - List all registered users
  - Grant/revoke permissions per user
  - Visual indication of admin users
  - Real-time permission updates
  - Bulk permission management

- **Security Features**
  - Anti-CSRF token protection on all forms
  - HTTPS enforcement
  - HSTS (HTTP Strict Transport Security)
  - Password hashing with PBKDF2
  - SQL injection protection via Entity Framework
  - Input validation
  - Comprehensive audit logging
  - Authorization checks on every action
  - Secure cookie settings

- **Database**
  - SQLite database (easily replaceable)
  - Entity Framework Core 10
  - Automatic migrations
  - Custom ApplicationUser model with permissions
  - Identity tables for authentication

- **Containerization**
  - Multi-stage Dockerfile for optimized image size
  - Non-root user for security
  - Docker Compose configuration
  - Volume mounting for data persistence
  - Environment variable configuration
  - Health check support

- **CI/CD**
  - GitHub Actions workflow
  - Automatic build on push to main
  - Multi-platform Docker images (linux/amd64, linux/arm64)
  - Push to Docker Hub
  - Automated tagging (latest, branch name, git SHA)
  - Build caching for faster builds

- **Documentation**
  - README.md - Project overview and features
  - QUICKSTART.md - Getting started guide
  - DEPLOYMENT.md - Production deployment instructions
  - SECURITY.md - Security guidelines and best practices
  - PROJECT_COMPLETE.md - Complete project summary
  - QUICK_REFERENCE.md - Quick command reference
  - CHANGELOG.md - This file
  - Inline code comments
  - .env.example for configuration template

- **Development Tools**
  - Local testing scripts (run-local.sh, run-local.bat)
  - Docker testing support
  - Development environment configuration
  - Git repository with comprehensive .gitignore

#### Security Considerations
- All pages require authentication except login/register
- Admin panel requires Admin role
- Each button requires specific permission
- Double verification on every action
- Permissions stored server-side, never client-side
- Anti-CSRF tokens on all state-changing operations
- Secure password storage (hashed, never plain text)
- Audit logging for all critical actions
- HTTPS enforcement in production

#### Known Limitations
- SQLite database (not suitable for high-concurrency scenarios)
- No 2FA implementation yet
- No rate limiting on login attempts
- No IP whitelisting
- Console logging only (no Home Assistant integration yet)
- No email confirmation required (set to false for easier testing)

#### Configuration
- Default admin user: admin@localhost / Admin123!
- Database: SQLite (app.db)
- Default port: 8080 (Docker), 5000 (local dev)
- Environment: Development by default

#### Dependencies
- .NET 10 SDK
- ASP.NET Core 10
- Entity Framework Core 10
- Microsoft.AspNetCore.Identity
- Microsoft.AspNetCore.Authentication.Google 10.0.3
- Microsoft.AspNetCore.Authentication.MicrosoftAccount 10.0.3
- SQLite database provider
- Bootstrap 5 (frontend)
- jQuery (frontend)

## [Future Enhancements]

### Planned Features
- [ ] Two-Factor Authentication (2FA)
- [ ] Rate limiting on login attempts
- [ ] IP whitelisting for admin panel
- [ ] Email confirmation on registration
- [ ] Home Assistant REST API integration
- [ ] MQTT support for Home Assistant
- [ ] Webhook support
- [ ] Activity dashboard
- [ ] Export logs to CSV/JSON
- [ ] User activity statistics
- [ ] Time-based permissions (schedule access)
- [ ] Temporary access tokens
- [ ] Notification system (email/SMS on access)
- [ ] Mobile app (iOS/Android)
- [ ] RESTful API for integrations
- [ ] Geolocation tracking
- [ ] Advanced logging (SIEM integration)
- [ ] Database migration to PostgreSQL/SQL Server
- [ ] Multi-tenancy support
- [ ] Localization (multiple languages)
- [ ] Dark mode UI

### Security Enhancements
- [ ] Rate limiting
- [ ] Account lockout policy
- [ ] Password complexity checker
- [ ] Breach password detection
- [ ] Session timeout configuration
- [ ] Concurrent session limits
- [ ] Device fingerprinting
- [ ] Suspicious login detection
- [ ] Security headers (CSP, X-Frame-Options)
- [ ] Database encryption at rest
- [ ] Encrypted backups
- [ ] Penetration testing results

---

## Version History

| Version | Date | Description |
|---------|------|-------------|
| 1.0.0 | 2026-03-07 | Initial release with core functionality |

## Contributing

This is a personal project. If you have suggestions or find security issues, please open an issue on GitHub.

## License

This project is provided as-is for personal use.
