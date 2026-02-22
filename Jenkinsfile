pipeline {
    agent any
    
    environment {
        DOCKER_IMAGE = 'quantm-backend'
        DOCKER_TAG = "${BUILD_NUMBER}"
        COMPOSE_PROJECT_NAME = 'quantm'
    }
    
    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }
        
        stage('Build Docker Image') {
            steps {
                script {
                    echo 'Building Docker image...'
                    sh """
                        docker build -t ${DOCKER_IMAGE}:${DOCKER_TAG} .
                        docker tag ${DOCKER_IMAGE}:${DOCKER_TAG} ${DOCKER_IMAGE}:latest
                    """
                }
            }
        }
        
        stage('Run Tests') {
            steps {
                script {
                    echo 'Running tests...'
                    // Add your test commands here
                    // sh 'dotnet test'
                }
            }
        }
        
        stage('Stop Old Containers') {
            steps {
                script {
                    echo 'Stopping old containers...'
                    sh """
                        cd /var/www/quantm/quantm-be
                        docker stop quantm-nginx || true
                        docker rm quantm-nginx || true
                        docker stop quantm-backend || true
                        docker rm quantm-backend || true
                        docker compose down || true
                    """
                }
            }
        }
        
        stage('Sync Code to Deployment Directory') {
            steps {
                script {
                    echo 'Syncing code from workspace to deployment directory...'
                    sh """
                        # Create deployment directory if it doesn't exist
                        mkdir -p /var/www/quantm/quantm-be
                        
                        # Sync all files except .git, bin, obj, logs, and .env
                        rsync -av --delete \
                            --exclude='.git' \
                            --exclude='bin' \
                            --exclude='obj' \
                            --exclude='logs' \
                            --exclude='.env' \
                            --exclude='Uploads' \
                            ${WORKSPACE}/ /var/www/quantm/quantm-be/
                        
                        echo 'Code sync completed!'
                    """
                }
            }
        }
        
        stage('Deploy') {
            steps {
                script {
                    echo 'Deploying new containers...'
                    sh """
                        cd /var/www/quantm/quantm-be
                        docker compose up -d --build
                    """
                }
            }
        }
        
        stage('Run Database Migrations') {
            steps {
                script {
                    echo 'Running database migrations...'
                    sh """
                        # Use the build Docker image (which has .NET SDK) to generate migration script
                        docker run --rm \
                            -v ${WORKSPACE}:/src \
                            -w /src \
                            -e "ConnectionStrings__DefaultConnection=Server=dummy;Database=dummy;User=dummy;Password=dummy;" \
                            mcr.microsoft.com/dotnet/sdk:9.0 \
                            bash -c "dotnet restore quantm-be.csproj && dotnet tool install --global dotnet-ef && export PATH=\"\$PATH:/root/.dotnet/tools\" && dotnet ef migrations script --project quantm-be.csproj --idempotent -o migration.sql"
                        
                        # Copy script to deployment directory
                        cp ${WORKSPACE}/migration.sql /var/www/quantm/quantm-be/
                        
                        # Apply migrations to MySQL container using credentials from .env
                        cd /var/www/quantm/quantm-be
                        export \$(grep -v '^#' .env | xargs)
                        docker exec -i quantm-mysql mysql -u\${MYSQL_USER} -p\${MYSQL_PASSWORD} \${MYSQL_DATABASE} < migration.sql
                        
                        echo 'Database migrations completed!'
                    """
                }
            }
        }
        
        stage('Health Check') {
            steps {
                script {
                    echo 'Performing health check...'
                    sleep 10
                    sh """
                        curl -f http://localhost:8081 || exit 1
                    """
                }
            }
        }
        
        stage('Cleanup') {
            steps {
                script {
                    echo 'Cleaning up old images...'
                    sh """
                        docker image prune -f
                    """
                }
            }
        }
    }
    
    post {
        success {
            echo 'Deployment successful!'
        }
        failure {
            echo 'Deployment failed!'
            sh 'docker compose logs quantm-backend'
        }
    }
}
