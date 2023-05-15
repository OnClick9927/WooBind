namespace WooBind
{
    /// <summary>
    /// VM
    /// </summary>
    public abstract class ViewModel : ObservableObject, IViewModel
    {
        internal MVVMGroup group { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        protected IMVVMModel model { get { return group.model; } }

        void IViewModel.SyncModelValue()
        {
            SyncModelValue();
        }
        void IViewModel.Initialize()
        {
            Initialize();
        }
        void IViewModel.Listen(IMessage message)
        {
            Listen(message);
        }
        /// <summary>
        /// 初始化
        /// </summary>
        protected abstract void Initialize();
        /// <summary>
        /// 同步model数据
        /// </summary>
        protected abstract void SyncModelValue();
        /// <summary>
        /// 来自于view的消息
        /// </summary>
        /// <param name="message"></param>
        protected abstract void Listen(IMessage message);
       
    }
    /// <summary>
    /// 方便书写
    /// </summary>
    public abstract class ViewModel<T> : ViewModel where T : IMVVMModel
    {
        /// <summary>
        /// 方便书写
        /// </summary>
        protected T Tmodel { get { return (T)group.model; } }

    }

}
