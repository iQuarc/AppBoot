using System;
using System.Collections.Generic;

namespace iQuarc.AppBoot
{
    public class BootstrapperConfig
    {
        private readonly Dictionary<Type, object> settings = new Dictionary<Type, object>()
        {
            {typeof (IContextStore), new CallContextStore()}
        };

        public void SetSettingInstance<T>(T instance)
        {
            settings[typeof (T)] = instance;
        }

        public T GetSetting<T>()
        {
            object setting;
            if (settings.TryGetValue(typeof (T), out setting))
                return (T) setting;

            throw new InvalidOperationException(string.Format("Setting for {0} not found", typeof (T).Name));
        }
    }
}