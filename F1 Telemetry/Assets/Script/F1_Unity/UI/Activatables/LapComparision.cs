using F1_Data_Management;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;

namespace F1_Unity
{
    /// <summary>
    /// Updates visuals for lapComparision
    /// </summary>
    public class LapComparision : ActivatableBase
    {
        [Header("Settings")]

        [SerializeField, Range(0.01f, 10f)] float _updateFrequency = 1f;
        [SerializeField] string _lapNotInMemoryString = "---";
        [SerializeField] Color _fasterColor;
        [SerializeField] Color _slowerColor;
        [SerializeField] Color _blankColor = Color.white;
        [SerializeField] Text[] _lapTexts;

        [Header("Driver 1")]

        [SerializeField] Image _driver1TyreImage;
        [SerializeField] Text[] _driver1TimeText;

        [Header("Driver 2")]

        [SerializeField] Image _driver2TyreImage;
        [SerializeField] Text[] _driver2TimeText;

        /// <summary>
        /// Used to keep track of when to update values if staying with same drivers for long time. Lap for driver behind
        /// </summary>
        int _currentLap = byte.MinValue;
        //Makes sure information updates every so often
        Timer _updateTimer;

        private void Awake()
        {
            _updateTimer = new Timer(_updateFrequency);
        }

        protected override void SetData(DriverData d1Data, DriverData d2Data)
        {
            _updateTimer.Time += Time.deltaTime;

            //Flip so leader always is d1Data
            if (d2Data.LapData.carPosition < d1Data.LapData.carPosition)
            {
                DriverData temp = d1Data;
                d1Data = d2Data;
                d2Data = temp;
            }

            //Not showing correct driver info -> fix that
            if (_driver1ID != d1Data.ID || _driver2ID != d2Data.ID || _updateTimer.Expired())
            {
                _updateTimer.Reset();
                _currentLap = d2Data.LapData.currentLapNumber;
                _driver1ID = d1Data.ID;
                _driver2ID = d2Data.ID;
                SetVisuals(d1Data, d2Data);
            }
        }

        /// <summary>
        /// Only needs to be called very seldom. Sets everything.
        /// </summary>
        protected override void SetVisuals(DriverData d1Data, DriverData d2Data)
        {
            base.SetVisuals(d1Data, d2Data);
            SetTyreSprites(d1Data, d2Data);
            SetLapTexts();
            SetTimeTexts(d1Data, d2Data);
        }

        /// <summary>
        /// Reads the lap times for driver 1 from storage and sets them if available.
        /// </summary>
        void SetTimeTexts(DriverData d1Data, DriverData d2Data)
        {
            for (int i = 0; i < _driver1TimeText.Length; i++)
            {
                //-1 because we don't count current lap
                int lap = _currentLap - i - 1;
                if (lap > 0)
                {
                    StoredLapData storedLapDataDriver1 = LapManager.ReadDriverLapData(d1Data.VehicleIndex, (byte)lap, out bool status1);
                    StoredLapData storedLapDataDriver2 = LapManager.ReadDriverLapData(d2Data.VehicleIndex, (byte)lap, out bool status2);
                    //Lap data is available for both drivers
                    if (status1 && status2 && storedLapDataDriver1.lapState == LapState.Sector_3 && storedLapDataDriver2.lapState == LapState.Sector_3)
                    {
                        //Driver behind -> delta
                        float behindDelta = storedLapDataDriver1.lapTime - storedLapDataDriver2.lapTime;
                        bool slower = behindDelta < 0;
                        char pre = slower ? '+' : '-';
                        _driver2TimeText[i].color = slower ? _slowerColor : _fasterColor;
                        _driver2TimeText[i].text = pre + GetStringFromTimeInSeconds(Mathf.Abs(behindDelta));

                        _driver1TimeText[i].text = GetStringFromTimeInSeconds(storedLapDataDriver1.lapTime);
                    }
                    //Don't have access to that in memory
                    else
                    {
                        _driver1TimeText[i].text = _lapNotInMemoryString;
                        _driver2TimeText[i].color = _blankColor;
                        _driver2TimeText[i].text = _lapNotInMemoryString;
                    }
                }
                //Blank out laps that are 0 or negative
                else
                {
                    _driver1TimeText[i].text = string.Empty;
                    _driver2TimeText[i].text = string.Empty;
                }
            }
        }

        /// <summary>
        /// Sets the sprites for tyres depending on compound type
        /// </summary>
        void SetTyreSprites(DriverData d1Data, DriverData d2Data)
        {
            //Sometimes first packet is wrong -> ignore in that case
            if (d1Data.StatusData.visualTyreCompound == 0 || d2Data.StatusData.visualTyreCompound == 0)
                return;

            _driver1TyreImage.sprite = ParticipantManager.GetVisualTyreCompoundSprite(d1Data.StatusData.visualTyreCompound);
            _driver2TyreImage.sprite = ParticipantManager.GetVisualTyreCompoundSprite(d2Data.StatusData.visualTyreCompound);
        }

        /// <summary>
        /// Sets the texts for lap counters in the middle
        /// </summary>
        void SetLapTexts()
        {
            for (int i = 0; i < _lapTexts.Length; i++)
            {
                //-1 because we don't count current lap
                int lap = _currentLap - i - 1;
                _lapTexts[i].text = lap > 0 ? "LAP " + lap : string.Empty;
            }
        }

        /// <summary>
        /// Converts seconds to +minute:seconds:millieseconds
        /// </summary>
        static string GetStringFromTimeInSeconds(float time)
        {
            TimeSpan span = TimeSpan.FromSeconds(time);
            StringBuilder builder = new StringBuilder();

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
}