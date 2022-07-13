using CMFDev.Toolkit.Configuration.AppData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMFDev.Toolkit.Configuration.Internal
{

    internal class AppDataSettingsService<TAppSettingsImpl> : IAppDataSettingsService<TAppSettingsImpl>
    {
        private readonly IConfiguration _config;

        public AppDataSettingsService(IConfiguration config)
        {
            _config = config;
            ChangeToken.OnChange<IAppDataSettingsService<TAppSettingsImpl>>(()=> config.GetReloadToken(), _OnConfigReloadTokenTriggered, this);
        }

        public event EventHandler<SettingsChangedEventArgs<TAppSettingsImpl>>? SettingsChanged;
        public TAppSettingsImpl Get() => _config.Get<TAppSettingsImpl>();

        private void _OnSettingsChanged()
        {
            var local = SettingsChanged;
            local?.Invoke(this, new SettingsChangedEventArgs<TAppSettingsImpl>(Get()));
        }

        private static void _OnConfigReloadTokenTriggered(IAppDataSettingsService<TAppSettingsImpl> state)
        {
            (state as AppDataSettingsService<TAppSettingsImpl>)?._OnSettingsChanged();
        }
    }
}
