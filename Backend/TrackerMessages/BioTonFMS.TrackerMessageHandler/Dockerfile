FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy everything
COPY . ./

RUN git describe \
    && dotnet restore Backend/BioTonFMSApp/BioTonFMSApp.sln \
    && dotnet build -c release Backend/.tools/BioTonFMS.Version \
    && dotnet build -c release Backend/TrackerMessages/BioTonFMS.TrackerMessageHandler

#Publish
FROM build AS publish
WORKDIR /app/
RUN dotnet publish -c Release -o out Backend/TrackerMessages/BioTonFMS.TrackerMessageHandler/BioTonFMS.TrackerMessageHandler.csproj

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=publish /app/out .
ENTRYPOINT ["dotnet", "BioTonFMS.TrackerMessageHandler.dll"]
