#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/SME.AE.Worker/SME.AE.Worker.csproj", "src/SME.AE.Worker/"]
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