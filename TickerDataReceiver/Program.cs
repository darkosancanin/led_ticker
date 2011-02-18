using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using Socket = System.Net.Sockets.Socket;
using Microsoft.SPOT.Net.NetworkInformation;
using System.IO.Ports;

namespace Netduino.TickerDataReceiver
{
    public class Program
    {
        private static SerialPort _serialPort;

        public static void Main()
        {
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            Debug.Assert(networkInterfaces[0] != null);
            var net = networkInterfaces[0];

            Debug.Print("DHCP Enabled: " + net.IsDhcpEnabled);
            Debug.Print("IP Address: " + net.IPAddress);

            var necRemoteControlDecoder = new NecRemoteControlDecoder(Pins.GPIO_PIN_D7);
            necRemoteControlDecoder.OnIrCommandReceived += necRemoteControlDecoder_OnIrCommandReceived;

            _serialPort = new SerialPort(SerialPorts.COM1, 9600, Parity.None, 8, StopBits.One);
            _serialPort.Open();

            Thread.Sleep(Timeout.Infinite);
        }

        static void necRemoteControlDecoder_OnIrCommandReceived(UInt32 irData)
        {
            Debug.Print("Ir Command Received: " + irData);
            var url = RemoteControlCodes.GetUrlFromCode(irData);
            var tickerData = GetDataFromWeb(url);
            Debug.Print(tickerData);
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            var bytesToSend = encoder.GetBytes(tickerData);
            _serialPort.Write(bytesToSend, 0, bytesToSend.Length);
        }

        private static string GetDataFromWeb(String url)
        {
            var httpRequest = HttpWebRequest.Create(url);
            WebResponse webResponse = null;
            try
            {
                webResponse = httpRequest.GetResponse();
            }
            catch (Exception e)
            {
                Debug.Print("Exception in HttpWebRequest.GetResponse(): " + e.ToString());
            }
            
            var responseStream = webResponse.GetResponseStream();
            var byteData = new byte[500];
            var charData = new char[500];
            var bytesRead = responseStream.Read(byteData, 0, byteData.Length);
            int byteUsed, charUsed;
            bool completed = false;
            System.Text.Encoding.UTF8.GetDecoder().Convert(byteData, 0, bytesRead, charData, 0, bytesRead, true, out byteUsed, out charUsed, out completed);
            return new String(charData, 0, charUsed);
        }

        public static class RemoteControlCodes
        {
            public static long Zero = 217268479;
            public static long One = 284115199;
            public static long Two = 300826879;
            public static long Three = 317538559;
            public static long Four = 350961919;
            public static long Five = 367673599;
            public static long Six = 384385279;
            public static long Seven = 417808639;
            public static long Eight = 434520319;
            public static long Nine = 451231999;
            public static long Power = 16728319;
            public static long FunctionStop = 50151679;
            public static long Up = 183845119;

            public static string GetUrlFromCode(UInt32 irData)
            {
                var baseUrl = "http://darkosancanin.com/tickerdata/?option=";

                if (irData == Zero)
                    return baseUrl + 0;
                if (irData == One)
                    return baseUrl + 1;
                if (irData == Two)
                    return baseUrl + 2;
                if (irData == Three)
                    return baseUrl + 3;
                if (irData == Four)
                    return baseUrl + 4;
                if (irData == Five)
                    return baseUrl + 5;
                if (irData == Six)
                    return baseUrl + 6;
                
                return baseUrl + 0;
            }
        }
    }
}