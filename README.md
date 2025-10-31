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
docker build -t backend backend/
docker build -t rabbitmq-mqtt rabbitMQ/
docker network create mqtt-net
docker run -d --name rabbitmq-mqtt --network mqtt-net -p 5672:5672 -p 15672:15672 -p 1883:1883 rabbitmq-mqtt
docker run -d  -p 8080:8080 --name backend --network mqtt-net backend
```

### Check if setup works correctly:  
After running all the components, run the python script `main.py` in `devices/` folder. Backend application should log to the console sent message. It is possible to see console output with `docker logs -f backend`.

Site API will be available on http://localhost:8080/api/demo. For now it should display simple JSON message containing 'Hello World' string.
