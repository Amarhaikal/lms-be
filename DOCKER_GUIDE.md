# Docker Setup Guide for LMS Backend

Complete guide for containerizing and running the LMS backend using Docker.

## 📋 Prerequisites

- Docker installed (version 20.10+)
- Docker Compose installed (version 2.0+)

## 🏗️ Project Structure

```
lms-be/
├── Dockerfile              # Production build (optimized, small)
├── Dockerfile.dev          # Development build (with hot reload)
├── docker-compose.yml      # Production setup (for server)
├── docker-compose.dev.yml  # Development setup (for local Mac)
├── .dockerignore          # Files to exclude from Docker build
├── Jenkinsfile            # CI/CD pipeline
└── DOCKER_GUIDE.md        # This file
```

## 🎯 Understanding Dev vs Production

### **Development (Your Mac)**

- **File:** `docker-compose.dev.yml`
- **Port:** `5001` (backend), `3307` (MySQL)
- **Database:** Local MySQL in Docker (isolated, safe to experiment)
- **Hot Reload:** ✅ Code changes auto-restart
- **Use Case:** Daily coding, testing migrations, learning

### **Production (Server)**

- **File:** `docker-compose.yml`
- **Port:** `5000` (backend), `3306` (MySQL)
- **Database:** Production MySQL (real data)
- **Optimized:** ✅ Small image, fast startup
- **Use Case:** Deployment, real users

---

## 🚀 Quick Start

### Development Mode (Local Mac)

**First time setup:**

```bash
# 1. Start containers (MySQL + Backend + Adminer)
docker compose -f docker-compose.dev.yml up -d

# 2. Wait ~10 seconds for MySQL to be ready

# 3. Check status
docker compose -f docker-compose.dev.yml ps

# 4. Run migrations (create tables) - on your Mac
dotnet ef database update

# 5. Access API
open http://localhost:5001

# 6. Access Adminer (Database viewer)
open http://localhost:8081
```

**Daily workflow:**

```bash
# Start
docker compose -f docker-compose.dev.yml up -d

# Code in VS Code (changes auto-reload!)

# Stop (data is kept!)
docker compose -f docker-compose.dev.yml down
```

### Production Mode (Server)

```bash
# On server
cd /var/www/lms/lms-be

# Start
docker-compose up -d

# Run migrations
docker exec -it lms-backend dotnet ef database update

# Check logs
docker-compose logs -f
```

---

## 🔄 Switching Between Databases

### **Using Environment Files**

You can easily switch between local and staging databases using `.env` files.

**Setup:**

1. Create `.env.local` (for local Docker database):

```env
DB_HOST=mysql
DB_PORT=3306
DB_NAME=lms_db
DB_USER=lms_user
DB_PASSWORD=lms_password
```

2. Create `.env.staging` (for server database):

```env
DB_HOST=your-server-ip
DB_PORT=3306
DB_NAME=lms
DB_USER=root
DB_PASSWORD=your-server-password
```

3. Add to `.gitignore`:

```
.env.local
.env.staging
```

**Usage:**

```bash
# Use local database (default)
docker compose -f docker-compose.dev.yml up -d

# Use staging database
docker compose -f docker-compose.dev.yml --env-file .env.staging up -d

# Switch back to local
docker compose -f docker-compose.dev.yml down
docker compose -f docker-compose.dev.yml --env-file .env.local up -d
```

**Note:** `.env` files are ignored by Git, keeping your credentials safe! 🔒

---

## 📊 Resource Limits

Control CPU and memory usage for each container.

### **Current Configuration:**

**Development (docker-compose.dev.yml):**

- **Backend**: Max 1.5GB RAM, 0.5 CPU
- **MySQL**: Max 1GB RAM, 1.0 CPU

**Production (docker-compose.yml):**

- **Backend**: Max 1.5GB RAM, 0.5 CPU
- **MySQL**: Max 1GB RAM, 1.0 CPU

### **Monitor Resource Usage:**

```bash
# View real-time stats
docker stats

# View specific container
docker stats lms-backend-dev
```

### **Adjust Limits:**

Edit `docker-compose.dev.yml` or `docker-compose.yml`:

