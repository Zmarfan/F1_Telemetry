using UnityEngine;
using F1_Data_Management;

namespace F1_Unity
{
    public delegate void EventOverDelegate();

    public class EventBase : MonoBehaviour
    {
        [Header("Settings")]

        [SerializeField, Range(0.01f, 10f)] float _showTime = 8f;
        [SerializeField, Range(0.01f, 10f)] float _fadeTime = 0.5f;

        public event EventOverDelegate EventOver;

        CanvasGroup _canvasGroup;
        bool _eventStarted = false;
        Timer _fadeTimer;
        bool _fadingIn = true;
        Timer _showTimer;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();

            _fadeTimer = new Timer(_fadeTime);
            _showTimer = new Timer(_showTime);
        }

        public virtual void Init(Packet packet)
        {
            //This should never be possible but here as a safe guard
            if (!Participants.ReadyToReadFrom)
                End();

            //Correct position relative to anchor
            GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        }

        /// <summary>
        /// Starts the event of and is called from EventManager when the event should trigger.
        /// </summary>
        public virtual void Begin()
        {
            _eventStarted = true;
        }

        /// <summary>
        /// Called when event is over. Signals outward that event is over.
        /// </summary>
        public virtual void End()
        {
            EventOver?.Invoke();
            Destroy(this.gameObject);
        }

        private void Update()
        {
            if (_eventStarted)
                Showing();
        }

        /// <summary>
        /// Controls how eventPrefab fades in and out and then removes
        /// </summary>
        void Showing()
        {
            _fadeTimer.Time += Time.deltaTime;

            if (!_fadeTimer.Expired())
            {
                //Fade in from black
                if (_fadingIn)
                    _canvasGroup.alpha = _fadeTimer.Ratio();
                //Fade out to black
                else
                    _canvasGroup.alpha = _fadeTimer.InverseRatio();
            }
            //Destroy when faded out
            else if (!_fadingIn)
                End();
            //Have up for a bit of time
            else
            {
                _showTimer.Time += Time.deltaTime;

                if (_showTimer.Expired())
                {
                    _fadingIn = false;
                    _fadeTimer.Reset();
                }
            }
        }
    }
}