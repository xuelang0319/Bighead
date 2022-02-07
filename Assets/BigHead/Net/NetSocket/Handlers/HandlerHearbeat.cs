//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年1月8日    |   
//

using BigHead.Net.NetSocket.Enums;

namespace BigHead.Net.NetSocket.Handlers
{
    public class HandlerHeartbeat : SocketHandlerBase
    {
        public override SocketHandlerTypes SocketHandlerTypes => SocketHandlerTypes.Heartbeat;
        public override void Invoke(string message)
        {
            "收到了服务器的心跳包".Log();
        }
    }
}