#define SINE_PIN DAC1 //25
#define SIN_PIN DAC2
#define TRIGGER_PIN 13
#define START_FR 11
#define START_PHASE 0
#define SINE_AMP 0.9 //0.9v is typical for audio line level


#define DIVS_PER_PERIOD 256
const int triggerDuration = DIVS_PER_PERIOD/8;
uint8_t SINE_LUT[DIVS_PER_PERIOD];
uint8_t SINE_LUT2[DIVS_PER_PERIOD];

#define TIMER_FR 40000000
hw_timer_t *timer0 = NULL;
hw_timer_t *timer1 = NULL;

int sineLUTIndex = 0;
int sineLUTIndex2 = 0;

int phase = START_PHASE;
int triggerCounter = 0;
int triggerCounter2 = 0;

void IRAM_ATTR onTimer() {
  dacWrite(DAC1, SINE_LUT[sineLUTIndex]);

  if (sineLUTIndex == phase){
    digitalWrite(TRIGGER_PIN, HIGH );
    triggerCounter = triggerDuration;
  }else{
    if (triggerCounter > 0){
      triggerCounter -= 1;
    }else{
      digitalWrite(TRIGGER_PIN, LOW );
    }
  }

  sineLUTIndex = (sineLUTIndex+1) % (DIVS_PER_PERIOD);
}

void IRAM_ATTR onTimer1() {
  dacWrite(DAC2, SINE_LUT2[sineLUTIndex2]);

  if (sineLUTIndex2 == phase){
    triggerCounter2 = triggerDuration;
  }else{
    if (triggerCounter2 > 0){
      triggerCounter2 -= 1;
    }
  }

  sineLUTIndex2 = (sineLUTIndex2+1) % (DIVS_PER_PERIOD);
}

void fillInLUT(){
  for(int i = 0; i < DIVS_PER_PERIOD; i++){
    const uint8_t value = SINE_AMP * (sin(2*PI *i/DIVS_PER_PERIOD)+1)/2 / 3.3*256;
    SINE_LUT[i] = value;
    SINE_LUT2[i] = value;
  }
}

void startWithFr(float fr){
  uint64_t ticks = TIMER_FR / DIVS_PER_PERIOD / fr;
  timerAlarm(timer0, ticks, true, 0);
}

void startWithFr2(float fr){
  uint64_t ticks = TIMER_FR / DIVS_PER_PERIOD / fr;
  timerAlarm(timer1, ticks, true, 0);
}

void setup() {
  fillInLUT();
  pinMode(26, OUTPUT);

  pinMode(TRIGGER_PIN, OUTPUT);
  pinMode(SINE_PIN, OUTPUT);
  digitalWrite(TRIGGER_PIN, LOW);
  digitalWrite(SINE_PIN, LOW);
  
  timer0 = timerBegin(TIMER_FR); 
  timerAttachInterrupt(timer0, &onTimer);
  
  timer1 = timerBegin(TIMER_FR); 
  timerAttachInterrupt(timer1, &onTimer1);

  startWithFr( START_FR );
  startWithFr2( START_FR );

  Serial.begin(115200);
  Serial.println("Sine + trigger");
}

void loop() {
  if (Serial.available() > 0){
    const int command = Serial.parseInt();
    
    if (command == 0){ //stop
      timerStop(timer0);
      digitalWrite(TRIGGER_PIN, LOW);
      digitalWrite(SINE_PIN, LOW);
      Serial.println("off");
    }else if (command == 1){ //set frequency
      const float fr = Serial.parseFloat();
      startWithFr( fr );
      Serial.print("FR set at ");
      Serial.println(fr);
    }else if (command == 2){ //set phase
      int nPhase = Serial.parseInt();
      if (nPhase >= DIVS_PER_PERIOD){
        nPhase = DIVS_PER_PERIOD-1;
      }else if (nPhase < 0){
        nPhase = 0;
      }
      phase = nPhase;
      Serial.print("Phase set at ");
      Serial.println(phase);
    }
    else if (command == 3){ //set frequency speaker
      const float fr = Serial.parseFloat();
      startWithFr2( fr );
      Serial.print("FR2 set at ");
      Serial.println(fr);
    }
    while (Serial.read() != '\n');
  }
  
}