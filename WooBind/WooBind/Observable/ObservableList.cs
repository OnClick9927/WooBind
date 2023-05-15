using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace WooBind
{
    /// <summary>
    /// 可观测List
    /// </summary>
    /// <typeparam name="T">Object</typeparam>
    public class ObservableList<T> : BindUnit, IList<T>, IList
    {
        #region 事件与变量定义
        private Action<int, T> onItemAdded;
        private Action<int, int, T> onItemMoved;
        private Action<int, T> onItemRemoved;
        private Action<int, T, T> onItemReplaced;
        private Action onItemCleared;

        private Action<IList, int> onRangeAdded;
        private Action<IList, int> onRangeRemoved;


        [NonSerialized]
        private Object syncRoot; //同步访问对象

        private Lazy<List<T>> _value = new Lazy<List<T>>(() => { return new List<T>(); });
        private IList<T> value { get { return _value.Value; } }
        #endregion

        #region 注册与移除监听方法
        /// <summary>
        /// 注册方法 添加一个元素
        /// </summary>
        /// <param name="action">
        ///     action arg1 : the index of the added item  <br/>
        ///     参数1为添加的元素的索引 <br/><br/>
        ///     action arg2 : the added item  <br/>
        ///     参数2为添加的元素，类型为<typeparamref name="T" />
        /// </param>
        public void SubscribeAddItem(Action<int, T> action)
        {
            onItemAdded += action;
        }
        /// <summary>
        /// 移除方法 添加一个元素
        /// </summary>
        /// <param name="action">
        ///     action arg1 : the index of the added item  <br/>
        ///     参数1为添加的元素的索引 <br/><br/>
        ///     action arg2 : the added item  <br/>
        ///     参数2为添加的元素，类型为<typeparamref name="T" />
        /// </param>
        public void UnSubscribeAddItem(Action<int, T> action)
        {
            onItemAdded -= action;
        }
        /// <summary>
        /// 注册方法 移动一个元素
        /// </summary>
        /// <param name="action">
        ///     action arg1 : the old index of the moved item  <br/>
        ///     参数1为元素移动前的索引 <br/><br/>
        ///     action arg2 : the new index of the moved item  <br/>
        ///     参数1为元素移动后的索引 <br/><br/>
        ///     action arg3 : the added item  <br/>
        ///     参数3为移动的元素，类型为<typeparamref name="T" />
        /// </param>
        public void SubscribeMoveItem(Action<int, int, T> action)
        {
            onItemMoved += action;
        }
        /// <summary>
        /// 移除方法 移动一个元素
        /// </summary>
        /// <param name="action">
        ///     action arg1 : the old index of the moved item  <br/>
        ///     参数1为元素移动前的索引 <br/><br/>
        ///     action arg2 : the new index of the moved item  <br/>
        ///     参数1为元素移动后的索引 <br/><br/>
        ///     action arg3 : the added item  <br/>
        ///     参数3为移动的元素，类型为<typeparamref name="T" />
        ///</param>
        public void UnSubscribeMoveItem(Action<int, int, T> action)
        {
            onItemMoved -= action;
        }
        /// <summary>
        /// 注册方法 移除一个元素
        /// </summary>
        /// <param name="action">
        ///     action arg1 : the index of the removed item  <br/>
        ///     参数1为移除的元素的索引 <br/><br/>
        ///     action arg2 : the removed item  <br/>
        ///     参数2为移除的元素，类型为<typeparamref name="T" />
        /// </param>
        public void SubscribeRemoveItem(Action<int, T> action)
        {
            onItemRemoved += action;
        }
        /// <summary>
        /// 移除方法 移除一个元素
        /// </summary>
        /// <param name="action">
        ///     action arg1 : the index of the removed item  <br/>
        ///     参数1为移除的元素的索引 <br/><br/>
        ///     action arg2 : the removed item  <br/>
        ///     参数2为移除的元素，类型为<typeparamref name="T" />
        /// </param>
        public void UnSubscribeRemoveItem(Action<int, T> action)
        {
            onItemRemoved -= action;
        }
        /// <summary>
        /// 注册方法 替换一个元素
        /// </summary>
        /// <param name="action">
        ///     action arg1 : the index of the replaced item  <br/>
        ///     参数1为被替换的元素的索引 <br/><br/>
        ///     action arg2 : the old item which has been replaced <br/>
        ///     参数2为被替换的元素，类型为<typeparamref name="T" /> <br/><br/>
        ///     action arg3 : the new item which replaced the old one <br/>
        ///     参数3为替换进来的元素，类型为<typeparamref name="T" />
        /// </param>
        public void SubscribeReplaceItem(Action<int, T, T> action)
        {
            onItemReplaced += action;
        }
        /// <summary>
        /// 移除方法 替换一个元素
        /// </summary>
        /// <param name="action">
        ///     action arg1 : the index of the replaced item  <br/>
        ///     参数1为被替换的元素的索引 <br/><br/>
        ///     action arg2 : the old item which has been replaced <br/>
        ///     参数2为被替换的元素，类型为<typeparamref name="T" /> <br/><br/>
        ///     action arg3 : the new item which replaced the old one <br/>
        ///     参数3为替换进来的元素，类型为<typeparamref name="T" />
        /// </param>
        public void UnSubscribeReplaceItem(Action<int, T, T> action)
        {
            onItemReplaced -= action;
        }
        /// <summary>
        /// 注册方法 清空元素
        /// </summary>
        /// <param name="action">none arg Action</param>
        public void SubscribeClearItem(Action action)
        {
            onItemCleared += action;
        }
        /// <summary>
        /// 移除方法 清空元素
        /// </summary>
        /// <param name="action">none arg Action</param>
        public void UnSubscribeClearItem(Action action)
        {
            onItemCleared -= action;
        }
        /// <summary>
        /// 注册方法 插入一个集合
        /// </summary>
        /// <param name="action">
        ///     action arg1 : the list of the insert items<br/>
        ///     参数1为插入的IList列表对象 <br/><br/>
        ///     action arg2 : the index of the insert items<br/>
        ///     参数2为插入开始时的索引
        /// </param>
        public void SubScribeAddRange(Action<IList, int> action)
        {
            onRangeAdded += action;
        }
        /// <summary>
        /// 移除方法 插入一个集合
        /// </summary>
        /// <param name="action">
        ///     action arg1 : the list of the insert items<br/>
        ///     参数1为插入的IList列表对象 <br/><br/>
        ///     action arg2 : the index of the insert items<br/>
        ///     参数2为插入开始时的索引
        /// </param>
        public void UnSubScribeAddRange(Action<IList, int> action)
        {
            onRangeAdded -= action;
        }
        /// <summary>
        /// 注册方法 删除多个元素
        /// </summary>
        /// <param name="action">
        ///     action arg1 : the list of the deleted items<br/>
        ///     参数1为删除的IList列表对象 <br/><br/>
        ///     action arg2 : the first index of the deleted items<br/>
        ///     参数2为删除时对应的索引
        /// </param>
        public void SubScribeRemoveRange(Action<IList, int> action)
        {
            onRangeRemoved += action;
        }
        /// <summary>
        /// 移除方法 删除多个元素
        /// </summary>
        /// <param name="action">
        ///     action arg1 : the list of the deleted items<br/>
        ///     参数1为删除的IList列表对象 <br/><br/>
        ///     action arg2 : the first index of the deleted items<br/>
        ///     参数2为删除时对应的索引
        /// </param>
        public void UnSubScribeRemoveRange(Action<IList, int> action)
        {
            onRangeRemoved -= action;
        }
        #endregion

        #region 接口实现

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public T this[int index]
        {
            get { return value[index]; }
            set
            {
                if (this.value.IsReadOnly)
                    throw new NotSupportedException("ReadOnlyCollection");
                if (index < 0 || index >= this.value.Count)
                    throw new ArgumentOutOfRangeException($"index:{index}");

                ReplaceItem(index, value);
            }
        }
        /// <summary>
        /// 数量
        /// </summary>
        public int Count
        {
            get { return value.Count; }
        }
        /// <summary>
        /// 只读
        /// </summary>
        public bool IsReadOnly
        {
            get { return value.IsReadOnly; }
        }

        /// <summary>
        /// 将对象添加到List末尾
        /// </summary>
        /// <param name="item">要添加到List末尾的对象</param>
        public void Add(T item)
        {
            int index = value.Count;
            Insert(index, item);
        }
        /// <summary>
        /// 移除List中的所有元素
        /// </summary>
        public void Clear()
        {
            if (value.IsReadOnly)
                throw new NotSupportedException("ReadOnlyCollection");

            if (value.Count > 0)
            {
                value.Clear();
                onItemCleared?.Invoke();
            }
        }
        /// <summary>
        /// 确定某元素是否在List中
        /// </summary>
        /// <param name="item">要在List中定位的对象</param>
        /// <returns>如果存在返回true,否则为false</returns>
        public bool Contains(T item)
        {
            return value.Contains(item);
        }
        /// <summary>
        /// 从目标数组的指定索引处开始将整个List复制到兼容的一维Array
        /// </summary>
        /// <param name="array">从List中复制的元素的目标一维Array</param>
        /// <param name="arrayIndex">array中从零开始的索引，从此处开始复制</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            value.CopyTo(array, arrayIndex);
        }
        /// <summary>
        /// 返回循环访问List的IEnumerator
        /// </summary>
        /// <returns>List的泛型枚举器</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return value.GetEnumerator();
        }
        /// <summary>
        /// 搜索指定的对象，并返回整个List中第一个匹配项的从零开始的索引
        /// </summary>
        /// <param name="item">要在List中定位的对象</param>
        /// <returns>List中第一个匹配项的从零开始的索引,如果未匹配到则为-1</returns>
        public int IndexOf(T item)
        {
            return value.IndexOf(item);
        }
        /// <summary>
        /// 将元素插入List的指定索引处
        /// </summary>
        /// <param name="index">插入 item 的从零开始的索引</param>
        /// <param name="item">要插入的对象</param>
        public void Insert(int index, T item)
        {
            if (value.IsReadOnly)
                throw new NotSupportedException("ReadOnlyCollection");

            if (index < 0 || index > value.Count)
                throw new ArgumentOutOfRangeException($"index:{index}");

            value.Insert(index, item);
            onItemAdded?.Invoke(index, item);
        }

        /// <summary>
        /// 从List中移除特定对象的第一个匹配项
        /// </summary>
        /// <param name="item">要删除的对象</param>
        /// <returns>成功移除返回true，否则为false；如果未找到也返回false</returns>
        public bool Remove(T item)
        {
            if (value.IsReadOnly)
                throw new NotSupportedException("ReadOnlyCollection");

            int index = IndexOf(item);
            if (index < 0)
                return false;
            var result = value.Remove(item);
            if (result)
            {
                onItemRemoved?.Invoke(index, item);
            }
            return result;
        }
        /// <summary>
        /// 移除List的指定索引处的元素
        /// </summary>
        /// <param name="index">要移除的元素的从零开始的索引</param>
        public void RemoveAt(int index)
        {
            if (value.IsReadOnly)
                throw new NotSupportedException("ReadOnlyCollection");

            if (index < 0 || index >= value.Count)
                throw new ArgumentOutOfRangeException($"index:{index}");


            T item = value[index];
            value.RemoveAt(index);
            onItemRemoved?.Invoke(index, item);
        }
        /// <summary>
        /// 替换指定索引处的元素
        /// </summary>
        /// <param name="index">待替换元素的从零开始的索引</param>
        /// <param name="item">位于指定索引处的元素的新值</param>
        private void ReplaceItem(int index, T item)
        {
            var oldItem = value[index];
            value[index] = item;
            onItemReplaced?.Invoke(index, oldItem, item);
        }
        /// <summary>
        /// 列表对象释放时调用（继承自Unit）
        /// </summary>
        protected override void OnDispose()
        {
            value.Clear();
            onItemAdded -= onItemAdded;
            onItemMoved -= onItemMoved;
            onItemRemoved -= onItemRemoved;
            onItemReplaced -= onItemReplaced;
            onItemCleared -= onItemCleared;
        }

        int IList.Add(object value)
        {
            if (this.value.IsReadOnly)
                throw new NotSupportedException("ReadOnlyCollection");

            if (value == null && !(default(T) == null))
                throw new ArgumentNullException("value is null");

            try
            {
                Add((T)value);
            }
            catch (InvalidCastException e)
            {
                throw new ArgumentException("", e);
            }

            return this.Count - 1;
        }
        bool IList.Contains(object value)
        {
            if ((value is T) || (value == null && default(T) == null))
            {
                return Contains((T)value);
            }
            return false;
        }
        int IList.IndexOf(object value)
        {
            int index = -1;
            if ((value is T) || (value == null && default(T) == null))
            {
                index = IndexOf((T)value);
            }
            return index;
        }
        void IList.Insert(int index, object value)
        {
            if (this.value.IsReadOnly)
                throw new NotSupportedException("ReadOnlyCollection");

            if (value == null && !(default(T) == null))
                throw new ArgumentNullException("value is null");

            try
            {
                Insert(index, (T)value);
            }
            catch (InvalidCastException e)
            {
                throw new ArgumentException("", e);
            }
        }
        void IList.Remove(object value)
        {
            if (this.value.IsReadOnly)
                throw new NotSupportedException("ReadOnlyCollection");

            if ((value is T) || (value == null && default(T) == null))
            {
                Remove((T)value);
            }
        }
        object IList.this[int index]
        {
            get { return value[index]; }
            set
            {
                if (value == null && !(default(T) == null))
                    throw new ArgumentNullException("value is null");

                try
                {
                    this[index] = (T)value;
                }
                catch (InvalidCastException e)
                {
                    throw new ArgumentException("", e);
                }
            }
        }
        bool IList.IsFixedSize
        {
            get
            {
                IList list = value as IList;
                if (list != null)
                {
                    return list.IsFixedSize;
                }
                return list.IsReadOnly;
            }
        }
        bool IList.IsReadOnly
        {
            get
            {
                return value.IsReadOnly;
            }
        }
        object ICollection.SyncRoot
        {
            get
            {
                if (this.syncRoot == null)
                {
                    if (value is ICollection c)
                    {
                        this.syncRoot = c.SyncRoot;
                    }
                    else
                    {
                        Interlocked.CompareExchange<Object>(ref this.syncRoot, new Object(), null);
                    }
                }
                return this.syncRoot;
            }
        }
        bool ICollection.IsSynchronized
        {
            get { return false; }
        }
        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException("array is null");

            if (array.Rank != 1)
                throw new ArgumentException("RankMultiDimNotSupported");

            if (array.GetLowerBound(0) != 0)
                throw new ArgumentException("NonZeroLowerBound");

            if (index < 0)
                throw new ArgumentOutOfRangeException($"index:{index}");

            if (array.Length - index < Count)
                throw new ArgumentException("ArrayPlusOffTooSmall");

            if (array is T[] tArray)
            {
                value.CopyTo(tArray, index);
            }
            else
            {
                Type targetType = array.GetType().GetElementType();
                Type sourceType = typeof(T);
                if (!(targetType.IsAssignableFrom(sourceType) || sourceType.IsAssignableFrom(targetType)))
                    throw new ArgumentException("InvalidArrayType");

                if (!(array is object[] objects))
                    throw new ArgumentException("InvalidArrayType");

                int count = value.Count;
                try
                {
                    for (int i = 0; i < count; i++)
                    {
                        objects[index++] = value[i];
                    }
                }
                catch (ArrayTypeMismatchException)
                {
                    throw new ArgumentException("InvalidArrayType");
                }
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)value).GetEnumerator();
        }
        #endregion

        #region 额外方法实现
        /// <summary>
        /// 将指定索引处的项移至列表中的新位置
        /// </summary>
        /// <param name="oldIndex">指定要移动的项的位置的从零开始的索引</param>
        /// <param name="newIndex">当前状态下指定项的新位置的从零开始的索引</param>
        public void Move(int oldIndex, int newIndex)
        {
            if (oldIndex < 0 || oldIndex >= value.Count)
                throw new ArgumentOutOfRangeException($"oldIndex:{oldIndex}");
            if (newIndex < 0 || newIndex >= value.Count)
                throw new ArgumentOutOfRangeException($"newIndex:{newIndex}");

            if (oldIndex == newIndex) return;

            T item = value[oldIndex];

            value.RemoveAt(oldIndex);
            value.Insert(newIndex, item);

            onItemMoved?.Invoke(oldIndex, newIndex, item);
        }

        /// <summary>
        /// 在List末尾处添加一个集合
        /// </summary>
        /// <param name="collection"></param>
        /// <exception cref="NotSupportedException">只读警告</exception>
        public void AddRange(IEnumerable<T> collection)
        {
            int index = value.Count;
            InsertRange(index, collection);
        }
        /// <summary>
        /// 从索引处插入一个集合
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="collection">集合</param>
        /// <exception cref="NotSupportedException">只读错误</exception>
        /// <exception cref="ArgumentOutOfRangeException">越界报错</exception>
        public void InsertRange(int index, IEnumerable<T> collection)
        {
            if (value.IsReadOnly)
                throw new NotSupportedException("ReadOnlyCollection");

            if (index < 0 || index > value.Count)
                throw new ArgumentOutOfRangeException($"index:{index}");

            (value as List<T>).InsertRange(index, collection);

            onRangeAdded?.Invoke(ToList(collection), index);
        }
        /// <summary>
        /// 从索引处删除对应数量的元素
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="count">数量</param>
        /// <exception cref="NotSupportedException">只读报错</exception>
        /// <exception cref="ArgumentOutOfRangeException">越界报错</exception>
        public void RemoveRange(int index, int count)
        {
            if (value.IsReadOnly)
                throw new NotSupportedException("ReadOnlyCollection");

            if (index < 0 || index >= value.Count)
                throw new ArgumentOutOfRangeException($"index:{index}");

            List<T> list = value as List<T>;
            List<T> deletedItems = list.GetRange(index, count);
            list.RemoveRange(index, count);
            onRangeRemoved?.Invoke(deletedItems, index);
        }

        /// <summary>
        /// IEnumerable to IList
        /// </summary>
        /// <param name="collection">IEnumerable Collection</param>
        /// <returns>IList Collection</returns>
        private IList ToList(IEnumerable<T> collection)
        {
            if (collection is IList list)
                return list;

            List<T> newList = new List<T>();
            newList.AddRange(collection);
            return newList;
        }
        #endregion
    }

}
