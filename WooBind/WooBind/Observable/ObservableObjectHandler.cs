using System;
using System.Collections.Generic;

namespace WooBind
{
    /// <summary>
    /// ObservableObject 注册监听Helper
    /// </summary>
    public class ObservableObjectHandler : BindUnit
    {
        struct ObserveEntity
        {
            private string _propertyName;
            private ObservableObject _observableObject;
            private Action _listenner;

            public string propertyName { get { return _propertyName; } }
            public ObservableObject observableObject { get { return _observableObject; } }
            public ObserveEntity(ObservableObject obj, string propertyName, Action listenner)
            {
                this._propertyName = propertyName;
                this._observableObject = obj;
                this._listenner = listenner;
            }

            public void Bind()
            {
                observableObject.Subscribe(propertyName, _listenner);
            }
            public void UnBind()
            {
                observableObject.UnSubscribe(propertyName, _listenner);
            }
        }

        internal static ObservableObjectHandler handler { get; private set; }

        private List<ObserveEntity> _entitys = new List<ObserveEntity>();
        private Action listenner;
        internal ObservableObjectHandler Subscribe(ObservableObject _object, string propertyName)
        {
            Subscribe(_object, propertyName, listenner);
            return this;
        }
        /// <summary>
        /// 对一个 ObservableObject 注册一个监听
        /// </summary>
        /// <param name="_object"> ObservableObject </param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="listenner">回调</param>
        /// <returns></returns>
        public ObservableObjectHandler Subscribe(ObservableObject _object, string propertyName, Action listenner)
        {
            var bindTarget = new ObserveEntity(_object, propertyName, listenner);
            bindTarget.Bind();
            _entitys.Add(bindTarget);
            return this;
        }
        /// <summary>
        /// 绑定一个监听
        /// </summary>
        /// <param name="setter"> 回调</param>
        /// <returns></returns>
        public ObservableObjectHandler BindProperty(Action setter)
        {
            this.listenner = setter;
            handler = this;
            setter.Invoke();
            listenner = null;
            handler = null;
            return this;
        }
        /// <summary>
        /// 绑定一个监听
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="setter"> 设置值 </param>
        /// <param name="getter"> 获取值 </param>
        /// <returns></returns>
        public ObservableObjectHandler BindProperty<T>(Action<T> setter, Func<T> getter)
        {
            this.listenner = () => { setter(getter()); };
            setter(AddExpressionListener(getter));
            return this;
        }
        private T AddExpressionListener<T>(Func<T> expression)
        {
            handler = this;
            var result = expression.Invoke();
            handler = null;
            listenner = null;
            return result;
        }
        /// <summary>
        /// 取消所有监听
        /// </summary>
        public void UnSubscribe()
        {
            _entitys.ForEach((entity) =>
            {
                entity.UnBind();
            });
            _entitys.Clear();
        }
        /// <summary>
        /// 取消符合条件的监听
        /// </summary>
        /// <param name="_object"> ObservableObject </param>
        /// <param name="propertyName"> 属性名称 </param>
        public void UnSubscribe(ObservableObject _object, string propertyName)
        {
            var result = _entitys.RemoveAll((entity) =>
            {
                if (entity.observableObject != _object || entity.propertyName != propertyName) return false;
                entity.UnBind();
                return true;
            });
        }
        /// <summary>
        /// 释放
        /// </summary>
        protected override void OnDispose()
        {
            UnSubscribe();
        }
    }
}
