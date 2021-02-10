using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using F1_Data_Management;

namespace F1_Unity
{
    public class TimingScreenEntry : MonoBehaviour
    {
        [Header("Settings")]

        [SerializeField] bool _useColorIntervals = true;
        [SerializeField] Gradient _intervalColorGradient;
        /// <summary>
        /// How close the car must be to car in front to go into gradient for interval text color
        /// </summary>
        [SerializeField] float _intervalColorMaxDistance;
        [SerializeField] AnimationCurve _changeBackColorCurve;
        [SerializeField] float _outAlpha = 0.75f;
        [SerializeField] Vector3 _driverHolderInPosition;
        [SerializeField] Vector3 _driverHolderOutPosition;
        [SerializeField] Color _timingColor = Color.white;
        [SerializeField] Color _pittingColor = Color.cyan;
        [SerializeField] Color _fastestLapColor;
        [SerializeField] string _dnfString = "OUT";
        [SerializeField] string _dsqString = "DSQ";
        [SerializeField] string _startingString = "-";
        [SerializeField] string _noTimeString = "NO TIME";
        [SerializeField] string _outLapString = "OUT LAP";
        [SerializeField] string _inLapString = "IN LAP";
        [SerializeField] string _leaderString = "Leader";
        [SerializeField] string _intervalString = "Interval";
        [SerializeField] string _lappedString = "LAP";
        [SerializeField] string _pitString = "Pitting";
        [SerializeField] string _pitAreaString = "Pit";

        [Header("Drop")]

        [SerializeField] TimingStats _timingStats;
        [SerializeField] CanvasGroup _alphaHolder;

        [SerializeField] RectTransform _darkBackgroundRectTransform;
        [SerializeField] Transform _positionTransform;
        [SerializeField] RectTransform _driverHolder;
        [SerializeField] Image _positionImage;        //The white image under position number -> flashes red/green during overtakes
        [SerializeField] Text _positionText;          //Only used on init on Awake, never changes
        [SerializeField] Transform _fastestLap;       //transform for fastest lap image, deactivated on default
        [SerializeField] Transform _spectatorIcon;
        [SerializeField] Image _teamColorImage;
        [SerializeField] Text _nameText;
        [SerializeField] Text _timeTextLeader;        //Time text against leader
        [SerializeField] Text _timeTextInterval;      //Time text against guy ahead
        [SerializeField] Text _fastestLapText;        //Fastest lap for this guy

        [SerializeField] Image _darkBackground;
        [SerializeField] Image _lightBackground;

        Color _darkBackgroundColor;
        Color _lightBackgroundColor;
        Color _currentFromColorDark;
        Color _currentFromColorLight;


        public DriverData DriverData { get; private set; }
        public bool IsActive { get; private set; }
        public bool OutOfSession { get; private set; }
        public DriverTimeState TimeState { get; private set; } = DriverTimeState.Starting;
        public int LapsLapped { get; private set; }
        public float DeltaToLeader { get; private set; }
        public float DeltaToCarInFront { get; private set; }
        public bool InPit { get; private set; }

        public string CurrentDelta { get { return _timeTextInterval.text; } }

        int _position = 0;           //The position of this template -> static never changes
        Timer _overtakeColorTimer;   //Timer for how long the positionImage shall flash
        Timer _colorTimer;           //Timer for how long the color after event should last
        bool _resetColor = false;

        bool _initialsMode;

        private void Update()
        {
            if (!OutOfSession)
            {
                UpdateOvertakeColor();
                UpdateColor();
            }
        }

        /// <summary>
        /// Return this entries color status (used to copy between entries)
        /// </summary>
        public void GetColorValues(out Color currentFromDarkColor, out Color currentFromLightColor, out Color currentDarkColor, out Color currentLightColor, out float colorTime)
        {
            currentFromDarkColor = _currentFromColorDark;
            currentFromLightColor = _currentFromColorLight;
            currentDarkColor = _darkBackground.color;
            currentLightColor = _lightBackground.color;
            colorTime = _colorTimer.Time;
        }

        /// <summary>
        /// Set Color values (used to copy between entries)
        /// </summary>
        public void SetColorValues(Color currentFromDarkColor, Color currentFromLightColor, Color currentDarkColor, Color currentLightColor, float colorTime)
        {
            _currentFromColorDark = currentFromDarkColor;
            _currentFromColorLight = currentFromLightColor;
            _darkBackground.color = currentDarkColor;
            _lightBackground.color = currentLightColor;
            _colorTimer.Reset();
            _colorTimer.Time = colorTime;
        }

