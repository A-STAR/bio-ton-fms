FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy everything
COPY . ./

RUN git describe
# Restore as distinct layers
RUN dotnet restore BioTonFMSApp/
RUN dotnet build BioTonFMSApp/

# run the unit tests
FROM build AS test
WORKDIR /app/.UnitTests/BiotonFMS.Telematica.Tests
CMD ["dotnet", "test", "--logger:trx"]

#Publish
FROM build AS publish
WORKDIR /app/
RUN dotnet publish -c Release -o out  BioTonFMSApp/

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=publish /app/out .
ENTRYPOINT ["dotnet", "BioTonFMSApp.dll"]
