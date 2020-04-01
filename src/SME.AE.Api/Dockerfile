#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/SME.AE.Api/SME.AE.Api.csproj", "src/SME.AE.Api/"]
RUN dotnet restore "src/SME.AE.Api/SME.AE.Api.csproj"
COPY . .
WORKDIR "/src/src/SME.AE.Api"
RUN dotnet build "SME.AE.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SME.AE.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SME.AE.Api.dll"]