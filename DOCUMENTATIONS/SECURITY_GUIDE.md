# Security Implementation Guide for Banking LMS

## Overview

This document provides comprehensive security recommendations and implementation guidelines for a Loan Management System (LMS) used by bank officers. This system handles sensitive financial data and requires enterprise-grade security measures.

---

## Current Security Status

### ✅ Implemented Features

| Feature                    | Status         | Description                                         |
| -------------------------- | -------------- | --------------------------------------------------- |
| JWT Authentication         | ✅ Implemented | Token-based authentication with 4-hour expiration   |
| Password Hashing           | ✅ Implemented | BCrypt with salt for secure password storage        |
| Session Management         | ✅ Implemented | Database-tracked sessions with JTI validation       |
| Single Session Enforcement | ✅ Implemented | Prevents concurrent logins from multiple devices    |
| Device Tracking            | ✅ Implemented | Logs IP address, user agent, device type            |
| Session History            | ✅ Implemented | Complete audit trail of all login/logout activities |
| Logout Functionality       | ✅ Implemented | Manual logout, logout all, logout specific session  |

### ⚠️ Recommended Enhancements

| Priority    | Feature                          | Impact | Complexity |
| ----------- | -------------------------------- | ------ | ---------- |
| 🔴 Critical | Token Validation Middleware      | High   | Medium     |
| 🔴 Critical | HTTPS/TLS Enforcement            | High   | Low        |
| 🟡 High     | Two-Factor Authentication (2FA)  | High   | High       |
| 🟡 High     | Failed Login Attempt Tracking    | Medium | Low        |
| 🟡 High     | Audit Logging System             | High   | Medium     |
| 🟡 High     | Role-Based Access Control (RBAC) | High   | Medium     |
| 🟡 High     | Sensitive Data Encryption        | High   | High       |
| 🟢 Medium   | IP Address Validation            | Medium | Low        |
| 🟢 Medium   | Session Timeout/Auto-Logout      | Medium | Low        |
| 🟢 Medium   | Rate Limiting                    | Medium | Medium     |

---

## Critical Security Enhancements

### 1. Token Validation Middleware

**Purpose**: Validate JWT token and session status on every API request.

**Implementation**:

```csharp
// File: Middleware/SessionValidationMiddleware.cs
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using LMS.Data;

namespace LMS.Middleware
{
    public class SessionValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
        {
            // Skip validation for public endpoints
            var path = context.Request.Path.Value?.ToLower();
            if (path == "/api/auth/login" || path == "/api/auth/register" || path == "/")
            {
                await _next(context);
                return;
            }

            // Extract token from Authorization header
            var authHeader = context.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                await _next(context);
                return;
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            try
            {
                // Decode JWT token
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                // Extract JTI claim
                var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

                if (!string.IsNullOrEmpty(jti))
                {
                    // Check if session is still active
                    var session = await dbContext.Sessions
                        .FirstOrDefaultAsync(s => s.TokenJti == jti && s.IsActive);

                    if (session == null)
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            Status = 401,
                            Message = "Session has been logged out or expired"
                        });
                        return;
                    }

                    // Check if session has expired
                    if (session.ExpiresAt < DateTime.UtcNow)
                    {
                        session.IsActive = false;
                        session.LogoutReason = "Expired";
                        await dbContext.SaveChangesAsync();

                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            Status = 401,
                            Message = "Session has expired"
                        });
                        return;
                    }

                    // Update last activity time
                    session.LastActivityAt = DateTime.UtcNow;
                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                // Invalid token format, let the authentication middleware handle it
            }

            await _next(context);
        }
    }
}
```

**Registration in Program.cs**:

```csharp
// Add after authentication middleware
app.UseAuthentication();
app.UseMiddleware<SessionValidationMiddleware>();
app.UseAuthorization();
```

**Benefits**:

- ✅ Validates session on every request
- ✅ Prevents use of logged-out tokens
- ✅ Tracks user activity
- ✅ Auto-expires old sessions

---

### 2. HTTPS/TLS Enforcement

**Purpose**: Encrypt all data in transit.

**Implementation**:

```csharp
// In Program.cs
var builder = WebApplication.CreateBuilder(args);

// Force HTTPS in production
if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddHsts(options =>
    {
        options.MaxAge = TimeSpan.FromDays(365);
        options.IncludeSubDomains = true;
        options.Preload = true;
    });
}

var app = builder.Build();

// Redirect HTTP to HTTPS
app.UseHttpsRedirection();

// Enable HSTS
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}
```

