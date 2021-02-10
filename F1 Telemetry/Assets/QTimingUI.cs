using UnityEngine;
using UnityEngine.UI;
using F1_Data_Management;
using System;

namespace F1_Unity
{
    public class QTimingUI : DriverName
    {
        #region Fields

        [Header("Settings Child")]

        [SerializeField, Range(0.01f, 100f)] float _showCompletedLapTime = 4f;
        [SerializeField, Range(0.01f, 100f)] float _showCompletedSectorTime = 2f;
        [SerializeField, Range(1, 3)] byte _displayLapDecimalCount = 1;
        [SerializeField] Color _standardDisplayLapColor;
        [SerializeField] Color _invalidDisplayLapColor;
        [SerializeField] Color _yellowSectorColor;
        [SerializeField] Color _greenSectorColor;
        [SerializeField] Color _purpleSectorColor;

        [Header("Drop")]

        [SerializeField] Image[] _sectors;
        [SerializeField] GameObject[] _statesObjects;
        [SerializeField] Image _tyreImage;
        [SerializeField] Text _finishedLapText;
        [SerializeField] GameObject _leaderInfoObj;
        [SerializeField] Text _displayLapText;
        [SerializeField] Text _leaderNameText;
        [SerializeField] Text _leaderLapTimeText;
        [SerializeField] Text _finishedText;
        [SerializeField] GameObject[] _invalidObjs;

        readonly float CONVERT_SECONDS_TO_MILLISECONDS = 1000;
        readonly float SECTOR_3_EPSILON = 2.5f; //25 hundreds of a second

        readonly int SECTOR_1_INDEX = 0;
        readonly int SECTOR_2_INDEX = 1;
        readonly int SECTOR_3_INDEX = 2;

        readonly int PIT_INDEX = 0;
        readonly int OUT_LAP_INDEX = 1;
        readonly int FINISHED_LAP_INDEX = 2;
        readonly int NORMAL_DISPLAY_INDEX = 3;
        readonly int FINISHED_INDEX = 4;

        Timer _showCompletedLapTimer;
        Timer _showCompletedSectorTimer;
        float _currentLapTime;
        float _currentDelta;
        bool _finishedLap = false;
        bool _finishedSector = false;
        bool _leaderDoneLap = false;
        bool _invalidLap = false;
        float _leaderFastestLapTime;
        float _savedSector1 = 0;
        float _savedSector2 = 0;
        float _savedSector3 = 0;

        bool _outLap = false;
        bool _inPit = false;
        bool _finished = false;

        //Used to update tyre image if driver changes tyres while this is active
        VisualTyreCompound _currentDriverTyre;

        #endregion

        #region Init

        private void Awake()
        {
            _showCompletedLapTimer = new Timer(_showCompletedLapTime);
            _showCompletedSectorTimer = new Timer(_showCompletedSectorTime);
        }

        #endregion

        #region Override methods

        protected override void SetVisuals(DriverData spectatorDriverData)
        {
            base.SetVisuals(spectatorDriverData);
            _tyreImage.sprite = GameManager.ParticipantManager.GetVisualTyreCompoundSprite(spectatorDriverData.StatusData.visualTyreCompound);
            _currentDriverTyre = spectatorDriverData.StatusData.visualTyreCompound;
            _currentLapTime = spectatorDriverData.LapData.currentLapTime;

            DriverData leaderData = GameManager.DriverDataManager.GetFastestLapDriverData(out bool status);
            BlankSectorsResetValues(leaderData);
        }

        /// <summary>
        /// Runs once per frame -> is used to set one time things and for frame operations
        /// </summary>
        protected override void UpdateVisuals()
        {
            //DriverData spectatorDriverData = GameManager.F1Info.ReadSpectatingCarData(out bool statusDriver);
            DriverData spectatorDriverData = GameManager.F1Info.ReadPlayerData(out bool statusDriver);

            //Has to be seperated as a position change doesn't need to flush data
            if (statusDriver && spectatorDriverData.LapData.carPosition != _currentDriverPosition)
            {
                _currentDriverPosition = spectatorDriverData.LapData.carPosition;
                _positionText.text = spectatorDriverData.LapData.carPosition.ToString();
            }

            //Flush data
            if (statusDriver && (_currentDriverId != spectatorDriverData.ID || _currentDriverTyre != spectatorDriverData.StatusData.visualTyreCompound))
            {
                Show(true);
                SetVisuals(spectatorDriverData);
                MainUpdate(spectatorDriverData);
            }
            else if (!statusDriver)
                Show(false);
            //Normal frame update for valid data
            else
                MainUpdate(spectatorDriverData);
        }

