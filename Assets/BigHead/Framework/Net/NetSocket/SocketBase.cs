//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年1月6日    |   套接字基类，继承后可自动初始化及使用。
//  Eric    |  2021年1月9日    |   分离Encode和Decode方法提供客户端继承改写、心跳包的接收时间上限
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using BigHead.Framework.Core;
using BigHead.Framework.Net.NetSocket.Args;
using BigHead.Framework.Net.NetSocket.Enums;
using BigHead.Framework.Net.NetSocket.Handlers;
using BigHead.Framework.Utility.Helper;
using UnityEngine;
using Timer = System.Timers.Timer;

namespace BigHead.Framework.Net.NetSocket
{
    /*
     * 目前Socket连接只完成了简单部分，功能尚未齐全。
     * 考虑到其它核心功能尚未完成，所以暂时搁置。
     *  TODO: 完善 SocketBase
     */
    
    /// <summary>
    /// 单例部分
    /// </summary>
    public abstract partial class SocketBase<T> where T : SocketBase<T>,  new()
    {
        protected static readonly object Locker = new object();
        
        protected static T _instance;

        public static T Instance
        {
            get
            {
                lock (Locker) return _instance ?? (_instance = new T());
            }
        }
    }

    /// <summary>
    /// 处理器部分
    /// </summary>
    public abstract partial class SocketBase<T>
    {
        protected static readonly Dictionary<int, SocketHandlerBase> Handlers = new Dictionary<int, SocketHandlerBase>();

        static SocketBase()
        {
            foreach (var socketHandlerBase in ClassHelper.CreateAllDerivedClass<SocketHandlerBase>())
            {
                Handlers.Add((int) socketHandlerBase.SocketHandlerTypes, socketHandlerBase);
            }
        }
    }

    /// <summary>
    /// 套接字部分
    /// </summary>
    public abstract partial class SocketBase<T>
    {
        /// <summary>
        /// 连接地址
        /// </summary>
        protected abstract string Ip { get; }

        /// <summary>
        /// 连接端口
        /// </summary>
        protected abstract int Port { get; }

        /// <summary>
        /// 套接字
        /// </summary>
        protected Socket Socket;

        /// <summary>
        /// 网络终结点
        /// </summary>
        protected IPEndPoint IPEndPoint;

        /// <summary>
        /// 连接类型
        /// </summary>
        protected abstract ProtocolType ProtocolType { get; }

        /// <summary>
        /// 信息传输大小
        /// </summary>
        protected const int BufferSize = 1024;

        /// <summary>
        /// 心跳计时器
        /// </summary>
        protected Timer HearbeatTimer;

        /// <summary>
        /// 心跳包频率
        /// </summary>
        protected abstract int Heartbeat { get; }

        /// <summary>
        /// 接收列表
        /// </summary>
        protected List<byte> Receives = new List<byte>();

        /// <summary>
        /// 待传输队列
        /// </summary>
        protected Queue<SendArgs> Messages = new Queue<SendArgs>();

        /// <summary>
        /// 接收线程
        /// </summary>
        protected Task ReceiveTask;

        /// <summary>
        /// 线程取消
        /// </summary>
        protected CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// 套接字状态
        /// </summary>
        protected bool State = false;

        /// <summary>
        /// 接收状态
        /// </summary>
        protected bool Receiving = false;

        /// <summary>
        /// 启动时间
        /// </summary>
        protected static DateTime LastPing;

        /// <summary>
        /// 加密状态
        /// </summary>
        protected abstract bool Crypto { get; }

