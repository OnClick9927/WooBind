using System;
using System.Collections.Generic;

namespace WooBind
{
    /// <summary>
    /// 数据存储器   类型,名字,实例
    /// </summary>
    class TypeNameMap
    {
        private interface ITypeMap
        {
            object Get(string name, bool autoCreate = true);
            void Set(string name, object obj);
            bool Exist(string name);
        }
        private interface ITypeMap<T> : ITypeMap
        {
            T GetValue(string name, bool autoCreate = true);
            void SetValue(string name, T obj);
        }
        private class TypeMap<T> : ITypeMap<T>
        {
            private class Value
            {
                public string name;
                public T value { get; set; }
            }
            private List<Value> values;
            public TypeMap()
            {
                values = new List<Value>();
            }
            public T GetValue(string name, bool autoCreate = true)
            {
                for (int i = 0; i < values.Count; i++)
                {
                    if (values[i].name == name)
                    {
                        return values[i].value;
                    }
                }
                T t = default(T);
                if (autoCreate)
                {
                    values.Add(new Value()
                    {
                        value = t,
                        name = name
                    });
                }
                return t;
            }
            public void SetValue(string name, T obj)
            {
                for (int i = 0; i < values.Count; i++)
                {
                    if (values[i].name == name)
                    {
                        values[i].value = obj;
                        return;
                    }
                }
                values.Add(new Value()
                {
                    value = obj,
                    name = name
                });
            }

            public void Set(string name, object obj)
            {
                for (int i = 0; i < values.Count; i++)
                {
                    if (values[i].name == name)
                    {
                        values[i].value = (T)obj;
                        return;
                    }
                }
                values.Add(new Value()
                {
                    value = (T)obj,
                    name = name
                });
            }
            public object Get(string name, bool autoCreate = true)
            {
                return GetValue(name, autoCreate);
            }

            public bool Exist(string name)
            {
                for (int i = 0; i < values.Count; i++)
                {
                    if (values[i].name == name)
                    {
                        return true; ;
                    }
                }
                return false;
            }
        }
        private Dictionary<Type, ITypeMap> values = new Dictionary<Type, ITypeMap>();

        /// <summary>
        /// 是否存在对应名字类型的实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Exist<T>(string name)
        {
            return Exist(typeof(T), name);
        }
        /// <summary>
        /// 是否存在对应名字类型的实例
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Exist(Type type, string name)
        {
            ITypeMap map;

            if (!values.TryGetValue(type, out map))
            {
                return false;
            }
            return map.Exist(name);
        }
        /// <summary>
        /// 获取对应名字类型的实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="autoCreate"></param>
        /// <returns></returns>
        public T Get<T>(string name, bool autoCreate = true)
        {
            Type type = typeof(T);
            ITypeMap map;
            if (!values.TryGetValue(type, out map))
            {
                map = new TypeMap<T>();
                values.Add(type, map);
            }
            return (map as TypeMap<T>).GetValue(name, autoCreate);
        }
        /// <summary>
        /// 设置对应名字类型的实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="t"></param>
        public void Set<T>(string name, T t)
        {
            Type type = typeof(T);
            ITypeMap map;
            if (!values.TryGetValue(type, out map))
            {
                map = new TypeMap<T>();
                values.Add(type, map);
            }
            (map as TypeMap<T>).SetValue(name, t);
        }
        /// <summary>
        /// 获取对应名字类型的实例
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="autoCreate"></param>
        /// <returns></returns>
        public object Get(Type type, string name, bool autoCreate = true)
        {
            ITypeMap map;
            if (!values.TryGetValue(type, out map))
            {
                map = Activator.CreateInstance(typeof(TypeMap<>).MakeGenericType(type)) as ITypeMap;
                values.Add(type, map);
            }
            return map.Get(name, autoCreate);
        }
        /// <summary>
        /// 设置对应名字类型的实例
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="t"></param>
        public void Set(Type type, string name, object t)
        {
            ITypeMap map;
            if (!values.TryGetValue(type, out map))
            {
                map = Activator.CreateInstance(typeof(TypeMap<>).MakeGenericType(type)) as ITypeMap;
                values.Add(type, map);
            }
            map.Set(name, t);
        }

        /// <summary>
        /// 获取第一个名字对应的实例
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetFirst(string name)
        {
            foreach (var item in values.Values)
            {
                object obj = item.Get(name, false);
                if (obj != null)
                    return obj;
            }
            return null;
        }

        /// <summary>
        /// 清除
        /// </summary>
        public void Clear()
        {
            values.Clear();
        }


    }
}
