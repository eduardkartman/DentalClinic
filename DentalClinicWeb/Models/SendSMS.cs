
using System.IO.Ports;
using System.Text;

namespace DentalClinicWeb.Models
{
    public class SendSMS
    {
        // Initializarea portului in maniera globala
        public static SerialPort sPort = new SerialPort("COM3", 115200);

        // Functia de transmitere SMS
        public static bool sendSMS(string phoneNumber, string message)
        {
            bool result = true;
            String modemResponse = "";
            // deschide portul serial o singura data
            if (!sPort.IsOpen)
            {
                sPort.NewLine = Environment.NewLine;
                sPort.Handshake = Handshake.None;
                sPort.RtsEnable = true;
                sPort.Encoding = Encoding.ASCII;
                sPort.WriteBufferSize = 8192;
                sPort.Open();
            }

            sPort.DiscardInBuffer();
            sPort.DiscardOutBuffer();

            Thread.Sleep(5000);  // Se vor astepta 5 secunde

            // verificare daca modemul gsm este conectat la retea
            modemResponse = sPort.ReadExisting();
            sPort.WriteLine("AT+CREG?");
            System.Threading.Thread.Sleep(5000);
            modemResponse = sPort.ReadExisting();

            // raspunsul trebuie sa fie de forma: AT+CREG?\r\r\n+CREG: 0,0\r\n\r\nOK\r\n
            if (modemResponse.IndexOf("OK") >= 0)
            {
                int i1 = modemResponse.IndexOf("+CREG:");
                String networkStatus = modemResponse.Substring(i1 + 6);
                int i2 = networkStatus.IndexOf("\r", 0);
                networkStatus = networkStatus.Substring(0, i2).Trim();
                int nStat;
                bool parseOk = Int32.TryParse(networkStatus[networkStatus.Length - 1].ToString(), out nStat);
                
                if ((nStat != 1) && (nStat != 5))
                {
                    // modemul nu este conectat la retea
                    System.Threading.Thread.Sleep(3000);
                }

                // modemul este conectat la retea
                // comanda pentru trimiterea SMS-ului
                sPort.DiscardInBuffer();
                sPort.DiscardOutBuffer();
                sPort.WriteLine("AT+CMGF=1");
                System.Threading.Thread.Sleep(1000);
                modemResponse = sPort.ReadExisting();

                if (modemResponse.IndexOf("OK") >= 0)
                {
                    sPort.DiscardInBuffer();
                    sPort.DiscardOutBuffer();
                    sPort.WriteLine("AT+CMGS=\"" + phoneNumber + "\"");
                    System.Threading.Thread.Sleep(1000);
                    sPort.Write(message);
                    System.Threading.Thread.Sleep(100);
                    byte[] _b = new byte[1];
                    _b[0] = 0x1A; // Ctrl+Z - ascii code
                    sPort.Write(_b, 0, 1);
                    Thread.Sleep(4000);
                    modemResponse = sPort.ReadExisting();
                }
            }
           
            
            else
            {
                throw new Exception("OK not found in the response from the gsm modem.");
            }

            return result;
        }
    }
}






