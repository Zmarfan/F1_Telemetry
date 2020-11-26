using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;

namespace F1_Unity
{
    public class DriverTemplate : MonoBehaviour
    {
        [Header("Settings")]

        [SerializeField] AnimationCurve _changeBackColorCurve;
        [SerializeField] Color _outDarkColor;
        [SerializeField] Color _outLightColor;
        [SerializeField] Vector3 _driverHolderInPosition;
        [SerializeField] Vector3 _driverHolderOutPosition;
        [SerializeField] string _dnfString;
        [SerializeField] string _dsqString;

        [Header("Drop")]

        [SerializeField] Transform _positionTransform;
        [SerializeField] RectTransform _driverHolder;
        [SerializeField] Image _positionImage;  //The white image under position number -> flashes red/green during overtakes
        [SerializeField] Text _positionText;    //Only used on init on Awake, never changes
        [SerializeField] Transform _fastestLap; //transform for fastest lap image, deactivated on default
        [SerializeField] Image _teamColorImage;
        [SerializeField] Text _initialsText;
        [SerializeField] Text _timeTextLeader;        //Time text against leader
        [SerializeField] Text _timeTextInterval;      //Time text against leader

        [SerializeField] Image _darkBackground;
        [SerializeField] Image _lightBackground;

        Color _darkBackgroundColor;
        Color _lightBackgroundColor;
        Color _currentFromColorDark;
        Color _currentFromColorLight;

        public bool IsActive { get; private set; }
        public bool OutOfSession { get; private set; }
        public DriverTimeState TimeState { get; private set; } = DriverTimeState.Starting;
        public int LapsLapped { get; private set; }
        public float DeltaToLeader { get; private set; }
        public float DeltaToCarInFront { get; private set; }

        int _position = 0;         //The position of this template -> static never changes
        Timer _overtakeColorTimer;         //Timer for how long the positionImage shall flash
        Timer _colorTimer;         //Timer for how long the color after event should last
        bool _resetColor = false;

        private void Update()
        {
            if (!OutOfSession)
            {
                UpdateOvertakeColor();
                UpdateColor();
            }
        }

        /// <summary>
        /// Makes sure color goes back to white after set time
        /// </summary>
        void UpdateOvertakeColor()
        {
            //colorTimer needs to be created from TimingScreen before we can use it
            if (_overtakeColorTimer != null && _resetColor)
            {
                _overtakeColorTimer.Time += Time.deltaTime;

                if (_overtakeColorTimer.Expired())
                {
                    _resetColor = false;
                    _overtakeColorTimer.Reset();
                    _positionImage.color = Color.white;
                }
            }
        }

        /// <summary>
        /// Lerps the color back to original after being changed
        /// </summary>
        void UpdateColor()
        {
            //colorTimer needs to be created from TimingScreen before we can use it
            if (_colorTimer != null && (_darkBackground.color != _darkBackgroundColor || _lightBackground.color != _lightBackgroundColor))
            {
                _colorTimer.Time += Time.deltaTime;

                _darkBackground.color = Color.Lerp(_currentFromColorDark, _darkBackgroundColor, _changeBackColorCurve.Evaluate(_colorTimer.Ratio()));
                _lightBackground.color = Color.Lerp(_currentFromColorLight, _lightBackgroundColor, _changeBackColorCurve.Evaluate(_colorTimer.Ratio()));

                if (_colorTimer.Expired())
                    _colorTimer.Reset();
            }
        }

        /// <summary>
        /// Called when first creating the template. Sets position based on index.
        /// </summary>
        public void Init(int initPosition, float overtakeColorDuration, float colorDuration)
        {
            OutOfSession = false;

            _position = initPosition;
            _overtakeColorTimer = new Timer(overtakeColorDuration);
            _colorTimer = new Timer(colorDuration);
            _positionText.text = _position.ToString();

            _darkBackgroundColor = _darkBackground.color;
            _lightBackgroundColor = _lightBackground.color;
        }

        /// <summary>
        /// Sets to show correct time standing, interval or to leader
        /// </summary>
        public void SetMode(bool interval)
        {
            _timeTextInterval.enabled = interval;
            _timeTextLeader.enabled = !interval;
        }

        /// <summary>
        /// Only called on start of a race
        /// </summary>
        public void SetActive(bool state)
        {
            IsActive = state;
            transform.gameObject.SetActive(state);
        }

        /// <summary>
        /// Activate/Deactivate fastest lap symbol next to player in timestandings
        /// </summary>
        public void SetFastestLap(bool state)
        {
            _fastestLap.gameObject.SetActive(state);
        }

        /// <summary>
        /// Called when a overtake just happened. Depending on if driver went up or down the color differ
        /// </summary>
        /// <param name="oldPosition"></param>
        public void UpdatePositionColor(int oldPosition, Color movedUpColor, Color movedDownColor)
        {
            Color color = oldPosition < _position ? movedDownColor : movedUpColor;
            _positionImage.color = color;
            _resetColor = true;
        }

