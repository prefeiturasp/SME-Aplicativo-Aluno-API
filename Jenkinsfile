pipeline {
    environment {
      branchname =  env.BRANCH_NAME.toLowerCase()
      kubeconfig = getKubeconf(env.branchname)
      registryCredential = 'jenkins_registry'
    }
  
    agent {
      node { label 'dotnet31-appaluno-rc' }
    }

    options {
      buildDiscarder(logRotator(numToKeepStr: '20', artifactNumToKeepStr: '20'))
      disableConcurrentBuilds()
      skipDefaultCheckout()
    }
  
    stages {

        stage('CheckOut') {            
            steps { checkout scm }            
        }

        stage('Build projeto') {
        steps {
          sh "echo executando build de projeto"
          sh 'dotnet build'
        }
      }

        stage('Testes de integração') {
        steps {
          
          //Executa os testes gerando um relatorio formato trx
            sh 'dotnet test --logger "trx;LogFileName=TestResults.trx"'
          //Publica o relatorio de testes
            mstest failOnError: false
          
        }
     }

        stage('AnaliseCodigo') {
	      when { branch 'homolog' }
          steps {
              withSonarQubeEnv('sonarqube-local'){
                sh 'echo "[ INFO ] Iniciando analise Sonar..." && dotnet-sonarscanner begin /k:"SME-Aplicativo-Aluno"'
                sh 'dotnet build'
                sh 'dotnet-sonarscanner end'

            }
          }
        }

        

        stage('Build') {
          when { anyOf { branch 'master'; branch 'main'; branch "story/*"; branch 'development'; branch 'release'; branch 'homolog';  } } 
          steps {
            script {
              imagename1 = "registry.sme.prefeitura.sp.gov.br/${env.branchname}/sme-appaluno-api"
              imagename2 = "registry.sme.prefeitura.sp.gov.br/${env.branchname}/appaluno-worker"
              imagename3 = "registry.sme.prefeitura.sp.gov.br/${env.branchname}/sme-ea-worker"
              dockerImage1 = docker.build(imagename1, "-f src/SME.AE.Api/Dockerfile .")
              dockerImage2 = docker.build(imagename2, "-f src/SME.AE.Worker.Service/Dockerfile .")
              dockerImage3 = docker.build(imagename3, "-f src/SME.AE.Worker/Dockerfile .")
              docker.withRegistry( 'https://registry.sme.prefeitura.sp.gov.br', registryCredential ) {
              dockerImage1.push()
              dockerImage2.push()
              dockerImage3.push()
              }
              sh "docker rmi $imagename1 $imagename2 $imagename3"
              //sh "docker rmi $imagename2"
            }
          }
        }
	    
        stage('Deploy'){
            when { anyOf {  branch 'master'; branch 'main'; branch 'development'; branch 'release'; branch 'homolog';  } }        
            steps {
                script{
                    if ( env.branchname == 'main' ||  env.branchname == 'master' || env.branchname == 'homolog' || env.branchname == 'release' ) {
                        sendTelegram("🤩 [Deploy ${env.branchname}] Job Name: ${JOB_NAME} \nBuild: ${BUILD_DISPLAY_NAME} \nMe aprove! \nLog: \n${env.BUILD_URL}")
                        timeout(time: 24, unit: "HOURS") {
                            input message: 'Deseja realizar o deploy?', ok: 'SIM', submitter: 'marlon_goncalves, bruno_alevato'
                        }
                        withCredentials([file(credentialsId: "${kubeconfig}", variable: 'config')]){
                            sh('cp $config '+"$home"+'/.kube/config')
                            sh 'kubectl rollout restart deployment/sme-appaluno-api -n sme-appaluno'
                            sh 'kubectl rollout restart deployment/appaluno-worker -n sme-appaluno'
                            sh 'kubectl rollout restart deployment/sme-ea-worker -n sme-appaluno'
                            sh('rm -f '+"$home"+'/.kube/config')
                        }
                    }
                    else{
                        withCredentials([file(credentialsId: "${kubeconfig}", variable: 'config')]){
                            sh('cp $config '+"$home"+'/.kube/config')
                            sh 'kubectl rollout restart deployment/sme-appaluno-api -n sme-appaluno'
                            sh 'kubectl rollout restart deployment/appaluno-worker -n sme-appaluno'
                            sh 'kubectl rollout restart deployment/sme-ea-worker -n sme-appaluno'
                            sh('rm -f '+"$home"+'/.kube/config')
                        }
                    }
                }
            }           
        }    
    }

  post {
    success { sendTelegram("🚀 Job Name: ${JOB_NAME} \nBuild: ${BUILD_DISPLAY_NAME} \nStatus: Success \nLog: \n${env.BUILD_URL}console") }
    unstable { sendTelegram("💣 Job Name: ${JOB_NAME} \nBuild: ${BUILD_DISPLAY_NAME} \nStatus: Unstable \nLog: \n${env.BUILD_URL}console") }
    failure { sendTelegram("💥 Job Name: ${JOB_NAME} \nBuild: ${BUILD_DISPLAY_NAME} \nStatus: Failure \nLog: \n${env.BUILD_URL}console") }
    aborted { sendTelegram ("😥 Job Name: ${JOB_NAME} \nBuild: ${BUILD_DISPLAY_NAME} \nStatus: Aborted \nLog: \n${env.BUILD_URL}console") }
  }
}
def sendTelegram(message) {
    def encodedMessage = URLEncoder.encode(message, "UTF-8")
    withCredentials([string(credentialsId: 'telegramToken', variable: 'TOKEN'),
    string(credentialsId: 'telegramChatId', variable: 'CHAT_ID')]) {
        response = httpRequest (consoleLogResponseBody: true,
                contentType: 'APPLICATION_JSON',
                httpMode: 'GET',
                url: 'https://api.telegram.org/bot'+"$TOKEN"+'/sendMessage?text='+encodedMessage+'&chat_id='+"$CHAT_ID"+'&disable_web_page_preview=true',
                validResponseCodes: '200')
        return response
    }
}
def getKubeconf(branchName) {
    if("main".equals(branchName)) { return "config_prd"; }
    else if ("master".equals(branchName)) { return "config_prd"; }
    else if ("homolog".equals(branchName)) { return "config_hom"; }
    else if ("release".equals(branchName)) { return "config_hom"; }
    else if ("development".equals(branchName)) { return "config_dev"; }
}
