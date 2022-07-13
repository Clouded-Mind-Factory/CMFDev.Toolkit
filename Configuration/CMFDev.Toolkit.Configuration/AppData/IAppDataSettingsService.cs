using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMFDev.Toolkit.Configuration.AppData
{
    public class SettingsChangedEventArgs<TAppSettingsImpl> : EventArgs
    {
        internal SettingsChangedEventArgs(TAppSettingsImpl settings)
        {
            Settings = settings;
        }

        public TAppSettingsImpl Settings { get; }
    }

    public interface IAppDataSettingsService<TAppSettingsImpl>
    {
        event EventHandler<SettingsChangedEventArgs<TAppSettingsImpl>>? SettingsChanged;
        TAppSettingsImpl Get();
    }
}
