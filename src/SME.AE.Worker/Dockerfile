#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
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
ENTRYPOINT ["dotnet", "SME.AE.Worker.dll"]