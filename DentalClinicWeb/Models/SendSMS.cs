using System.Configuration;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using Umbraco.Core.Events;

namespace DentalClinicWeb.Models
{
    public class SendSMS
    {
       /* public static SerialPort sPort = new SerialPort( 
            ConfigurationSettings.AppSettings["COM3"],
            int.Parse(ConfigurationSettings.AppSettings["9600"]),
            Parity.None,
            int.Parse(ConfigurationSettings.AppSettings["8"]),
            StopBits.One);*/

        public static bool sendSMS(string phoneNumber, string message)
        {
            bool result = true;

            String modemResponse = "";
            var sPort = new SerialPort("COM3", 115200);
            // deschide portul serial
            if (!sPort.IsOpen)
            {
                sPort.NewLine = Environment.NewLine;
                sPort.Handshake = Handshake.None;
                sPort.RtsEnable = true;
                sPort.Encoding = Encoding.ASCII;
                //sPort.WriteBufferSize = 8192;
                sPort.Open();
            }


            // verificare daca modemul gsm este conectat la retea
            sPort.DiscardInBuffer();
                sPort.DiscardOutBuffer();

                Thread.Sleep(5000);

                modemResponse = sPort.ReadExisting();

                sPort.WriteLine("AT+CREG?");
                System.Threading.Thread.Sleep(2000);
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
                    // modemul nu este conectat la retea
                    if ((nStat != 1) && (nStat != 5))
                    {
                        /*if (ConfigurationSettings.AppSettings["modemPIN"].Length > 0)
                        {
                            // trimite comanda de introducere a pin-ului
                            sPort.DiscardInBuffer();
                            sPort.DiscardOutBuffer();
                            sPort.WriteLine("AT+CPIN=" + ConfigurationSettings.AppSettings["modemPIN"]);
                            System.Threading.Thread.Sleep(5000);
                            modemResponse = sPort.ReadExisting();
                            // raspunsul trebuie sa fie de forma: AT+CPIN=1234\r\r\nOK\r\n
                            // verificam corectitudinea pin-ului
                            if (modemResponse.IndexOf("OK") == -1)
                            {
                                throw new Exception("PIN incorect");
                            }
                        }
                        */
                        System.Threading.Thread.Sleep(3000); // be sure is registeres
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
