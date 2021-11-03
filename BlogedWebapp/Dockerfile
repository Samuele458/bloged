#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.



FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80


FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["BlogedWebapp/BlogedWebapp.csproj", "BlogedWebapp/"]
RUN dotnet restore "BlogedWebapp/BlogedWebapp.csproj"
COPY . .
WORKDIR "/src/BlogedWebapp"
ENV ASPNETCORE_ENVIRONMENT=Production
ENV PATH="/root/.dotnet/tools:${PATH}"
RUN dotnet build "BlogedWebapp.csproj" -c Release -o /app/build
RUN dotnet tool install --global dotnet-ef
RUN chmod +x Scripts/migrate_db.sh
ARG BLOGED_DB_CONN
ENV BLOGED_DB_CONN ${BLOGED_DB_CONN}
RUN dotnet ef database update

FROM build AS publish
RUN dotnet publish "BlogedWebapp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlogedWebapp.dll", "--environment=Production"]