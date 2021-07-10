FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

COPY src ./
RUN dotnet publish ./CribblyBackend -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/out .
COPY ./wait-for-it.sh .
ENTRYPOINT [ "dotnet", "CribblyBackend.dll" ]