FROM microsoft/dotnet:2.0-sdk

COPY . /app

WORKDIR /app

RUN ["dotnet", "restore"]

RUN ["dotnet", "build"]

EXPOSE 80/tcp

ENTRYPOINT ["dotnet", "run", "--server.urls", "http://0.0.0.0:80"]
