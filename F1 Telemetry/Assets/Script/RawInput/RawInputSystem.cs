using System.Linq;
using System.Collections.Generic;
using System;

namespace RawInput
{
    /// <summary>
    /// Delegate for raw input events -> down or up
    /// </summary>
    public delegate void RawInputDelegate();

    /// <summary>
    /// Main point of access to rawInput system
    /// </summary>
    public class RawInputSystem
    {
        /// <summary>
        /// The listeners that listens to low level input events
        /// </summary>
        LowLevelKeyboardListener _inputListener;

        //Maps Keys to KeyEventInstances that trigger events for each key -> subscribe to keys you want
        Dictionary<Key, KeyEventInstance> _keyStatus = new Dictionary<Key, KeyEventInstance>();

        public RawInputSystem()
        {
            Init();
        }

        ~RawInputSystem()
        {
            StopListening();
        }

        #region Public Methods

        /// <summary>
        /// Starts up the listener
        /// </summary>
        public void BeginListening()
        {
            _inputListener = new LowLevelKeyboardListener();
            _inputListener.OnKeyDown += KeyDown;
            _inputListener.OnKeyUp += KeyUp;
            _inputListener.HookKeyboard();
        }

        /// <summary>
        /// Stops listening on low level -> will be called in destructor 
        /// </summary>
        public void StopListening()
        {
            if (_inputListener != null)
            {
                _inputListener.UnHookKeyBoard();
                //Clear Key values
                List<KeyEventInstance> instances = _keyStatus.Values.ToList();
                for (int i = 0; i < instances.Count; i++)
                    instances[i].Clear();
            }
        }

        /// <summary>
        /// Listen for input on specific key.
        /// </summary>
        /// <param name="key">What key do you want to listen to?</param>
        /// <param name="listenMethodKeyDown">On Keydown method to be called</param>
        /// <param name="listenMethodKeyUp">On Keyup method to be called</param>
        public void SubscribeToKeyEvent(Key key, RawInputDelegate listenMethodKeyDown, RawInputDelegate listenMethodKeyUp)
        {
            //Checks so key that user is trying to subsribe to exist
            if (_keyStatus.ContainsKey(key))
            {
                //Subscribe to this key's specific input events
                _keyStatus[key].KeyDown += listenMethodKeyDown;
                _keyStatus[key].KeyUp += listenMethodKeyUp;
            }
            else
                throw new System.Exception("There is no key event instance support for this key: " + key);
        }

        /// <summary>
        /// Is this key currently being pressed down?
        /// </summary>
        /// <param name="key">What key</param>
        /// <returns>true if being pressed down</returns>
        public bool IsKeyDown(Key key)
        {
            if (_keyStatus.ContainsKey(key))
                return _keyStatus[key].IsCurrentlyPressedDown;
            throw new System.Exception("There is no key event instance support for this key: " + key);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initilized key dictionary
        /// </summary>
        void Init()
        {
            Array allKeys = Enum.GetValues(typeof(Key));
            foreach (Key key in allKeys)
                _keyStatus.Add(key, new KeyEventInstance(key));
        }

        /// <summary>
        /// KeyDown event occoured on Lower level input. 
        /// </summary>
        /// <param name="sender">LowerLevelKeyboardListener instance</param>
        /// <param name="argument">The key data</param>
        void KeyDown(object sender, KeyPressedArgs argument)
        {
            //Should always be true -> Invokes event for KeyDown for that specific key
            if (_keyStatus.ContainsKey(argument.KeyPressed))
                _keyStatus[argument.KeyPressed].KeyDownEvent();
            else
                throw new System.Exception("There is no key event instance support for this key: " + argument.KeyPressed);
        }

        /// <summary>
        /// KeyUp event occoured on Lower level input. 
        /// </summary>
        /// <param name="sender">LowerLevelKeyboardListener instance</param>
        /// <param name="argument">The key data</param>
        void KeyUp(object sender, KeyPressedArgs argument)
        {
            //Should always be true -> Invokes event for KeyUp for that specific key
            if (_keyStatus.ContainsKey(argument.KeyPressed))
                _keyStatus[argument.KeyPressed].KeyUpEvent();
            else
                throw new System.Exception("There is no key event instance support for this key: " + argument.KeyPressed);
        }

        #endregion
    }
}