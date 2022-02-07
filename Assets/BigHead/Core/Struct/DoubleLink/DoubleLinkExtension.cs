//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2022年2月7日    |   双向链表拓展方法
//

public static class DoubleLinkExtension
{
    public static IDoubleLinkNode GetLastNode(this IDoubleLinkNode param)
    {
        var node = param;
        while (true)
        {
            if (node.Next == null)
                return node;

            node = node.Next;
        }
    }

    public static void LinkNext(this IDoubleLinkNode front, IDoubleLinkNode next)
    {
        front.Next = next;
        next.Front = front;
    }

    public static void Remove(this IDoubleLinkNode node)
    {
        node.Front.LinkNext(node.Next);
    }
}