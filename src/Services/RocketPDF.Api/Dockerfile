#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime-deps:7.0-alpine-amd64 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine-amd64 AS build
WORKDIR /
COPY ["src/Services/RocketPDF.Api/RocketPDF.Api.csproj", "src/RocketPDF.Api/"]
RUN dotnet restore "src/RocketPDF.Api/RocketPDF.Api.csproj" -r linux-musl-x64
COPY "src" "src"
WORKDIR "/src/RocketPDF.Api"
#RUN dotnet build "RocketPDF.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RocketPDF.Api.csproj" -c Release -p:PublishSingleFile=true -r linux-musl-x64 --self-contained true -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN rm /bin/ash
RUN rm /bin/sh
ENTRYPOINT ["./RocketPDF.Api"]