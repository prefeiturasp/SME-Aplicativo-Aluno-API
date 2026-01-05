#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base

WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
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

FROM base AS final
ENV TZ=America/Sao_Paulo

RUN apt-get update -y \
    && apt-get install -y tzdata \
    && ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone


# Configuração OpenSSL para suportar TLS 1.0/1.1 (necessário para SQL Server 2014 em Windows Server 2012 R2)
RUN echo "\n[openssl_init]" >> /etc/ssl/openssl.cnf
RUN sed -i '/[openssl_init]/a ssl_conf = ssl_configuration' /etc/ssl/openssl.cnf

RUN echo "\n[ssl_configuration]" >> /etc/ssl/openssl.cnf \ 
    && echo "system_default = tls_system_default" >> /etc/ssl/openssl.cnf

RUN echo "\n[tls_system_default]" >> /etc/ssl/openssl.cnf \ 
    && echo "MinProtocol = TLSv1" >> /etc/ssl/openssl.cnf \
    && echo "CipherString = DEFAULT@SECLEVEL=0" >> /etc/ssl/openssl.cnf``
    
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 5000-5001
ENTRYPOINT ["dotnet", "SME.AE.Api.dll"]
