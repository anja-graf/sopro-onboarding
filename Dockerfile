# Use a lean container to run the application
FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base

# Create a directory for the app
WORKDIR /app

# Expose the ports used by the application
EXPOSE 5000
EXPOSE 5001
ENV ASPNETCORE_URLS=http://+:5000

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

# Use a container that contains the complete SDK to build the application
FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build

# Set the working directory
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["Replay.csproj", "./"]
RUN dotnet restore "Replay.csproj"

# Copy the rest of the project files
COPY . .

RUN mkdir -p /Data/persistence
RUN dotnet tool install --global dotnet-ef --version 7.0.5
ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet ef database update --context ApplicationDbContext

RUN dotnet publish "Replay.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY --from=build /src/Data/ ./Data/
LABEL com.centurylinklabs.watchtower.enable="True"
USER root
RUN chown -R appuser /app
USER appuser
ENTRYPOINT ["dotnet", "Replay.dll"]