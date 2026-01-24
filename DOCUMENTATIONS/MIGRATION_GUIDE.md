# Database Migration Guide

Complete guide for managing database migrations in the QUANTM Backend project.

---

## 📋 Table of Contents

1. [Understanding Migrations](#understanding-migrations)
2. [Local Development Migrations](#local-development-migrations)
3. [Server/Production Migrations](#serverproduction-migrations)
4. [Common Scenarios](#common-scenarios)
5. [Troubleshooting](#troubleshooting)

---

## 🎯 Understanding Migrations

**What are migrations?**

- Migrations are version control for your database schema
- Each migration represents a change to the database structure
- Migrations are stored as code files in the `Migrations/` folder
- They can be applied, rolled back, and shared across environments

**Migration Files:**

- `YYYYMMDDHHMMSS_MigrationName.cs` - The migration code
- `YYYYMMDDHHMMSS_MigrationName.Designer.cs` - Metadata
- `ApplicationDbContextModelSnapshot.cs` - Current schema snapshot

---

## 💻 Local Development Migrations

### **Prerequisites**

Make sure your local Docker containers are running:

```bash
# Check status
docker compose -f docker-compose.dev.yml ps

# If not running, start them
docker compose -f docker-compose.dev.yml up -d
```

### **Step 1: Make Changes to Your Models**

Edit your model files (e.g., `Models/User/User.cs`, `DbContext.cs`)

**Example - Adding a new property:**

```csharp
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string PhoneNumber { get; set; }  // ← New property
}
```

**Example - Adding seed data in DbContext.cs:**

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Country>().HasData(
        new Country { Id = 1, Name = "Malaysia", Code = "MY" },
        new Country { Id = 2, Name = "Singapore", Code = "SG" }
    );
}
```

### **Step 2: Create Migration**

```bash
cd /Users/amarhaikal/Documents/coding/aspnet/quantm-be

# Create migration with descriptive name
dotnet ef migrations add AddPhoneNumberToUser

# Or for seed data
dotnet ef migrations add AddCountryAndStateSeedData
```

**Migration naming conventions:**

- `Add[Feature]` - Adding new tables/columns
- `Update[Feature]` - Modifying existing structure
- `Remove[Feature]` - Removing tables/columns
- `Fix[Issue]` - Fixing data or schema issues

### **Step 3: Review Generated Migration**

Check the generated file in `Migrations/` folder:

```csharp
public partial class AddPhoneNumberToUser : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "PhoneNumber",
            table: "Users",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "PhoneNumber",
            table: "Users");
    }
}
```

### **Step 4: Apply Migration to Local Database**

```bash
# Apply migration
dotnet ef database update

# Verify in Adminer
open http://localhost:8081
```

**Login to Adminer:**

- System: MySQL
- Server: mysql
- Username: quantm_user
- Password: quantm_password
- Database: quantm_db

### **Step 5: Test Your Changes**

```bash
# Run your application
docker compose -f docker-compose.dev.yml up -d

# Test API endpoints
curl http://localhost:5001/api/your-endpoint
```

---

## 🚀 Server/Production Migrations

### **Step 1: Commit and Push Migration Files**

```bash
# Add migration files
git add Migrations/
git add Models/
git add DbContext.cs

# Commit
git commit -m "Add phone number field to User model"

# Push to GitHub
git push origin develop
```

### **Step 2: Wait for Jenkins Deployment**

Jenkins will automatically:

1. ✅ Pull latest code
2. ✅ Build Docker image
3. ✅ Deploy to server
4. ⚠️ **Does NOT run migrations** (you do this manually)

Check Jenkins: http://your-jenkins-url

### **Step 3: Run Migration on Server Database**

**From your Mac, run:**

```bash
# Set connection string to server
export ConnectionStrings__DefaultConnection="Server=YOUR_SERVER_IP;Port=3307;Database=lms_db;User=lms_user;Password=YOUR_SERVER_PASSWORD;"

# Apply migration
dotnet ef database update

# Verify
dotnet ef migrations list
```

**Replace:**

- `YOUR_SERVER_IP` - Your server's IP address (e.g., `103.123.45.67`)
- `YOUR_SERVER_PASSWORD` - Password from `/var/www/quantm/quantm-be/.env` on server

**Example:**

```bash
export ConnectionStrings__DefaultConnection="Server=103.123.45.67;Port=3307;Database=quantm_db;User=quantm_user;Password=MySecurePass123;"
dotnet ef database update
```

### **Step 4: Verify on Server**

```bash
# SSH to server
ssh root@your-server-ip

# Check database
docker exec quantm-mysql mysql -u quantm_user -pquantm_password quantm_db -e "DESCRIBE Users;"

# Check application logs
docker compose logs quantm-backend
```

---

## 📚 Common Scenarios

### **Scenario 1: Adding a New Table**

```bash
# 1. Create model class
# Models/Course/Course.cs

# 2. Add DbSet to DbContext
# public DbSet<Course> Courses { get; set; }

# 3. Create migration
dotnet ef migrations add AddCourseTable

# 4. Apply locally
dotnet ef database update

# 5. Test, commit, push
git add .
git commit -m "Add Course table"
git push

# 6. Apply to server
export ConnectionStrings__DefaultConnection="Server=server-ip;Port=3307;Database=quantm_db;User=quantm_user;Password=pass;"
dotnet ef database update
```

### **Scenario 2: Adding Seed Data**

```bash
# 1. Update DbContext.cs OnModelCreating method
# modelBuilder.Entity<Country>().HasData(...)

# 2. Create migration
dotnet ef migrations add AddCountrySeedData

# 3. Apply locally
dotnet ef database update

# 4. Verify in Adminer
open http://localhost:8081

# 5. Commit and push
git add .
git commit -m "Add country seed data"
git push

# 6. Apply to server
export ConnectionStrings__DefaultConnection="Server=server-ip;Port=3307;Database=quantm_db;User=quantm_user;Password=pass;"
dotnet ef database update
```

### **Scenario 3: Modifying Existing Column**

```bash
# 1. Update model property
# public string Username { get; set; }  // Changed from nullable

# 2. Create migration
dotnet ef migrations add MakeUsernameRequired

# 3. Review migration (may need manual adjustments)
# Check Migrations/YYYYMMDDHHMMSS_MakeUsernameRequired.cs

# 4. Apply locally
dotnet ef database update

# 5. Test thoroughly!

# 6. Commit, push, and apply to server
```

### **Scenario 4: Removing a Migration (Not Yet Applied)**

```bash
# Remove last migration
dotnet ef migrations remove

# This deletes the migration files
# Only works if migration hasn't been applied to database
```

### **Scenario 5: Rolling Back a Migration**

```bash
# Rollback to specific migration
dotnet ef database update PreviousMigrationName

# Rollback all migrations
dotnet ef database update 0

# Then remove the migration file
dotnet ef migrations remove
```

---

## 🔍 Useful Commands

### **Check Migration Status**

```bash
# List all migrations
dotnet ef migrations list

# Check pending migrations
dotnet ef migrations list | grep -v "Applied"

# View database connection
dotnet ef dbcontext info
```

### **Generate SQL Script (Without Applying)**

```bash
# Generate SQL for review
dotnet ef migrations script

# Generate SQL for specific migration
dotnet ef migrations script PreviousMigration TargetMigration

# Output to file
dotnet ef migrations script > migration.sql
```

### **Reset Database (Development Only!)**

```bash
# Drop all tables and reapply migrations
dotnet ef database drop
dotnet ef database update

# Or using Docker
docker compose -f docker-compose.dev.yml down -v
docker compose -f docker-compose.dev.yml up -d
dotnet ef database update
```

---

## ⚠️ Troubleshooting

### **Error: "Unable to connect to any of the specified MySQL hosts"**

**Cause:** Connection string is incorrect or database is not running.

**Solution:**

```bash
# Check if Docker MySQL is running
docker compose -f docker-compose.dev.yml ps

# Verify connection string in appsettings.Development.json
cat appsettings.Development.json

# Should be:
# "Server=localhost;Port=3307;Database=quantm_db;User=quantm_user;Password=quantm_password;"
```

### **Error: "Access denied for user 'root'@'192.168.65.1'"**

**Cause:** Using wrong credentials or connecting to wrong database.

**Solution:**

```bash
# For local development, update appsettings.Development.json:
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3307;Database=quantm_db;User=quantm_user;Password=quantm_password;"
  }
}
```

### **Error: "The command could not be loaded... 'ef' does not exist"**

**Cause:** dotnet-ef tools not installed.

**Solution:**

```bash
# Install globally
dotnet tool install --global dotnet-ef

