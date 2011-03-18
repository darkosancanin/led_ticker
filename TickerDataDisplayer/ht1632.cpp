#include "ht1632.h"
#include "ht1632_font.h"
#include "WProgram.h"

HT1632::HT1632(uint8_t data_pin, uint8_t write_clock_pin, uint8_t chip_select_pin) {
  this->data_pin = data_pin;
  this->write_clock_pin = write_clock_pin;
  this->chip_select_pin = chip_select_pin;
}

void HT1632::initialize() {
  pinMode(chip_select_pin, OUTPUT);
  pinMode(write_clock_pin, OUTPUT);
  pinMode(data_pin, OUTPUT);
  digitalWrite(chip_select_pin, HIGH);
  send_command(HT1632_CMD_SYSDIS); 
  send_command(HT1632_CMD_COMS00);
  send_command(HT1632_CMD_MSTMD);
  send_command(HT1632_CMD_SYSON);
  send_command(HT1632_CMD_LEDON);
  send_command(HT1632_CMD_BLOFF);
}

void HT1632::send_command(uint8_t command){
  digitalWrite(chip_select_pin, HIGH);
  digitalWrite(chip_select_pin, LOW);
  write_bits(HT1632_ID_CMD, 1<<2);
  write_bits(command, 1<<7); 
  write_bits(0, 1); 
  digitalWrite(chip_select_pin, HIGH);
}

void HT1632::write_bits(uint8_t bits, uint8_t firstBit){
  while (firstBit) {
    digitalWrite(write_clock_pin, LOW);
    if (bits & firstBit) {
	digitalWrite(data_pin, HIGH);
    }
    else {
	digitalWrite(data_pin, LOW);
    }
    digitalWrite(write_clock_pin, HIGH);
    firstBit >>= 1;
  }
}

void HT1632::scroll_text(){
  digitalWrite(chip_select_pin, HIGH);
  digitalWrite(chip_select_pin, LOW);
  write_bits(HT1632_ID_WR, 1<<2);
  uint8_t section;
  write_bits(0, 1<<6);
  for(section = 0; section > 64; section++){
    write_bits(0xF, 1<<3);
    delay(1000);
  }
  digitalWrite(chip_select_pin, HIGH);
}

