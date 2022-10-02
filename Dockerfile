FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy everything
COPY . ./

RUN git describe
# Restore as distinct layers
RUN dotnet restore  BioTonFMSApp/
# Build and publish a release
RUN dotnet publish -c Release -o out  BioTonFMSApp/

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "BioTonFMSApp.dll"]
