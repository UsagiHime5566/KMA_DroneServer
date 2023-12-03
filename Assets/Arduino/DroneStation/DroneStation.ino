String inputString = "";     // a String to hold incoming data
bool stringComplete = false; // whether the string is complete

void setup()
{
  // initialize serial:
  Serial.begin(9600);
  // reserve 200 bytes for the inputString:
  inputString.reserve(200);

  pinMode(2, OUTPUT);
  digitalWrite(2, HIGH);
  pinMode(3, OUTPUT);
  digitalWrite(3, HIGH);
  pinMode(4, OUTPUT);
  digitalWrite(4, HIGH);
  pinMode(5, OUTPUT);
  digitalWrite(5, HIGH);
  pinMode(6, OUTPUT);
  digitalWrite(6, HIGH);
  pinMode(7, OUTPUT);
  digitalWrite(7, HIGH);
  pinMode(8, OUTPUT);
  digitalWrite(8, HIGH);
  pinMode(9, OUTPUT);
  digitalWrite(9, HIGH);
  pinMode(10, OUTPUT);
  digitalWrite(10, HIGH);
  pinMode(11, OUTPUT);
  digitalWrite(11, HIGH);
}

void loop()
{
  // print the string when a newline arrives:
  if (stringComplete)
  {
    // Serial.println(inputString);
    relay(inputString);
    // clear the string:
    inputString = "";
    stringComplete = false;
  }
}

/*
  SerialEvent occurs whenever a new data comes in the hardware serial RX. This
  routine is run between each time loop() runs, so using delay inside loop can
  delay response. Multiple bytes of data may be available.
*/

void relay(String cmd)
{
  if (cmd.substring(0, cmd.length() - 1) == "1on")
  {
    Serial.println("1 on");
    digitalWrite(4, HIGH);
    digitalWrite(5, LOW);
    digitalWrite(6, HIGH);
    digitalWrite(7, LOW);
  }
  else if (cmd.substring(0, cmd.length() - 1) == "1off")
  {
    Serial.println("1 off");
    digitalWrite(4, LOW);
    digitalWrite(5, HIGH);
    digitalWrite(6, LOW);
    digitalWrite(7, HIGH);
  }
  else if (cmd.substring(0, cmd.length() - 1) == "2on")
  {
    Serial.println("2 on");
    digitalWrite(8, HIGH);
    digitalWrite(9, LOW);
    digitalWrite(10, HIGH);
    digitalWrite(11, LOW);
  }
  else if (cmd.substring(0, cmd.length() - 1) == "2off")
  {
    Serial.println("2 off");
    digitalWrite(8, LOW);
    digitalWrite(9, HIGH);
    digitalWrite(10, LOW);
    digitalWrite(11, HIGH);
  }
  else if (cmd.substring(0, cmd.length() - 1) == "close")
  {
    Serial.println("all off");

    digitalWrite(2, HIGH);
    digitalWrite(3, HIGH);
    digitalWrite(4, HIGH);
    digitalWrite(5, HIGH);
    digitalWrite(6, HIGH);
    digitalWrite(7, HIGH);
    digitalWrite(8, HIGH);
    digitalWrite(9, HIGH);
    digitalWrite(10, HIGH);
    digitalWrite(11, HIGH);
  }
  else if (cmd.substring(0, cmd.length() - 1) == "pon")
  {
    Serial.println("power on");
  }
  else if (cmd.substring(0, cmd.length() - 1) == "poff")
  {
    Serial.println("power off");
  }
  else if (cmd.substring(0, cmd.length() - 1) == "con")
  {
    Serial.println("charging on");
    digitalWrite(3, LOW);
  }
  else if (cmd.substring(0, cmd.length() - 1) == "coff")
  {
    Serial.println("charging off");
    digitalWrite(3, HIGH);
  }
}

void serialEvent()
{
  while (Serial.available())
  {
    // get the new byte:
    char inChar = (char)Serial.read();
    // add it to the inputString:
    inputString += inChar;
    // if the incoming character is a newline, set a flag so the main loop can
    // do something about it:
    if (inChar == '\n')
    {
      stringComplete = true;
    }
  }
}
