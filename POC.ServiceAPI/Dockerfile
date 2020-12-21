#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["POC.Domain/POC.Domain.csproj", "POC.Domain/"]
COPY ["POC.Infra/POC.Infra.csproj", "POC.Infra/"]
COPY ["POC.Application/POC.Application.csproj", "POC.Application/"]
COPY ["POC.ServiceAPI/POC.ServiceAPI.csproj", "POC.ServiceAPI/"]
RUN dotnet restore "POC.ServiceAPI/POC.ServiceAPI.csproj"
COPY . .
WORKDIR "/src/POC.ServiceAPI"
RUN dotnet build "POC.ServiceAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "POC.ServiceAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "POC.ServiceAPI.dll"]