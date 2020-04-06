FROM mcr.microsoft.com/dotnet/core/sdk:3.1

ARG SME_AE_ENVIRONMENT=dev

ENV AEConnection=$SME_AE_CONNECTION_STRING
ENV EolConnection=$SME_EOL_CONNECTION_STRING
ENV SgpConnection=$SME_SGP_CONNECTION_STRING
ENV CoreSSOConnection=$SME_CORE_SSO_CONNECTION_STRING
ENV SME_AE_JWT_TOKEN_SECRET=$SME_AE_JWT_TOKEN_SECRET

ADD . /src
WORKDIR /src
RUN dotnet publish -c Release && \  
    cp -R /src/src/SME.AE.Api/bin/Release/netcoreapp3.1/publish /app

EXPOSE 5001 5000
ENTRYPOINT ["dotnet", "/app/SME.AE.Api.dll"]
