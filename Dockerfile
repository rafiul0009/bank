FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
WORKDIR /src
COPY . .
RUN dotnet restore "./PayBill.API.Endpoint/PayBill.API.Endpoint.csproj"
WORKDIR "/src/PayBill.API.Endpoint"
RUN dotnet build "PayBill.API.Endpoint.csproj" -c Release -o /app

FROM build AS publish
WORKDIR "/src/PayBill.API.Endpoint"
RUN dotnet publish "PayBill.API.Endpoint.csproj" -c Release -o /app

# Use Distroless .NET as the base image
FROM gcr.io/distroless/dotnet:7.0 AS final
WORKDIR /app
COPY --from=publish /app .
RUN addgroup -S mygroup && adduser -S -G mygroup myuser
USER myuser
ENTRYPOINT ["dotnet", "PayBill.API.Endpoint.dll"]
