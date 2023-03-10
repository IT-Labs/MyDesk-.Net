# Get .NET SDK image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

# Copy files and restore it's dependencies using NUGET
COPY . .
RUN dotnet restore "./MyDesk/MyDesk.Application/MyDesk.Application.csproj" --disable-parallel

# Build and publish project
RUN dotnet publish "./MyDesk/MyDesk.Application/MyDesk.Application.csproj" -c release -o /app --no-restore

# Generate runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app ./
EXPOSE 80
ENTRYPOINT [ "dotnet", "MyDesk.Application.dll" ]
