//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2022年2月7日    |  双向链表节点
//

public interface IDoubleLinkNode
{
    IDoubleLinkNode Front { get; set; }
    IDoubleLinkNode Next { get; set; }
}