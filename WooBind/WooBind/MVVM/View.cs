namespace WooBind
{
    /// <summary>
    /// 界面
    /// </summary>
    public abstract class View : BindUnit
    {
        private ObservableValue<ViewModel> _context = new ObservableValue<ViewModel>(null);
        /// <summary>
        /// 数据绑定
        /// </summary>
        protected ObservableObjectHandler handler;
        /// <summary>
        /// VM
        /// </summary>
        public ViewModel context
        {
            get { return _context; }
            set
            {
                _context.value = value;
            }
        }
        /// <summary>
        /// ctor
        /// </summary>
        public View()
        {
            handler = new ObservableObjectHandler();
            _context.Subscribe(BindProperty);
        }

        /// <summary>
        /// 绑定数据
        /// </summary>
        protected virtual void BindProperty()
        {
            handler.UnSubscribe();
        }
        /// <summary>
        /// 释放
        /// </summary>
        public new void Dispose()
        {
            handler.UnSubscribe();
            base.Dispose();
        }
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="message"></param>
        protected void Publish(IMessage message)
        {
            (context as IViewModel).Listen(message);
        }


    }
    /// <summary>
    /// 方便书写
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class View<T> : View where T : ViewModel
    {
        /// <summary>
        /// 方便书写
        /// </summary>
        public T Tcontext { get { return context as T; } set { context = value; } }
    }
}
