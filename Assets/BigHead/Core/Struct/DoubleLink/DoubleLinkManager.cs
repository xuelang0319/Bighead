//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2022年2月7日  |
//

using System;

public class DoubleLinkManager
{
    public int Count { get; private set; }

    public IDoubleLinkNode FirstNode { get; private set; }

    public void Add(IDoubleLinkNode linkNode)
    {
        ++Count;
        if (FirstNode == null)
        {
            FirstNode = linkNode;
            return;
        }

        var node = FirstNode.GetLastNode();
        node.LinkNext(linkNode);
    }

    public void Remove(IDoubleLinkNode linkNode)
    {
        --Count;
        linkNode.Remove();
    }

    public void Do(Action<IDoubleLinkNode> callback)
    {
        if (Count == 0) return;
        var node = FirstNode;
        while (true)
        {
            callback?.Invoke(node);
            if (node.Next == null) return;
            node = node.Next;
        }
    }
}