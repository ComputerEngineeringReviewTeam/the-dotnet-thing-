import paho.mqtt.client as mqtt
import json
import time
import random
import argparse
from datetime import datetime

# Command line arguments
parser = argparse.ArgumentParser()
parser.add_argument("--type", required=True, help="Sensor type (e.g., Temperature, Humidity)")
parser.add_argument("--id", type=int, required=True, help="Please specify the sensor id")
parser.add_argument("--interval", type=float, required=True, help="Interval between messages in seconds")
args = parser.parse_args()

BROKER = "rabbitmq-mqtt"
PORT = 1883
TOPIC = "sensor_data"

def on_message(client, userdata, msg):
    print(f"[x] Received: {msg.payload.decode()}")

client = mqtt.Client()
client.on_message = on_message

client.connect(BROKER, PORT)
client.subscribe(TOPIC) # could be removed, used just to see if RabbitMQ works

client.loop_start()

try:
    while True:
        measurement = {
            "sensorId": f"{args.type}_{args.id}",
            "sensorType": args.type,
            "value": round(random.uniform(0, 100), 2),
            "timestamp": datetime.utcnow().isoformat()
        }
        payload = json.dumps(measurement)
        client.publish(TOPIC, payload)
        print(f"[>] Sent: {payload}")
        time.sleep(args.interval)
except KeyboardInterrupt:
    print("Exiting...")

client.loop_stop()
client.disconnect()