**Production Configuration**:

- Use valid SSL/TLS certificates
- Configure reverse proxy (Nginx/Apache) for HTTPS
- Disable HTTP port in production

---

## High Priority Enhancements

### 3. Two-Factor Authentication (2FA)

**Purpose**: Add an extra layer of security for sensitive operations.

**Implementation Options**:

#### Option A: SMS OTP

```csharp
// Generate 6-digit OTP
var otp = new Random().Next(100000, 999999).ToString();

// Store in database with expiration
var otpRecord = new OtpVerification
{
    UserId = user.Id,
    Code = otp,
    ExpiresAt = DateTime.UtcNow.AddMinutes(5),
    IsUsed = false
};

// Send via SMS service (Twilio, AWS SNS, etc.)
await _smsService.SendAsync(user.PhoneNo, $"Your OTP: {otp}");
```

#### Option B: Email OTP

```csharp
// Send OTP via email
await _emailService.SendAsync(user.Email, "Login Verification",
    $"Your verification code is: {otp}");
```

#### Option C: Authenticator App (Google Authenticator)

```csharp
// Use library like OtpNet
var secretKey = KeyGeneration.GenerateRandomKey(20);
var totp = new Totp(secretKey);
var code = totp.ComputeTotp();
```

**When to Require 2FA**:

- ✅ Login from new device
- ✅ Login from new IP address
- ✅ Loan approval/disbursement
- ✅ Changing password
- ✅ Updating bank account details

---

### 4. Failed Login Attempt Tracking

**Purpose**: Prevent brute force attacks.

**Database Schema**:

```csharp
// Add to User model
[Column("failed_login_attempts")]
public int FailedLoginAttempts { get; set; }

[Column("locked_until")]
public DateTime? LockedUntil { get; set; }
```

**Implementation**:

```csharp
// In Login method
if (user.LockedUntil.HasValue && user.LockedUntil.Value > DateTime.UtcNow)
{
    var minutesLeft = (user.LockedUntil.Value - DateTime.UtcNow).TotalMinutes;
    return Unauthorized($"Account locked. Try again in {Math.Ceiling(minutesLeft)} minutes.");
}

// Verify password
bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
if (!isPasswordValid)
{
    user.FailedLoginAttempts++;

    // Lock account after 5 failed attempts
    if (user.FailedLoginAttempts >= 5)
    {
        user.LockedUntil = DateTime.UtcNow.AddMinutes(30);
        await _context.SaveChangesAsync();

        // Send alert email
        await _emailService.SendAsync(user.Email, "Account Locked",
            "Your account has been locked due to multiple failed login attempts.");

        return Unauthorized("Account locked due to multiple failed login attempts.");
    }

    await _context.SaveChangesAsync();
    return Unauthorized("Invalid username or password");
}

// Reset failed attempts on successful login
user.FailedLoginAttempts = 0;
user.LockedUntil = null;
```

---

### 5. Audit Logging System

**Purpose**: Track all sensitive operations for compliance and security.

**Database Schema**:

```csharp
// File: Models/Audit/AuditLog.cs
[Table("audit_logs")]
public class AuditLog
{
    [Column("id")]
    public int Id { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

    [Column("action")]
    [MaxLength(100)]
    public string Action { get; set; } = null!;

    [Column("entity_type")]
    [MaxLength(50)]
    public string? EntityType { get; set; }

    [Column("entity_id")]
    public int? EntityId { get; set; }

    [Column("old_values")]
    public string? OldValues { get; set; }

    [Column("new_values")]
    public string? NewValues { get; set; }

    [Column("ip_address")]
    [MaxLength(45)]
    public string? IpAddress { get; set; }

    [Column("user_agent")]
    [MaxLength(500)]
    public string? UserAgent { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
```

**Service Implementation**:

```csharp
// File: Services/AuditService.cs
public class AuditService
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public async Task LogAsync(string action, string? entityType = null,
        int? entityId = null, object? oldValues = null, object? newValues = null)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var userId = GetUserIdFromToken(httpContext);

        var auditLog = new AuditLog
        {
            UserId = userId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            OldValues = oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
            NewValues = newValues != null ? JsonSerializer.Serialize(newValues) : null,
            IpAddress = httpContext?.Connection.RemoteIpAddress?.ToString(),
            UserAgent = httpContext?.Request.Headers["User-Agent"].ToString(),
            CreatedAt = DateTime.UtcNow
        };

        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();
    }
}
```

