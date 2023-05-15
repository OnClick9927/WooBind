using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace WooBind
{
    /// <summary>
    /// 绑定对象
    /// </summary>
    public abstract class BindableObject : BindUnit
    {
        /// <summary>
        /// 绑定方式
        /// </summary>
        public enum BindOperation
        {
            /// <summary>
            /// 监听+发布
            /// </summary>
            Both,
            /// <summary>
            /// 监听
            /// </summary>
            Listen,
        }
        /// <summary>
        /// 绑定方式
        /// </summary>
        public BindOperation bindOperation = BindOperation.Both;

    
        private Dictionary<string, Action<string, object>> _callmap;
        /// <summary>
        /// ctor
        /// </summary>
        protected BindableObject()
        {
            _callmap = new Dictionary<string, Action<string, object>>();
        }

        /// <summary>
        /// 注册监听
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="listener"></param>
        public void Subscribe(string propertyName, Action<string, object> listener)
        {
            if (!_callmap.ContainsKey(propertyName))
                _callmap.Add(propertyName, null);
            _callmap[propertyName] += listener;
        }

        /// <summary>
        /// 移除监听
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="listener"></param>
        public void UnSubscribe(string propertyName, Action<string, object> listener)
        {
            if (!_callmap.ContainsKey(propertyName))
                return;
            _callmap[propertyName] -= listener;
            if (_callmap[propertyName] == null)
                _callmap.Remove(propertyName);
        }
        /// <summary>
        /// 获取属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected T GetProperty<T>(ref T property, [CallerMemberName]string propertyName = "")
        {
            if (BindableObjectHandler.handler != null)
            {
                //if (string.IsNullOrEmpty(propertyName))
                //    propertyName = GetProperyName(new StackTrace(true).GetFrame(1).GetMethod().Name);
                BindableObjectHandler.handler.Subscribe(this, typeof(T), propertyName);
            }
            return property;
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        protected void SetProperty<T>(ref T property, T value, [CallerMemberName]string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(property, value)) return;
            //if (string.IsNullOrEmpty(propertyName))
            //    propertyName = GetProperyName(new StackTrace(true).GetFrame(1).GetMethod().Name);
            property = value;
            //if (bindOperation == BindOperation.Listen) return;
            PublishPropertyChange(propertyName, value);
        }
        private void PublishPropertyChange(string propertyName, object obj)
        {
            if (!_callmap.ContainsKey(propertyName)) return;
            if (_callmap[propertyName] == null) return;
            _callmap[propertyName].Invoke(propertyName, obj);
        }

        /// <summary>
        /// 释放
        /// </summary>
        protected override void OnDispose()
        {
            _callmap.Clear();
            _callmap = null;
        }
        //private string GetProperyName(string methodName)
        //{
        //    if (methodName.StartsWith("get_") || methodName.StartsWith("set_") ||
        //        methodName.StartsWith("put_"))
        //    {
        //        return methodName.Substring("get_".Length);
        //    }
        //    throw new Exception(methodName + " not a method of Property");
        //}
    }
}
