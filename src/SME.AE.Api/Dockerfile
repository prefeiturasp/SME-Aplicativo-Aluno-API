#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base

ARG SME_AE_ENVIRONMENT=dev
ENV AEConnection=$SME_AE_CONNECTION_STRING
ENV EolConnection=$SME_EOL_CONNECTION_STRING
ENV SgpConnection=$SME_SGP_CONNECTION_STRING
ENV CoreSSOConnection=$SME_CORE_SSO_CONNECTION_STRING
ENV SME_AE_JWT_TOKEN_SECRET=$SME_AE_JWT_TOKEN_SECRET
ENV ChaveIntegracao=$ChaveIntegracao
ENV SentryDsn=$SentryDsn
ENV TZ=America/Sao_Paulo
ENV DEBIAN_FRONTEND=noninteractive

RUN apt-get update && apt-get install -yq tzdata && dpkg-reconfigure --frontend noninteractive tzdata

WORKDIR /app



FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["src/SME.AE.Api/SME.AE.Api.csproj", "src/SME.AE.Api/"]
COPY ["src/SME.AE.Infra/SME.AE.Infra.csproj", "src/SME.AE.Infra/"]
COPY ["src/SME.AE.Dominio/SME.AE.Dominio.csproj", "src/SME.AE.Dominio/"]
COPY ["src/SME.AE.Aplicacao/SME.AE.Aplicacao.csproj", "src/SME.AE.Aplicacao/"]
RUN dotnet restore "src/SME.AE.Api/SME.AE.Api.csproj"
COPY . .
WORKDIR "/src/src/SME.AE.Api"
RUN dotnet build "SME.AE.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SME.AE.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5000-5001
ENTRYPOINT ["dotnet", "SME.AE.Api.dll"]