//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2020年12月18日  |   单例ID
//

namespace BigHead.Framework.Core
{
    public class SingleId
    {
        public static SingleId New(int startId = 0)
        {
            return new SingleId(startId);
        }

        private int _id;

        private SingleId(int startId)
        {
            _id = startId - 1;
        }

        public int Get()
        {
            ++_id;
            return _id;
        }
    }
}