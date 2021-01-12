using System;
using System.Net.Sockets;
using BigHead.Framework.Extension;
using BigHead.Framework.Net.NetSocket;
using BigHead.Framework.Net.NetSocket.Args;
using BigHead.Framework.Net.NetSocket.Enums;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TcpSocket.Instance.Start(response =>
        {
            $"{response}".Print();
        });
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            $"正在发送测试消息".Print();
            TcpSocket.Instance.SendMessage(new SendArgs(SocketHandlerTypes.Test, "这是一个测试消息"));
        }
    }
}
