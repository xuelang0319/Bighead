//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年1月8日    |   套接字接收参数类
//

using System.Net.Sockets;

namespace BigHead.Framework.Net.NetSocket.Args
{
    public class ReceiveArgs
    {
        public Socket Socket = null;
        public const int BufferSize = 1024;
        public byte[] Buffer = new byte[BufferSize];
    }
}