        #endregion

        /// <summary>
        /// Handles all visual info that is created dynamicly
        /// </summary>
        void MainUpdate(DriverData driverData)
        {
            DriverData leaderData = GameManager.DriverDataManager.GetFastestLapDriverData(out bool status);
            
            //Handle sector 1
            HandleSector(0, _sectors[SECTOR_1_INDEX], driverData.LapData.sector1Time, leaderData.LapData.bestLapSector1Time, driverData.LapData.sector1Time, driverData.LapData.bestLapSector1Time, GameManager.LapManager.CurrentFastestSector1 * CONVERT_SECONDS_TO_MILLISECONDS, ref _savedSector1);
            //Handle sector 2
            HandleSector(0, _sectors[SECTOR_2_INDEX], driverData.LapData.sector1Time + driverData.LapData.sector2Time, leaderData.LapData.bestLapSector1Time + leaderData.LapData.bestLapSector2Time, driverData.LapData.sector2Time, driverData.LapData.bestLapSector2Time, GameManager.LapManager.CurrentFastestSector2 * CONVERT_SECONDS_TO_MILLISECONDS, ref _savedSector2);

            //Keep invalid status until finish lap resets it
            if (driverData.LapData.currentLapInvalid && !_finishedLap)
                _invalidLap = true;

            HandleFinishedLap(driverData, leaderData);
            HandleFinishedSector();

            _inPit = driverData.LapData.driverStatus == DriverStatus.In_Garage;
            _outLap = driverData.LapData.driverStatus == DriverStatus.Out_Lap;
            ResultStatus s = driverData.LapData.resultStatus;
            _finished = s == ResultStatus.Finished || s == ResultStatus.Disqualified || s == ResultStatus.Retired;

            //Used to know if this is the first driver to set a lap
            //If true delta should be ignored
            _leaderDoneLap = leaderData.LapData.bestLapTime != 0;

            if (_inPit || _outLap || _finished)
                BlankSectorsResetValues(leaderData);

            UpdateDisplay(driverData, leaderData);
        }

        #region Handle finished states

        /// <summary>
        /// Calculates if lap is finished and if it is -> activate bool that handles that
        /// </summary>
        /// <returns>It's a finished lap</returns>
        void HandleFinishedLap(DriverData driverData, DriverData leaderData)
        {
            bool finishedLap = _currentLapTime > driverData.LapData.currentLapTime;
            if (finishedLap && !_outLap)
            {
                HandleSector(SECTOR_3_EPSILON, _sectors[SECTOR_3_INDEX], driverData.LapData.lastLapTime * CONVERT_SECONDS_TO_MILLISECONDS, _leaderFastestLapTime * CONVERT_SECONDS_TO_MILLISECONDS, driverData.LapData.lastLapTime * CONVERT_SECONDS_TO_MILLISECONDS - _savedSector1 - _savedSector2, driverData.LapData.bestLapSector3Time, GameManager.LapManager.CurrentFastestSector3 * CONVERT_SECONDS_TO_MILLISECONDS, ref _savedSector3);
                _finishedLap = true;
                _showCompletedLapTimer.Reset();
            }

            _currentLapTime = driverData.LapData.currentLapTime;
            _leaderFastestLapTime = leaderData.LapData.bestLapTime;

            //Driver has finished a lap -> keep it finished status until timer is out
            //Finished sector takes priority to finished lap (Makes finished sector go first then finished lap state)
            if (_finishedLap && !_finishedSector)
            {
                _showCompletedLapTimer.Time += Time.deltaTime;
                if (_showCompletedLapTimer.Expired())
                    BlankSectorsResetValues(leaderData);
            }
        }

        /// <summary>
        /// Counts the timer for how long a finished sector should display finished sector state
        /// </summary>
        void HandleFinishedSector()
        {
            //Driver has finished a sector -> keep it finished status until timer is out
            if (_finishedSector)
            {
                _showCompletedSectorTimer.Time += Time.deltaTime;
                if (_showCompletedSectorTimer.Expired())
                    _finishedSector = false;
            }
        }

        #endregion

