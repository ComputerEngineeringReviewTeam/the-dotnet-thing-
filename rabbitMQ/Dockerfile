FROM rabbitmq:3-management

# Enable MQTT plugin
RUN rabbitmq-plugins enable --offline rabbitmq_mqtt

# Expose RabbitMQ ports
EXPOSE 5672 15672 1883

# Start RabbitMQ
CMD ["rabbitmq-server"]