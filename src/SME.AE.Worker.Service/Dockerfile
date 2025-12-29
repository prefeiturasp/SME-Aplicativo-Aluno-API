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

ARG AEConnection
ARG EolConnection
ARG SgpConnection
ARG CoreSSOConnection

ENV AEConnection=${AEConnection}
ENV EolConnection=${EolConnection}
ENV SgpConnection=${SgpConnection}
ENV CoreSSOConnection=${CoreSSOConnection}

ENTRYPOINT ["dotnet", "SME.AE.Worker.Service.dll"]