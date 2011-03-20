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
  ht1632 = new HT1632(7,5,3);
  ht1632->initialize();
  ht1632->set_text("LOADING...");
  Serial.begin(9600);
}

void loop ()
{    
  int char_index = 0;
  while (Serial.available() && char_index < 512){
    ht1632->replace_char(toupper(Serial.read()), char_index, char_index + 1);
    char_index ++;
    delay(10);
  }   
  ht1632->scroll_text();
  delay(120);
}
