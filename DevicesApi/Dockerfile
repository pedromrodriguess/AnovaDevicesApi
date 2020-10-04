FROM mcr.microsoft.com/dotnet/core/sdk as build
WORKDIR /app

COPY DevicesApi.csproj ./
RUN dotnet restore 

COPY . ./
RUN dotnet publish -c Release -o out 

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app

COPY --from=build /app/out .

EXPOSE 80

ENTRYPOINT ["dotnet", "DevicesApi.dll"]