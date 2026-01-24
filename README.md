# QUANTM Backend Documentation

Complete documentation for the Quantm Learning Management System Backend.

---

## 📚 Documentation Index

### **Getting Started**

- [Docker Setup Guide](DOCUMENTATIONS/DOCKER_GUIDE.md) - Complete Docker setup for development and production
- [Migration Guide](DOCUMENTATIONS/MIGRATION_GUIDE.md) - Database migration workflows

### **Quick Links**

| Topic              | Guide                                                                                                     | Description                                       |
| ------------------ | --------------------------------------------------------------------------------------------------------- | ------------------------------------------------- |
| 🐳 **Docker**      | [DOCKER_GUIDE.md](DOCUMENTATIONS/DOCKER_GUIDE.md)                                                         | Container setup, development workflow, deployment |
| 🗄️ **Migrations**  | [MIGRATION_GUIDE.md](DOCUMENTATIONS/MIGRATION_GUIDE.md)                                                   | Database migrations for local and server          |
| 🔧 **Environment** | [DOCKER_GUIDE.md#switching-between-databases](DOCUMENTATIONS/DOCKER_GUIDE.md#switching-between-databases) | .env files and database switching                 |
| 🚀 **Deployment**  | [DOCKER_GUIDE.md#production-mode-server](DOCUMENTATIONS/DOCKER_GUIDE.md#production-mode-server)           | Server deployment with Jenkins                    |

---

## 🚀 Quick Start

### **First Time Setup**

```bash
# 1. Clone repository
git clone https://github.com/Amarhaikal/quantm-be.git
cd quantm-be

# 2. Create environment files
cp .env.example .env.local
cp appsettings.example.json appsettings.json
cp appsettings.Development.example.json appsettings.Development.json

# 3. Start Docker containers
docker compose -f docker-compose.dev.yml up -d

# 4. Run migrations
dotnet ef database update

# 5. Access application
open http://localhost:5001        # API
open http://localhost:8081        # Adminer (Database viewer)
```

---

## 📖 Documentation Structure

```
DOCUMENTATIONS/
├── DOCKER_GUIDE.md          # Docker setup and workflows
└── MIGRATION_GUIDE.md       # Database migration guide
```

---

## 🔗 External Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [Docker Documentation](https://docs.docker.com/)
- [MySQL Documentation](https://dev.mysql.com/doc/)

---

## 💡 Common Tasks

### **Development**

```bash
# Start development environment
docker compose -f docker-compose.dev.yml up -d

# View logs
docker compose -f docker-compose.dev.yml logs -f

# Stop environment
docker compose -f docker-compose.dev.yml down
```

### **Database**

```bash
# Create migration
dotnet ef migrations add MigrationName

# Apply migration locally
dotnet ef database update

# View database
open http://localhost:8081
```

### **Deployment**

```bash
# Commit and push
git add .
git commit -m "Your message"
git push

# Apply migration to server
export ConnectionStrings__DefaultConnection="Server=SERVER_IP;Port=3307;Database=quantm_db;User=quantm_user;Password=PASSWORD;"
dotnet ef database update
```

---

## 🆘 Need Help?

1. Check the relevant guide in `DOCUMENTATIONS/`
2. Review troubleshooting sections
3. Check Docker/application logs
4. Verify environment configuration

---

**Happy coding!** 🚀
