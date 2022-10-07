#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Vincent.UidGenerator.Web/Vincent.UidGenerator.Web.csproj", "src/Vincent.UidGenerator.Web/"]
COPY ["src/Vincent.UidGenerator/Vincent.UidGenerator.csproj", "src/Vincent.UidGenerator/"]
RUN dotnet restore "src/Vincent.UidGenerator.Web/Vincent.UidGenerator.Web.csproj"
COPY . .
WORKDIR "/src/src/Vincent.UidGenerator.Web"
RUN dotnet build "Vincent.UidGenerator.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Vincent.UidGenerator.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Vincent.UidGenerator.Web.dll"]
