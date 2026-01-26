FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/SME.AE.Worker.Service/SME.AE.Worker.Service.csproj", "src/SME.AE.Worker.Service/"]
RUN dotnet restore "src/SME.AE.Worker.Service/SME.AE.Worker.Service.csproj"

COPY . .
RUN dotnet publish "src/SME.AE.Worker.Service/SME.AE.Worker.Service.csproj" -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/out .

ARG SME_AE_ENVIRONMENT=dev
ENV ASPNETCORE_ENVIRONMENT=${SME_AE_ENVIRONMENT}
ENV TZ=America/Sao_Paulo

# Configuração OpenSSL para suportar TLS 1.0/1.1 (necessário para SQL Server 2014 em Windows Server 2012 R2)
RUN echo "\n[openssl_init]" >> /etc/ssl/openssl.cnf
RUN sed -i '/[openssl_init]/a ssl_conf = ssl_configuration' /etc/ssl/openssl.cnf

RUN echo "\n[ssl_configuration]" >> /etc/ssl/openssl.cnf \ 
    && echo "system_default = tls_system_default" >> /etc/ssl/openssl.cnf

RUN echo "\n[tls_system_default]" >> /etc/ssl/openssl.cnf \ 
    && echo "MinProtocol = TLSv1" >> /etc/ssl/openssl.cnf \
    && echo "CipherString = DEFAULT@SECLEVEL=0" >> /etc/ssl/openssl.cnf``

ARG AEConnection
ARG EolConnection
ARG SgpConnection
ARG CoreSSOConnection

ENV AEConnection=${AEConnection}
ENV EolConnection=${EolConnection}
ENV SgpConnection=${SgpConnection}
ENV CoreSSOConnection=${CoreSSOConnection}

ENTRYPOINT ["dotnet", "SME.AE.Worker.Service.dll"]