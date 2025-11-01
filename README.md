# The Dotnet Thingy
The Dotnet Thingy...  

### Requirements:  
- Python 3
- Docker
- Terminal/Command Prompt  
- Visual Studio Code (optional)  
- .NET SDK 9.0+ (optional)

### Build and run the application:  
```bash

docker build -t rabbitmq-mqtt rabbitMQ/
docker build -t mongo-db mongoDB/
docker build -t backend backend/
docker network create backend-net

docker run -d --name rabbitmq-mqtt --network backend-net -p 5672:5672 -p 15672:15672 -p 1883:1883 rabbitmq-mqtt
docker run -d --name mongodb -p 27017:27017 --network backend-net -e MONGO_INITDB_ROOT_USERNAME=admin -e MONGO_INITDB_ROOT_PASSWORD=secret mongo:latest
docker run -d  -p 8080:8080 --name backend --network backend-net backend
```

### Check if setup works correctly:  
After running all the components, run the python script `main.py` in `devices/` folder. Backend application should log to the console sent message. It is possible to see console output with `docker logs -f backend`.

Site API will be available on http://localhost:8080/api/demo. For now it should display simple JSON message containing 'Hello World' string.
