using System.Web;

namespace iQuarc.AppBoot.WebApi
{
    public class HttpRequestContextStore : IContextStore
    {
        public object Get(string key)
        {
            return HttpContext.Current.Items[key];
        }

        public void Set(object context, string key)
        {
            HttpContext.Current.Items[key] = context;
        }
    }
}