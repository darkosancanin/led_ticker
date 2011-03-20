#include "ht1632.h"
#include <stdint.h> 
#include <ctype.h>

void * operator new(size_t size); 
void * operator new(size_t size) { 
  return malloc(size); 
} 

HT1632 *ht1632;

void setup ()
{  
  ht1632 = new HT1632((uint8_t)7,(uint8_t)5,(uint8_t)3);
  ht1632->initialize();
  ht1632->set_text("LOADING...");
  Serial.begin(9600);
}

void loop ()
{    
  int char_count = 0;
  while (Serial.available() && char_count < 512){
    ht1632->replace_char(toupper(Serial.read()), char_count, char_count);
    char_count ++;
    delay(10);
  }   
  ht1632->scroll_text();
  delay(120);
}
