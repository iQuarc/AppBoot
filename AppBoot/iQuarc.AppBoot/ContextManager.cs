using Microsoft.Practices.Unity;

namespace iQuarc.AppBoot
{
    internal static class ContextManager
    {
        public static IDependencyContainer GlobalContainer { get; set; }

        private static IContextStore store = new CallContextStore();
        public static void SetContextStore(IContextStore contextStore)
        {
            store = contextStore;
        }

        private const string KEY = "OperationContext_KEY";

        public static OperationContext Current
        {
            get { return store.GetContext(KEY) as OperationContext; }
        }

        public static void SwitchContext(OperationContext newContext)
        {
            OperationContext oldContext = Current;

            if (oldContext != newContext)
            {
                DisposeContext(oldContext);
                store.SetContext(newContext, KEY);
            }
        }

        private static void DisposeContext(OperationContext context)
        {
            if (context != null)
                context.Dispose();
        }
    }
}