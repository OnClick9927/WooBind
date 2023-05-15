using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace WooBind
{
    /// <summary>
    /// 可观测 Object
    /// </summary>
    public abstract class ObservableObject : BindUnit
    {
        private Dictionary<string, Action> _callmap;
        /// <summary>
        /// Ctor
        /// </summary>
        protected ObservableObject()
        {
            _callmap = new Dictionary<string, Action>();
        }
        /// <summary>
        /// 注册数值变化监听
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="listener"></param>
        public void Subscribe(string propertyName, Action listener)
        {
            if (!_callmap.ContainsKey(propertyName))
                _callmap.Add(propertyName, null);
            _callmap[propertyName] += listener;
        }
        /// <summary>
        /// 取消注册数值变化监听
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="listener"></param>
        public void UnSubscribe(string propertyName, Action listener)
        {
            if (!_callmap.ContainsKey(propertyName))
                throw new Exception($"Have not Subscribe {propertyName}");
            _callmap[propertyName] -= listener;
            if (_callmap[propertyName] == null)
                _callmap.Remove(propertyName);
        }
        /// <summary>
        /// 获取属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property">获取的属性</param>
        /// <param name="propertyName">属性名称</param>
        /// <returns></returns>
        protected T GetProperty<T>(ref T property, [CallerMemberName]string propertyName = "")
        {
            if (ObservableObjectHandler.handler != null)
            {
                //if (string.IsNullOrEmpty(propertyName))
                //    propertyName = GetProperyName(new StackTrace(true).GetFrame(1).GetMethod().Name);
                ObservableObjectHandler.handler.Subscribe(this, propertyName);
            }
            return property;
        }
        /// <summary>
        /// 设置属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property">赋值的变量</param>
        /// <param name="value">变化的值</param>
        /// <param name="propertyName">属性名称</param>
        protected void SetProperty<T>(ref T property, T value, [CallerMemberName]string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(property, value)) return;
            //if (string.IsNullOrEmpty(propertyName))
            //    propertyName = GetProperyName(new StackTrace(true).GetFrame(1).GetMethod().Name);
            property = value;
            PublishPropertyChange(propertyName);
        }
        /// <summary>
        /// 发布属性发生变化
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        protected void PublishPropertyChange(string propertyName)
        {
            if (!_callmap.ContainsKey(propertyName)) return;
            if (_callmap[propertyName] == null) return;
            _callmap[propertyName].Invoke();
        }
        /// <summary>
        /// 释放时
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
