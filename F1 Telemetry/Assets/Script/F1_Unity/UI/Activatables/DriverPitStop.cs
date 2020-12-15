using F1_Data_Management;
using UnityEngine;
using UnityEngine.UI;

namespace F1_Unity
{
    public delegate void DriverPitStopDead(int vehicleIndex);

    public class DriverPitStop : MonoBehaviour
    {
        public enum PitStopStatus
        {
            Entering,
            Started
        }

        [Header("Settings")]

        [SerializeField, Range(0.01f, 10f)] float _showAfterEndedTime = 2.0f;
        [SerializeField] Color _boxColor;

        [Header("Drop")]

        [SerializeField] Text _positionText;
        [SerializeField] Text _driverNameText;
        [SerializeField] Image _teamColorImage;

        [SerializeField] Text _totalPitTimeText;
        [SerializeField] Text _inBoxPitTimeText;
        [SerializeField] Image[] _inBoxCornerImages;

        public event DriverPitStopDead DeadEvent;

        /// <summary>
        /// Indicates if the pit info should show or not -> is set to false when exiting pit
        /// </summary>
        public bool Running { get; private set; } = true;

        /// <summary>
        /// Time this instance was created relative to in game time
        /// </summary>
        float _startTime;
        /// <summary>
        /// Time pit box was entered relative to in game time.
        /// </summary>
        float _startActualPitTime;
        /// <summary>
        /// Indicates which state the car is in its pit stop.
        /// </summary>
        PitStopStatus _currentPitStopStatus = PitStopStatus.Entering;

        Timer _showAfterEndedTimer;
        byte _carPosition = byte.MaxValue;
        int _vehicleIndex;

        private void Awake()
        {
            _showAfterEndedTimer = new Timer(_showAfterEndedTime);
            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

        private void Update()
        {
            if (GameManager.F1Info.ReadyToReadFrom)
                UpdateValues();
        }

        /// <summary>
        /// Called from outer class to set name and team image -> never changes
        /// </summary>
        public void Init(DriverData driverData)
        {
            _vehicleIndex = driverData.VehicleIndex;
            //Sets reference time to compare with
            _startTime = GameManager.F1Info.SessionTime;

            _driverNameText.text = GameManager.ParticipantManager.GetNameFromNumber(driverData.RaceNumber);
            _teamColorImage.color = GameManager.F1Utility.GetColorByTeam(driverData.ParticipantData.team);
        }

        /// <summary>
        /// Called from outer class to set time values.
        /// </summary>
        public void UpdateValues()
        {
            DriverData driverData = GameManager.F1Info.ReadCarData(_vehicleIndex, out bool status);
            if (status)
            {
                UpdatePosition(driverData);

                PitStatus pitStatus = driverData.LapData.pitStatus;

                //Currently not pitting anymore
                if (pitStatus == PitStatus.None)
                {
                    _showAfterEndedTimer.Time += Time.deltaTime;
                    if (_showAfterEndedTimer.Expired())
                    {
                        DeadEvent?.Invoke(_vehicleIndex);
                        Destroy(gameObject);
                    }
                }
                else
                {
                    //It's in pit -> count time in total
                    float totalElapsedTime = GameManager.F1Info.SessionTime - _startTime;
                    _totalPitTimeText.text = totalElapsedTime.ToString("0.0").Replace(',', '.');

                    if (pitStatus == PitStatus.In_Pit_Area)
                        InPitArea();
                }
            }
        }

        /// <summary>
        /// Updates visuals for what position car is in
        /// </summary>
        void UpdatePosition(DriverData driverData)
        {
            if (_carPosition != driverData.LapData.carPosition)
            {
                _carPosition = driverData.LapData.carPosition;
                _positionText.text = _carPosition.ToString();
            }
        }
        
        /// <summary>
        /// Set visuals for when the car is in box area
        /// </summary>
        void InPitArea()
        {
            switch (_currentPitStopStatus)
            {
                //Just started it's actual pit stop! Set corner colors and init
                case PitStopStatus.Entering:
                    {
                        _currentPitStopStatus = PitStopStatus.Started;
                        _startActualPitTime = GameManager.F1Info.SessionTime;
                        _inBoxPitTimeText.text = "0.0";
                        for (int i = 0; i < _inBoxCornerImages.Length; i++)
                            _inBoxCornerImages[i].color = _boxColor;
                        break;
                    }
                case PitStopStatus.Started:
                    {
                        float boxTime = GameManager.F1Info.SessionTime - _startActualPitTime;
                        _inBoxPitTimeText.text = boxTime.ToString("0.0").Replace(',', '.');
                        break;
                    }
                default: { throw new System.Exception("There is no support for this enum type: " + _currentPitStopStatus); }
            }
        }
    }
}