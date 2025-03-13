#define SINE_PIN DAC1 //25
#define TRIGGER_PIN 33
#define START_FR 11.5
#define START_PHASE 0
#define SINE_AMP 0.9 //0.9v is typical for audio line level

#define DIVS_PER_PERIOD 256

uint8_t SINE_LUT[DIVS_PER_PERIOD];

hw_timer_t *timer0 = NULL;

int count = 0;

int phase = START_PHASE;
int triggerCounter = 0;
const int triggerDuration = DIVS_PER_PERIOD/2;

void IRAM_ATTR onTimer() {
  dacWrite(DAC1, SINE_LUT[count]);

  if ((count - phase) == 0){
    digitalWrite(TRIGGER_PIN, HIGH );
    triggerCounter = triggerDuration;
  }else{
    if (triggerCounter > 0){
      triggerCounter -= 1;
    }else{
      digitalWrite(TRIGGER_PIN, LOW );
    }
  }

  count = (count+1) % DIVS_PER_PERIOD;
}

void fillInLUT(){
  for(int i = 0; i < DIVS_PER_PERIOD; i++){
    const uint8_t value = SINE_AMP * (sin(2*PI *i/DIVS_PER_PERIOD)+1)/2 / 3.3*256;
    SINE_LUT[i] = value;
  }
}

void startWithFr(float fr){
  uint64_t ticks = 80000000 / DIVS_PER_PERIOD / fr;
  timerAlarmWrite(timer0, ticks, true);
  timerWrite(timer0, 0);
  timerAlarmEnable(timer0);
}

void setup() {
  fillInLUT();

  pinMode(TRIGGER_PIN, OUTPUT);
  pinMode(SINE_PIN, OUTPUT);
  digitalWrite(TRIGGER_PIN, LOW);
  digitalWrite(SINE_PIN, LOW);
  
  timer0 = timerBegin(0, 1, true); //timer0 with no divider, repeat 
  timerAttachInterrupt(timer0, &onTimer, true);
  
  // timer_set_alarm();
  // timer_set_alarm_value(155);
  // timer_set_counter_value(200);
  // timer_set_auto_reload(132);


  startWithFr( START_FR );

  Serial.begin(115200);
}

void loop() {
  if (Serial.available() > 0){
    const int command = Serial.parseInt();
    
    if (command == 0){ //stop
      timerAlarmDisable(timer0);
      digitalWrite(TRIGGER_PIN, LOW);
      digitalWrite(SINE_PIN, LOW);
    }else if (command == 1){ //set frequency
      const float fr = Serial.parseFloat();
      startWithFr( fr );
    }else if (command == 2){ //set phase
      int nPhase = Serial.parseInt();
      if (nPhase >= DIVS_PER_PERIOD){
        nPhase = DIVS_PER_PERIOD-1;
      }else if (nPhase < 0){
        nPhase = 0;
      }
      phase = nPhase;
    }

    while (Serial.read() != '\n');
  }
}