# Or update
dotnet tool update --global dotnet-ef

# Verify
dotnet ef --version
```

### **Migration Applied to Wrong Database**

**Solution:**

```bash
# 1. Rollback the migration
export ConnectionStrings__DefaultConnection="Server=wrong-db;Port=3307;..."
dotnet ef database update PreviousMigrationName

# 2. Apply to correct database
export ConnectionStrings__DefaultConnection="Server=correct-db;Port=3307;..."
dotnet ef database update
```

### **Migration Files Out of Sync**

**Cause:** Different migrations on local vs server.

**Solution:**

```bash
# 1. Check migration status on both
dotnet ef migrations list

# 2. Pull latest code
git pull

# 3. Apply missing migrations
dotnet ef database update
```

---

## ✅ Best Practices

### **1. Always Test Locally First**

```bash
# Create → Apply → Test → Commit
dotnet ef migrations add FeatureName
dotnet ef database update
# Test thoroughly
git add . && git commit -m "..." && git push
```

### **2. Use Descriptive Migration Names**

```bash
# Good ✅
dotnet ef migrations add AddEmailVerificationToUser
dotnet ef migrations add UpdateUserStatusEnum
dotnet ef migrations add AddCountrySeedData

# Bad ❌
dotnet ef migrations add Update1
dotnet ef migrations add Fix
dotnet ef migrations add Changes
```

### **3. Review Generated Migrations**

- Always check the generated migration code
- Ensure `Up()` and `Down()` methods are correct
- Add custom SQL if needed

### **4. Backup Before Major Changes**

```bash
# On server, backup database before migration
ssh root@server
docker exec quantm-mysql mysqldump -u quantm_user -pquantm_password quantm_db > backup.sql
```

### **5. Keep Migrations Small**

- One migration per logical change
- Easier to review and rollback
- Better Git history

### **6. Never Edit Applied Migrations**

- Once a migration is applied and pushed, don't edit it
- Create a new migration to fix issues
- Editing causes sync problems

---

## 📋 Quick Reference

### **Local Development**

```bash
# Create migration
dotnet ef migrations add MigrationName

