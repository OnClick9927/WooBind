namespace WooBind
{
    interface IViewModel
    {
        void Initialize();
        void SyncModelValue();
        void Listen(IMessage message);
    }

}
