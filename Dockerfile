FROM mcr.microsoft.com/dotnet/sdk:5.0
ARG TELEGRAM_API
ARG TMDB_API_KEY
RUN apt-get update && apt-get upgrade -y
RUN git clone --branch test-branch https://github.com/Maior1998/TelegramMoviesBot.git \
        && cd TelegramMoviesBot/TelegramMoviesBot \
        && dotnet build TelegramMoviesBot.csproj --configuration Release
WORKDIR /TelegramMoviesBot/TelegramMoviesBot/
ENTRYPOINT ["/usr/bin/env -v TELEGRAM_API=$TELEGRAM_API TMDB_API_KEY=$TMDB_API_KEY /usr/bin/dotnet run /TelegramMoviesBot/TelegramMoviesBot/TelegramMoviesBot.csproj -c Release"]
