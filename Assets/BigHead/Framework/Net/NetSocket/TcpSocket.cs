//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年1月8日  |
//

using System.Net.Sockets;

namespace BigHead.Framework.Net.NetSocket
{
    public class TcpSocket : SocketBase<TcpSocket>
    {
        protected override string Ip => "127.0.0.1";
        protected override int Port => 12121;
        protected override ProtocolType ProtocolType => ProtocolType.Tcp;
        protected override int Heartbeat => 3000;
        protected override bool Crypto => false;
        
        protected override void OnLostConnection()
        {
            
        }

        protected override void Reconnect()
        {
            
        }

        protected override string Encrypt(string data)
        {
            return data;
        }

        protected override string Decrypt(string data)
        {
            return data;
        }
    }
}