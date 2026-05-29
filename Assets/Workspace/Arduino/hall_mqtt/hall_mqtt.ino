#include <WiFi.h>
#include <PubSubClient.h>

// WiFi setup - Default setup to connect to Extrality Lab WiFi
const char *wifi_ssid = "dsv-extrality-lab";            // Enter your Wi-Fi name
const char *wifi_password = "expiring-unstuck-slider";  // Enter Wi-Fi password

WiFiClient espClient;  // variable in charge of WiFi connection

// MQTT Broker setup - Default setup to connect to ExtralityLab Broker
const char *mqtt_broker = "inf-liva-server-eth.dsv.local.su.se";
const int mqtt_port = 1883;
const char *mqtt_username = "esp32";
const char *mqtt_password = "extrality";

PubSubClient client(espClient);  // variable in charge of Mqtt connection
String client_id = "MMYC/";

const int hall_0 = A0;
const int hall_1 = A1;
const int hall_2 = A2;
const int hall_3 = A3;

void setup() {
  Serial.begin(9600);
  while (!Serial && millis() < 5000)
    ;
  delay(100);

  // Connecting to a WiFi network
  WiFi.begin(wifi_ssid, wifi_password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.println("Connecting to WiFi..");
  }
  Serial.println("Connected to the Wi-Fi network");

  // Connecting to MQTT broker
  client.setServer(mqtt_broker, mqtt_port);
  bool connectedMqtt = ensureMqtt();
}

void loop() {
  // Ensure connections
  ensureMqtt();
  client.loop();

  int value_0 = analogRead(hall_0);
  int value_1 = analogRead(hall_1);
  int value_2 = analogRead(hall_2);
  int value_3 = analogRead(hall_3);

  client.publish((client_id + "hall_0").c_str(), String(value_0).c_str());
  client.publish((client_id + "hall_1").c_str(), String(value_1).c_str());
  client.publish((client_id + "hall_2").c_str(), String(value_2).c_str());
  client.publish((client_id + "hall_3").c_str(), String(value_3).c_str());
  delay(1000);
}

bool ensureMqtt() {
  if (client.connected()) return true;
  while (!client.connected()) {
    Serial.printf("The client %s connects to the ExtralityLab MQTT broker\n", client_id.c_str());
    bool isConnected = client.connect(client_id.c_str(), mqtt_username, mqtt_password);
    if (isConnected) {
      Serial.println("ExtralityLab MQTT broker connected");
      return true;
    } else {
      Serial.print("Failed with state ");
      Serial.print(client.state());
      delay(2000);
    }
  }
}
