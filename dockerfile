# Use the official .NET 8 SDK image as a build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory
WORKDIR /app

# Copy the project file and restore dependencies
COPY ./src/Pokedex.API/*.csproj ./Pokedex.API/
COPY ./src/Pokedex.Application/*.csproj ./Pokedex.Application/
COPY ./src/Pokedex.Infrastructure/*.csproj ./Pokedex.Infrastructure/

RUN dotnet restore ./Pokedex.API/Pokedex.API.csproj

# Copy the rest of the application code
COPY ./src ./

# Build the application
RUN dotnet publish ./Pokedex.API/Pokedex.API.csproj -c Release -o out

# Use the official .NET 8 runtime image as a runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Set the working directory
WORKDIR /app

# Copy the built application from the build stage
COPY --from=build /app/out .

# Expose the port the application runs on
EXPOSE 8080

# Set the entry point for the container
ENTRYPOINT ["dotnet", "Pokedex.API.dll"]