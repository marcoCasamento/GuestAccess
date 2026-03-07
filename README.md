# GuestAccess - Secure Gate Control System

A secure ASP.NET Core 10 application for managing gate and door access with fine-grained user permissions.

## 🔒 Security Features

- **Authentication Required**: All pages require authentication
- **Role-Based Authorization**: Admin-only access to user management
- **Permission-Based Access Control**: Each user must be explicitly granted permission for each button
- **Default Deny**: New users have NO permissions by default
- **Anti-Forgery Tokens**: All POST requests are protected against CSRF
- **Audit Logging**: All gate operations are logged with user information
- **Secure Passwords**: Strong password requirements enforced
- **External Auth Support**: Microsoft and Google OAuth integration

## 🚀 Features

- **Three Access Controls**:
  - Apri Cancello (Open Main Gate)
  - Apri Cancelletto (Open Small Gate)
  - Apri Portone (Open Main Door)

- **User Management**:
  - Email/Password registration
  - Google OAuth login
  - Microsoft OAuth login
  - Password reset functionality

- **Admin Panel**:
  - View all registered users
  - Grant/revoke permissions per user per button
  - Real-time permission updates

## 🐳 Docker Deployment

### Using Docker Compose (Recommended)

```bash
docker-compose up -d
```

### Using Docker CLI

```bash
docker run -d \
  -p 8080:8080 \
  -v $(pwd)/data:/app/data \
  -e AdminUser__Email=admin@yourdomain.com \
  -e AdminUser__Password=YourSecurePassword123! \
  -e Authentication__Google__ClientId=your-google-client-id \
  -e Authentication__Google__ClientSecret=your-google-secret \
  -e Authentication__Microsoft__ClientId=your-microsoft-client-id \
  -e Authentication__Microsoft__ClientSecret=your-microsoft-secret \
  --name guestaccess \
  YOUR_DOCKERHUB_USERNAME/guestaccess:latest
```

## ⚙️ Configuration

### Environment Variables

| Variable | Description | Default | Required |
|----------|-------------|---------|----------|
| `AdminUser__Email` | Admin user email | admin@localhost | Yes |
| `AdminUser__Password` | Admin user password | Admin123! | Yes |
| `Authentication__Google__ClientId` | Google OAuth Client ID | - | No* |
| `Authentication__Google__ClientSecret` | Google OAuth Secret | - | No* |
| `Authentication__Microsoft__ClientId` | Microsoft OAuth Client ID | - | No* |
| `Authentication__Microsoft__ClientSecret` | Microsoft OAuth Secret | - | No* |

*External authentication is **optional**. If not configured, users can register/login with email and password only.

### External Authentication Setup

**Note:** External authentication providers are completely optional. The application works fine with just email/password authentication.

#### Google OAuth
1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project or select existing
3. Enable Google+ API
4. Create OAuth 2.0 credentials
5. Add authorized redirect URI: `https://yourdomain.com/signin-google`

#### Microsoft OAuth
1. Go to [Azure Portal](https://portal.azure.com/)
2. Register a new application
3. Add redirect URI: `https://yourdomain.com/signin-microsoft`
4. Create a client secret

## 🔧 Development

### Prerequisites
- .NET 10 SDK
- SQLite

### Run Locally

```bash
dotnet restore
dotnet ef database update
dotnet run
```

Navigate to `https://localhost:5001`

### Default Admin Credentials
- **Email**: admin@localhost
- **Password**: Admin123!

⚠️ **IMPORTANT**: Change these in production!

## 📦 Building

```bash
dotnet build -c Release
dotnet publish -c Release -o ./publish
```

## 🏗️ Architecture

- **Framework**: ASP.NET Core 10 (Razor Pages)
- **Database**: SQLite (easily replaceable with SQL Server/PostgreSQL)
- **Authentication**: ASP.NET Core Identity
- **Authorization**: Role-based + Custom permissions

## 🔐 Security Recommendations for Production

1. **Change Default Admin Password** immediately
2. **Use HTTPS** - Configure SSL certificates
3. **Set Strong Passwords** - Use environment variables
4. **Enable 2FA** - For admin accounts
5. **Regular Backups** - Backup the database regularly
6. **Monitor Logs** - Check for unauthorized access attempts
7. **Update Dependencies** - Keep packages up to date
8. **Network Security** - Use firewall rules to restrict access
9. **Database Encryption** - Consider encrypting the database file
10. **Rate Limiting** - Add rate limiting for login attempts

## 📊 Database

The application uses SQLite by default. The database file (`app.db`) contains:
- User accounts (with hashed passwords)
- User permissions (per-button access control)
- Roles (Admin role)

### Persistent Storage

Mount a volume to `/app/data` to persist the database:

```yaml
volumes:
  - ./data:/app/data
```

## 🤝 Integration with Home Assistant

The application currently logs actions to the console. To integrate with Home Assistant:

1. **Option 1**: Call Home Assistant REST API from the button handlers
2. **Option 2**: Use MQTT to publish events
3. **Option 3**: Use webhooks to trigger Home Assistant automations

Example for REST API integration (add to `Index.cshtml.cs`):

```csharp
using var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Authorization = 
    new AuthenticationHeaderValue("Bearer", "YOUR_HA_TOKEN");
await httpClient.PostAsync(
    "http://homeassistant.local:8123/api/services/switch/turn_on",
    new StringContent("{\"entity_id\":\"switch.cancello\"}", 
    Encoding.UTF8, "application/json"));
```

## 📝 License

This project is provided as-is for personal use.

## ⚠️ Disclaimer

This software controls physical access points (gates, doors). Ensure proper testing in a safe environment before deployment. The authors are not responsible for any security breaches or physical damages resulting from the use of this software.