```yaml
services:
  lms-backend:
    deploy:
      resources:
        limits:
          cpus: "1.0" # Maximum CPU cores
          memory: 2G # Maximum RAM
        reservations:
          cpus: "0.5" # Guaranteed CPU
          memory: 512M # Guaranteed RAM
```

---

## 💾 Data Persistence

### **Your data is SAFE!**

Docker uses **volumes** to store database data permanently:

| Action                    | Database Data  |
| ------------------------- | -------------- |
| Stop containers (`down`)  | ✅ **Kept**    |
| Turn off laptop           | ✅ **Kept**    |
| Restart laptop            | ✅ **Kept**    |
| Start containers again    | ✅ **Kept**    |
| Delete volume (`down -v`) | ❌ **DELETED** |

**Where is data stored?**

- Docker manages volumes automatically
- Data survives container restarts
- Only deleted if you use `docker-compose down -v`

**Fresh start (delete all data):**

```bash
docker-compose -f docker-compose.dev.yml down -v  # ← -v deletes volumes!
docker-compose -f docker-compose.dev.yml up -d    # Fresh database
```

---

## 🔍 Viewing Database Data

### **Method 1: Adminer (Web Interface) - Recommended! ✨**

**Access:** http://localhost:8081

**Login:**

- **System:** MySQL
- **Server:** mysql
- **Username:** lms_user
- **Password:** lms_password
- **Database:** lms_db

**Features:**

- ✅ Easy to use web interface
- ✅ View and edit tables
- ✅ Run SQL queries
- ✅ Export data
- ✅ No installation needed

**Already included in docker-compose.dev.yml!**

---

### **Method 2: Command Line**

```bash
# Access MySQL CLI
docker exec -i lms-mysql-dev mysql -u lms_user -plms_password lms_db

# Inside MySQL:
mysql> SHOW TABLES;
mysql> SELECT * FROM Users;
mysql> exit;
```

**One-liner queries:**

```bash
# Show tables
docker exec lms-mysql-dev mysql -u lms_user -plms_password lms_db -e "SHOW TABLES;"

# Query data
docker exec lms-mysql-dev mysql -u lms_user -plms_password lms_db -e "SELECT * FROM Users;"
```

### **Method 3: GUI Tools**

**Connection Settings:**

- **Host:** `localhost` or `127.0.0.1`
- **Port:** `3307` (dev) or `3306` (prod)
- **Username:** `lms_user`
- **Password:** `lms_password`
- **Database:** `lms_db`

**Recommended Tools:**

1. **MySQL Workbench** (Free)

   - Download: https://dev.mysql.com/downloads/workbench/
   - Full-featured, official MySQL tool

2. **TablePlus** (Mac)

   - Download: https://tableplus.com/
   - Beautiful, native Mac app

3. **DBeaver** (Free, Cross-platform)

   - Download: https://dbeaver.io/
   - Powerful, supports many databases

4. **VS Code Extension**
   - Install: "MySQL" by Jun Han
   - View database directly in VS Code

### **Method 3: Web Interface (Adminer)**

Add to `docker-compose.dev.yml`:

```yaml
services:
  # ... existing services ...

  adminer:
    image: adminer
    container_name: lms-adminer
    restart: unless-stopped
    ports:
      - "8081:8080"
    networks:
      - lms-network-dev
```

Then access at: http://localhost:8081

- **System:** MySQL
- **Server:** mysql
- **Username:** lms_user
- **Password:** lms_password
- **Database:** lms_db

---

## 🗄️ Database Management

### **Understanding Migration Operations**

There are **two different operations** when working with migrations:

1. **Creating Migrations** (design-time) - Must be done on your Mac
2. **Applying Migrations** (runtime) - Can be done either way

---

### **Create New Migration** ✏️

**Always run this on your Mac (outside Docker):**

```bash
# On your Mac
dotnet ef migrations add MigrationName
```

**Why?** Creating migrations requires design-time tools and connection configuration that works best outside the container.

---

### **Apply Migrations** 🚀

**Recommended: Run on your Mac (easiest for local development)**

```bash
# On your Mac - connects to Docker MySQL on localhost:3307
dotnet ef database update
```

**Alternative: Run inside Docker container**

