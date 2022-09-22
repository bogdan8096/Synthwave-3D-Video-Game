// Zone to declare Preprocessor Directives
#define joystickPinX     A0       // Analog Pin A0 of Arduino Nano connecetd to X pin of the Joystick
#define joystickPinY     A1       // Analog Pin A1 of Arduino Nano connecetd to Y pin of the Joystick
#define buttonPin0       PD4      // Digital Pin PD4 of Arduino Nano connected to Button 0
#define buttonPin1       PD5      // Digital Pin PD5 of Arduino Nano connected to Button 1
#define buttonPin2       PD6      // Digital Pin PD6 of Arduino Nano connected to Button 2
#define buttonPin3       PD7      // Digital Pin PD7 of Arduino Nano connected to Button 3
#define dataBufferLength 5        // Length of the Data Buffer Array

// Zone to declare variables to store Analog Inputs of the Joystick Module
int analogValueX     = 0;         // Variable to store the value from Analog Pin A0 of Arduino Nano
int analogValueY     = 0;         // Variable to store the value from Analog Pin A1 of Arduino Nano 
int lastAnalogValueX = 500;       // Store last read Analog Value to compare to current Analog Value of Pin A0
int lastAnalogValueY = 500;       // Store last read Analog Value to compare to current Analog Value of Pin A1
int analogThreshold  = 500;       // Treshold to check if New Joystick Direction Inputs has been made

// Zone to declare variables to store Digital Inputs of the Buttons
byte digitalValueB0 = 0;          // Variable to store the value from Digital Pin PD4 of Arduino Nano
byte digitalValueB1 = 0;          // Variable to store the value from Digital Pin PD5 of Arduino Nano
byte digitalValueB2 = 0;          // Variable to store the value from Digital Pin PD6 of Arduino Nano
byte digitalValueB3 = 0;          // Variable to store the value from Digital Pin PD7 of Arduino Nano
byte lastDigitalValueB0 = 0;      // Store last read Digital Value to compare to current Digital Value of Pin PD4
byte lastDigitalValueB1 = 0;      // Store last read Digital Value to compare to current Digital Value of Pin PD5
byte lastDigitalValueB2 = 0;      // Store last read Digital Value to compare to current Digital Value of Pin PD6
byte lastDigitalValueB3 = 0;      // Store last read Digital Value to compare to current Digital Value of Pin PD7

// Zone to delcare variables used in Data Transmision using Bluetooth Module HC-05
#define bitMaskFirstByte  1       // Bit Mask used to store Button State in the Command Data Buffer on bit 1
#define bitMaskSecondByte 2       // Bit Mask used to store Button State in the Command Data Buffer on bit 2
byte dataBuffer[dataBufferLength];// Data Buffer to store Command Byte Array to be sent via Bluetooth Module HC-05 to Unity

void setup()
{
  // Set the Baud Rate to communicate with the Bluetooth Module HC-05 via Serial Comunication
  Serial.begin(9600);
  // Configure Arduino Nano Pins to be used as Inputs
  pinMode(joystickPinX, INPUT);
  pinMode(joystickPinY, INPUT);
  pinMode(buttonPin0, INPUT);
  pinMode(buttonPin1, INPUT);
  pinMode(buttonPin2, INPUT);
  pinMode(buttonPin3, INPUT);
  // The last byte of the Data Buffer equals 255, we will use it as a seperator between packets
  dataBuffer[4] = 255;
}

void loop()
{
  // Read and store the Analog Inputs of the Arduino Nano from the Joystick Module
  analogValueX = analogRead(joystickPinX);
  delay(10);
  analogValueY = analogRead(joystickPinY);
  delay(10);
  
  // Read and store the Digital Inputs of the Arduino Nano from the Buttons
  digitalValueB0 = digitalRead(buttonPin0);
  digitalValueB1 = digitalRead(buttonPin1);
  digitalValueB2 = digitalRead(buttonPin2);
  digitalValueB3 = digitalRead(buttonPin3);
  delay(10);

  // Check if we can send the Command Data Array based on the current Inputs
  if(newInputDataToSend())
  {
    fillDataBuffer();
    Serial.write(dataBuffer, dataBufferLength);
  }
  saveLastReadData();
}

void fillDataBuffer() 
{ 
  /* We want to make the byte 255 (11111111) a seperator between packets
   * So 255 must never transmited as an AnalogRead.
   * We will exploit the fact that the Joystick Analog values x1 and x2 are 0 to 1023.
   * 0 to 1023 needs 10 bits because of (2^10 = 1024)
   * 
   * Second Byte | First Byte
   * 00 00 00 xx | xx xx xx xx
   * 
   * where x is a used bit
   * 
   * by shifting 3 bits
   * 
   * Second Byte | First Byte
   * 00 0x xx xx | xx xx x0 00 
   * 
   * Now it's impposible to have (11111111) or (255)
   * 
   * Lastly, for performance, save the button states, 0 or 1, as bits in the Command Data Buffer
   * Using the Bit Masks for Bit Position 1 and 2; b represents state of buttons
   * Second Byte | First Byte
   * 00 0x xx xx | xx xx xb b0 
   */

  // Store the Shifted Analog Inputs to local Variables
  int tempAnalogValueX = analogValueX << 3; 
  int tempAnalogValueY = analogValueY << 3; 

  // Build the Command Data Buffer based on the Shifted Analog Inputs
  dataBuffer[0] = lowByte(tempAnalogValueX);
  dataBuffer[1] = highByte(tempAnalogValueX);
  dataBuffer[2] = lowByte(tempAnalogValueY);
  dataBuffer[3] = highByte(tempAnalogValueY);
  // Use the Defined Bit Masks to store the States of all 4 Buttons
  dataBuffer[0] |= (digitalValueB0 << bitMaskFirstByte);
  dataBuffer[0] |= (digitalValueB1 << bitMaskSecondByte);
  dataBuffer[2] |= (digitalValueB2 << bitMaskFirstByte);
  dataBuffer[2] |= (digitalValueB3 << bitMaskSecondByte);
}

bool newInputDataToSend()
{
  // Check if there is a New Input value from the Buttons
  if(digitalValueB0 != lastDigitalValueB0) return true;
  if(digitalValueB1 != lastDigitalValueB1) return true;
  if(digitalValueB2 != lastDigitalValueB2) return true;
  if(digitalValueB3 != lastDigitalValueB3) return true;
  
  // Check if there is a New Input value from the Joystick Module
  if((analogValueX <= (lastAnalogValueX - analogThreshold)) || (analogValueX >= (lastAnalogValueX + analogThreshold))) return true;
  if((analogValueY <= (lastAnalogValueY - analogThreshold)) || (analogValueY >= (lastAnalogValueY + analogThreshold))) return true;

  // There has been no New Input Values. No duplicate Command Data Array will be sent.
  return false;
}

void saveLastReadData()
{ 
  // We should store the Last Read Digital Values so we can compare it with the Current Read Digital Values
  lastDigitalValueB0 = digitalValueB0;
  lastDigitalValueB1 = digitalValueB1;
  lastDigitalValueB2 = digitalValueB2;
  lastDigitalValueB3 = digitalValueB3;
  // We should store the Last Read Analog Values so we can compare it with the Current Read Analog Values
  lastAnalogValueX = analogValueX;
  lastAnalogValueY = analogValueY;
}
