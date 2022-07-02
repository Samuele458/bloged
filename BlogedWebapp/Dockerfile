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
RUN dotnet tool install --global dotnet-ef --version 5.0.11
RUN chmod +x Scripts/migrate_db.sh
ARG AWS_ACCESS_KEY_ID
ENV AWS_ACCESS_KEY_ID ${AWS_ACCESS_KEY_ID}
ARG AWS_SECRET_ACCESS_KEY
ENV AWS_SECRET_ACCESS_KEY ${AWS_SECRET_ACCESS_KEY}
RUN /bin/bash Scripts/migrate_db.sh

FROM build AS publish
RUN dotnet publish "BlogedWebapp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlogedWebapp.dll", "--environment=Production"]