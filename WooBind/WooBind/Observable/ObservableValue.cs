using System;

namespace WooBind
{
    /// <summary>
    /// 可观测树值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableValue<T> : ObservableObject
    {
        /// <summary>
        /// 默认的名字
        /// </summary>
        public const string ValuePropertyName = "value";
        private T _value;
        /// <summary>
        /// 具体的数值
        /// </summary>
        public T value
        {
            get { return GetProperty(ref _value, ValuePropertyName); }
            set
            {
                SetProperty(ref _value, value, ValuePropertyName);
            }
        }
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="value"></param>
        public ObservableValue(T value) : base()
        {
            _value = value;
        }



        /// <summary>
        /// 注册 value 变化监听
        /// </summary>
        /// <param name="listener"></param>
        public void Subscribe(Action listener)
        {
            base.Subscribe(ValuePropertyName, listener);
        }
        /// <summary>
        /// 取消注册 value 变化监听
        /// </summary>
        /// <param name="listener"></param>
        public void UnSubscribe(Action listener)
        {
            base.UnSubscribe(ValuePropertyName, listener);
        }
        /// <summary>
        /// 方便书写，缩减代码
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator T(ObservableValue<T> value)
        {
            return value.value;
        }
    }
}
