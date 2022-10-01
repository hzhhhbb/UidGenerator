FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Vincent.UidGenerator.Web/Vincent.UidGenerator.Web.csproj", "Vincent.UidGenerator.Web/"]
RUN dotnet restore "Vincent.UidGenerator.Web/Vincent.UidGenerator.Web.csproj"
COPY . .
WORKDIR "/src/Vincent.UidGenerator.Web"
RUN dotnet build "Vincent.UidGenerator.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Vincent.UidGenerator.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Vincent.UidGenerator.Web.dll"]
