using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WooBind
{
    /// <summary>
    /// 可观测Dictionary
    /// </summary>
    /// <typeparam name="TKey">TKey</typeparam>
    /// <typeparam name="TValue">TValue</typeparam>
    public class ObservableDictionary<TKey, TValue> : BindUnit, IDictionary<TKey, TValue>, IDictionary
    {
        #region 事件与变量定义
        private Action<KeyValuePair<TKey, TValue>> onKeyValuePairAdded;
        private Action<KeyValuePair<TKey, TValue>, KeyValuePair<TKey, TValue>> onKeyValuePairReplaced;
        private Action<KeyValuePair<TKey, TValue>> onKeyValuePairRemoved;
        private Action onClear;
        private Action<KeyValuePair<TKey, TValue>[]> onKeyValuePairRangeAdded;

        private Lazy<Dictionary<TKey, TValue>> _dictionary = new Lazy<Dictionary<TKey, TValue>>(() => { return new Dictionary<TKey, TValue>(); });
        private Dictionary<TKey, TValue> dictionary { get { return _dictionary.Value; } }
        #endregion

        #region 注册与移除监听方法
        /// <summary>
        /// 注册方法 添加键值对
        /// </summary>
        /// <param name="action">
        ///     action arg1 : the Added KeyValuePair of <typeparamref name="TKey" /> and <typeparamref name="TValue" /><br/>
        ///     参数1为添加的键值对对象,键的类型为<typeparamref name="TKey" />，值的类型为<typeparamref name="TValue" />
        /// </param>
        public void SubscribeAddKeyValuePair(Action<KeyValuePair<TKey, TValue>> action)
        {
            onKeyValuePairAdded += action;
        }
        /// <summary>
        /// 移除方法 添加键值对
        /// </summary>
        /// <param name="action">
        ///     action arg1 : the Added KeyValuePair of <typeparamref name="TKey" /> and <typeparamref name="TValue" /><br/>
        ///     参数1为添加的键值对对象,键的类型为<typeparamref name="TKey" />，值的类型为<typeparamref name="TValue" />
        /// </param>
        public void UnSubscribeAddKeyValuePair(Action<KeyValuePair<TKey, TValue>> action)
        {
            onKeyValuePairAdded -= action;
        }
        /// <summary>
        /// 注册方法 替换键值对
        /// </summary>
        /// <param name = "action" >
        ///     action arg1 : the old KeyValuePair of <typeparamref name="TKey" /> and <typeparamref name="TValue" /> which has been replaced<br/>
        ///     参数1为被替换的键值对对象,键的类型为<typeparamref name="TKey" />，值的类型为<typeparamref name="TValue" /><br/><br/>
        ///     action arg2 : the new KeyValuePair of <typeparamref name="TKey" /> and <typeparamref name="TValue" /> which replaced the old one<br/>
        ///     参数1为新的键值对对象,键的类型为<typeparamref name="TKey" />，值的类型为<typeparamref name="TValue" /><br/>
        /// </param>
        public void SubscribeReplaceKeyValuePair(Action<KeyValuePair<TKey, TValue>, KeyValuePair<TKey, TValue>> action)
        {
            onKeyValuePairReplaced += action;
        }
        /// <summary>
        /// 移除方法 替换键值对
        /// </summary>
        /// <param name = "action" >
        ///     action arg1 : the old KeyValuePair of <typeparamref name="TKey" /> and <typeparamref name="TValue" /> which has been replaced<br/>
        ///     参数1为被替换的键值对对象,键的类型为<typeparamref name="TKey" />，值的类型为<typeparamref name="TValue" /><br/><br/>
        ///     action arg2 : the new KeyValuePair of <typeparamref name="TKey" /> and <typeparamref name="TValue" /> which replaced the old one<br/>
        ///     参数1为新的键值对对象,键的类型为<typeparamref name="TKey" />，值的类型为<typeparamref name="TValue" /><br/>
        /// </param>
        public void UnSubscribeReplaceKeyValuePair(Action<KeyValuePair<TKey, TValue>, KeyValuePair<TKey, TValue>> action)
        {
            onKeyValuePairReplaced -= action;
        }
        /// <summary>
        /// 注册方法 移除键值对
        /// </summary>
        /// <param name="action">
        ///     action arg1 : the removed KeyValuePair with <typeparamref name="TKey" /> and <typeparamref name="TValue" /><br/>
        ///     参数1为移除的键值对对象,键的类型为<typeparamref name="TKey" />，值的类型为<typeparamref name="TValue" />
        /// </param>
        public void SubscribeRemoveKeyValuePair(Action<KeyValuePair<TKey, TValue>> action)
        {
            onKeyValuePairRemoved += action;
        }
        /// <summary>
        /// 移除方法 移除键值对
        /// </summary>
        /// <param name="action">
        ///     action arg1 : the removed KeyValuePair with <typeparamref name="TKey" /> and <typeparamref name="TValue" /><br/>
        ///     参数1为移除的键值对对象,键的类型为<typeparamref name="TKey" />，值的类型为<typeparamref name="TValue" />
        /// </param>
        public void UnSubscribeRemoveKeyValuePair(Action<KeyValuePair<TKey, TValue>> action)
        {
            onKeyValuePairRemoved -= action;
        }
        /// <summary>
        /// 注册方法 清除所有键值对
        /// </summary>
        /// <param name="action">none arg Action</param>
        public void SubscribeClear(Action action)
        {
            onClear += action;
        }
        /// <summary>
        /// 移除方法 清除所有键值对
        /// </summary>
        /// <param name="action">none arg Action</param>
        public void UnSbscribeClear(Action action)
        {
            onClear -= action;
        }
        /// <summary>
        /// 注册方法 添加多个键值对
        /// </summary>
        /// <param name="action">
        ///     action arg1 : the Added array of KeyValuePair with <typeparamref name="TKey" /> and <typeparamref name="TValue" /><br/>
        ///     参数1为添加的键值对数组对象,键的类型为<typeparamref name="TKey" />，值的类型为<typeparamref name="TValue" />
        /// </param>
        public void SubscribeAddRange(Action<KeyValuePair<TKey, TValue>[]> action)
        {
            onKeyValuePairRangeAdded += action;
        }
        /// <summary>
        /// 移除方法 添加添加多个键值对
        /// </summary>
        /// <param name="action">
        ///     action arg1 : the Added array of KeyValuePair with <typeparamref name="TKey" /> and <typeparamref name="TValue" /><br/>
        ///     参数1为添加的键值对数组对象,键的类型为<typeparamref name="TKey" />，值的类型为<typeparamref name="TValue" />
        /// </param>
        public void UnSubscribeAddRange(Action<KeyValuePair<TKey, TValue>[]> action)
        {
            onKeyValuePairRangeAdded -= action;
        }
        #endregion

        #region 接口实现
        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        public TValue this[TKey key]
        {
            get
            {
                if (!dictionary.ContainsKey(key))
                    return default(TValue);
                return dictionary[key];
            }
            set
            {
                Insert(key, value, false);
            }
        }
        /// <summary>
        /// 键集合
        /// </summary>
        public ICollection<TKey> Keys
        {
            get { return dictionary.Keys; }
        }
        /// <summary>
        /// 值集合
        /// </summary>
        public ICollection<TValue> Values
        {
            get { return dictionary.Values; }
        }
        /// <summary>
        /// 数量
        /// </summary>
        public int Count
        {
            get { return dictionary.Count; }
        }
        /// <summary>
        /// 是否只读
        /// </summary>
        public bool IsReadOnly
        {
            get { return ((IDictionary)this.dictionary).IsReadOnly; }
        }

        /// <summary>
        /// 添加键值对
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void Add(TKey key, TValue value)
        {
            Insert(key, value, true);
        }
        /// <summary>
        /// 添加键值对
        /// </summary>
        /// <param name="pair">键值对</param>
        public void Add(KeyValuePair<TKey, TValue> pair)
        {
            Insert(pair.Key, pair.Value, true);
        }
        /// <summary>
        /// 清除所有键值对
        /// </summary>
        public void Clear()
        {
            if (dictionary.Count > 0)
            {
                dictionary.Clear();
                onClear?.Invoke();
            }
        }
        /// <summary>
        /// 是否存在键值对
        /// </summary>
        /// <param name="pair">键值对</param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<TKey, TValue> pair)
        {
            return dictionary.Contains(pair);
        }
        /// <summary>
        /// 是否存在键
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>存在情况</returns>
        public bool ContainsKey(TKey key)
        {
            return dictionary.ContainsKey(key);
        }
        /// <summary>
        /// 将字典里的键值对拷贝到指定数组中
        /// </summary>
        /// <param name="array">接收的数组</param>
        /// <param name="arrayIndex">开始保存在数组的索引</param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((IDictionary)this.dictionary).CopyTo(array, arrayIndex);
        }
        /// <summary>
        /// 获取迭代器
        /// </summary>
        /// <returns>迭代器</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }
        /// <summary>
        /// 删除键值对
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>是否删除成功</returns>
        public bool Remove(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException("key is null");

            dictionary.TryGetValue(key, out TValue value);
            var removed = dictionary.Remove(key);
            if (removed)
            {
                onKeyValuePairRemoved.Invoke(new KeyValuePair<TKey, TValue>(key, value));
            }

            return removed;
        }
        /// <summary>
        /// 删除键值对
        /// </summary>
        /// <param name="pair">键值对</param>
        /// <returns>是否删除成功，如果对应的键值对不存在，返回false</returns>
        public bool Remove(KeyValuePair<TKey, TValue> pair)
        {
            if (Contains(pair))
            {
                return Remove(pair.Key);
            }
            return false;
        }
        /// <summary>
        /// 获取指定键对应的值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">保存值的对象</param>
        /// <returns>是否获取成功</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)dictionary).GetEnumerator();
        }
        object IDictionary.this[object key]
        {
            get
            {
                return ((IDictionary)this.dictionary)[key];
            }
            set
            {
                Insert((TKey)key, (TValue)value, false);
            }
        }
        ICollection IDictionary.Keys
        {
            get { return ((IDictionary)this.dictionary).Keys; }
        }
        ICollection IDictionary.Values
        {
            get { return ((IDictionary)this.dictionary).Values; }
        }
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)this.dictionary).GetEnumerator();
        }
        bool IDictionary.Contains(object key)
        {
            return ((IDictionary)this.dictionary).Contains(key);
        }
        void IDictionary.Add(object key, object value)
        {
            this.Add((TKey)key, (TValue)value);
        }
        void IDictionary.Remove(object key)
        {
            this.Remove((TKey)key);
        }
        bool IDictionary.IsFixedSize
        {
            get { return ((IDictionary)this.dictionary).IsFixedSize; }
        }
        object ICollection.SyncRoot
        {
            get { return ((IDictionary)this.dictionary).SyncRoot; }
        }
        bool ICollection.IsSynchronized
        {
            get { return ((IDictionary)this.dictionary).IsSynchronized; }
        }
        void ICollection.CopyTo(Array array, int index)
        {
            ((IDictionary)this.dictionary).CopyTo(array, index);
        }

        /// <summary>
        /// 对象释放时调用（继承自Unit）
        /// </summary>
        protected override void OnDispose()
        {
            dictionary.Clear();
            onKeyValuePairAdded -= onKeyValuePairAdded;
            onKeyValuePairReplaced -= onKeyValuePairReplaced;
            onKeyValuePairRemoved -= onKeyValuePairRemoved;
        }
        #endregion

        #region 额外方法实现

        private void Insert(TKey key, TValue value, bool isAdd)
        {
            if (key == null)
                throw new ArgumentNullException("key is null");

            if (dictionary.TryGetValue(key, out TValue oldValue))
            {
                if (isAdd)
                    throw new ArgumentException("this key has already been added");

                if (Equals(oldValue, value))
                    return;

                dictionary[key] = value;
                onKeyValuePairReplaced?.Invoke(new KeyValuePair<TKey, TValue>(key, oldValue), new KeyValuePair<TKey, TValue>(key, value));
            }
            else
            {
                dictionary[key] = value;
                onKeyValuePairAdded?.Invoke(new KeyValuePair<TKey, TValue>(key, value));
            }
        }

        /// <summary>
        /// 将一个字典添加到另一个字典中
        /// </summary>
        /// <param name="items">字典对象</param>
        public void AddRange(IDictionary<TKey, TValue> items)
        {
            if (items == null)
                throw new ArgumentNullException("items is null");

            if (items.Count > 0)
            {
                if (items.Keys.Any((k) => this.dictionary.ContainsKey(k)))
                    throw new ArgumentException("a key or some keys in this dictionary has already been added");
                else
                {
                    foreach (var item in items)
                        ((IDictionary<TKey, TValue>)this.dictionary).Add(item);
                }
                onKeyValuePairRangeAdded?.Invoke(items.ToArray());
            }
        }

        #endregion
    }
}
