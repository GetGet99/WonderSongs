#if DESKTOP
// wrapper of DesktopFlyouts's SystemTrayIcon in Linux
using Olbrasoft.SystemTray.Linux;
using Microsoft.Extensions.Logging;
namespace DesktopFlyouts;

class SystemTrayIcon
{
    ILogger<TrayIcon> logger;
    IconRenderer iconRenderer;
    readonly TrayIcon trayIcon;
    public SystemTrayIcon(string iconPath, string? tooltip, string id)
    {
        // Create logger and icon renderer
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        logger = loggerFactory.CreateLogger<TrayIcon>();
        iconRenderer = new IconRenderer(loggerFactory.CreateLogger<IconRenderer>());

        // Create tray icon
        trayIcon = new TrayIcon(logger, iconRenderer, id);
        this.iconPath = iconPath;
        _tooltip = tooltip;
        _ = InitAsync();
        async Task InitAsync()
        {
            await trayIcon.InitializeAsync();

            // Set icon
            trayIcon.SetIcon(iconPath, tooltip);
            
            // Handle clicks
            trayIcon.Clicked += (sender, args) =>
            {
                LeftClicked?.Invoke(sender, new MouseEventReceivedEventArgs(default));
            };
        }
    }

    string? _tooltip;
    public string? Tooltip {
        get => _tooltip;
        set {
            _tooltip = value;
            trayIcon.SetIcon(iconPath, _tooltip);
        }
    }
    public string IconPath => iconPath;
    string iconPath;
    public void SetIcon(string iconPath)
    {
        this.iconPath = iconPath;
        trayIcon.SetIcon(iconPath, _tooltip);
    }

    public void Show() => trayIcon.Show();
    public void Destroy() => trayIcon.Hide();
    public void Dispose() => trayIcon.Dispose();

    ~SystemTrayIcon() => trayIcon.Dispose();

    /// <summary>
    /// Gets or sets whether the tray icon is visible.
    /// </summary>
    /// <value><see langword="true"/> if the tray icon is visible; otherwise,
    /// <see langword="false"/>. The default is <see langword="false"/>.</value>
    /// <remarks>
    /// If the tray icon has already been created in the shell, setting this property updates
    /// the icon state immediately. Otherwise, the value is recorded and applied when
    /// <see cref="Show"/> is called.
    /// </remarks>
    public bool IsVisible
    {
        get => trayIcon.IsVisible;
        set
        {
            if (value)
            {
                trayIcon.Show();
            } else
            {
                trayIcon.Hide();
            }
        }
    }

    /// <summary>
    /// Gets the stable identifier used for the tray icon.
    /// </summary>
    /// <value>The GUID used by the shell to identify this notification icon.</value>
    public string Id => trayIcon.Id;

    /// <summary>
    /// Occurs when the tray icon receives a left-click.
    /// </summary>
    /// <remarks>
    /// The event argument contains the center point of the tray icon in physical screen pixels.
    /// </remarks>
    public event EventHandler<MouseEventReceivedEventArgs>? LeftClicked;

    /// <summary>
    /// Occurs when the tray icon receives a right-click.
    /// </summary>
    /// <remarks>
    /// The event argument contains the center point of the tray icon in physical screen pixels.
    /// </remarks>
    public event EventHandler<MouseEventReceivedEventArgs>? RightClicked;

    /// <summary>
    /// Occurs when the tray icon receives a left double-click.
    /// </summary>
    /// <remarks>
    /// The event argument contains the center point of the tray icon in physical screen pixels.
    /// </remarks>
    public event EventHandler<MouseEventReceivedEventArgs>? LeftDoubleClicked;

    /// <summary>
    /// Occurs when the tray icon receives a right double-click.
    /// </summary>
    /// <remarks>
    /// The event argument contains the center point of the tray icon in physical screen pixels.
    /// </remarks>
    public event EventHandler<MouseEventReceivedEventArgs>? RightDoubleClicked;
}

/// <summary>
/// Provides the screen point associated with a tray icon mouse event.
/// </summary>
/// <remarks>
/// The point is the center of the tray icon in physical screen pixels.
/// </remarks>
public class MouseEventReceivedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the screen point associated with the mouse event.
    /// </summary>
    /// <value>The screen point in physical pixels.</value>
    public System.Drawing.Point Point { get; }

    internal MouseEventReceivedEventArgs(System.Drawing.Point point)
    {
        Point = point;
    }
}
#endif