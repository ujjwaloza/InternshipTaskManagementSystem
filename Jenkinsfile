pipeline {
    agent any

    stages {

        stage('Clone Code') {
            steps {
                git branch: 'main',
                url: 'https://github.com/ujjwaloza/InternshipTaskManagementSystem.git'
            }
        }

        stage('Restore Packages') {
            steps {
                sh 'dotnet restore'
            }
        }

        stage('Build Project') {
            steps {
                sh 'dotnet build --configuration Release'
            }
        }

        stage('Publish Project') {
            steps {
                sh 'dotnet publish -c Release -o publish'
            }
        }

        stage('Build Docker Image') {
            steps {
                sh 'docker build -t internship-app .'
            }
        }

        stage('Run Docker Container') {
            steps {
                sh 'docker stop internship-container || true'
                sh 'docker rm internship-container || true'
                sh 'docker run -d --name internship-container -p 8080:80 internship-app'
            }
        }
    }
}