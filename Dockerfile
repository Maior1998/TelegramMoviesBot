FROM mcr.microsoft.com/dotnet/sdk:5.0
RUN apt-get update && apt-get upgrade -y
ENV TELEGRAM_API=$TELEGRAM_API
ENV TMDB_API_KEY=$TMDB_API_KEY
RUN git clone --branch test-branch https://github.com/Maior1998/TelegramMoviesBot.git \
        && cd TelegramMoviesBot/TelegramMoviesBot \
        && dotnet build TelegramMoviesBot.csproj --configuration Release \
        && env -v TELEGRAM_API=$TELEGRAM_API TMDB_API_KEY=$TMDB_API_KEY dotnet run TelegramMoviesBot.csproj -c Release
