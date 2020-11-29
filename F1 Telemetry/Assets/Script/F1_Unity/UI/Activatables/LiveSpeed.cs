using UnityEngine;
using UnityEngine.UI;
using F1_Data_Management;

namespace F1_Unity
{
    public class LiveSpeed : MonoBehaviour
    {
        public const float CONVERT_KMH_TO_MPH = 0.621371192f;

        [SerializeField] UnityEngine.Color _slowColor;
        [SerializeField] UnityEngine.Color _fastColor;
        [SerializeField] int _transitionToFastColorSpeedKMH = 200;

        [SerializeField] Text _driverText;
        [SerializeField] Text _kmhSpeedText;
        [SerializeField] Text _mphSpeedText;

        int turn = 1;
        float startPoint = 0;

        private void Update()
        {
            if (GameManager.F1Info.ReadyToReadFrom)
            {
                UpdateValues();

                bool start = Input.GetKeyDown(KeyCode.T);
                bool end = Input.GetKeyDown(KeyCode.Y);

                //REMOVE
                DriverData playerDriverData = GameManager.F1Info.ReadPlayerData(out bool valid);
                if (valid)
                {
                    //Debug.Log(TrackTurns.GetTurn(Track.Abu_Dhabi, playerDriverData.LapData.lapDistance));

                    if (start)
                        startPoint = playerDriverData.LapData.lapDistance;

                    if (end)
                    {
                        Debug.Log("Turn: " + turn + ", start: " + startPoint + ", end: " + playerDriverData.LapData.lapDistance);
                        turn++;
                    }
                }
                //REMOVE
            }
        }

        /// <summary>
        /// Sets speed and driver name every frame this object is activated
        /// </summary>
        void UpdateValues()
        {
            DriverData spectatorDriverData = GameManager.F1Info.ReadSpectatingCarData(out bool status);
            if (status)
            {
                //Speed is in kmh so convert to mph as well
                float preciseSpeedInKMH = spectatorDriverData.TelemetryData.speed;
                float preciseSpeedInMPH = preciseSpeedInKMH * CONVERT_KMH_TO_MPH;

                int showSpeedKMH = (int)(preciseSpeedInKMH + 0.5f);
                int showSpeedMPH = (int)(preciseSpeedInMPH + 0.5f);

                _kmhSpeedText.text = showSpeedKMH.ToString();
                _mphSpeedText.text = showSpeedMPH.ToString();

                _driverText.text = ParticipantManager.GetNameFromNumber(spectatorDriverData.RaceNumber).ToUpper();

                SetColor(showSpeedKMH);
            }
        }

        /// <summary>
        /// Sets color on text based on speed
        /// </summary>
        void SetColor(int speed)
        {
            if (speed >= _transitionToFastColorSpeedKMH)
            {
                _kmhSpeedText.color = _fastColor;
                _mphSpeedText.color = _fastColor;
            }
            else
            {
                _kmhSpeedText.color = _slowColor;
                _mphSpeedText.color = _slowColor;
            }
        }
    }
}

