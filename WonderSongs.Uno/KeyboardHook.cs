#if WINAPPSDK_PACKAGED
using WinWrapper;

namespace WonderSongs;

partial class KeyboardHook
{
    public static KeyboardHook Instance { get; } = new();
    private KeyboardHook()
    {
        LowLevelKeyboard.KeyPressed += LowLevelKeyboard_KeyPressed;
    }
    public void Initialize()
    {
        // do nothing, making sure constructor is invoked
    }
    public event Func<bool>? ModifierPressed;
#if UNPKG
    public const string HOTKEY_MAIN = "R-ALT";
    public const WinWrapper.Input.VirtualKey HOTKEY_MAIN_VK = WinWrapper.Input.VirtualKey.RMENU;
#else
    public const string HOTKEY_MAIN = "R-ALT";
    public const WinWrapper.Input.VirtualKey HOTKEY_MAIN_VK = WinWrapper.Input.VirtualKey.RMENU;
    //public const string HOTKEY_MAIN = "R-CTRL";
    //public const WinWrapper.Input.VirtualKey HOTKEY_MAIN_VK = WinWrapper.Input.VirtualKey.RCONTROL;
#endif
    private void LowLevelKeyboard_KeyPressed(KeyboardHookInfo eventDetails, KeyboardState state, ref bool Handled)
    {
        bool isDown = state is KeyboardState.KeyDown or KeyboardState.SystemKeyDown;
        if (isDown && eventDetails.KeyCode == HOTKEY_MAIN_VK)
        {
            Handled = ModifierPressed?.Invoke() ?? false;
        }
    }
}
#endif