**Usage Examples**:

```csharp
// Login
await _auditService.LogAsync("USER_LOGIN");

// Loan approval
await _auditService.LogAsync("LOAN_APPROVED", "Loan", loanId,
    oldValues: new { Status = "Pending" },
    newValues: new { Status = "Approved", ApprovedBy = userId });

// Disbursement
await _auditService.LogAsync("LOAN_DISBURSED", "Loan", loanId,
    newValues: new { Amount = 50000, AccountNumber = "***1234" });
```

---

### 6. Role-Based Access Control (RBAC)

**Purpose**: Restrict access based on user roles.

**Implementation**:

```csharp
// Add [Authorize] attribute with roles
[Authorize(Roles = "Officer,Supervisor")]
[HttpPost("approve-loan/{loanId}")]
public async Task<IActionResult> ApproveLoan(int loanId)
{
    // Only officers and supervisors can approve
}

[Authorize(Roles = "Supervisor")]
[HttpPost("disburse-loan/{loanId}")]
public async Task<IActionResult> DisburseLoan(int loanId)
{
    // Only supervisors can disburse
}

[Authorize(Roles = "SuperAdmin")]
[HttpDelete("delete-user/{userId}")]
public async Task<IActionResult> DeleteUser(int userId)
{
    // Only super admin can delete users
}
```

**Configure in Program.cs**:

```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireOfficerRole", policy =>
        policy.RequireRole("Officer", "Supervisor", "SuperAdmin"));

    options.AddPolicy("RequireSupervisorRole", policy =>
        policy.RequireRole("Supervisor", "SuperAdmin"));
});
```

---

### 7. Sensitive Data Encryption

**Purpose**: Protect sensitive data at rest.

**Implementation**:

```csharp
// File: Services/EncryptionService.cs
using System.Security.Cryptography;
using System.Text;

public class EncryptionService
{
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public EncryptionService(IConfiguration configuration)
    {
        // Store these in environment variables or Azure Key Vault
        _key = Encoding.UTF8.GetBytes(configuration["Encryption:Key"]!);
        _iv = Encoding.UTF8.GetBytes(configuration["Encryption:IV"]!);
    }

    public string Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;

        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var msEncrypt = new MemoryStream();
        using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        using (var swEncrypt = new StreamWriter(csEncrypt))
        {
            swEncrypt.Write(plainText);
        }
        return Convert.ToBase64String(msEncrypt.ToArray());
    }

    public string Decrypt(string cipherText)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;

        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText));
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);
        return srDecrypt.ReadToEnd();
    }
}
```

**Usage**:

```csharp
// Encrypt sensitive fields before saving
user.EncryptedIdNo = _encryptionService.Encrypt(user.IdNo);
loan.EncryptedAccountNumber = _encryptionService.Encrypt(loan.AccountNumber);

// Decrypt when needed
var idNo = _encryptionService.Decrypt(user.EncryptedIdNo);
```

---

## Medium Priority Enhancements

### 8. IP Address Validation

**Purpose**: Detect and alert on suspicious login locations.

```csharp
// Check if login is from new IP
var recentSessions = await _context.Sessions
    .Where(s => s.UserId == user.Id)
    .OrderByDescending(s => s.CreatedAt)
    .Take(10)
    .ToListAsync();

var knownIps = recentSessions.Select(s => s.IpAddress).Distinct().ToList();
var currentIp = HttpContext.Connection.RemoteIpAddress?.ToString();

if (!knownIps.Contains(currentIp))
{
    // New IP detected - send alert email
    await _emailService.SendAsync(user.Email, "New Login Location",
        $"A login was detected from a new IP address: {currentIp}");

    // Optionally require 2FA
    // return RequireTwoFactorAuth();
}
```

---

### 9. Session Timeout & Auto-Logout

**Purpose**: Automatically logout inactive sessions.

```csharp
// In SessionValidationMiddleware
if (session.LastActivityAt < DateTime.UtcNow.AddMinutes(-15))
{
    session.IsActive = false;
    session.LogoutReason = "Timeout";
    session.LoggedOutAt = DateTime.UtcNow;
    await dbContext.SaveChangesAsync();

    context.Response.StatusCode = 401;
    await context.Response.WriteAsJsonAsync(new
    {
        Status = 401,
        Message = "Session timed out due to inactivity"
    });
    return;
}
```

