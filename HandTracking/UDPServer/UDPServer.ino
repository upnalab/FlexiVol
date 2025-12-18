#include <WiFi.h>
#include <WiFiUdp.h>

const char *ssid = "MakerSkills";
const char *password = "12345678";
IPAddress localIP(192, 168, 1, 1); 

WiFiUDP udp;

void setup() {
  Serial.begin(115200);
  Serial.println();

  // Set up WiFi access point
  WiFi.softAP(ssid, password);
  WiFi.softAPConfig(localIP, localIP, IPAddress(255, 255, 255, 0));
  IPAddress IP = WiFi.softAPIP();
  Serial.print("AP IP address: ");
  Serial.println(IP);

  // Begin UDP
  udp.begin(12345); // Port to listen for UDP messages
}

char packetBuffer[255];
void loop() {
  // Check if data has been received
  int packetSize = udp.parsePacket();
  if (packetSize) {
    
    // Allocate buffer to hold incoming packet
   
    // Read packet into buffer
    int len = udp.read(packetBuffer, 255);
    if (len > 0) {
      packetBuffer[len] = 0; // Null-terminate the string
    }

    // Get IP address and port of sender
    IPAddress remoteIP = udp.remoteIP();
    uint16_t remotePort = udp.remotePort();

    // Print received packet, sender's IP, and sender's port
    Serial.print("UDP packet contents: ");
    Serial.println(packetBuffer);
    Serial.print("From IP: ");
    Serial.print(remoteIP);
    Serial.print(", port: ");
    Serial.println(remotePort);
    Serial.println("");
  }
  delay(10);
}
