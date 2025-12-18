FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

COPY . ./

RUN dotnet restore
RUN dotnet publish -c Release -o /src/out

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app
COPY --from=base /src/out .

ARG SME_AE_ENVIRONMENT=dev

ENV TZ=America/Sao_Paulo
ENV DEBIAN_FRONTEND=noninteractive

ARG AEConnection
ARG EolConnection
ARG SgpConnection
ARG CoreSSOConnection

ENV AEConnection=$AEConnection
ENV EolConnection=$EolConnection
ENV SgpConnection=$SgpConnection
ENV CoreSSOConnection=$CoreSSOConnection

CMD ["dotnet", "SME.AE.Worker.Service.dll"]