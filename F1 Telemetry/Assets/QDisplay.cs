using UnityEngine.UI;
using UnityEngine;
using F1_Data_Management;
using System;
using System.Text;

namespace F1_Unity
{
    /// <summary>
    /// Display an estimation of how much time is left in session depending on type
    /// </summary>
    public class QDisplay : MonoBehaviour
    {
        [SerializeField] float _updateFrequency = 0.5f;
        [SerializeField] string _shortQText = "Q";
        [SerializeField] string _shortPText = "P";
        [SerializeField] Text _displayText;
        [SerializeField] Text _sessionTypeText;

        //In seconds
        readonly static float P1_P2_P3_LENGTH = 5400f;
        readonly static float Q1_LENGTH = 1080f;
        readonly static float Q2_LENGTH = 900f;
        readonly static float Q3_LENGTH = 720f;
        readonly static float SHORT_Q_LENGTH = 1080f;

        float _timePassed = 0;
        float _sessionMaxTime;
        //For short P practice time is unknown so count up instead
        bool _countUp = false;
        float _lastSessionStamp;
        float _lastUnityTime;

        Timer _updateFrequencyTimer;

        private void Awake()
        {
            _updateFrequencyTimer = new Timer(_updateFrequency);
        }

        private void Start()
        {
            Session sessionData = GameManager.F1Info.ReadSession(out bool status);
            if (status)
            {
                _lastSessionStamp = GameManager.F1Info.SessionTime;
                _lastUnityTime = Time.time;

                SessionType type = sessionData.SessionType;
                switch (type)
                {
                    case SessionType.Short_P:    { _sessionTypeText.text = _shortPText;      _countUp = true;                  break; }
                    case SessionType.P1:         { _sessionTypeText.text = type.ToString(); _sessionMaxTime = P1_P2_P3_LENGTH; break; }
                    case SessionType.P2:         { _sessionTypeText.text = type.ToString(); _sessionMaxTime = P1_P2_P3_LENGTH; break; }
                    case SessionType.P3:         { _sessionTypeText.text = type.ToString(); _sessionMaxTime = P1_P2_P3_LENGTH; break; }
                    case SessionType.Q1:         { _sessionTypeText.text = type.ToString(); _sessionMaxTime = Q1_LENGTH;       break; }
                    case SessionType.Q2:         { _sessionTypeText.text = type.ToString(); _sessionMaxTime = Q2_LENGTH;       break; }
                    case SessionType.Q3:         { _sessionTypeText.text = type.ToString(); _sessionMaxTime = Q3_LENGTH;       break; }
                    case SessionType.Short_Q:    { _sessionTypeText.text = _shortQText;     _sessionMaxTime = SHORT_Q_LENGTH;  break; }
                    default:
                        throw new System.Exception("There is currently no implementation for this session type: " + type);
                }
            }
        }

        private void Update()
        {
            _updateFrequencyTimer.Time += Time.deltaTime;
            if (_updateFrequencyTimer.Expired())
            {
                _updateFrequencyTimer.Reset();
                UpdateDisplay();
            }
        }

        /// <summary>
        /// Calculates estimated session time of start timestamp and current timestamp + session type
        /// </summary>
        void UpdateDisplay()
        {
            //Only keep counting if new data keeps coming in (stops when game is paused -> doesn't matter for multiplayer but better for singleplayer)
            if (GameManager.F1Info.SessionTime != _lastSessionStamp)
            {
                _timePassed += Time.time - _lastUnityTime;

                _lastSessionStamp = GameManager.F1Info.SessionTime;
                _lastUnityTime = Time.time;

                if (_countUp)
                    SetDisplayText(_timePassed);
                else
                {
                    float displayTimeSeconds = _sessionMaxTime - _timePassed;
                    SetDisplayText(Mathf.Clamp(displayTimeSeconds, 0, _sessionMaxTime));
                }
            }
            //Set new reference each frequency update to fix time offset for pauses
            else
                _lastUnityTime = Time.time;
        }

        /// <summary>
        /// Sets the display text for Q session.
        /// </summary>
        /// <param name="time">Time in seconds -> will be converted to min:sec</param>
        void SetDisplayText(float time)
        {
            TimeSpan span = TimeSpan.FromSeconds(time);
            StringBuilder builder = new StringBuilder();
            builder.Append(span.Minutes.ToString("00:"));
            builder.Append(span.Seconds.ToString("00"));
            _displayText.text = builder.ToString();
        }
    }
}