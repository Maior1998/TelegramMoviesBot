FROM mcr.microsoft.com/dotnet/aspnet:5.0
COPY bin/Release/net5.0 ref/
WORKDIR /ref
ENTRYPOINT ["dotnet", "TelegramMoviesBot.dll"]
