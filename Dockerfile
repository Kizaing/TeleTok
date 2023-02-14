#Builds the bot from source
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-task

COPY . /build
RUN cd /build && dotnet publish

# Actually runs the bot
FROM mcr.microsoft.com/dotnet/runtime:6.0-alpine3

WORKDIR /app/teletok

COPY --from=build-task /build/bin/Debug/net6.0/publish/* /app/teletok/

CMD [ "TeleTok" ]