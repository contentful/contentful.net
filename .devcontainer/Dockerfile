ARG DOTNET_VERSION=6.0.428-1-bookworm-slim
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION}

RUN apt-get update \
    && export DEBIAN_FRONTEND=noninteractive \
    && apt-get install -y --no-install-recommends \
        curl \
        git \
        openssh-client \
    && rm -rf /var/lib/apt/lists/*