# Apply migration
dotnet ef database update

# View in browser
open http://localhost:8081
```

### **Server/Production**

```bash
# After git push and Jenkins deploy
export ConnectionStrings__DefaultConnection="Server=SERVER_IP;Port=3307;Database=quantm_db;User=quantm_user;Password=PASSWORD;"
dotnet ef database update
```

### **Rollback**

```bash
# Rollback to previous migration
dotnet ef database update PreviousMigrationName

# Remove last migration (if not applied)
dotnet ef migrations remove
```

---

## 🎯 Complete Workflow Example

```bash
# 1. Start local environment
docker compose -f docker-compose.dev.yml up -d

# 2. Make changes to models
# Edit Models/User/User.cs

# 3. Create migration
dotnet ef migrations add AddPhoneNumberToUser

# 4. Apply locally
dotnet ef database update

# 5. Test
open http://localhost:8081
curl http://localhost:5001/api/users

# 6. Commit and push
git add .
git commit -m "Add phone number field to User"
git push

# 7. Wait for Jenkins (~1-2 minutes)

# 8. Apply to server
export ConnectionStrings__DefaultConnection="Server=103.123.45.67;Port=3307;Database=quantm_db;User=quantm_user;Password=MyPass123;"
dotnet ef database update

# 9. Verify
dotnet ef migrations list
```

---

**You're all set! Happy migrating!** 🚀