---

### 10. Rate Limiting

**Purpose**: Prevent API abuse and DDoS attacks.

```csharp
// Install package: AspNetCoreRateLimit
// In Program.cs
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.GeneralRules = new List<RateLimitRule>
    {
        new RateLimitRule
        {
            Endpoint = "POST:/api/auth/login",
            Limit = 5,
            Period = "1m" // 5 attempts per minute
        }
    };
});

builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// Add middleware
app.UseIpRateLimiting();
```

---

## Security Best Practices

### Password Requirements

```csharp
// Minimum requirements for banking system
- Minimum 12 characters
- At least 1 uppercase letter
- At least 1 lowercase letter
- At least 1 number
- At least 1 special character
- Cannot contain username
- Cannot be same as last 5 passwords
- Must change every 90 days
```

### Token Configuration

```csharp
// Recommended settings
- Token expiration: 4 hours (current)
- Refresh token: Consider implementing for better UX
- Token rotation: Generate new token on sensitive operations
```

### Database Security

```csharp
// Best practices
- Use parameterized queries (EF Core does this)
- Encrypt connection strings
- Use read-only database user for reports
- Regular backups with encryption
- Implement database audit logging
```

---

## Compliance Considerations

### Banking Regulations

- **PCI DSS**: If handling card data
- **GDPR**: If operating in EU
- **SOC 2**: For service providers
- **ISO 27001**: Information security management

### Required Features

1. **Data Retention**: Keep audit logs for 7+ years
2. **Right to be Forgotten**: Ability to delete user data
3. **Data Portability**: Export user data in standard format
4. **Breach Notification**: Alert users within 72 hours
5. **Access Controls**: Document who can access what

---

## Implementation Priority

### Phase 1: Critical (Week 1-2)

1. ✅ Token Validation Middleware
2. ✅ HTTPS/TLS Enforcement
3. ✅ Failed Login Tracking

### Phase 2: High Priority (Week 3-4)

4. ✅ Audit Logging System
5. ✅ Two-Factor Authentication
6. ✅ RBAC Enhancement

### Phase 3: Medium Priority (Week 5-6)

7. ✅ Sensitive Data Encryption
8. ✅ IP Validation
9. ✅ Session Timeout
10. ✅ Rate Limiting

---

## Testing Security

### Security Testing Checklist

- [ ] Penetration testing
- [ ] SQL injection testing
- [ ] XSS vulnerability testing
- [ ] CSRF protection testing
- [ ] Authentication bypass testing
- [ ] Authorization testing
- [ ] Session management testing
- [ ] Password strength testing
- [ ] Rate limiting testing
- [ ] Encryption verification

### Tools

- **OWASP ZAP**: Web application security scanner
- **Burp Suite**: Security testing platform
- **Postman**: API testing
- **SonarQube**: Code quality and security

---

## Monitoring & Alerts

### What to Monitor

1. **Failed login attempts** (> 3 in 5 minutes)
2. **Unusual IP addresses** (new countries/regions)
3. **Multiple concurrent sessions** (should be prevented)
4. **Large data exports** (potential data breach)
5. **After-hours access** (outside business hours)
6. **Privilege escalation attempts**
7. **Unusual API usage patterns**

### Alert Channels

- Email notifications
- SMS alerts for critical events
- Slack/Teams integration
- Dashboard monitoring

---

## Incident Response Plan

### Steps to Take

1. **Detect**: Automated monitoring alerts
2. **Contain**: Disable affected accounts/sessions
3. **Investigate**: Review audit logs
4. **Remediate**: Fix vulnerability
5. **Recover**: Restore normal operations
6. **Learn**: Update security measures

### Contact Information

```
Security Team: security@yourbank.com
IT Support: support@yourbank.com
Emergency Hotline: +XX-XXX-XXXX
```

---

## Additional Resources

- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [Microsoft Security Best Practices](https://docs.microsoft.com/en-us/security/)
- [ASP.NET Core Security](https://docs.microsoft.com/en-us/aspnet/core/security/)
- [JWT Best Practices](https://tools.ietf.org/html/rfc8725)

---

## Conclusion

This security implementation guide provides a roadmap for enhancing the LMS to meet banking industry standards. Implement features in the suggested priority order, and regularly review and update security measures as threats evolve.

**Remember**: Security is an ongoing process, not a one-time implementation.
