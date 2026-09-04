pipeline {
    agent any

    options {
        disableConcurrentBuilds()
        timestamps()
        buildDiscarder(logRotator(
            numToKeepStr: '10',
            artifactNumToKeepStr: '5'
        ))
    }

    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
        DOTNET_NOLOGO = '1'
        STAGING_DIRECTORY =
            '/var/lib/jenkins/bancosol-finance-staging'
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Restore') {
            steps {
                sh '''
                    dotnet restore BancoSol.Finance.slnx
                '''
            }
        }

        stage('Build') {
            steps {
                sh '''
                    dotnet build BancoSol.Finance.slnx \
                        --configuration Release \
                        --no-restore
                '''
            }
        }

        stage('Test') {
            steps {
                withCredentials([
                    string(
                        credentialsId: 'finance-test-db',
                        variable: 'ConnectionStrings__FinanceTestDatabase'
                    )
                ]) {
                    sh '''
                        dotnet test BancoSol.Finance.slnx \
                            --configuration Release \
                            --no-build \
                            --logger "console;verbosity=normal"
                    '''
                }
            }
        }

        stage('Publish') {
            steps {
                sh '''
                    dotnet publish \
                        src/BancoSol.Finance.Api/BancoSol.Finance.Api.csproj \
                        --configuration Release \
                        --no-build \
                        --output ./publish
                '''

                archiveArtifacts(
                    artifacts: 'publish/**',
                    fingerprint: true
                )
            }
        }

        stage('Prepare deployment') {
            steps {
                sh '''
                    rsync \
                        --archive \
                        --delete \
                        ./publish/ \
                        "$STAGING_DIRECTORY/"
                '''
            }
        }

        stage('Deploy') {
            steps {
                sh '''
                    sudo /usr/local/sbin/deploy-bancosol-finance
                '''
            }
        }

        stage('Public health check') {
            steps {
                sh '''
                    curl \
                        --fail \
                        --silent \
                        --show-error \
                        --retry 5 \
                        --retry-delay 5 \
                        https://bancsol-api.servernux.com/health
                '''
            }
        }
    }

    post {
        success {
            echo 'BancoSol Finance API desplegada correctamente.'
        }

        failure {
            echo 'El pipeline falló. Revisa la etapa y los registros.'
        }

        always {
            deleteDir()
        }
    }
}