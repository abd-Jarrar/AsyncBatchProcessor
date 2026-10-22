FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY *.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c release -o output

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /src/output .

ENV ASPNETCORE_HTTP_PORTS=5046
EXPOSE 5046
 
ENTRYPOINT ["dotnet", "AsyncBatchProcessor.dll"]
 