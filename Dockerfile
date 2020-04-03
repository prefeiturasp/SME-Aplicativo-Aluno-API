FROM mcr.microsoft.com/dotnet/core/sdk:3.1

ARG SME_AE_ENVIRONMENT=dev

ENV AEConnection=$SME_AE_CONNECTION_STRING
ENV EolConnection=$SME_EOL_CONNECTION_STRING
ENV SgpConnection=$SME_SGP_CONNECTION_STRING
ENV CoreSSOConnection=$SME_CORE_SSO_CONNECTION_STRING

ADD . /src
WORKDIR /src
RUN dotnet publish -c Release
COPY /src/src/SME.AE.Api/bin/Release/netcoreapp3.1/publish /app

EXPOSE 5001
ENTRYPOINT ["dotnet", "/app/SME.AE.Api.dll"]
