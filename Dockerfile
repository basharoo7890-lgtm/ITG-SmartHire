FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY SmartHire/*.csproj ./SmartHire/
RUN dotnet restore SmartHire/SmartHire.csproj
COPY . .
RUN dotnet publish SmartHire/SmartHire.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
RUN mkdir -p /app/uploads
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000
ENTRYPOINT ["dotnet", "SmartHire.dll"]
