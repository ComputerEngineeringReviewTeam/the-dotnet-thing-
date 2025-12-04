FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY *.csproj .

# Restore dependencies for the project
RUN dotnet restore

COPY . .

RUN dotnet add package Nethereum.Web3
RUN dotnet add package Nethereum.Accounts
RUN dotnet add package Nethereum.Hex

RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app .

COPY MyToken.sol .
COPY deploy.js .
COPY setup.sh .

EXPOSE 5253

# Debug purposes
RUN apt update && apt install -y netcat-openbsd curl gnupg

RUN curl -fsSL https://deb.nodesource.com/setup_20.x | bash - \
	&& apt install -y nodejs

RUN npm init -y
RUN npm install solc@0.8.19 web3 @openzeppelin/contracts@4.9.2

RUN mkdir build/
RUN npx solcjs --base-path ./node_modules --include-path ./node_modules --optimize --bin --abi MyToken.sol -o build/

ENTRYPOINT ["/bin/bash", "setup.sh"]