        /// <summary>
        /// Sets the color of interval text to point on gradient based on how close to car ahead it is
        /// </summary>
        public void UpdateTimingColor()
        {
            ResultStatus rStatus = DriverData.LapData.resultStatus;
            //Set color for text while pitting
            if (InPit && !OutOfSession && rStatus != ResultStatus.Finished)
            {
                _timeTextLeader.color = _pittingColor;
                _timeTextInterval.color = _pittingColor;
            }
            //Default color for leader or DNF/DSQ or lapped or finished
            else if (_position == 1 || OutOfSession || rStatus == ResultStatus.Finished || TimeState == DriverTimeState.Lapped || TimeState == DriverTimeState.Starting)
            {
                _timeTextInterval.color = _intervalColorGradient.Evaluate(0);
                _timeTextLeader.color = _intervalColorGradient.Evaluate(0);
            }
            //Not leader, not pitting, not lapped
            else if (_useColorIntervals)
            {
                float point = 1 - Mathf.Clamp(DeltaToCarInFront / _intervalColorMaxDistance, 0, _intervalColorMaxDistance);
                _timeTextInterval.color = _intervalColorGradient.Evaluate(point);
                _timeTextLeader.color = _intervalColorGradient.Evaluate(point);
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
        public void SetMode(TimeScreenState timeScreenState)
        {
            switch (timeScreenState)
            {
                case TimeScreenState.None:
                    {
                        _timeTextLeader.gameObject.SetActive(false);
                        _timeTextInterval.gameObject.SetActive(false);
                        _fastestLapText.gameObject.SetActive(false);
                        break;
                    }
                case TimeScreenState.Leader:
                    {
                        _timeTextLeader.gameObject.SetActive(true);
                        _timeTextInterval.gameObject.SetActive(false);
                        _fastestLapText.gameObject.SetActive(false);
                        break;
                    }
                case TimeScreenState.Interval:
                    {
                        _timeTextLeader.gameObject.SetActive(false);
                        _timeTextInterval.gameObject.SetActive(true);
                        _fastestLapText.gameObject.SetActive(false);
                        break;
                    }
                case TimeScreenState.Fastest_Lap:
                    {
                        _timeTextLeader.gameObject.SetActive(false);
                        _timeTextInterval.gameObject.SetActive(false);
                        _fastestLapText.gameObject.SetActive(true);
                        break;
                    }
                default: { throw new Exception("Timing enum for changing state is not implemented properly: enum: " + timeScreenState); }
            }
        }

        /// <summary>
        /// Resize dark area for initial or fullname mode, save mode to write either initals or fullname
        /// </summary>
        /// <param name="initialsMode">true if initials mode</param>
        /// <param name="initialsWidth">Width of dark area in initials mode</param>
        /// <param name="fullNameWidth">Width of dark area in fullname mode</param>
        public void ChangeInitialMode(bool initialsMode, float initialsWidth, float fullNameWidth, int initialsFontSize, int fullNameFontSize)
        {
            _initialsMode = initialsMode;
            _darkBackgroundRectTransform.sizeDelta = new Vector2(_initialsMode ? initialsWidth : fullNameWidth, _darkBackgroundRectTransform.sizeDelta.y);
            _nameText.fontSize = _initialsMode ? initialsFontSize : fullNameFontSize;
            SetName(DriverData.RaceNumber);
        }

        /// <summary>
        /// Changes stats state to specific state
        /// </summary>
        public void SetStatsState(TimingStats.TimingStatsState state)
        {
            _timingStats.ChangeState(state);
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
        /// Updates drivers fastest lap
        /// </summary>
        /// <param name="driverData"></param>
        public void SetFastestLap(DriverData driverData)
        {
            if (driverData.LapData.bestLapTime == 0)
                _fastestLapText.text = _startingString;
            else
                //Set color and lap value for fastest lap for this driver
                _fastestLapText.text = F1Utility.GetDeltaString(driverData.LapData.bestLapTime);

            bool isFastestOverall = GameManager.LapManager.FastestLapTime == driverData.LapData.bestLapTime;
            _fastestLapText.color = isFastestOverall ? _fastestLapColor : _timingColor;
            _fastestLap.gameObject.SetActive(isFastestOverall);
        }

        /// <summary>
        /// Activates spectator icon for this driver if it is the one being spectated
        /// </summary>
        public void SetSpectator(DriverData driverData)
        {
            DriverData spectatorDriverData = GameManager.F1Info.ReadSpectatingCarData(out bool valid);
            _spectatorIcon.gameObject.SetActive(valid && driverData.ID == spectatorDriverData.ID);
        }

        /// <summary>
        /// Sets Drive Data
        /// </summary>
        public void SetDriverData(DriverData driverData) { DriverData = driverData; }
        /// <summary>
        /// Sets timing state
        /// </summary>
        public void SetTimingState(DriverTimeState state) { TimeState = state; }
        /// <summary>
        /// Sets delta to leader
        /// </summary>
        public void SetDeltaToLeader(float deltaToLeader) { DeltaToLeader = deltaToLeader; }
        /// <summary>
        /// Sets laps lapped to leader
        /// </summary>
        public void SetLapsLapped(int lapsLapped) { LapsLapped = lapsLapped; }
        /// <summary>
        /// Calculates and sets car in front delta
        /// </summary>
        public void SetCarAheadDelta(float carAheadTimeToLeader) { DeltaToCarInFront = DeltaToLeader - carAheadTimeToLeader; }
        /// <summary>
        /// Sets the car in pit or not
        /// </summary>
        public void SetInPit(bool inPit) { InPit = inPit; }

        /// <summary>
        /// Updates visual stats for this driver
        /// </summary>
        public void UpdateStats(DriverData driverData, TimingStats.TimingStatsState currentState)
        {
            _timingStats.UpdateValues(driverData, currentState);
        }

        /// <summary>
        /// Updates timing state
        /// </summary>
        /// <param name="state">What state is driver currently in compared with leader</param>
        /// <param name="time">time to leader in seconds</param>
        /// <param name="laps">laps lapped compared with leader</param>
        public void SetTiming()
        {
            UpdateTimingColor();

            if (OutOfSession)
                return;

            switch (TimeState)
            {
                //Shared between Q and Race
                case DriverTimeState.Delta:
                    {
                        _timeTextLeader.text = F1Utility.GetDeltaStringSigned(DeltaToLeader);
                        _timeTextInterval.text = F1Utility.GetDeltaStringSigned(DeltaToCarInFront);
                        break;
                    }
                //Q specific
                case DriverTimeState.Leader_Q:
                    {
                        _timeTextLeader.text = F1Utility.GetDeltaString(DriverData.LapData.bestLapTime);
                        _timeTextInterval.text = F1Utility.GetDeltaString(DriverData.LapData.bestLapTime);
                        break;
                    }
                case DriverTimeState.No_Time_Q:
                    {
                        _timeTextLeader.text = _noTimeString;
                        _timeTextInterval.text = _noTimeString;
                        break;
                    }
                case DriverTimeState.Out_Lap_Q:
                    {
                        _timeTextLeader.text = _outLapString;
                        _timeTextInterval.text = _outLapString;
                        break;
                    }
                case DriverTimeState.In_Lap_Q:
                    {
                        _timeTextLeader.text = _inLapString;
                        _timeTextInterval.text = _inLapString;
                        break;
                    }
                case DriverTimeState.Pit_Q: break; // Do nothing -> it will only change color when pitting but not the text
                //Race specific
                case DriverTimeState.Pit:
                    {
                        _timeTextLeader.text = _pitString;
                        _timeTextInterval.text = _pitString;
                        break;
                    }
                case DriverTimeState.Pit_Area:
                    {
                        _timeTextLeader.text = _pitAreaString;
                        _timeTextInterval.text = _pitAreaString;
                        break;
                    }
                case DriverTimeState.Leader:
                    {
                        _timeTextLeader.text = _leaderString;
                        _timeTextInterval.text = _intervalString;
                        break;
                    }
                case DriverTimeState.Lapped:
                    {
                        string text = "+" + LapsLapped + " " + _lappedString;
                        _timeTextLeader.text = text;
                        _timeTextInterval.text = text;
                        break;
                    }
                case DriverTimeState.Starting:
                    {
                        _timeTextLeader.text = _startingString;
                        _timeTextInterval.text = _startingString;
                        break;
                    }
                default: { throw new Exception("UpdateTimingState has been called with a DriverTimeState not yet implemented here!"); }
            }
        }

        /// <summary>
        /// Sets the team color to color
        /// </summary>
        public void SetTeamColor(Color color)
        {
            _teamColorImage.color = color;
        }

        /// <summary>
        /// Sets the 3 letter initials for driver in time standings
        /// </summary>
        public void SetName(byte raceNumber)
        {
            _nameText.text = _initialsMode ? GameManager.ParticipantManager.GetDriverInitials(raceNumber) : GameManager.ParticipantManager.GetNameFromNumber(raceNumber);
        }

        /// <summary>
        /// Sets new color on driver when event occour
        /// </summary>
        public void SetColor(Color color)
        {
            if (OutOfSession)
                return;

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
            _alphaHolder.alpha = 1.0f;

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
        public void Out(ResultStatus status)
        {
            OutOfSession = true;

            _alphaHolder.alpha = _outAlpha;

            _darkBackground.color = _darkBackgroundColor;
            _lightBackground.color = _lightBackgroundColor;

            _driverHolder.anchoredPosition = _driverHolderOutPosition;
            _positionImage.color = Color.white;
            _positionTransform.gameObject.SetActive(false);

            _timeTextLeader.color = _timingColor;

            if (status == ResultStatus.Retired)
            {
                _timeTextLeader.text = _dnfString;
                _timeTextInterval.text = _dnfString;
            }
            if (status == ResultStatus.Disqualified)
            {
                _timeTextLeader.text = _dsqString;
                _timeTextInterval.text = _dsqString;
            }
        }
    }

    /// <summary>
    /// What state a driver is in compared to leader timing wise
    /// </summary>
    public enum DriverTimeState
    {
        //Shared
        Delta,
        //Q
        Leader_Q,
        No_Time_Q,
        Out_Lap_Q,
        In_Lap_Q,
        Pit_Q,
        //Race
        Leader,
        Lapped,
        Starting,
        Pit_Area,
        Pit
    }
}
