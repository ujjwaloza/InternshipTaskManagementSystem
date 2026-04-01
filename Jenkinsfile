pipeline {
    agent any

    stages {

        stage('Clone Code') {
    steps {
        git branch: 'main', url: 'https://github.com/ujjwaloza/InternshipTaskManagementSystem.git'
    }
}

        stage('Build .NET Project') {
            steps {
                sh 'dotnet restore'
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

        stage('Run Container') {
            steps {
                sh 'docker run -d -p 8080:80 internship-app || true'
            }
        }
    }
}