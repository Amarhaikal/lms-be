pipeline {
    agent any
    
    environment {
        DOCKER_IMAGE = 'lms-backend'
        DOCKER_TAG = "${BUILD_NUMBER}"
        COMPOSE_PROJECT_NAME = 'lms'
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
                        docker-compose down || true
                    """
                }
            }
        }
        
        stage('Deploy') {
            steps {
                script {
                    echo 'Deploying new containers...'
                    sh """
                        docker-compose up -d
                    """
                }
            }
        }
        
        stage('Run Migrations') {
            steps {
                script {
                    echo 'Running database migrations...'
                    sh """
                        docker exec lms-backend dotnet ef database update --verbose
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
                        curl -f http://localhost:5000/ || exit 1
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
            sh 'docker-compose logs lms-backend'
        }
    }
}
