#include "ht1632.h"
#include <stdint.h> 

void * operator new(size_t size); 
void * operator new(size_t size) { 
  return malloc(size); 
} 

HT1632 *ht1632;

void setup ()
{  
  ht1632 = new HT1632((uint8_t)7,(uint8_t)5,(uint8_t)3);
  ht1632->initialize();
}

void loop ()
{    
  ht1632->scroll_text();
  delay(1000);
}
