## Overview 
Led Ticker is a Netduino/Arduino based project which displays ticker style data (nba scores, weather, nba news headlines, finance data etc) on a scrolling LED matrix (or any display that uses the HT1632 LED driver). The ticker is controlled by a remote control which allows you to select what is being displayed.

The components of the project are:
TickerData - This is a ASP.NET application that returns the text to display when queried. Depending on the option querystring parameter passed in, it will download the appropriate data, parse the source data and return the parsed text. The menu is displayed by requesting '/Index.aspx?option=0'. Current data includes NBA scores, NBA Headlines, Weather, Financial Market updates. 
e.g. '/Index.aspx?option=1' returns the NBA scores such as 'LA 100 NY 105 Final   DET POR 10:30ET'

TickerDataReceiver - This is a Netduino project which decodes the remote control requests, then downloads the appropriate ticker data from the web and sends it to the serial port for displaying on the LED matrix. The remote control uses the NEC protocol implemented in the NecRemoteControlDecoder class.

TickerDataDisplayer - This is a Arduino project which listens on the serial port for any data and then displays any data received on the HT1632 driver compatible LED matrix display.