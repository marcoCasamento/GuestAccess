# Quick Start Guide

## 🚀 Getting Started

### 1. First Time Setup

**Change the default admin credentials** in `appsettings.json`:

```json
{
  "AdminUser": {
    "Email": "your-admin@yourdomain.com",
    "Password": "YourSecurePassword123!"
  }
}
```

### 2. Run the Application

```bash
dotnet run
```

Navigate to: `https://localhost:5001`

### 3. Login as Admin

Use the credentials you set in step 1 to login.

### 4. Create Test Users

1. Logout from the admin account
2. Click "Register" and create a new user account
3. Login with the new account - you should see "No permissions assigned"
4. Logout

### 5. Grant Permissions

1. Login as admin
2. Click "Admin" in the navigation menu
3. You'll see the list of all users
4. Check the boxes for the permissions you want to grant
5. Click "Save" for each user

### 6. Test Access Controls

1. Logout and login as the regular user
2. You should now see the buttons you granted permission for
3. Click each button - check the console output for the log messages

## 🐳 Docker Deployment

### Build Docker Image

```bash
docker build -t guestaccess:local .
```

### Run with Docker

```bash
docker run -d \
  -p 8080:8080 \
  -v $(pwd)/data:/app/data \
  -e AdminUser__Email=admin@yourdomain.com \
  -e AdminUser__Password=YourPassword123! \
  --name guestaccess \
  guestaccess:local
```

Access at: `http://localhost:8080`

### Using Docker Compose

1. Copy `.env.example` to `.env`
2. Edit `.env` and set your credentials
3. Run:

```bash
docker-compose up -d
```

## 📋 GitHub Setup

### 1. Create GitHub Repository

```bash
git add .
git commit -m "Initial commit - Secure GuestAccess application"
git branch -M main
git remote add origin https://github.com/YOUR_USERNAME/GuestAccess.git
git push -u origin main
```

### 2. Configure GitHub Secrets

Go to your repository → Settings → Secrets and variables → Actions

Add these secrets:
- `DOCKERHUB_USERNAME`: Your Docker Hub username
- `DOCKERHUB_TOKEN`: Your Docker Hub access token

### 3. Trigger GitHub Action

Push to main branch or manually trigger the workflow:
- Go to Actions tab
- Select "Build and Push Docker Image"
- Click "Run workflow"

The action will build and push the Docker image to Docker Hub.

## 🔐 Security Checklist

Before deploying to production:

- [ ] Change default admin password
- [ ] Configure HTTPS/SSL certificates
- [ ] Set strong authentication secrets for Google/Microsoft OAuth
- [ ] Review and restrict network access
- [ ] Enable firewall rules
- [ ] Set up database backups
- [ ] Review security logs regularly
- [ ] Keep dependencies updated
- [ ] Consider enabling 2FA for admin accounts
- [ ] Use environment variables for all secrets (never commit them!)

## 🏠 Home Assistant Integration (Future)

To integrate with Home Assistant, modify the POST handlers in `Pages/Index.cshtml.cs`:

```csharp
public async Task<IActionResult> OnPostOpenCancelloAsync()
{
    var user = await _userManager.GetUserAsync(User);
    if (user == null || !user.CanOpenCancello)
    {
        return Forbid();
    }

    // Log the action
    _logger.LogInformation("Opening Cancello for user {Email}", user.Email);
    Console.WriteLine($"🚪 APRI CANCELLO - User: {user.Email}");

    // Call Home Assistant
    using var httpClient = new HttpClient();
    httpClient.DefaultRequestHeaders.Authorization = 
        new AuthenticationHeaderValue("Bearer", _configuration["HomeAssistant:Token"]);
    
    var content = new StringContent(
        "{\"entity_id\":\"switch.cancello\"}", 
        Encoding.UTF8, 
        "application/json");
    
    await httpClient.PostAsync(
        "http://homeassistant.local:8123/api/services/switch/turn_on",
        content);

    Message = "Cancello aperto con successo!";
    return RedirectToPage();
}
```

## 🆘 Troubleshooting

### Database locked error
- Stop the application
- Delete `app.db-shm` and `app.db-wal`
- Restart

### Can't login after password change
- Delete the database file
- Run `dotnet ef database update` to recreate

### Docker container won't start
- Check logs: `docker logs guestaccess`
- Ensure volume paths exist
- Check port conflicts

### External auth not working
- Verify redirect URIs in Google/Microsoft console
- Check that secrets are properly configured
- Ensure HTTPS is enabled (required by OAuth)

## 📞 Support

Check the logs for detailed error messages:
- Application logs: Check console output or Docker logs
- Database issues: Check SQLite database with `sqlite3 app.db`
- Authentication issues: Check ASP.NET Core Identity logs
