FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app
COPY . .
RUN dotnet restore
ENTRYPOINT ["dotnet", "test", "-l", "\"console;verbosity=detailed\""]