        /// <summary>
        /// Socket启动函数。
        /// 启动方法：
        /// T 继承 SocketBase
        /// T.Instance.Start(Callback);
        /// </summary>
        /// <param name="callback">返回连接结果</param>
        public virtual void Start(Action<ConnectResponse> callback)
        {
            // 无连接状态
            if (Equals(Application.internetReachability, NetworkReachability.NotReachable))
            {
                callback?.Invoke(ConnectResponse.PermissionFailed);
                return;
            }

            try
            {
                Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType);
                IPEndPoint = new IPEndPoint(IPAddress.Parse(Ip), Port);
                var result = Socket.BeginConnect(IPEndPoint, ar =>
                {
                    var socket = ar.AsyncState as Socket;
                    State = true;
                    socket.EndConnect(ar);
                    callback?.Invoke(ConnectResponse.Success);
                    while (Messages.Count > 0)
                    {
                        SendMessage(Messages.Dequeue());
                    }
                    ReceiveTask = Task.Run(ReceiveMessage, CancellationTokenSource.Token);
                    LastPing = DateTime.UtcNow;
                    InvokeTimer();

                }, Socket);

                Task.Run(() =>
                {
                    result.AsyncWaitHandle.WaitOne(2000, true);
                    if (!result.IsCompleted)
                    {
                        callback?.Invoke(ConnectResponse.ConnectFailed);
                    }
                }, CancellationTokenSource.Token);

                BigHeadManager.Instance.DestroyEvent += Close;
            }
            catch (Exception e)
            {
                Close();
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Socket关闭方法
        /// </summary>
        protected virtual void Close()
        {
            if(!State) return;
            
            "Close".Warning();
            State = false;
            StopTimer();
            Socket.Shutdown(SocketShutdown.Both);
            CancellationTokenSource.Cancel();
            Socket.Close();
        }

        /// <summary>
        /// 启动心跳
        /// </summary>
        protected void InvokeTimer()
        {
            if (!State) return;

            HearbeatTimer = new Timer();
            HearbeatTimer.Interval = Heartbeat;
            HearbeatTimer.Elapsed += InvokeHeartbeatFoo;
            HearbeatTimer.Enabled = true;
            HearbeatTimer.Start();
        }

        /// <summary>
        /// 停止心跳
        /// </summary>
        protected void StopTimer()
        {
            if (!Equals(null, HearbeatTimer))
            {
                HearbeatTimer.Elapsed -= InvokeHeartbeatFoo;
                HearbeatTimer.Close();
            }
        }
        
        /// <summary>
        /// 心跳方法
        /// </summary>
        protected void InvokeHeartbeatFoo(object source, ElapsedEventArgs e)
        {
            var time = (int) ((DateTime.UtcNow - LastPing).TotalSeconds * 1000);
            if (time > Heartbeat * 4)
            {
                "网络连接中断".Error();
                Close();
                OnLostConnection();
            }
            else
            {
                "正在发送心跳包。".Highlight();
                SendMessage(new SendArgs(SocketHandlerTypes.Heartbeat, ""));
            }
        }


        /// <summary>
        /// 加密
        /// </summary>
        protected abstract string Encrypt(string data);

        /// <summary>
        /// 解密
        /// </summary>
        protected abstract string Decrypt(string data);

        /// <summary>
        /// 掉线处理
        /// </summary>
        protected abstract void OnLostConnection();

        /// <summary>
        /// 重连处理
        /// </summary>
        protected abstract void Reconnect();

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="args">参数中包含处理器和需要发送的消息</param>
        public void SendMessage(SendArgs args)
        {
            try
            {
                if (!State || Equals(null, Socket))
                {
                    Messages.Enqueue(args);
                    return;
                }

                args.Socket = Socket;
                var data = Encode((int) args.HandlerType, args.Message);
                Socket.BeginSend(data, 0, data.Length, args.SocketFlags, ar =>
                {
                    var sendArgs = ar.AsyncState as SendArgs;
                    sendArgs.Socket.EndSend(ar);
                }, args);
            }
            catch (Exception e)
            {
                Close();
                OnLostConnection();
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// 收取消息，默认新线程启动，无需进行额外处理。
        /// </summary>
        protected void ReceiveMessage()
        {
            while (State)
            {
                var buffer = new byte[BufferSize];
                var length = Socket.Receive(buffer, BufferSize, SocketFlags.None);
                lock (Receives)
                {
                    foreach (var b in buffer.Take(length))
                        Receives.Add(b);

                    while (Decode(Receives, out var handlerType, out var data))
                    {
                        try
                        {
                            LastPing = DateTime.UtcNow;
                            Handlers[handlerType].Invoke(data);
                        }
                        catch
                        {
                            $"没有找到对应的Socket处理器， HandlerType: {(SocketHandlerTypes) handlerType}".Exception();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 转码方法，继承后可根据业务需求重写。
        /// 注意： Encode和Decode为对应函数转码解码，如需重写则必须同时改写，否则会出现问题。
        /// 注意： Crypto属性如果在继承类中设置为True,必须填写Encrypt函数的加密方法。
        /// 注意： 如果存在业务需求或需要特殊加密方式，可重写Encode函数并在其中重新定义加密方法，如此可跳过基类中的加密逻辑。
        /// </summary>
        /// <param name="handlerType">处理器编号 - 即：（int）SocketHandlerTypes</param>
        /// <param name="message">需要传输的信息</param>
        /// <returns>转码后的输出</returns>
        protected byte[] Encode(int handlerType, string message)
        {
            if (Crypto) message = Encrypt(message);
                
            var head = GetByte(handlerType);
            var body = GetBytes(message);
            var length = GetBytes(body.Length + 1);
            // total length + head length + Body length
            var data = new byte[4 + 1 + body.Length];
            length.CopyTo(data, 0);
            data[4] = head;
            body.CopyTo(data, 5);

            return data;
        }

        /// <summary>
        /// 解码方法，继承后可根据业务需求重写。
        /// 注意： Encode和Decode为对应函数转码解码，如需重写则必须同时改写，否则会出现问题。
        /// 注意： Crypto属性如果在继承类中设置为True,必须填写Decrypt函数的解密方法。
        /// 注意： 如果存在业务需求或需要特殊加密方式，可重写Decode函数并在其中重新定义解密方法，如此可跳过基类中的解密逻辑。
        /// 注意： 为了防止粘包和分包的情况，该方法在重写时需要Lock列表以防止多线程同时写入或删除。
        /// </summary>
        /// <param name="receives">
        /// 当前接受到的所有byte数据，根据业务需求提取使用。
        /// 注意： 当重写Decode函数时，该列表的自动数据维护将停止运行。使用者需对该列表进行维护，比如提取数据后，无效数据需要进行删除等操作。
        /// </param>
        /// <param name="handlerType">处理器编号 - 即：（int）SocketHandlerTypes</param>
        /// <param name="data">转码后获取到的信息</param>
        /// <returns>
        /// 当前表单是否满足提取条件。
        /// 注意： 只有当返回值为True时才会对返回的handlerType和data传入指定的处理器。否则直接忽略本次解码。
        /// </returns>
        protected bool Decode(List<byte> receives, out int handlerType, out string data)
        {
            handlerType = -1;
            data = string.Empty;

            // 这里的5分别为： 长度 - 4， 处理器类型 - 1
            if (receives.Count > 5)
            {
                var total = GetInt(receives.Take(4).ToArray());
                if (receives.Count > 4 + total)
                {
                    handlerType = Convert.ToInt16(receives[4]);
                    data = GetString(receives.Skip(5).Take(total - 1).ToArray());
                    if (Crypto) data = Decrypt(data);
                    receives.RemoveRange(0, 4 + total);
                    return true;
                }
            }

            return false;
        }
    }

    /// <summary>
    /// 辅助方法部分
    /// </summary>
    public abstract partial class SocketBase<T>
    {
        /// <summary>
        /// 将0-255范围的整数转换为字节类型
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        protected static byte GetByte(int number)
        {
            if (number > 255 || number < 0)
            {
                $"整数转为单字节失败，传入参数需取值范围为0-255，当前参数为： {number}".Exception();
                return byte.MinValue;
            }

            return Convert.ToByte(number);
        }

        /// <summary>
        /// 将整数类型转换为字节数组，结果固定长度为4
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        protected static byte[] GetBytes(int number) =>
            BitConverter.GetBytes(number);

        /// <summary>
        /// 将字符串转换为UTF8格式的字节数组。
        /// </summary>
        protected static byte[] GetBytes(string message) => 
            Encoding.UTF8.GetBytes(message);

        /// <summary>
        /// 字节数组提取前4位转换为整数类型数据
        /// </summary>
        protected static int GetInt(byte[] data) =>
            BitConverter.ToInt32(data, 0);
        
        /// <summary>
        /// 将字符串转换为UTF8格式的字节数组，并在前面加上字节的长度。
        /// 主要用于TCP协议传输的分包、沾包的计算。
        /// 长度数据始终为：4 （sizeof(int) = 4)
        /// </summary>
        protected static byte[] GetBytesWithLengthAtFront(string message)
        {
            var data = GetBytes(message);
            return BitConverter.GetBytes(data.Length).Concat(data).ToArray();
        }

        /// <summary>
        /// 将UTF8字节数组转换为字符串
        /// </summary>
        protected static string GetString(byte[] data) =>
            Encoding.UTF8.GetString(data);

        /// <summary>
        /// 获取返回数据中第一条消息，并在原数据上进行裁剪。
        /// 由于TCP协议存在沾包现象，所以需要将包进行分离。
        /// </summary>
        protected static string GetHeadMessage(int headLength, ref byte[] data)
        {
            var array = data.Take(headLength).ToArray();
            var l = int.Parse(GetString(array));
            var bytes = data.Skip(headLength).Take(l).ToArray();
            var message = GetString(bytes);
            data = data.Skip(headLength + l).ToArray();
            return message;
        }
    }
}