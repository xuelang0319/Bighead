//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2020年12月17日  |   普通单例模式。 -- 鉴于书写方便以线程锁作为单例处理，虽然增加了运行成本但非大量调用不会过于影响性能。如果需要大量调用，请自行进行其它方式处理。
//

namespace BigHead.Framework.Core
{
    public abstract class Singleton<T> where T : class, new()
    {
        private static readonly object Locker = new object();
        
        private static T _instance;

        public static T Instance
        {
            get
            {
                lock (Locker) return _instance ?? (_instance = new T());
            }
        }
    }
}