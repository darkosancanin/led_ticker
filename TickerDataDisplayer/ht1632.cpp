#include "ht1632.h"
#include "ht1632_font.h"
#include "WProgram.h"
#include <avr/pgmspace.h>

HT1632::HT1632(uint8_t data_pin, uint8_t write_clock_pin, uint8_t chip_select_pin) {
  this->data_pin = data_pin;
  this->write_clock_pin = write_clock_pin;
  this->chip_select_pin = chip_select_pin;
}

void HT1632::initialize() {
  start_char_column_index = -31;
  pinMode(chip_select_pin, OUTPUT);
  pinMode(write_clock_pin, OUTPUT);
  pinMode(data_pin, OUTPUT);
  digitalWrite(chip_select_pin, HIGH);
  send_command(HT1632_CMD_SYSDIS); 
  send_command(HT1632_CMD_COMS00);
  send_command(HT1632_CMD_MSTMD);
  send_command(HT1632_CMD_SYSON);
  send_command(HT1632_CMD_LEDON);
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
  write_bits(0, 1<<6);
  
  int16_t total_char_columns = total_chars * 8;
  int16_t total_columns = 32;
  int16_t current_column_index = 0;
  int16_t current_char_index = 0;
  if(start_char_column_index > total_char_columns) {
    start_char_column_index = -31;
  }
  int16_t char_column_to_display;
  while(current_column_index < total_columns){
    char_column_to_display = start_char_column_index + current_column_index;
    if(char_column_to_display >= 0 && char_column_to_display < total_char_columns){
      current_char_index = char_column_to_display / 8; 
      set_buffer(text[current_char_index]); 
      uint16_t start_column_index = char_column_to_display % 8;
      while(start_column_index <= 7 && current_column_index < total_columns){
      	uint8_t bits = 0;
        uint8_t count;
        for(count = 0; count < 8; count++){
          if(current_char_buffer[count][start_column_index] == '0') bits |= (1<<(7-count));
        }
        write_bits(bits,1<<7);
      	start_column_index++;
      	current_column_index++;
      }
    }
    else{
      write_bits(0,1<<7);
      current_column_index ++;
    }
  }
  
  start_char_column_index++;
  digitalWrite(chip_select_pin, HIGH);
}

void HT1632::null_buffer(){
  for(int i=0;i<8;i++)
    for(int j=0; j<8;j++)
      current_char_buffer[i][j] = 0;
}

void HT1632::set_buffer(char chr){
  for(int i=0; i<sizeof(chl); i++){
    if(chl[i] == chr){
      int pos = i*8;
      for(int j=0;j<8;j++){
        strcpy_P(current_char_buffer[j], (PGM_P)pgm_read_word(&(CHL[j+pos])));
      }
    }
  }
}

void HT1632::set_text(char text[]){
  this->start_char_column_index = -31;
  this->total_chars = 0;
  while(text[total_chars]){
    this->text[total_chars] = text[this->total_chars];
    this->total_chars++;
  }
}

void HT1632::replace_char(char letter, uint16_t pos, uint16_t total_chars){
  this->start_char_column_index = -31;
  this->total_chars = total_chars;
  this->text[pos] = letter;
}


















