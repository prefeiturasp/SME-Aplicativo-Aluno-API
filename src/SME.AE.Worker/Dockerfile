#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/SME.AE.Worker/SME.AE.Worker.csproj", "src/SME.AE.Worker/"]
COPY ["src/SME.AE.Infra/SME.AE.Infra.csproj", "src/SME.AE.Infra/"]
COPY ["src/SME.AE.Dominio/SME.AE.Dominio.csproj", "src/SME.AE.Dominio/"]
COPY ["src/SME.AE.Aplicacao/SME.AE.Aplicacao.csproj", "src/SME.AE.Aplicacao/"]
COPY ["src/SME.AE.COMUM/SME.AE.Comum.csproj", "src/SME.AE.COMUM/"]

RUN dotnet restore "src/SME.AE.Worker/SME.AE.Worker.csproj"
COPY . .
WORKDIR "/src/src/SME.AE.Worker"
RUN dotnet build "SME.AE.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SME.AE.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Configuração OpenSSL para suportar TLS 1.0/1.1 (necessário para SQL Server 2014 em Windows Server 2012 R2)
RUN echo "\n[openssl_init]" >> /etc/ssl/openssl.cnf
RUN sed -i '/[openssl_init]/a ssl_conf = ssl_configuration' /etc/ssl/openssl.cnf

RUN echo "\n[ssl_configuration]" >> /etc/ssl/openssl.cnf \ 
    && echo "system_default = tls_system_default" >> /etc/ssl/openssl.cnf

RUN echo "\n[tls_system_default]" >> /etc/ssl/openssl.cnf \ 
    && echo "MinProtocol = TLSv1" >> /etc/ssl/openssl.cnf \
    && echo "CipherString = DEFAULT@SECLEVEL=0" >> /etc/ssl/openssl.cnf``

ENTRYPOINT ["dotnet", "SME.AE.Worker.dll"]