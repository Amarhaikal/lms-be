# Server Migration Guide ☁️

Moving your QUANTM Backend to a new cloud provider is easy because everything is Dockerized! 🐳

---

## 📋 Migration Checklist

### **1. On OLD Server (Backup)**

1. **Backup Database** 💾

   ```bash
   # Create a backup file of your data
   cd /var/www/quantm/quantm-be
   docker exec quantm-mysql mysqldump -u quantm_user -pquantm_password quantm_db > quantm_backup.sql
   ```

2. **Download Important Files** 📥
   Download these files to your local computer (using SCP or FileZilla):
   - `quantm_backup.sql` (The database backup)
   - `.env` (Your production secrets)

### **2. On NEW Server (Setup)**

1. **Install Dependencies** 🛠️

   ```bash
   # Update system
   apt update && apt upgrade -y

   # Install Docker
   curl -fsSL https://get.docker.com -o get-docker.sh
   sh get-docker.sh

   # Install Git
   apt install git -y
   ```

2. **Clone Repository** 📂

   ```bash
   mkdir -p /var/www/quantm
   cd /var/www/quantm
   git clone https://github.com/Amarhaikal/quantm-be.git
   cd quantm-be
   ```

3. **Restore Secrets** 🔐
   - Upload your `.env` file to `/var/www/quantm/quantm-be/.env`
   - OR create it manually and copy the content:
     ```bash
     nano .env
     # Paste your credentials
     ```

4. **Start Application** 🚀

   ```bash
   docker compose up -d
   ```

5. **Restore Database** ♻️

   ```bash
   # 1. Copy backup file to server
   # (Upload quantm_backup.sql to /var/www/quantm/quantm-be/)

   # 2. Wait for MySQL to start (~30 seconds)
   docker compose ps

   # 3. Restore data
   cat quantm_backup.sql | docker exec -i quantm-mysql mysql -u quantm_user -pquantm_password quantm_db
   ```

### **3. DNS Switch (Final Step)** 🌐

1. Point your domain (e.g., `api.quantm.com`) to the **NEW** server IP.
2. Wait for propagation.

---

## ❓ FAQ

**Q: What about Jenkins?**
A: You will need to reinstall Jenkins on the new server.

1. Install Java & Jenkins
2. Create the pipeline again (connect to GitHub repo)
3. Copy the `jenkins_home` directory from old server if you want to keep build history (optional).

**Q: Do I need to install .NET or MySQL on the new server?**
A: **NO!** Docker handles all of that. You only need Docker and Git.

**Q: How long will it take?**
A: About 15-30 minutes.