        /// <summary>
        /// Activate/Deactivate sectors and color according to driver performance
        /// </summary>
        /// <param name="driverData"></param>
        void UpdateDisplay(DriverData driverData, DriverData leaderData)
        {
            //Gray out sectors -> no values to show -> reset sectors
            if (_finished)
                Display(DisplayMode.Finished, driverData, leaderData);
            else if (_inPit || _outLap)
                Display(_inPit ? DisplayMode.Pit : DisplayMode.Out_Lap, driverData, leaderData);
            //Finished sector mode
            else if (_finishedSector)
                Display(DisplayMode.Finished_Sector, driverData, leaderData);
            //Car is on track currently doing a lap
            else if (!_finishedLap)
                Display(DisplayMode.Counting, driverData, leaderData);
            //Car has finished
            else
                Display(DisplayMode.Finished_Lap, driverData, leaderData);
        }

        /// <summary>
        /// Resets values related to other states and display pit if true or out lap if false
        /// </summary>
        /// <param name="pit"></param>
        void BlankSectorsResetValues(DriverData leaderData)
        {
            for (int i = 0; i < _sectors.Length; i++)
                _sectors[i].gameObject.SetActive(false);

            //Reset all values 
            _savedSector1 = 0;
            _savedSector2 = 0;
            _savedSector3 = 0;
            _leaderFastestLapTime = leaderData.LapData.bestLapTime;
            _finishedSector = false;
            _finishedLap = false;
            _invalidLap = false;
            _showCompletedSectorTimer.Reset();
            _showCompletedLapTimer.Reset();
        }

        /// <summary>
        /// Sets the sector to active/not active and color -> also calculates delta relative to leader lap
        /// </summary>
        /// <param name="epsilon">Only needed when doing sector 3, calculation of sector 3 time is estimate -> epsilon needed</param>
        /// <param name="sectorImage">What sector image is being manipulated</param>
        /// <param name="elapsedTime">(in millieseconds) Sum of previous sector and this one (sector 1 + sector 2 in sector 2) Used to calculate delta</param>
        /// <param name="leaderElapsedTime">(in millieseconds) Sum of previous sector to this one for the leader(The combined sector time up to and including this sector (handling sector2 => sector1 + sector2)) Used to calculate delta to leader</param>
        /// <param name="sectorTime">(in millieseconds) Sector time for this sector (0 if not finished it yet)</param>
        /// <param name="bestLapSectorTime">(in millieseconds) Sector time for best lap for this driver</param>
        /// <param name="overallBestSectorTime">(in millieseconds) Sector time for overall session best sector time for this sector</param>
        /// <param name="savedSectorTime">reference variable that store current/previous lap sector -> used to calculate Sector 3 and final display. Is set here</param>
        void HandleSector(float epsilon, Image sectorImage, float elapsedTime, float leaderElapsedTime, float sectorTime, float bestLapSectorTime, float overallBestSectorTime, ref float savedSectorTime)
        {
            //fixes color setting to not display yellow sector on first lap out
            if (bestLapSectorTime == 0)
                bestLapSectorTime = float.MaxValue;

            //It's a finished sector -> activate (One time only)
            if (sectorTime != 0 && savedSectorTime == 0)
            {
                savedSectorTime = sectorTime;

                sectorImage.gameObject.SetActive(true);

                //In milliseconds
                _currentDelta = (elapsedTime - leaderElapsedTime) / CONVERT_SECONDS_TO_MILLISECONDS;

                //No delta to compare to so no need to enter sector mode
                if (_leaderDoneLap)
                    _finishedSector = true;

                _showCompletedSectorTimer.Reset();

                SetSectorColor(sectorImage, bestLapSectorTime, overallBestSectorTime, epsilon, savedSectorTime);
            }
            //Sector not yet completed
            else if (savedSectorTime == 0)
                sectorImage.gameObject.SetActive(false);
            //Sector is done set color
            else
                SetSectorColor(sectorImage, bestLapSectorTime, overallBestSectorTime, epsilon, savedSectorTime);
        }

        /// <summary>
        /// Set color for a sector based on performance
        /// </summary>
        /// <param name="sectorImage">What sector image should color changes apply to</param>
        /// <param name="bestLapSectorTime">This drivers best lap's best sector time for this sector</param>
        /// <param name="overallBestSectorTime">Best sector time in session</param>
        /// <param name="savedSectorTime">What sector time has this driver done</param>
        /// <param name="epsilon">Only needed when doing sector 3, calculation of sector 3 time is estimate -> epsilon needed</param>
        void SetSectorColor(Image sectorImage, float bestLapSectorTime, float overallBestSectorTime, float epsilon, in float savedSectorTime)
        {
            //purple sector
            if (savedSectorTime <= overallBestSectorTime + epsilon)
                sectorImage.color = _purpleSectorColor;
            //green sector
            else if (savedSectorTime < bestLapSectorTime + epsilon)
                sectorImage.color = _greenSectorColor;
            //yellow sector
            else
                sectorImage.color = _yellowSectorColor;     
        }

