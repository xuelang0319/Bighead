//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年1月8日    |   套接字发送参数
//

using System.Net.Sockets;
using BigHead.Framework.Net.NetSocket.Enums;

namespace BigHead.Framework.Net.NetSocket.Args
{
    public class SendArgs
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="handlerType">对接处理器类型</param>
        /// <param name="message">数据</param>
        /// <param name="callback">是否存在返回信息</param>
        public SendArgs(SocketHandlerTypes handlerType, string message)
        {
            HandlerType = handlerType;
            Message = message;
        }

        public Socket Socket;
        public SocketHandlerTypes HandlerType;
        public string Message;
        public SocketFlags SocketFlags = SocketFlags.None;
    }
}