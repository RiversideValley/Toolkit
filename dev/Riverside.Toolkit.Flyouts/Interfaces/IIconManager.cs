using H.NotifyIcon.Core;

namespace Riverside.Toolkit.Flyouts.Interfaces;

public interface IIconManager
{
    public TrayIcon FlyoutIcon { get; set; }

    public bool Initialize();

    public void Dispose();
}
