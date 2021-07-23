//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2020年12月17日  |   列表扩展方法
//

using System.Collections.Generic;
using BigHead.Framework.Core;

namespace BigHead.Framework.Extension
{
    public static class ListExtension
    {
        public static void Merge<T>(this List<T> list, List<T> beMergeList)
        {
            if (Equals(null, beMergeList) || beMergeList.Count == 0) return;
            if (Equals(null, list)) list = new List<T>();
            list.AddRange(beMergeList);
        }

        public static Queue<T> ToQueue<T>(this List<T> list, bool emptyReturnNull = false)
        {
            var queue = new Queue<T>();
            if (Equals(null, list)) return emptyReturnNull? null : queue;
            foreach (var t in list) queue.Enqueue(t);
            return queue;
        }

        public static bool AddUniqueValue<T>(this List<T> list, T value)
        {
            if (Equals(null, list) || list.Contains(value)) return false;
            list.Add(value); 
            return true;
        }
        
        public static bool AddUniqueValue<T>(this HashSet<T> list, T value)
        {
            if (Equals(null, list) || list.Contains(value)) return false;
            list.Add(value); 
            return true;
        }

        public static void AddValueAndLogError<T>(this List<T> list, T value)
        {
            if(!list.AddUniqueValue(value)) "向列表添加元素不成功，可能出现列表为空或元素重复的情况。".Error();
        }
    }
}