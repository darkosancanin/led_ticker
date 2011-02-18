using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace Netduino.TickerDataReceiver
{
    public class NecRemoteControlDecoder
    {
        private Cpu.Pin _irReceiverPin;
        private long[] pulses;
        private int currentPulseIndex;
        private Timer irPulseTimeOutTimer;
        public delegate void IrCommandReceivedEventHandler(UInt32 irData);
        public event IrCommandReceivedEventHandler OnIrCommandReceived;
        private InterruptPort irReceiverPort;

        public NecRemoteControlDecoder(Cpu.Pin irReceiverPin)
        {
            _irReceiverPin = irReceiverPin;
            pulses = new long[200];
            currentPulseIndex = 0;
            irReceiverPort = new InterruptPort(irReceiverPin, false, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeBoth);
            irReceiverPort.OnInterrupt += new NativeEventHandler(irReceiverPort_OnInterrupt);
            irPulseTimeOutTimer = new Timer(new TimerCallback(IrPulseTimeOut), null, Timeout.Infinite, Timeout.Infinite);
        }

        private void irReceiverPort_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            if (currentPulseIndex >= 200) currentPulseIndex = 0;
            pulses[currentPulseIndex] = time.Ticks;
            currentPulseIndex++;
            irPulseTimeOutTimer.Change(10, Timeout.Infinite);
        }

        private void ConvertPulsesToMicroseconds(long numberOfPulses)
        {
            var firstMicrosecondsValue = pulses[0] / (TimeSpan.TicksPerMillisecond / 1000);
            var lastMicosecondsValue = firstMicrosecondsValue;
            for (int i = 1; i < numberOfPulses; i++)
            {
                var currentPulseMicrosecondsValue = pulses[i] / (TimeSpan.TicksPerMillisecond / 1000);
                var currentPulseLength = currentPulseMicrosecondsValue - lastMicosecondsValue;
                lastMicosecondsValue = currentPulseMicrosecondsValue;
                pulses[i - 1] = currentPulseLength;
            }
        }

        private void IrPulseTimeOut(object state)
        {
            var numberOfPulses = currentPulseIndex;
            currentPulseIndex = 0;
            irPulseTimeOutTimer.Change(Timeout.Infinite, Timeout.Infinite);

            ConvertPulsesToMicroseconds(numberOfPulses);
            if (numberOfPulses != 68) return;
            if (!IsPulseMatch(pulses[0], 9000) || !IsPulseMatch(pulses[1], 4500)) return;

            UInt32 data = 0;
            for(int i = 65; i >= 3; i--){
                if((i % 2) == 1){
                    data = data << 1;
                    if (IsPulseMatch(pulses[i], 565))
                        data = data | 1;
                }
                else{
                    if(!IsPulseMatch(pulses[i], 560))
                        return;
                    }
            }

            if (OnIrCommandReceived != null)
                OnIrCommandReceived(data);
        }

        private bool IsPulseMatch(long actualValue, long expectedValue)
        {
            var marginOfError = 200;
            if ((expectedValue - marginOfError) < actualValue && (expectedValue + marginOfError) > actualValue)
                return true;
            return false;
        }
    }
}