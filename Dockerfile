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

# Set the locale
RUN sed -i -e 's/# en_US.UTF-8 UTF-8/en_US.UTF-8 UTF-8/' /etc/locale.gen && \
    locale-gen
ENV LANG en_US.UTF-8  
ENV LANGUAGE en_US:en  
ENV LC_ALL en_US.UTF-8   

ADD . /src
WORKDIR /src 
RUN apt-get update -y \
    && apt-get install -yq tzdata locales -y \
    && dpkg-reconfigure --frontend noninteractive tzdata \ 
	&& locale-gen en_US.UTF-8 \
    && dotnet restore \  
    && dotnet publish -c Release \   
    && cp -R /src/src/SME.AE.Api/bin/Release/netcoreapp3.0/publish /app \ 
    && rm -Rf /src

WORKDIR /app 
EXPOSE 5000-5001
CMD [ "dotnet", "/app/SME.AE.Api.dll"]
