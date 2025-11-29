# The Dotnet Thingy
The Dotnet Thingy...

### Requirements:  
- Python 3
- Docker (with compose plugin)
- Terminal/Command Prompt
- Visual Studio Code (optional)
- .NET SDK 9.0+ (optional)

### Build and run the web application:  

Unless stated otherwise, all commands are to be ran from the project's root directory.

**Starting the application:**

The following command will run both the web application and the simulated devices.

```bash
docker compose up -d
```

To stop all services run:

```bash
docker compose down
```

**Running containers manually:**

To run the web application:

```bash
docker build -t rabbitmq-mqtt rabbitMQ/
docker build -t mongodb mongoDB/
docker build -t backend backend/
docker build -t frontend frontend/

docker network create backend-net

docker run -d --name rabbitmq-mqtt --network backend-net -p 5672:5672 -p 15672:15672 -p 1883:1883 rabbitmq-mqtt
docker run -d --name mongodb -p 27017:27017 --network backend-net -e MONGO_INITDB_ROOT_USERNAME=admin -e MONGO_INITDB_ROOT_PASSWORD=secret mongo:latest
docker run -d  -p 8080:8080 --name backend --network backend-net backend
docker run -d -p 80:80 --name frontend frontend
```

To run the sensor simulation:

```bash
docker build -t devices devices/
docker run -d --name devices --network backend-net devices
```

**Health check:**

Simple test if the backend works can be done by searching http://localhost:8080/api/demo. It should display JSON message containing a `Hello World` string.

It is possible to see console output with `docker logs -f backend` and execute commands with `docker exec backend <command>`.

Website shuld be available at http://localhost:80


### Available API endpoints:  

All API endpoints listed below will be available on `http://localhost:8080/api/<endpoint>`. Example: http://localhost:8080/api/measurement/json

`[GET] measurement` - all measurements, can be filtered and sorted 
`[GET] measurement/json` - all measurements (the same as measurement) 
`[GET] measurement/csv` - all measurements in CSV format, can be filtered and sorted 
`[DELETE] measurement` - CLEARS ENTIRE DATABASE! 
`[POST] measurement` - add new measurement 
`[GET] measurement/{id}` - get measurement with given ID 
`[PUT] measurement/{id}` - update measurement with given ID 
`[DELETE] measurement/{id}` - delete measurement with given ID 


