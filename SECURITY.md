# GuestAccess - Security Documentation

## 🔒 Security Architecture

### Authentication Layers

1. **ASP.NET Core Identity**
   - Industry-standard authentication framework
   - Secure password hashing (PBKDF2)
   - Built-in protection against common attacks

2. **External Authentication**
   - Google OAuth 2.0
   - Microsoft OAuth 2.0
   - Reduces password fatigue
   - Leverages enterprise security

3. **Authorization**
   - Role-based access control (Admin role)
   - Custom permission system (per-user, per-button)
   - Default deny approach

### Security Features Implemented

#### 1. Authentication Requirements
- **ALL pages require authentication** except login/register
- Anonymous users are redirected to login page
- Session timeout configured
- Secure cookie settings

#### 2. Authorization Controls
```csharp
[Authorize]  // Required for all control pages
[Authorize(Roles = "Admin")]  // Admin panel only
```

#### 3. Permission Checks
Every button action performs **double verification**:
```csharp
// Check 1: User is authenticated
var user = await _userManager.GetUserAsync(User);
if (user == null) return Challenge();

// Check 2: User has specific permission
if (!user.CanOpenCancello) return Forbid();
```

#### 4. CSRF Protection
- All POST requests require anti-forgery tokens
- Automatically validated by ASP.NET Core
- Prevents cross-site request forgery attacks

```html
@Html.AntiForgeryToken()
```

#### 5. Audit Logging
Every gate operation is logged with:
- User email and ID
- Action performed
- Timestamp
- Logged to both application logs and console

```csharp
_logger.LogInformation("USER: {Email} - ACTION: Opening Cancello", user.Email);
Console.WriteLine($"🚪 APRI CANCELLO - User: {user.Email} - Time: {DateTime.Now}");
```

#### 6. Input Validation
- Model validation on all forms
- Server-side validation
- Protection against injection attacks

#### 7. Password Security
- Minimum 8 characters
- Requires uppercase and lowercase
- Requires digit
- Requires special character
- Passwords are hashed, never stored in plain text

#### 8. Default Deny Principle
```csharp
public class ApplicationUser : IdentityUser
{
    public bool CanOpenCancello { get; set; } = false;  // Default: NO permission
    public bool CanOpenCancelletto { get; set; } = false;
    public bool CanOpenPortone { get; set; } = false;
}
```

#### 9. Container Security
```dockerfile
# Non-root user in container
RUN adduser --disabled-password --gecos '' appuser
USER appuser

# Security options in docker-compose
security_opt:
  - no-new-privileges:true
```

#### 10. HTTPS Enforcement
```csharp
app.UseHttpsRedirection();  // Force HTTPS
app.UseHsts();  // HTTP Strict Transport Security
```

## 🛡️ Attack Vectors & Mitigations

### 1. Unauthorized Access Attempts
**Risk**: User tries to access buttons without permission

**Mitigation**:
- Buttons only rendered if user has permission
- Server-side permission check on every POST
- Returns HTTP 403 Forbidden if unauthorized
- Logged as security warning

### 2. Cross-Site Request Forgery (CSRF)
**Risk**: Malicious site tricks user into triggering gate actions

**Mitigation**:
- Anti-forgery tokens on all forms
- Tokens validated server-side
- Short token lifetime

### 3. Session Hijacking
**Risk**: Attacker steals session cookie

**Mitigation**:
- HTTPS only (encrypted transmission)
- Secure cookie flags
- HttpOnly cookies (not accessible via JavaScript)
- Session timeout

### 4. SQL Injection
**Risk**: Malicious input manipulates database

**Mitigation**:
- Entity Framework ORM (parameterized queries)
- No raw SQL execution
- Input validation

### 5. Password Attacks
**Risk**: Brute force or dictionary attacks

**Mitigation**:
- Strong password requirements
- Account lockout after failed attempts
- Password hashing with salt
- Consider adding rate limiting (future enhancement)

### 6. Privilege Escalation
**Risk**: Regular user gains admin access

**Mitigation**:
- Role checks on admin pages
- Permissions stored in database, not client
- Admin role required for permission changes
- Can't modify own admin status through UI

### 7. Information Disclosure
**Risk**: Sensitive data exposed

**Mitigation**:
- No sensitive data in URLs
- Error pages don't reveal stack traces in production
- Logs don't contain passwords
- Database connection strings in environment variables

## 📋 Security Checklist for Production

### Before Deployment
- [ ] Change default admin password
- [ ] Use environment variables for all secrets
- [ ] Never commit `.env` file
- [ ] Review all permissions granted to users
- [ ] Enable HTTPS with valid SSL certificate
- [ ] Configure firewall rules
- [ ] Set up database backups
- [ ] Review and test all authentication flows
- [ ] Enable security headers
- [ ] Configure rate limiting on login endpoints
- [ ] Set up monitoring and alerting
- [ ] Document incident response procedures

### Regular Maintenance
- [ ] Review audit logs weekly
- [ ] Update dependencies monthly
- [ ] Review user permissions monthly
- [ ] Test backup restoration quarterly
- [ ] Security audit annually
- [ ] Penetration testing annually

## 🚨 Incident Response

### Suspicious Activity Detected

1. **Check Logs**
   ```bash
   docker logs guestaccess | grep "Unauthorized"
   ```

2. **Identify User**
   - Check logged user ID and email
   - Review recent permission changes

3. **Immediate Actions**
   - Revoke user permissions in admin panel
   - Force logout all sessions (restart app)
   - Change admin password

4. **Investigation**
   - Review all recent actions by the user
   - Check if account was compromised
   - Review other users with similar permissions

5. **Prevention**
   - Update security policies
   - Additional training if needed
   - Consider 2FA implementation

### Compromised Admin Account

**IMMEDIATE ACTIONS**:
1. Stop the container: `docker stop guestaccess`
2. Backup the database: `cp data/app.db data/app.db.backup`
3. Access database directly:
   ```bash
   sqlite3 data/app.db
   # Remove compromised admin from role
   DELETE FROM AspNetUserRoles WHERE UserId='compromised-user-id';
   ```
4. Change admin password via database or recreate admin user
5. Review all permission changes
6. Restart with new credentials

## 🔐 Additional Hardening (Future Enhancements)

1. **Two-Factor Authentication (2FA)**
   - Google Authenticator support
   - SMS verification
   - Email confirmation codes

2. **Rate Limiting**
   - Limit login attempts per IP
   - Limit button presses per user per time period
   - Prevents abuse and DoS

3. **IP Whitelisting**
   - Restrict admin panel to specific IPs
   - VPN-only access option

4. **Advanced Logging**
   - Integration with SIEM systems
   - Real-time alerting
   - Geolocation tracking

5. **Database Encryption**
   - Encrypt database file at rest
   - Encrypted backups

6. **Session Management**
   - Concurrent session limits
   - Device fingerprinting
   - Suspicious login detection

7. **Security Headers**
   - Content Security Policy
   - X-Frame-Options
   - X-Content-Type-Options

8. **Penetration Testing**
   - Regular security assessments
   - Automated vulnerability scanning
   - Third-party audits

## 📖 References

- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [ASP.NET Core Security](https://docs.microsoft.com/aspnet/core/security/)
- [Docker Security Best Practices](https://docs.docker.com/engine/security/)
- [OAuth 2.0 Security](https://oauth.net/2/)