```bash
# Inside container (may require additional configuration)
docker exec -it lms-backend-dev dotnet ef database update
```

**Production (Server):**

```bash
# On server
docker exec -it lms-backend dotnet ef database update
```

---

### **Complete Migration Workflow**

```bash
# 1. Make changes to your DbContext.cs or models

# 2. Create migration (on your Mac)
dotnet ef migrations add AddNewFeature

# 3. Review the generated migration file in Migrations/ folder

# 4. Apply migration (on your Mac)
dotnet ef database update

# 5. Verify in database
docker exec lms-mysql-dev mysql -u lms_user -plms_password lms_db -e "SHOW TABLES;"
```

---

### **Other Migration Commands**

```bash
# View migration status (on your Mac)
dotnet ef migrations list

# Rollback to previous migration (on your Mac)
dotnet ef database update PreviousMigrationName

# Remove last migration (on your Mac, before applying it)
dotnet ef migrations remove
```

---

## 🔧 Common Commands

### **Container Management**

```bash
# Start containers
docker-compose -f docker-compose.dev.yml up -d

# Stop containers (keep data)
docker-compose -f docker-compose.dev.yml down

# Stop and delete data
docker-compose -f docker-compose.dev.yml down -v

# Restart containers
docker-compose -f docker-compose.dev.yml restart

# Rebuild containers
docker-compose -f docker-compose.dev.yml up -d --build
```

### **Logs & Debugging**

```bash
# View all logs
docker-compose -f docker-compose.dev.yml logs -f

# View specific service logs
docker-compose -f docker-compose.dev.yml logs -f lms-backend

# Check container status
docker-compose -f docker-compose.dev.yml ps

# Execute commands in container
docker exec -it lms-backend-dev bash
```

### **Database Backup & Restore**

```bash
# Backup database
docker exec lms-mysql-dev mysqldump -u lms_user -plms_password lms_db > backup.sql

# Restore database
docker exec -i lms-mysql-dev mysql -u lms_user -plms_password lms_db < backup.sql
```

---

## 🔄 Complete Development Workflow

### **Day 1 - Initial Setup**

```bash
# 1. Start Docker containers
cd /Users/amarhaikal/Documents/coding/aspnet/lms-be
docker-compose -f docker-compose.dev.yml up -d

# 2. Wait for MySQL to be ready (~10 seconds)
docker-compose -f docker-compose.dev.yml logs -f mysql

# 3. Create database tables (run on your Mac)
dotnet ef database update

# 4. Verify it's working
open http://localhost:5001

# 5. View database (optional)
# Use MySQL Workbench, Adminer (http://localhost:8081), or command line
```

### **Daily Development**

```bash
# Morning: Start containers
docker-compose -f docker-compose.dev.yml up -d

# Code in VS Code
# Changes auto-reload! ✨

# When you modify DbContext or models:
# 1. Create migration (on your Mac)
dotnet ef migrations add NewFeature

# 2. Apply migration (on your Mac)
dotnet ef database update

# Evening: Stop containers (data kept!)
docker-compose -f docker-compose.dev.yml down
```

### **Deploy to Server**

```bash
# 1. Commit and push code
git add .
git commit -m "Add new feature"
git push

# 2. SSH to server
ssh root@your-server
cd /var/www/lms/lms-be

# 3. Pull latest code
git pull

# 4. Deploy
docker-compose up -d --build

# 5. Run migrations
docker exec -it lms-backend dotnet ef database update

# 6. Verify
curl http://localhost:5000
```

---

## 🚢 Server Deployment

### **Option 1: Manual Deployment**

```bash
# On server
cd /var/www/lms/lms-be
git pull
docker-compose up -d --build
docker exec -it lms-backend dotnet ef database update
```

### **Option 2: Jenkins CI/CD**

The included `Jenkinsfile` automates:

1. Build Docker image
2. Run tests
3. Stop old containers
4. Deploy new containers
5. Health check
6. Cleanup old images

**Setup Jenkins:**

1. Create new Pipeline job
2. Point to your Git repository
3. Jenkins will use the `Jenkinsfile`
4. Push code → Auto-deploy! 🚀

---

## 🔐 Security Best Practices

