//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年1月8日    |   套接字管理器类型
//

namespace BigHead.Net.NetSocket.Enums
{
    /// <summary>
    /// 处理器存储键，由于需要转换为Byte类型值需在0-255之间
    /// </summary>
    public enum SocketHandlerTypes
    {
        Heartbeat = 0,
        Test = 1
    }
}