        #region Display

        /// <summary>
        /// Displays certain values in certain ways depending on display mode
        /// </summary>
        /// <param name="mode"></param>
        void Display(DisplayMode mode, DriverData driverData, DriverData leaderData)
        {
            DisplayLeaderInfo(leaderData);
            DisplayInvalid();

            switch (mode)
            {
                case DisplayMode.Pit:              { ActivateState(_statesObjects[PIT_INDEX]);      break; }
                case DisplayMode.Out_Lap:          { ActivateState(_statesObjects[OUT_LAP_INDEX]);  break; }
                case DisplayMode.Finished:
                    {
                        DisplayFinished(driverData);
                        break;
                    }
                case DisplayMode.Counting:
                    {
                        DisplayCounting();
                        break;
                    }
                case DisplayMode.Finished_Sector:
                    {
                        DisplayFinishedSector();
                        break;
                    }
                case DisplayMode.Finished_Lap:
                    {
                        DisplayFinishedLap(driverData);
                        break;
                    }
                default:
                    throw new System.Exception("There is no implementation for this display mode: " + mode);
            }
        }      

        /// <summary>
        /// Sets laptime and name of current pole sitter to display (visible in sector complete and count mode)
        /// </summary>
        private void DisplayLeaderInfo(DriverData leaderData)
        {
            _leaderInfoObj.SetActive(_leaderDoneLap);
            _leaderNameText.text = GameManager.ParticipantManager.GetNameFromNumber(leaderData.RaceNumber);
            _leaderLapTimeText.text = F1Utility.GetDeltaString(leaderData.LapData.bestLapTime);
        }

        /// <summary>
        /// Display invalid banner if lap is invalid
        /// </summary>
        void DisplayInvalid()
        {
            for (int i = 0; i < _invalidObjs.Length; i++)
                _invalidObjs[i].SetActive(_invalidLap);
        }

        /// <summary>
        /// State where it counts lap time
        /// </summary>
        private void DisplayCounting()
        {
            ActivateState(_statesObjects[NORMAL_DISPLAY_INDEX]);
            _displayLapText.text = F1Utility.GetDeltaString(_currentLapTime, _displayLapDecimalCount);
            _displayLapText.color = _invalidLap ? _invalidDisplayLapColor : _standardDisplayLapColor;
        }

        /// <summary>
        /// Displays the driver's best lap time
        /// </summary>
        void DisplayFinished(DriverData driverData)
        {
            ActivateState(_statesObjects[FINISHED_INDEX]);
            _finishedText.text = F1Utility.GetDeltaString(driverData.LapData.bestLapTime);
            _finishedText.color = driverData.LapData.bestLapTime == GameManager.LapManager.FastestLapTime ? _purpleSectorColor : _standardDisplayLapColor;
        }

        /// <summary>
        /// Displays delta so far through the lap compared to leader and color it
        /// </summary>
        private void DisplayFinishedSector()
        {
            ActivateState(_statesObjects[NORMAL_DISPLAY_INDEX]);
            _displayLapText.text = F1Utility.GetDeltaStringSigned(_currentDelta);

            //Set color depending on delta
            if (_currentDelta > 0)
                _displayLapText.color = _yellowSectorColor;
            else if (_currentDelta < 0)
                _displayLapText.color = _greenSectorColor;
            else
                _displayLapText.color = _standardDisplayLapColor;
        }

        /// <summary>
        /// Displays the finished lap time
        /// </summary>
        private void DisplayFinishedLap(DriverData driverData)
        {
            ActivateState(_statesObjects[FINISHED_LAP_INDEX]);
            _finishedLapText.text = F1Utility.GetDeltaString(driverData.LapData.lastLapTime);

            if (_invalidLap)
                _finishedLapText.color = _invalidDisplayLapColor;
            else if (driverData.LapData.bestLapTime == GameManager.LapManager.FastestLapTime)
                _finishedLapText.color = _purpleSectorColor;
            else
                _finishedLapText.color = _standardDisplayLapColor;
        }

        #endregion

        /// <summary>
        /// Activates specified state and deactivate all other
        /// </summary>
        void ActivateState(GameObject active)
        {
            for (int i = 0; i < _statesObjects.Length; i++)
                _statesObjects[i].SetActive(active == _statesObjects[i]);
        }

        enum DisplayMode
        {
            Finished,
            Pit,
            Out_Lap,
            Counting,
            Finished_Sector,
            Finished_Lap,
        }
    }
}