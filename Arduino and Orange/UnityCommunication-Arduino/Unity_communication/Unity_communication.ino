//*

#include <WiFi.h>
#include <WebServer.h>
#include <Adafruit_Sensor.h>
#include <Adafruit_ADXL345_U.h>

/*
const char* ssid = "Home_Net DECO";
const char* password = "2018_aoACASOzonnet1966#";
/*/
const char* ssid = "Visitors";
const char* password = "";
//*/
const int LEDPin = 23;
Adafruit_ADXL345_Unified accel = Adafruit_ADXL345_Unified(12345);
String newestData = "";

// Create a web server on port 80
WebServer server(80);

void setup() {
  Serial.begin(115200);
  pinMode(LEDPin, OUTPUT);


  // Initialise the sensor
  if(!accel.begin())
    Serial.println("Ooops, no ADXL345 detected ... Check your wiring!");
  accel.setRange(ADXL345_RANGE_16_G);
  
  
  // Connect to Wi-Fi
  WiFi.setMinSecurity(WIFI_AUTH_OPEN);
  WiFi.begin(ssid, password);
  Serial.print("Connecting to Wi-Fi");
  while (WiFi.status() != WL_CONNECTED) {
    delay(1000);
    Serial.print(".");
  }
  Serial.println("\nConnected to Wi-Fi!");
  Serial.println("IP address: ");
  Serial.println(WiFi.localIP());

  // Define routes
  server.on("/", handleRoot);
  server.on("/data", HTTP_POST, handleReceiveData);

  // Start the server
  server.begin();
  Serial.println("HTTP server started");
}

void handleRoot() {
  String message = "Hello from ESP32!";
  server.send(200, "text/plain", message);
}

void handleReceiveData() {
  if (server.hasArg("data")) {
    String receivedData = server.arg("data");

    if(receivedData == "ON"){
      digitalWrite(LEDPin, HIGH);
    }else if (receivedData == "OFF"){
      digitalWrite(LEDPin, LOW);
    }
    if(receivedData == "Accelerometer"){
      Serial.println("Received data: " + receivedData);
      Serial.println("Sent data: " + newestData);
      server.send(200, "text/plain", newestData);
    }
  } else {
    server.send(400, "text/plain", "No data sent.");
  }
}

void GetAccelerometerInfo(){
  sensors_event_t event; 
  accel.getEvent(&event);
 
  String data = "";
  data += String(event.acceleration.x);
  data += "\\";
  data += String(event.acceleration.y);
  data += "/";
  data += String(event.acceleration.z);
  newestData = data;

}

void loop() {
  server.handleClient();
  GetAccelerometerInfo();
  delay(100);
}

/*/


#include <WiFi.h>
#include <WebSocketsServer.h>

// Replace with your network credentials
const char* ssid = "Visitors";
const char* password = "";

// Create a WebSocket server on port 81
WebSocketsServer webSocket = WebSocketsServer(81);

void onWebSocketEvent(uint8_t client_num, WStype_t type, uint8_t* payload, size_t length) {
  switch (type) {
    case WStype_CONNECTED:
      Serial.printf("Client %u connected\n", client_num);
      webSocket.sendTXT(client_num, "Hello Client!");
      break;

    case WStype_TEXT:
      Serial.printf("Client %u sent: %s\n", client_num, payload);
      webSocket.sendTXT(client_num, String("Received: ") + (char*)payload);
      break;

    case WStype_DISCONNECTED:
      Serial.printf("Client %u disconnected\n", client_num);
      break;
  }
}

void setup() {
  Serial.begin(115200);
  
  // Connect to Wi-Fi
  WiFi.setMinSecurity(WIFI_AUTH_OPEN);
  WiFi.begin(ssid, password);
  Serial.print("Connecting to Wi-Fi");
  while (WiFi.status() != WL_CONNECTED) {
    delay(1000);
    Serial.print(".");
  }
  Serial.println("\nConnected to Wi-Fi!");
  Serial.println("IP address: " + WiFi.localIP().toString());

  // Initialize WebSocket server
  webSocket.begin();
  webSocket.onEvent(onWebSocketEvent);
}

void loop() {
  // Handle WebSocket events
  webSocket.loop();
}



//*/
