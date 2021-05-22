//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年1月8日    |   测试
//

using BigHead.Framework.Core;
using BigHead.Framework.Net.NetSocket.Enums;

namespace BigHead.Framework.Net.NetSocket.Handlers
{
    public class HandlerTest : SocketHandlerBase
    {
        public override SocketHandlerTypes SocketHandlerTypes => SocketHandlerTypes.Test;
        public override void Invoke(string message)
        {
            $"收到服务器消息： {message}".Highlight();
        }
    }
}