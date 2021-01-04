using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace RawInput
{
    /// <summary>
    /// Listens to low level input and invokes event when triggering
    /// </summary>
    public class LowLevelKeyboardListener
    {
        //Id for hook procedure that installs a hook procedure that monitors low-level keyboard input events.
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYUP = 0x0105;

        private const int SCARY_CRASH_NUMBER_0 = 202;
        private const int SCARY_CRASH_NUMBER_1 = 216;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr IParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string IpModuleName);

        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);

        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr IParam);

        /// <summary>
        /// Event invoked on any Key Down
        /// </summary>
        public event EventHandler<KeyPressedArgs> OnKeyDown;
        /// <summary>
        /// Event invoked on any Key Up
        /// </summary>
        public event EventHandler<KeyPressedArgs> OnKeyUp;

        //Delegate called when low level input is detected
        private LowLevelKeyboardProc _proc;
        //Pointer at hook
        private IntPtr _hookID = IntPtr.Zero;

        public LowLevelKeyboardListener()
        {
            _proc = HookCallback;
        }

        /// <summary>
        /// Destructor that unhooks
        /// </summary>
        ~LowLevelKeyboardListener()
        {
            UnHookKeyboard();
        }

        /// <summary>
        /// Install hook that listens to input on low level
        /// </summary>
        public void HookKeyboard()
        {
            //Only hook if there isn't already a hook already
            if (_hookID == IntPtr.Zero)
                _hookID = SetHook(_proc);
        }

        /// <summary>
        /// Remove hook that listens to low level input
        /// </summary>
        public void UnHookKeyboard()
        {
            if (_hookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookID);
                _hookID = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Installs hook that invokes proc when low level input occour.
        /// </summary>
        /// <param name="proc">Delegate to callback function</param>
        /// <returns>IntPtr for that hook</returns>
        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            //using (Process currentProcess = Process.GetCurrentProcess())
            //using (ProcessModule currentModule = currentProcess.MainModule)
            //{
            //    return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(currentModule.ModuleName), 0);
            //}

            IntPtr hInstance = LoadLibrary("User32");
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc, hInstance, 0);
        }

        /// <summary>
        /// Gets called every time a key is pressed. Callback for WindowsHook for keyboard inputs
        /// </summary>
        /// <param name="nCode">Hook procedure code for how to process the message. If the code is less than zero -> pass to CallNextHookEx with no processing, return CallNextHookEx value. 0 = wParam and IParam hold info about keystroke. 3 = holds info and keystroke message is not removed from message queue</param>
        /// <param name="wParam">Virtual-key code that generated the keystroke message</param>
        /// <param name="IParam">The repeat count, scan code, extended-key flag, context code, previous key-state flag, and transition-state flag. 0-15 -> repeat Count. 16-23 -> scan code (OEM dependant). 24 -> extended key? 1 if it is. 25-28 -> reserved. 29 -> context code (1 if ALT is pressed down). 30 -> previous key State (0 if key is up). 31 -> transition state. The value is 1 if it is being released</param>
        /// <returns></returns>
        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr IParam)
        {
            //Contains valid data
            if (nCode >= 0)
            {
                //Convert data to correct virtual key code
                int virtualKeyCode = Marshal.ReadInt32(IParam);
                //Avoid scary numbers that makes program crash. What are they? Who knows but they scare me. 
                //Are there more out there? Are we doomed? Like probably but not sure what to do besides this. (:
                if (!(virtualKeyCode == SCARY_CRASH_NUMBER_0 || virtualKeyCode == SCARY_CRASH_NUMBER_1))
                {
                    //Key Up
                    if (wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP)
                    {
                        //Invoke outer event that keypressed happened with key data
                        OnKeyUp?.Invoke(this, new KeyPressedArgs((Key)virtualKeyCode));
                    }
                    //Key Down
                    else if (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN)
                    {
                        //Invoke outer event that keypressed happened with key data
                        OnKeyDown?.Invoke(this, new KeyPressedArgs((Key)virtualKeyCode));
                    }
                }
                //COMMENT OUT THIS LATER TESTING ONLY
                else
                    UnityEngine.Debug.Log("Illegal input: " + virtualKeyCode);
                //COMMENT OUT THIS LATER TESTING ONLY
            }
            //Pass along data to other hooks
            return CallNextHookEx(_hookID, nCode, wParam, IParam);
        }
    }

    /// <summary>
    /// Holds Input data
    /// </summary>
    public class KeyPressedArgs : EventArgs
    {
        /// <summary>
        /// What key was pressed
        /// </summary>
        public Key KeyPressed { get; private set; }

        public KeyPressedArgs(Key key)
        {
            KeyPressed = key;
        }
    }
}