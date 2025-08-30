FROM mcr.microsoft.com/dotnet/sdk:9.0


RUN apt-get update && \
    apt-get install -y --no-install-recommends \
    build-essential   \ 
    && rm -rf /var/lib/apt/lists/*

COPY ../../Proxmox.Fqdn.Exporter /app

WORKDIR /app

CMD ["dotnet" , "publish", "-c", "Release", "-p:PublishAot=true", "--self-contained", "-o", "/out"]