### **Change Default Passwords**

Edit `docker-compose.yml` and `docker-compose.dev.yml`:

```yaml
environment:
  MYSQL_ROOT_PASSWORD: your_secure_password_here
  MYSQL_PASSWORD: your_secure_password_here
  ConnectionStrings__DefaultConnection: Server=mysql;Database=lms_db;User=lms_user;Password=your_secure_password_here;
```

### **Use Environment Files**

Create `.env` file:

```env
MYSQL_ROOT_PASSWORD=secure_root_pass
MYSQL_PASSWORD=secure_user_pass
```

Update `docker-compose.yml`:

```yaml
environment:
  MYSQL_ROOT_PASSWORD: ${MYSQL_ROOT_PASSWORD}
  MYSQL_PASSWORD: ${MYSQL_PASSWORD}
```

---

## 🐛 Troubleshooting

### **Container won't start**

```bash
# Check logs
docker-compose -f docker-compose.dev.yml logs

# Check specific service
docker-compose -f docker-compose.dev.yml logs lms-backend
```

### **Database connection failed**

```bash
# Check if MySQL is healthy
docker-compose -f docker-compose.dev.yml ps

# Should show "healthy" status
# If not, wait longer or check logs
docker-compose -f docker-compose.dev.yml logs mysql
```

### **Port already in use**

```bash
# Check what's using the port
lsof -i :5001

# Kill the process or change port in docker-compose.dev.yml:
ports:
  - "5002:8080"  # Use different port
```

### **Migrations fail**

```bash
# Check if database is ready
docker exec -it lms-mysql-dev mysql -u lms_user -plms_password -e "SHOW DATABASES;"

# Manually run migrations with verbose output
docker exec -it lms-backend-dev dotnet ef database update --verbose
```

### **Reset everything**

```bash
# Nuclear option: delete everything and start fresh
docker-compose -f docker-compose.dev.yml down -v
docker system prune -a -f
docker-compose -f docker-compose.dev.yml up -d --build
docker exec -it lms-backend-dev dotnet ef database update
```

---

## � Understanding Docker Concepts

### **Dockerfile vs docker-compose.yml**

| Dockerfile                   | docker-compose.yml                   |
| ---------------------------- | ------------------------------------ |
| Recipe for **one** container | Orchestrates **multiple** containers |
| How to build image           | How to run services together         |
| `FROM`, `COPY`, `RUN`        | `services`, `networks`, `volumes`    |

### **Images vs Containers**

- **Image:** Blueprint (like a class)
- **Container:** Running instance (like an object)

```bash
# List images
docker images

# List running containers
docker ps

# List all containers
docker ps -a
```

### **Volumes**

- Permanent storage for data
- Survives container restarts
- Only deleted with `down -v`

```bash
# List volumes
docker volume ls

# Inspect volume
docker volume inspect lms-be_mysql_data_dev

# Delete specific volume
docker volume rm lms-be_mysql_data_dev
```

---

## 🎯 Next Steps

- [x] Docker setup for backend
- [ ] Add Adminer for easy database viewing
- [ ] Create frontend Dockerfile (when lms-fe is ready)
- [ ] Configure Nginx reverse proxy
- [ ] Set up SSL certificates
- [ ] Configure monitoring (Prometheus/Grafana)
- [ ] Set up automated backups

---

## 📚 Additional Resources

- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [ASP.NET Core Docker Guide](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/docker/)
- [MySQL Docker Hub](https://hub.docker.com/_/mysql)

---

## 💡 Tips & Tricks

**View resource usage:**

```bash
docker stats
```

**Clean up disk space:**

```bash
docker system df  # Show disk usage
docker system prune -a  # Clean up everything
```

**Export/Import images:**

```bash
# Export
docker save lms-backend:latest > lms-backend.tar

# Import
docker load < lms-backend.tar
```

**Quick database reset:**

```bash
# Alias for fresh start (add to ~/.zshrc)
alias lms-reset='docker-compose -f docker-compose.dev.yml down -v && docker-compose -f docker-compose.dev.yml up -d && sleep 10 && docker exec -it lms-backend-dev dotnet ef database update'
```

---

**Happy Dockerizing! 🐳**
