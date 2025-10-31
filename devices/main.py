import paho.mqtt.client as mqtt

BROKER = "localhost"
PORT = 1883
TOPIC = "test"

def on_message(client, userdata, msg):
    print(f"[x] Received: {msg.payload.decode()}")

client = mqtt.Client()
client.on_message = on_message

client.connect(BROKER, PORT)
client.subscribe(TOPIC)

# Publish a test message
client.publish(TOPIC, "Got data from device!")

client.loop_start()
input("Press Enter to quit...\n")
client.loop_stop()
client.disconnect()