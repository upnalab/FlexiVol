#include "DacTone.h"
 
DacTone audio;

int chosenFrequency = 11;
int calculateMydelay;
int newFreq = 1;
int oldFreq = 1;
boolean newData = false;
const byte numChars = 32;
char receivedChars[numChars];   // an array to store the received data
int dataNumber = 0;             // new for this version



void setup() {
  // put your setup code here, to run once:
  Serial.begin(115200);
  pinMode(33, OUTPUT);
  pinMode(2, OUTPUT);
  chosenFrequency = 11;
}

void loop() {
  // put your main code here, to run repeatedly:
  receiveData();
  lesgo();

  // 1/Hz = seconds; multiply by 1000 to go millis
}

void receiveData()
{
  static byte ndx = 0;
  char endMarker = '\n';
  char rc;
  
  if (Serial.available() > 0) {
    rc = Serial.read();

    if (rc != endMarker) {
      receivedChars[ndx] = rc;
      ndx++;
      if (ndx >= numChars) {
          ndx = numChars - 1;
      }
    }
    else {
      receivedChars[ndx] = '\0'; // terminate the string
      ndx = 0;
      newData = true;
    }
  }
}

void lesgo()
{
  if (newData == true) {
    dataNumber = 0;             // new for this version
    dataNumber = atoi(receivedChars);   // new for this version
    chosenFrequency = dataNumber;
    newData = false;
    Serial.print("Frequency: ");
    Serial.println(chosenFrequency);
    calculateMydelay = ((float)1/chosenFrequency * 1000);
    Serial.print("Delay: ");
    Serial.println(calculateMydelay);

  }
 
 
  if(chosenFrequency != 0)
  {
    audio.tone(chosenFrequency);
    digitalWrite(33, HIGH);
    digitalWrite(2, HIGH);


    delay((int)calculateMydelay);

    digitalWrite(33, LOW);
    digitalWrite(2, LOW);


    delay((int)calculateMydelay);
    audio.noTone();
  }
}
