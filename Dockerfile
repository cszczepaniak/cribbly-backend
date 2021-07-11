FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

COPY src ./
RUN dotnet publish ./CribblyBackend -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0

ARG DB_USER
ARG DB_PASSWORD
ARG DB_HOST
ARG DB_PORT
ARG DB_NAME

ENV DB_USER $DB_USER
ENV DB_PASSWORD $DB_PASSWORD
ENV DB_HOST $DB_HOST
ENV DB_PORT $DB_PORT
ENV DB_NAME $DB_NAME

WORKDIR /app
COPY --from=build-env /app/out .
COPY ./wait-for-it.sh .
ENTRYPOINT [ "dotnet", "CribblyBackend.dll" ]