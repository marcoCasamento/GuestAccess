# Deployment Guide

## 📦 Complete Deployment to GitHub and Docker Hub

Follow these steps to deploy your GuestAccess application.

### Step 1: Prepare Your Repositories

#### 1.1 Create Docker Hub Account
1. Go to [hub.docker.com](https://hub.docker.com)
2. Sign up or log in
3. Create an access token:
   - Click your profile → Account Settings → Security
   - Click "New Access Token"
   - Name it "github-actions"
   - Save the token (you'll need it later)

#### 1.2 Create GitHub Repository
1. Go to [github.com](https://github.com)
2. Click "New repository"
3. Name it "GuestAccess"
4. Make it **Private** (recommended for security)
5. DO NOT initialize with README (we already have files)
6. Click "Create repository"

### Step 2: Configure GitHub Secrets

1. Go to your GitHub repository
2. Click **Settings** → **Secrets and variables** → **Actions**
3. Click **New repository secret**
4. Add these secrets:

   | Secret Name | Value |
   |-------------|-------|
   | `DOCKERHUB_USERNAME` | Your Docker Hub username |
   | `DOCKERHUB_TOKEN` | The access token you created |

### Step 3: Push Code to GitHub

```bash
# Set your GitHub username and repository name
GITHUB_USERNAME="your-username"
REPO_NAME="GuestAccess"

# Add remote and push
git remote add origin https://github.com/$GITHUB_USERNAME/$REPO_NAME.git
git branch -M main
git push -u origin main
```

### Step 4: Trigger GitHub Action

The GitHub Action will automatically:
1. Build your Docker image
2. Push it to Docker Hub with tags:
   - `latest`
   - `main`
   - Git SHA

**Manually trigger** (if needed):
1. Go to **Actions** tab in GitHub
2. Select "Build and Push Docker Image"
3. Click "Run workflow"
4. Wait for completion (~2-5 minutes)

### Step 5: Deploy to Your Docker Host

#### 5.1 Prepare Your Server

SSH into your Docker server:
```bash
ssh user@your-server.com
```

#### 5.2 Create Directory Structure

```bash
mkdir -p ~/guestaccess/data
cd ~/guestaccess
```

#### 5.3 Create Environment File

```bash
nano .env
```

Paste and modify:
```env
DOCKERHUB_USERNAME=your-dockerhub-username
ADMIN_EMAIL=admin@yourdomain.com
ADMIN_PASSWORD=YourVerySecurePassword123!

# Optional: External Authentication
GOOGLE_CLIENT_ID=
GOOGLE_CLIENT_SECRET=
MICROSOFT_CLIENT_ID=
MICROSOFT_CLIENT_SECRET=
```

Save and exit (Ctrl+X, Y, Enter)

#### 5.4 Create Docker Compose File

```bash
nano docker-compose.yml
```

Paste:
```yaml
version: '3.8'

services:
  guestaccess:
    image: ${DOCKERHUB_USERNAME}/guestaccess:latest
    container_name: guestaccess
    ports:
      - "8080:8080"
    volumes:
      - ./data:/app/data
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:8080
      - AdminUser__Email=${ADMIN_EMAIL}
      - AdminUser__Password=${ADMIN_PASSWORD}
      - Authentication__Google__ClientId=${GOOGLE_CLIENT_ID}
      - Authentication__Google__ClientSecret=${GOOGLE_CLIENT_SECRET}
      - Authentication__Microsoft__ClientId=${MICROSOFT_CLIENT_ID}
      - Authentication__Microsoft__ClientSecret=${MICROSOFT_CLIENT_SECRET}
    restart: unless-stopped
    security_opt:
      - no-new-privileges:true
    networks:
      - guestaccess-network

networks:
  guestaccess-network:
    driver: bridge
```

Save and exit

#### 5.5 Deploy Container

```bash
# Pull latest image
docker-compose pull

# Start container
docker-compose up -d

# Check logs
docker-compose logs -f
```

Look for: `Admin user created: your-email@domain.com`

### Step 6: Configure Reverse Proxy (HTTPS)

For production, use a reverse proxy like Nginx or Traefik.

#### Option A: Nginx

```nginx
server {
    listen 443 ssl http2;
    server_name guestaccess.yourdomain.com;

    ssl_certificate /path/to/cert.pem;
    ssl_certificate_key /path/to/key.pem;

    location / {
        proxy_pass http://localhost:8080;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}

# Redirect HTTP to HTTPS
server {
    listen 80;
    server_name guestaccess.yourdomain.com;
    return 301 https://$server_name$request_uri;
}
```

#### Option B: Traefik (with Docker Compose)

Update your docker-compose.yml:
```yaml
services:
  guestaccess:
    image: ${DOCKERHUB_USERNAME}/guestaccess:latest
    container_name: guestaccess
    volumes:
      - ./data:/app/data
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:8080
      - AdminUser__Email=${ADMIN_EMAIL}
      - AdminUser__Password=${ADMIN_PASSWORD}
    restart: unless-stopped
    labels:
      - "traefik.enable=true"
      - "traefik.http.routers.guestaccess.rule=Host(`guestaccess.yourdomain.com`)"
      - "traefik.http.routers.guestaccess.entrypoints=websecure"
      - "traefik.http.routers.guestaccess.tls.certresolver=letsencrypt"
      - "traefik.http.services.guestaccess.loadbalancer.server.port=8080"
    networks:
      - traefik

networks:
  traefik:
    external: true
```

### Step 7: Test Deployment

1. **Access the Application**
   ```
   http://your-server-ip:8080
   or
   https://guestaccess.yourdomain.com
   ```

2. **Login as Admin**
   - Use the credentials from your `.env` file

3. **Create Test User**
   - Logout
   - Register new account
   - Login with new account
   - Verify "No permissions" message

4. **Grant Permissions**
   - Logout
   - Login as admin
   - Go to Admin panel
   - Grant permissions to test user
   - Click Save

5. **Test Access**
   - Logout
   - Login as test user
   - Verify buttons appear
   - Click button
   - Check server logs for action confirmation:
     ```bash
     docker-compose logs -f
     ```

### Step 8: Monitoring & Maintenance

#### View Logs
```bash
docker-compose logs -f guestaccess
```

#### Update to Latest Version
```bash
docker-compose pull
docker-compose up -d
```

#### Backup Database
```bash
# Create backup
cp data/app.db data/app.db.backup-$(date +%Y%m%d)

# Restore backup
cp data/app.db.backup-YYYYMMDD data/app.db
docker-compose restart
```

#### View Running Container
```bash
docker ps
```

#### Restart Container
```bash
docker-compose restart
```

#### Stop Container
```bash
docker-compose stop
```

#### Remove Container (keeps data)
```bash
docker-compose down
```

### Step 9: Integration with Home Assistant

Once deployed, integrate with Home Assistant:

1. **Get Home Assistant Long-Lived Access Token**
   - In Home Assistant: Profile → Security → Long-Lived Access Tokens
   - Create new token, save it

2. **Update Application** (future enhancement)
   - Modify `Pages/Index.cshtml.cs`
   - Add HTTP calls to Home Assistant API
   - Redeploy

Example integration code (for reference):
```csharp
private async Task TriggerHomeAssistant(string entityId, string action)
{
    using var httpClient = new HttpClient();
    httpClient.DefaultRequestHeaders.Authorization = 
        new AuthenticationHeaderValue("Bearer", _configuration["HomeAssistant:Token"]);
    
    var url = $"{_configuration["HomeAssistant:Url"]}/api/services/switch/{action}";
    var content = new StringContent(
        $"{{\"entity_id\":\"{entityId}\"}}",
        Encoding.UTF8,
        "application/json");
    
    await httpClient.PostAsync(url, content);
}
```

## 🔒 Security Checklist

Before going live:

- [ ] Changed default admin password
- [ ] Using HTTPS (SSL certificate installed)
- [ ] Environment variables set (not using defaults)
- [ ] `.env` file permissions restricted (chmod 600)
- [ ] Firewall configured (only ports 80/443 open)
- [ ] Database backups scheduled
- [ ] Monitoring/alerting configured
- [ ] Logs being reviewed
- [ ] External auth configured (Google/Microsoft)
- [ ] Tested all access controls
- [ ] Documented admin procedures
- [ ] Incident response plan ready

## 🆘 Troubleshooting

### Container won't start
```bash
# Check logs
docker-compose logs

# Check if port is in use
netstat -tulpn | grep 8080

# Try different port
# Edit docker-compose.yml: "8081:8080"
```

### Can't login
```bash
# Reset database
docker-compose down
rm data/app.db*
docker-compose up -d
# Admin user will be recreated
```

### GitHub Action failed
1. Check Actions tab for error details
2. Verify Docker Hub secrets are correct
3. Check if Docker Hub account has space
4. Retry workflow

### Image not updating
```bash
# Force pull new image
docker-compose pull
docker-compose down
docker-compose up -d
```

## 📞 Support

- Check logs first: `docker-compose logs -f`
- Review SECURITY.md for security issues
- Review QUICKSTART.md for usage help
- Check GitHub Issues (if public repo)
