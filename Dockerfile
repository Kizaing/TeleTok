FROM mcr.microsoft.com/dotnet/runtime:6.0-alpine3

WORKDIR /app/teletok

RUN apk update && apk add --update nodejs nodejs-npm 

RUN npm i -g tiktok-scraper

COPY bin/Debug/net6.0/* /app/teletok/

CMD [ "TeleTok" ]