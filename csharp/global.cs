using Godot;
using System;
using System.Collections.Generic;

namespace GlobalSpace
{
    public static class Global
    {
        public static int GlobalVariable = 42;


        public static Dictionary<string, object> GlobalSingleClass = new Dictionary<string, object>(); // 使用字典存储对象


        public static T GetSingleClass<T>(string className) where T : class
        {
            if (GlobalSingleClass.TryGetValue(className, out var instance) && instance is T)
            {
                return instance as T;
            }
            return null;
        }

    }
}

//private static object lockObject = new object();
//private static int GlobalVariable = 42;

//public void UpdateGlobalVariable()
//{
//    lock (lockObject)
//    {
//        // 在锁内部修改 GlobalVariable
//        GlobalVariable = 123;
//    }
//}