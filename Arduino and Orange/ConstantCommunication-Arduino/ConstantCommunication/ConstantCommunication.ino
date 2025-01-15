#include <WiFi.h>
#include <Adafruit_Sensor.h>
#include <Adafruit_ADXL345_U.h>


const char* ssid = "Visitors";
const char* password = "";
const char* ssid = "Home_Net DECO";
const char* password = "2018_aoACASOzonnet1966#";

Adafruit_ADXL345_Unified accel = Adafruit_ADXL345_Unified(12345);
WiFiServer server(80);

void setup() {
    Serial.begin(115200);


    if(!accel.begin())
      Serial.println("Ooops, no ADXL345 detected ... Check your wiring!");
    accel.setRange(ADXL345_RANGE_16_G);


    IPAddress local_IP(192, 168, 68, 118);
    IPAddress gateway(192, 168, 68, 118);
    // IPAddress subnet(255, 255, 255, 0);
    // IPAddress primaryDNS(208, 67, 222, 222);
    // IPAddress secondaryDNS(208, 67, 220, 220);

    WiFi.config(local_IP, gateway);
    WiFi.setMinSecurity(WIFI_AUTH_OPEN);
    WiFi.begin(ssid, password);

    while (WiFi.status() != WL_CONNECTED) {
        delay(1000);
        Serial.println("Connecting to Wi-Fi...");
    }
    Serial.println("Connected!");
    Serial.println("IP address: ");
    Serial.println(WiFi.localIP());
    server.begin();
}

void loop() {
  static unsigned long lastUpdateTime = 0; // Tracks the last time data was sent
  const unsigned long updateInterval = 100; // 100 milliseconds = 10 Hz

  WiFiClient client = server.available(); // Listen for incoming clients
  
  if (client) {
    Serial.println("New client connected");

    while (client.connected()) {
      unsigned long currentTime = millis();
      if (currentTime - lastUpdateTime >= updateInterval) {
          lastUpdateTime = currentTime; // Update the last time

          sensors_event_t event; 
          accel.getEvent(&event);
          
          // Generate and send data
          float value1 = event.acceleration.x;
          float value2 = event.acceleration.y;
          float value3 = event.acceleration.z;

          String message = String(value1) + "\\" + String(value2) + "/" + String(value3) + "\n";
          client.print(message); // Send data to the client
          // Serial.print("Data sent: " + message);
      }
      
      if (client.available()) {
        String received = client.readStringUntil('\n'); // Read data from Unity
        Serial.println("Received: " + received);
      }
    }
    client.stop();
    Serial.println("Client disconnected");
  }
}
