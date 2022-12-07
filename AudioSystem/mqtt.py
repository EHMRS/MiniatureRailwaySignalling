"""! @brief MQTT Client functions"""

import os
import json
import ssl
import logging

from paho.mqtt import client as mqtt_client
from message_handlers import handle_message


class MqttConnector:
    """! Mqtt Connector class """
    broker = os.getenv("MQTT_BROKER")
    port = os.getenv("MQTT_PORT")
    client_id = 'audioplayer'
    username = os.getenv("MQTT_USERNAME")
    password = os.getenv("MQTT_PASSWORD")

    client = None
    connected = False

    def init(self):
        """! Initialise the module """
        logging.info("Connecting to MQTT")
        self.connect_mqtt()
        self.client.loop_start()
        while not self.connected:
            pass
        logging.info("Connected")
        self.client.subscribe("signalling/audio/play")

    def connect_mqtt(self):
        """! Connect to the MQTT server """
        # Set Connecting Client ID
        self.client = mqtt_client.Client(self.client_id)
        # Set the connection parameters
        self.client.username_pw_set(self.username, self.password)
        self.client.tls_set(cert_reqs=ssl.CERT_NONE)
        self.client.tls_insecure_set(True)

        # Set the callbacks
        self.client.on_connect = self.on_connect
        self.client.on_message = self.on_message

        # Actually connect
        self.client.connect(self.broker, int(self.port))

    def on_connect(self, client, userdata, flags, response_code):       # pylint:disable=unused-argument
        """! Callback on client connect 
        
        @param client The client object
        @param userdata Data on the user
        @param flags Flags!
        @param response_code Response code from the server

        @return None
        """
        if response_code == 0:
            logging.debug("Connected to MQTT broker!")
            self.connected = True
        else:
            logging.debug("Failed to connect, return code %s", response_code)

    def on_message(self, client, userdata, message):                    # pylint:disable=unused-argument
        """! Callback on message received 
        
        @param client The client object
        @param userdata Data on the user
        @param message Object representing the message that has been received

        @return None
        """
        outermessage = json.loads(message.payload.decode("utf-8"))
        try:
            payload = outermessage['payload']
        except KeyError:
            # Invalid message
            logging.debug("Invalid message: %s", message.payload)
            return

        handle_message(message.topic[17:], payload)


    def send_message(self, topic, payload):
        """! Helper function to send a message 
        
        @param topic The topic to which to send a message
        @param payload The payload to send in the message

        @return None
        """
        self.client.publish(topic, self.prep_payload(payload))

    def prep_payload(self, data):
        """! Helper function to prepare the wrapper for the message
        
        @param data The data to wrap within the envelope

        @return The wrapped message
        """
        payload = {
            "username": "system",
            "source": "audioplayer",
            "payload": data
        }
        return json.dumps(payload)

mqtt = MqttConnector()
