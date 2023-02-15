[![Build Status](https://ci.kizaing.ca/api/badges/kizaing/TeleTok/status.svg)](https://ci.kizaing.ca/kizaing/TeleTok)

# TeleTok Telegram Bot
This bot will monitor any chats for TikTok links that are posted, and then will run the link through a [ProxiTok](https://github.com/pablouser1/ProxiTok) instance to generate a download link. The resulting video file will then get directly uploaded directly to your chat.

## Building
Requirements: Dotnet SDK 6.0

Run `dotnet build` for a Debug instance
Run `dotnet publish` for a full release

## Installation
### Requirements
 * A telegram bot token
 * A working [ProxiTok](https://github.com/pablouser1/ProxiTok) instance, either public or hosted yourself. (Please be kind and take bandwidth into account when using a public instance)

### Binary
 1. Either build the application or download a release zip for your OS/architecture
 2. Edit the config.json and put your telegram bot token in the "token" field, and your chosen ProxiTok instance in the "ptInstance" field
 3. Run the TeleTok.exe/TeleTok executable

### Docker
 1. You can build an image for yourself with `docker build teletok .` or you can pull a prebuilt image with `docker pull kizaing/teletok:latest`
 2. Copy the placeholder config.json from the repo to a location of your choosing
 3. Edit the config.json and put your telegram bot token in the "token" field, and your chosen ProxiTok instance in the "ptInstance" field
 4. Run the container with `docker run -v ${PWD}/config.json:/app/teletok/config.json -d kizaing/teletok:latest`


## To Do
 - [x] Add better error and link handling
 - [x] Process videos into telegram
 - [ ] Add [Matrix](https://matrix.org) support
 - [ ] Automate binary release publishing