FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /src

copy . ./

run dotnet restore
run dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app
COPY --from=build-env /src/out .

ARG SME_AE_ENVIRONMENT=dev

ENV TZ=America/Sao_Paulo
ENV DEBIAN_FRONTEND=noninteractive

ENV AEConnection=$AEConnection
ENV EolConnection=$EolConnection
ENV SgpConnection=$SgpConnection
ENV CoreSSOConnection=$CoreSSOConnection

CMD ["dotnet", "SME.AE.Worker.Service.dll"]
