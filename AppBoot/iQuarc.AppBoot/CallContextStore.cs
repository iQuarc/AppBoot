using System.Runtime.Remoting.Messaging;

namespace iQuarc.AppBoot
{
    public class CallContextStore : IContextStore
    {
        public object Get(string key)
        {
            return CallContext.LogicalGetData(key);
        }

        public void Set(object context, string key)
        {
            CallContext.LogicalSetData(key, context);
        }
    }
}