#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ProjectSchedulerAuth.Api/ProjectSchedulerAuth.Api.csproj", "ProjectSchedulerAuth.Api/"]
COPY ["ProjectSchedulerAuth.Application/ProjectSchedulerAuth.Application.csproj", "ProjectSchedulerAuth.Application/"]
COPY ["ProjectSchedulerAuth.Domain/ProjectSchedulerAuth.Domain.csproj", "ProjectSchedulerAuth.Domain/"]
COPY ["ProjectSchedulerAuth.Infrastructure/ProjectSchedulerAuth.Infrastructure.csproj", "ProjectSchedulerAuth.Infrastructure/"]
RUN dotnet restore "ProjectSchedulerAuth.Api/ProjectSchedulerAuth.Api.csproj"
COPY . .
WORKDIR "/src/ProjectSchedulerAuth.Api"
RUN dotnet build "ProjectSchedulerAuth.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProjectSchedulerAuth.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProjectSchedulerAuth.Api.dll"]