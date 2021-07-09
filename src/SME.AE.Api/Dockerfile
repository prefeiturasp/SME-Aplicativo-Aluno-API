#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/sdk:3.1-bionic as base

WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:3.1-bionic as build
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
    
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5000-5001
ENTRYPOINT ["dotnet", "SME.AE.Api.dll"]