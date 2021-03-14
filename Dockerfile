FROM mcr.microsoft.com/dotnet/sdk:5.0
RUN apt-get update && apt-get install -y

RUN git clone --branch test-branch https://github.com/Maior1998/TelegramMoviesBot.git \
        && cd TelegramMoviesBot/TelegramMoviesBot \
        && dotnet restore TelegramMoviesBot.csproj \
        && dotnet build TelegramMoviesBot.csproj --configuration Release --no-restore \
        && dotnet run TelegramMoviesBot.csproj -c Release
