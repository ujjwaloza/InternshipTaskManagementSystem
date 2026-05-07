pipeline {
    agent any

    stages {

        stage('Clone Code') {
            steps {
                git branch: 'main',
                url: 'https://github.com/ujjwaloza/InternshipTaskManagementSystem.git'
            }
        }

        stage('Show Success') {
            steps {
                sh 'echo "GitHub Connected Successfully"'
                sh 'echo "CI/CD Pipeline Running Successfully"'
            }
        }
    }
}