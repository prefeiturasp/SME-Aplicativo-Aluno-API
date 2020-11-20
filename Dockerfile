FROM mcr.microsoft.com/dotnet/sdk:3.1-alpine as build

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

ENV TZ America/Sao_Paulo
ENV LANG pt_BR.UTF-8
ENV LANGUAGE pt_BR.UTF-8
ENV LC_ALL pt_BR.UTF-8 

ADD . /src
WORKDIR /src 

RUN apk update \
    && apk add tzdata \ 
    && cp /usr/share/zoneinfo/America/Sao_Paulo /etc/localtime \
    && echo "America/Sao_Paulo" > /etc/timezone \ 
    && dotnet restore \
    && dotnet build \ 
    && dotnet publish -c Release \   
    && ls -la  /src/src/SME.AE.Api/bin/Release/ \ 
    && cp -R /src/src/SME.AE.Api/bin/Release/netcoreapp3.1/publish /app \ 
    && rm -Rf /src

FROM mcr.microsoft.com/dotnet/aspnet:3.1-alpine as final
COPY --from=build /app /app
WORKDIR /app

EXPOSE 5000-5001
CMD ["/app/SME.AE.Api"]