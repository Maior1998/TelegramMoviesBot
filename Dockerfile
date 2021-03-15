FROM mcr.microsoft.com/dotnet/sdk:5.0
ARG TELEGRAM_API
ARG TMDB_API_KEY
RUN apt-get update && apt-get upgrade -y
RUN git clone --branch test-branch https://github.com/Maior1998/TelegramMoviesBot.git \
        && cd TelegramMoviesBot/TelegramMoviesBot \
        && dotnet build TelegramMoviesBot.csproj --configuration Release
RUN echo #!/bin/sh > /root/init.sh \
        && echo /usr/bin/dotnet run /TelegramMoviesBot/TelegramMoviesBot/TelegramMoviesBot.csproj -c Release >> /root/init.sh \
        && chmod +x /root/init.sh
WORKDIR /TelegramMoviesBot/TelegramMoviesBot/
CMD  /root/init.sh