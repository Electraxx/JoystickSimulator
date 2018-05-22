using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace JoystickSimulator.Controllers
{
    public class MonitoringController
    {
        public IPEndPoint EP { get; set; }
        private Socket _socket;

        public MonitoringController(String StrIp, AddressFamily addFam)
        {
            IPAddress ip = null;

            if (!IPAddress.TryParse(StrIp, out IPAddress MonitoringIp) || StrIp.Split('.').Length != 4)
            {
                //Si ce n'est pas le cas, on default sur localhost
                EP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000);
            }
            else
            {
                EP = new IPEndPoint(IPAddress.Parse(StrIp), 11000);
            }

            _socket = new Socket(addFam,
                SocketType.Dgram,
                ProtocolType.Udp);
        }

        public void SendTo(UDPPacket packet)
        {
            //Le packet nous renvoie une version sérialisée
            String csv = packet.GetCsv();
            Console.WriteLine("sent : " + csv);
            //On la transforme en bytes en précisant l'encodage
            byte[] data = Encoding.UTF8.GetBytes(csv);

            //On envoie les données à la cible
            _socket.SendTo(data, EP);
        }

    }
}
}
