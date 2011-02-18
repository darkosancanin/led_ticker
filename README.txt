Led Ticker is a scrolling led display project which is controlled via remote control. Data is pulled from the web and displayed on a led matrix.

TickerData is the web component written in .NET (C#), this is a simple web page which returns the text to display. Depending on the option querystring parameter passed in, it will download the appropriate data, parse it and return the parsed text. The menu is displayed by requesting '/Index.aspx?option=0'. Current data includes NBA scores, NBA Headlines, Weather, Financial Market updates. 
e.g. '/Index.aspx?option=1' returns the NBA scores such as 'LA 100 NY 105 Final   DET POR 10:30ET'

TickerDataReceiver is a Netduino project which decodes the remote control requests, then downloads the appropriate ticker data from the web and sends it to the serial port for displaying on the led matrix. The remote control uses the NEC protocol, so this project includes a NEC decoder class.

TickerDataDisplayer
This part is not yet implemented, this will read from the serial port and display the text on a led matrix.