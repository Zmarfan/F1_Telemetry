using UnityEngine;

namespace RawInput
{
    public class RawInputManager : MonoBehaviour
    {
        LowLevelKeyboardListener _inputListener;

        private void Awake()
        {
            _inputListener = new LowLevelKeyboardListener();
            _inputListener.OnKeyPressed += InputOccour;
            _inputListener.HookKeyboard();
        }

        private void OnApplicationQuit()
        {
            _inputListener.UnHookKeyBoard();
        }

        void InputOccour(object sender, KeyPressedArgs argument)
        {
            Debug.Log(argument.KeyPressed.ToString());
        }
    }
}