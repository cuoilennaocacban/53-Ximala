int i0 = 23;
int l0 = 46;
int i1 = 25;
int l1 = 44;
int i2 = 27;
int l2 = 45;
int i3 = 29;
int l3 = 6;
int i4 = 22;
int l4 = 7;
int i5 = 24;
int l5 = 8;
 
void setup()
{
  pinMode(l0,OUTPUT);
  pinMode(i0, INPUT_PULLUP);
  pinMode(l1,OUTPUT);
  pinMode(i1, INPUT_PULLUP);
  pinMode(l2,OUTPUT);
  pinMode(i2, INPUT_PULLUP);
  pinMode(l3,OUTPUT);
  pinMode(i3, INPUT_PULLUP);
  pinMode(l4,OUTPUT);
  pinMode(i4, INPUT_PULLUP);
  pinMode(l5,OUTPUT);
  pinMode(i5, INPUT_PULLUP);
}
void loop() 
{
  if(digitalRead(i0) == 0)
  {
    digitalWrite(l0,HIGH);
    digitalWrite(l1,HIGH); 
    delay(100);   
  }
  else
  {   
    analogWrite(l0,3);   
  }
  if(digitalRead(i1) == 0)
  {
    digitalWrite(l2,HIGH);
    digitalWrite(l1,HIGH);
    delay(100);    
  }
  else
  {   
    analogWrite(l1,10); 
  }
  if(digitalRead(i2) == 0)
  {
    digitalWrite(l2,HIGH);
    digitalWrite(l3,HIGH);
    delay(100);   
  }
  else
  {   
    analogWrite(l2,10); 
  }
   if(digitalRead(i3) == 0)
  {
    digitalWrite(l3,HIGH);
    digitalWrite(l4,HIGH);
    digitalWrite(l2,HIGH);
  delay(200);    
  }
  else
  {   
    analogWrite(l3,10);
}
  if(digitalRead(i4) == 0)
  {
    digitalWrite(l4,HIGH);
    digitalWrite(l5,HIGH);
    digitalWrite(l3,HIGH);
  delay(100);    
  }
  else
  {   
    analogWrite(l4,10);
}
  if(digitalRead(i5) == 0)
  {
    digitalWrite(l5,HIGH);
    //digitalWrite(l6,HIGH);
  delay(100);    
  }
  else
  {   
    analogWrite(l5,20);
}
}
