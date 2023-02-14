FROM mcr.microsoft.com/dotnet/runtime:6.0-alpine3

WORKDIR /app/teletok

COPY bin/Debug/net6.0/publish/* /app/teletok/

CMD [ "TeleTok" ]