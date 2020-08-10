FROM mcr.microsoft.com/dotnet/core/sdk:3.0-bionic

ARG SME_AE_ENVIRONMENT=dev

ENV AEConnection=$SME_AE_CONNECTION_STRING
ENV EolConnection=$SME_EOL_CONNECTION_STRING
ENV SgpConnection=$SME_SGP_CONNECTION_STRING
ENV CoreSSOConnection=$SME_CORE_SSO_CONNECTION_STRING
ENV SME_AE_JWT_TOKEN_SECRET=$SME_AE_JWT_TOKEN_SECRET
ENV FirebaseToken=$FirebaseToken
ENV FirebaseProjectId=$FirebaseProjectId
ENV ChaveIntegracao=$ChaveIntegracao
ENV SentryDsn=$SentryDsn
ENV TZ=America/Sao_Paulo
ENV DEBIAN_FRONTEND=noninteractive

ADD . /src
WORKDIR /src 
RUN apt-get update \
    && apt-get install -yq tzdata \
    && dpkg-reconfigure --frontend noninteractive tzdata \ 
    && dotnet restore \  
    && dotnet publish -c Release \   
    && cp -R /src/src/SME.AE.Api/bin/Release/netcoreapp3.0/publish /app \ 
    && cp -R /src/src/SME.AE.Api/wwwroot /app/wwwroot \ 
    && rm -Rf /src

EXPOSE 5000-5001
CMD [ "dotnet", "/app/SME.AE.Api"]
