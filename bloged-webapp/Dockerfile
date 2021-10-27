#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["bloged-webapp/bloged-webapp.csproj", "bloged-webapp/"]
RUN dotnet restore "bloged-webapp/bloged-webapp.csproj"
COPY . .
WORKDIR "/src/bloged-webapp"
RUN dotnet build "bloged-webapp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "bloged-webapp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "bloged-webapp.dll"]