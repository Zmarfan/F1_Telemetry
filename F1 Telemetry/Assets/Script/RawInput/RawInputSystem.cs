using System.Linq;
using System.Collections.Generic;
using System;

namespace RawInput
{
    /// <summary>
    /// Delegate for raw input events -> down or up
    /// </summary>
    public delegate void RawInputDelegate(Key key);

    /// <summary>
    /// Main point of access to rawInput system
    /// </summary>
    public class RawInputSystem
    {
        /// <summary>
        /// Specifies if input is locked, toggled by pressing lockKey (lockKey is never locked)
        /// </summary>
        public bool InputLock { get; private set; }

        /// <summary>
        /// The listeners that listens to low level input events
        /// </summary>
        LowLevelKeyboardListener _inputListener;

        //Maps Keys to KeyEventInstances that trigger events for each key -> subscribe to keys you want
        Dictionary<Key, KeyEventInstance> _keyStatus = new Dictionary<Key, KeyEventInstance>();

        KeyEventInstance _lockKeyInstance;

        /// <summary>
        /// Creates new instance of RawInputSystem
        /// </summary>
        /// <param name="lockKey">The key that locks all input except this key when pressed (toggle). This key can't hold other inputs</param>
        public RawInputSystem(Key lockKey)
        {
            _lockKeyInstance = new KeyEventInstance(lockKey);
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
                _inputListener.UnHookKeyboard();
                //Clear Key values
                List<KeyEventInstance> instances = _keyStatus.Values.ToList();
                for (int i = 0; i < instances.Count; i++)
                    instances[i].Clear();
            }
        }

        #region Subscribe

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
        /// Listen for input on specific key down.
        /// </summary>
        /// <param name="key">What key do you want to listen to?</param>
        /// <param name="listenMethodKeyDown">On Keydown method to be called</param>
        public void SubscribeToKeyEventDown(Key key, RawInputDelegate listenMethodKeyDown)
        {
            //Checks so key that user is trying to subsribe to exist
            if (_keyStatus.ContainsKey(key))
            {
                //Subscribe to this key's specific input events
                _keyStatus[key].KeyDown += listenMethodKeyDown;
            }
            else
                throw new System.Exception("There is no key event instance support for this key: " + key);
        }

        /// <summary>
        /// Listen for input on specific key up.
        /// </summary>
        /// <param name="key">What key do you want to listen to?</param>
        /// <param name="listenMethodKeyDown">On Keyup method to be called</param>
        public void SubscribeToKeyEventUp(Key key, RawInputDelegate listenMethodKeyUp)
        {
            //Checks so key that user is trying to subsribe to exist
            if (_keyStatus.ContainsKey(key))
            {
                //Subscribe to this key's specific input events
                _keyStatus[key].KeyUp += listenMethodKeyUp;
            }
            else
                throw new System.Exception("There is no key event instance support for this key: " + key);
        }

        #endregion

        #region Unsubscribe

        /// <summary>
        /// Stop listen for input on specific key.
        /// </summary>
        /// <param name="key">What key do you want to stop listening to?</param>
        /// <param name="listenMethodKeyDown">On Keydown method to stop be called</param>
        /// <param name="listenMethodKeyUp">On Keyup method to stop be called</param>
        public void UnsubscribeToKeyEvent(Key key, RawInputDelegate listenMethodKeyDown, RawInputDelegate listenMethodKeyUp)
        {
            //Checks so key that user is trying to subsribe to exist
            if (_keyStatus.ContainsKey(key))
            {
                //Subscribe to this key's specific input events
                _keyStatus[key].KeyDown -= listenMethodKeyDown;
                _keyStatus[key].KeyUp -= listenMethodKeyUp;
            }
            else
                throw new System.Exception("There is no key event instance support for this key: " + key);
        }

        /// <summary>
        /// Stop listen for input on specific key for key down.
        /// </summary>
        /// <param name="key">What key do you want to stop listening to?</param>
        /// <param name="listenMethodKeyDown">On Keydown method to stop be called</param>
        public void UnsubscribeToKeyEventDown(Key key, RawInputDelegate listenMethodKeyDown)
        {
            //Checks so key that user is trying to subsribe to exist
            if (_keyStatus.ContainsKey(key))
            {
                //Subscribe to this key's specific input events
                _keyStatus[key].KeyDown -= listenMethodKeyDown;
            }
            else
                throw new System.Exception("There is no key event instance support for this key: " + key);
        }

        /// <summary>
        /// Stop listen for input on specific key for key up.
        /// </summary>
        /// <param name="key">What key do you want to stop listening to?</param>
        /// <param name="listenMethodKeyUp">On Keyup method to stop be called</param>
        public void UnsubscribeToKeyEventUp(Key key, RawInputDelegate listenMethodKeyUp)
        {
            //Checks so key that user is trying to subsribe to exist
            if (_keyStatus.ContainsKey(key))
            {
                //Subscribe to this key's specific input events
                _keyStatus[key].KeyUp -= listenMethodKeyUp;
            }
            else
                throw new System.Exception("There is no key event instance support for this key: " + key);
        }

        #endregion

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
            //Toggle Lock for input
            if (_lockKeyInstance.Key == argument.KeyPressed)
                InputLock = !InputLock;
            //Listen for normal input if lock is not activated
            else if (!InputLock)
            {
                //Should always be true -> Invokes event for KeyDown for that specific key
                if (_keyStatus.ContainsKey(argument.KeyPressed))
                    _keyStatus[argument.KeyPressed].KeyDownEvent(argument.KeyPressed);
                else
                    throw new System.Exception("There is no key event instance support for this key: " + argument.KeyPressed);
            }  
        }

        /// <summary>
        /// KeyUp event occoured on Lower level input. 
        /// </summary>
        /// <param name="sender">LowerLevelKeyboardListener instance</param>
        /// <param name="argument">The key data</param>
        void KeyUp(object sender, KeyPressedArgs argument)
        {
            //Ignore lock key key up
            if (_lockKeyInstance.Key == argument.KeyPressed)
                return;

            //Lock all input if lock is activated
            if (!InputLock)
            {
                //Should always be true -> Invokes event for KeyUp for that specific key
                if (_keyStatus.ContainsKey(argument.KeyPressed))
                    _keyStatus[argument.KeyPressed].KeyUpEvent(argument.KeyPressed);
                else
                    throw new System.Exception("There is no key event instance support for this key: " + argument.KeyPressed);
            }  
        }

        #endregion
    }
}