namespace iQuarc.AppBoot
{
    public interface IContextStore
    {
        object Get(string key);
        void Set(object context, string key);
    }
}