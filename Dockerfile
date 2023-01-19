FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy everything
COPY . ./

RUN git describe
# Restore as distinct layers
RUN dotnet restore BioTonFMSApp/BioTonFMSApp.sln
RUN dotnet build -c release .tools/BioTonFMS.Version
RUN dotnet build -c release BioTonFMSApp/BioTonFMSApp.csproj
RUN dotnet build -c release .UnitTests/BiotonFMS.Telematica.Tests/

# run the unit tests 
FROM build AS test
WORKDIR /app/.UnitTests/BiotonFMS.Telematica.Tests
CMD ["dotnet", "test", "--logger:trx"]

#Publish
FROM build AS publish
WORKDIR /app/
RUN dotnet publish -c Release -o out  BioTonFMSApp/BioTonFMSApp.csproj 

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=publish /app/out .
ENTRYPOINT ["dotnet", "BioTonFMSApp.dll"]