        /// <summary>
        /// Updates values for time keeping
        /// </summary>
        public void UpdateTimingValues(DriverTimeState state, float deltaToLeader = 0, int lapsLapped = 0)
        {
            TimeState = state;
            DeltaToLeader = deltaToLeader;
            LapsLapped = lapsLapped;
        }

        /// <summary>
        /// Calculates and sets car in front delta
        /// </summary>
        public void SetCarAheadDelta(float carAheadTimeToLeader)
        {
            DeltaToCarInFront = DeltaToLeader - carAheadTimeToLeader;
        }

        /// <summary>
        /// Updates timing state
        /// </summary>
        /// <param name="state">What state is driver currently in compared with leader</param>
        /// <param name="time">time to leader in seconds</param>
        /// <param name="laps">laps lapped compared with leader</param>
        public void SetTiming()
        {
            if (OutOfSession)
                return;

            switch (TimeState)
            {
                case DriverTimeState.Leader:
                    {
                        _timeTextLeader.text = "Leader";
                        _timeTextInterval.text = "Interval";
                        break;
                    } 
                case DriverTimeState.Lapped:
                    {
                        string text = "+" + LapsLapped + " LAP";
                        _timeTextLeader.text = text;
                        _timeTextInterval.text = text;
                        break;
                    }
                case DriverTimeState.Delta:
                    {
                        _timeTextLeader.text = GetDeltaString(DeltaToLeader);
                        _timeTextInterval.text = GetDeltaString(DeltaToCarInFront);
                        break;
                    }
                case DriverTimeState.Starting:
                    {
                        _timeTextLeader.text = "-";
                        _timeTextInterval.text = "-";
                        break;
                    }
                default: { throw new Exception("UpdateTimingState has been called with a DriverTimeState not yet implemented here!"); }
            }
        }

        /// <summary>
        /// Sets the team color to color
        /// </summary>
        public void SetTeamColor(F1_Data_Management.Color color)
        {
            _teamColorImage.color = new Color(color.r, color.g, color.b, color.a);
        }

        /// <summary>
        /// Sets the 3 letter initials for driver in time standings
        /// </summary>
        public void SetInitials(string initials)
        {
            _initialsText.text = initials;
        }

        /// <summary>
        /// Sets new color on driver when event occour
        /// </summary>
        public void SetColor(Color color)
        {
            _colorTimer.Reset();

            color.a = _darkBackgroundColor.a;
            _darkBackground.color = color;
            _currentFromColorDark = color;
            color.a = _lightBackgroundColor.a;
            _lightBackground.color = color;
            _currentFromColorLight = color;
        }

        /// <summary>
        /// Called when switching from out state to normal state
        /// </summary>
        public void NotOut()
        {
            OutOfSession = false;
            _darkBackground.color = _darkBackgroundColor;
            _lightBackground.color = _lightBackgroundColor;

            _currentFromColorDark = _darkBackgroundColor;
            _currentFromColorLight = _lightBackgroundColor;

            _positionTransform.gameObject.SetActive(true);
            _driverHolder.anchoredPosition = _driverHolderInPosition;

            SetTiming();
        }

        /// <summary>
        /// Called when the car is out of the session (DNF, DSQ) -> grays it out, rearrange and won't do timing no more
        /// </summary>
        public void Out(F1_Data_Management.ResultStatus status)
        {
            OutOfSession = true;

            _driverHolder.anchoredPosition = _driverHolderOutPosition;
            _darkBackground.color = _outDarkColor;
            _lightBackground.color = _outLightColor;
            _positionImage.color = Color.white;
            _positionTransform.gameObject.SetActive(false);

            if (status == F1_Data_Management.ResultStatus.Retired)
            {
                _timeTextLeader.text = _dnfString;
                _timeTextInterval.text = _dnfString;
            }
            if (status == F1_Data_Management.ResultStatus.Disqualified)
            {
                _timeTextLeader.text = _dsqString;
                _timeTextInterval.text = _dsqString;
            }
        }

        /// <summary>
        /// Converts seconds to +minute:seconds:millieseconds
        /// </summary>
        static string GetDeltaString(float time)
        {
            TimeSpan span = TimeSpan.FromSeconds(time);
            StringBuilder builder = new StringBuilder();
            builder.Append('+');
            if (span.Minutes > 0)
            {
                builder.Append(span.Minutes);
                builder.Append(':');
                builder.Append(span.Seconds.ToString("0#")); //Start with zero if one digit long
            }
            else
                builder.Append(span.Seconds);

            builder.Append('.');
            builder.Append(span.Milliseconds.ToString("000")); //Appends with 3 decimals

            return builder.ToString();
        }
    }

    /// <summary>
    /// What state a driver is in compared to leader timing wise
    /// </summary>
    public enum DriverTimeState
    {
        Leader,
        Lapped,
        Delta,
        Starting,
        DNF,
        DSQ
    }
}
