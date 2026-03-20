FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

RUN apt-get update && apt-get install -y clang zlib1g-dev

COPY . .
RUN dotnet publish Api.csproj -c Release -r linux-x64 --self-contained -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime-deps:10.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["./Api"]
