//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年1月8日    |   套接字处理器基类
//

using BigHead.Net.NetSocket.Enums;

namespace BigHead.Net.NetSocket.Handlers
{
    public abstract class SocketHandlerBase
    {
        public abstract SocketHandlerTypes SocketHandlerTypes { get; }

        public abstract void Invoke(string message);
    }
}