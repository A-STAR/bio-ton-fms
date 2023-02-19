FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy everything
COPY . ./

RUN git describe
# Restore as distinct layers
RUN dotnet restore BioTonFMSApp/BioTonFMSApp.sln
RUN dotnet build -c release .tools/BioTonFMS.Version
RUN dotnet build -c release TrackerMessages/BioTonFMS.TrackerMessageHandler

#Publish
FROM build AS publish
WORKDIR /app/
RUN dotnet publish -c Release -o out TrackerMessages/BioTonFMS.TrackerMessageHandler/BioTonFMS.TrackerMessageHandler.csproj

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=publish /app/out .
ENTRYPOINT ["dotnet", "BioTonFMS.TrackerMessageHandler.dll"]
