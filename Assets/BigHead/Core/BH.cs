//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2022年2月7日    |   Bighead系统总入口
//

using BigHead.Framework.Game;

public static class BH
{
    static BH()
    {
        SystemListener = new GameListener();
    }

    public static readonly GameListener SystemListener;
}