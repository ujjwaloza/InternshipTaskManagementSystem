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
                bat 'dotnet restore'
            }
        }

        stage('Build Project') {
            steps {
                bat 'dotnet build --configuration Release'
            }
        }

        stage('Publish Project') {
            steps {
                bat 'dotnet publish -c Release -o publish'
            }
        }

        stage('Build Docker Image') {
            steps {
                bat 'docker build -t internship-app .'
            }
        }

        stage('Run Docker Container') {
            steps {
                bat 'docker stop internship-container || exit 0'
                bat 'docker rm internship-container || exit 0'
                bat 'docker run -d --name internship-container -p 8080:80 internship-app'
            }
        }
    }
}