# 🚀 Quick Reference Card

## Essential Commands

### Local Development
```bash
# Run application
dotnet run --urls "http://localhost:5000"

# Or use script
./run-local.sh        # Linux/Mac
run-local.bat         # Windows

# Access: http://localhost:5000
# Admin: admin@localhost / Admin123!
```

### Docker Testing
```bash
# Build image
docker build -t guestaccess:test .

# Run container
docker run -d -p 8080:8080 \
  -e AdminUser__Email=admin@test.com \
  -e AdminUser__Password=Test123! \
  guestaccess:test

# Access: http://localhost:8080
```

### Production Deployment
```bash
# On server
cd ~/guestaccess
docker-compose pull
docker-compose up -d
docker-compose logs -f
```

## Default Credentials
⚠️ **CHANGE IN PRODUCTION!**
- Email: `admin@localhost`
- Password: `Admin123!`

## Quick Setup Checklist

### 1️⃣ Local Testing
- [ ] Run `dotnet run`
- [ ] Login at http://localhost:5000
- [ ] Test admin panel
- [ ] Create test user
- [ ] Grant permissions
- [ ] Test buttons

### 2️⃣ GitHub Setup
- [ ] Create GitHub repo
- [ ] Get Docker Hub token
- [ ] Add GitHub secrets:
  - `DOCKERHUB_USERNAME`
  - `DOCKERHUB_TOKEN`
- [ ] Push code: `git push origin main`
- [ ] Verify GitHub Action runs

### 3️⃣ Server Deployment
- [ ] Create `.env` file
- [ ] Set strong admin password
- [ ] Create `docker-compose.yml`
- [ ] Run `docker-compose up -d`
- [ ] Configure HTTPS/SSL
- [ ] Test all functionality

## Directory Structure
```
GuestAccess/
├── .github/workflows/        # CI/CD
├── Models/                   # User & permissions
├── Pages/                    # UI & logic
├── Data/                     # Database
├── wwwroot/                  # Static files
├── Dockerfile                # Container
├── docker-compose.yml        # Orchestration
└── *.md                      # Documentation
```

## Key Files to Configure

### appsettings.json
```json
{
  "AdminUser": {
    "Email": "admin@yourdomain.com",
    "Password": "YourSecurePassword123!"
  },
  "Authentication": {
    "Google": { "ClientId": "...", "ClientSecret": "..." },
    "Microsoft": { "ClientId": "...", "ClientSecret": "..." }
  }
}
```

### .env (Production)
```env
ADMIN_EMAIL=admin@yourdomain.com
ADMIN_PASSWORD=SecurePass123!
GOOGLE_CLIENT_ID=your-google-client-id
GOOGLE_CLIENT_SECRET=your-google-secret
MICROSOFT_CLIENT_ID=your-microsoft-client-id
MICROSOFT_CLIENT_SECRET=your-microsoft-secret
```

## Common Issues & Solutions

### Build Fails
```bash
dotnet clean
dotnet restore
dotnet build
```

### Database Issues
```bash
# Reset database
rm app.db*
dotnet ef database update
```

### Container Won't Start
```bash
# Check logs
docker logs guestaccess

# Restart
docker-compose restart
```

### Can't Login
- Check credentials in appsettings.json
- Clear browser cache/cookies
- Check console for errors

## URLs & Endpoints

| Purpose | URL |
|---------|-----|
| Home Page | `/` |
| Login | `/Identity/Account/Login` |
| Register | `/Identity/Account/Register` |
| Admin Panel | `/Admin` |
| Logout | `/Identity/Account/Logout` |

## Security Quick Tips

1. ✅ Always use HTTPS in production
2. ✅ Change default admin password
3. ✅ Use environment variables for secrets
4. ✅ Review logs regularly
5. ✅ Keep dependencies updated
6. ✅ Backup database regularly
7. ✅ Restrict network access
8. ✅ Enable 2FA (future)

## Useful Docker Commands

```bash
# View logs
docker-compose logs -f

# Restart
docker-compose restart

# Stop
docker-compose stop

# Update
docker-compose pull && docker-compose up -d

# Backup database
docker-compose exec guestaccess cp /app/data/app.db /app/data/backup.db

# Shell into container
docker-compose exec guestaccess /bin/bash
```

## Permission Management

### Grant Permissions
1. Login as admin
2. Go to `/Admin`
3. Check boxes for user
4. Click "Save"

### Revoke Permissions
1. Login as admin
2. Go to `/Admin`
3. Uncheck boxes for user
4. Click "Save"

## Testing Workflow

```mermaid
User Registers → No Permissions → Admin Grants → User Sees Buttons → User Clicks → Action Logged
```

## Monitoring

### Check Application Health
```bash
curl http://localhost:8080/
```

### View Recent Actions
```bash
docker-compose logs | grep "APRI"
```

### Database Size
```bash
ls -lh data/app.db
```

## Support Documentation

| Topic | File |
|-------|------|
| Overview | README.md |
| Getting Started | QUICKSTART.md |
| Deployment | DEPLOYMENT.md |
| Security | SECURITY.md |
| Summary | PROJECT_COMPLETE.md |
| Quick Ref | QUICK_REFERENCE.md |

## Next Steps After Deployment

1. 🔐 Change admin password
2. 🌐 Configure HTTPS
3. 👥 Create user accounts
4. 🎫 Grant permissions
5. 🏠 Integrate Home Assistant
6. 📊 Set up monitoring
7. 💾 Schedule backups

---

**Keep this card handy for quick reference!**
