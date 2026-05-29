const int hallPin = A5;

void setup() {
  Serial.begin(9600);
}

void loop() {
  int value = analogRead(hallPin);
  Serial.println(value);
  delay(100);
}u
