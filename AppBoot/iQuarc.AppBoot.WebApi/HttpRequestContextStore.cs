using System.Web;

namespace iQuarc.AppBoot.WebApi
{
    public class HttpRequestContextStore : IContextStore
    {
        public object GetContext(string key)
        {
            return HttpContext.Current.Items[key];
        }

        public void SetContext(object context, string key)
        {
            HttpContext.Current.Items[key] = context;
        }
    }
}