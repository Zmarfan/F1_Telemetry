namespace RawInput
{
    /// <summary>
    /// Converts raw input event for a specific key to key specific input events.
    /// <para>Holds events that trigger for input for a specific key. Can check if currently being pressed down.</para>
    /// </summary>
    [System.Serializable]
    public class KeyEventInstance
    {
        /// <summary>
        /// What key is this keyEventInstance for
        /// </summary>
        public Key _key { get; private set; }
        /// <summary>
        /// Current status of key. true = held down, false = not active
        /// </summary>
        public bool IsCurrentlyPressedDown { get; private set; }

        /// <summary>
        /// Invoked when key is pressed down. Triggers once if key is held down.
        /// </summary>
        public event RawInputDelegate KeyDown;
        /// <summary>
        /// Invoked when key is lifted up. Triggers once.
        /// </summary>
        public event RawInputDelegate KeyUp;

        public KeyEventInstance(Key key)
        {
            this._key = key;
        }

        /// <summary>
        /// Keydown event for this key is invoked
        /// </summary>
        public void KeyDownEvent(Key key)
        {
            //If it is pressed down from being up -> invoke KeyDown Event
            if (!IsCurrentlyPressedDown)
            {
                KeyDown?.Invoke(key);
                IsCurrentlyPressedDown = true;
            }
        }

        /// <summary>
        /// KeyUp event for this key is invoked. Only happens if key previously was down.
        /// </summary>
        public void KeyUpEvent(Key key)
        {
            KeyUp?.Invoke(key);
            IsCurrentlyPressedDown = false;
        }

        /// <summary>
        /// Resets saved data
        /// </summary>
        public void Clear()
        {
            IsCurrentlyPressedDown = false;
        }
    }
}