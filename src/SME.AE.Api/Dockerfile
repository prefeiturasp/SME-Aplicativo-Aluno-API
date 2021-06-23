#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS base

WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
WORKDIR /src
COPY ["src/SME.AE.Api/SME.AE.Api.csproj", "src/SME.AE.Api/"]
COPY ["src/SME.AE.Infra/SME.AE.Infra.csproj", "src/SME.AE.Infra/"]
COPY ["src/SME.AE.Aplicacao/SME.AE.Aplicacao.csproj", "src/SME.AE.Aplicacao/"]
COPY src/SME.AE.Api/wwwroot src/wwwroot
RUN dotnet restore "src/SME.AE.Api/SME.AE.Api.csproj"
COPY . .
WORKDIR "/src/src/SME.AE.Api"
RUN dotnet build "SME.AE.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SME.AE.Api.csproj" -c Release -o /app/publish

RUN apk add tzdata \ 
    && cp /usr/share/zoneinfo/America/Sao_Paulo /etc/localtime \
    && echo "America/Sao_Paulo" >  /etc/timezone \
    && apk del tzdata

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5000-5001
ENTRYPOINT ["dotnet", "SME.AE.Api.dll"]