using UnityEngine;
using UnityEngine.UI;
using F1_Data_Management;
using System;
using System.Text;

namespace F1_Unity
{
    public class DetailDelta : ActivatableBase
    {
        [Header("Settings")]

        [SerializeField, Range(0.01f, 10f)] float _timeBetweenDeltaUpdates = 1.0f; 
        [SerializeField] string _defaultDeltaString = "---";
        [SerializeField] Color _startingDeltaColor = Color.grey;
        [SerializeField] Color _slowerColor = Color.red;
        [SerializeField] Color _fasterColor = Color.green;

        [Header("Drop")]

        [SerializeField] Text _deltaText;
        [SerializeField] Image _driver1CarImage;
        [SerializeField] Image _driver2CarImage;

        Timer _deltaTimer;
        float _lastDelta;

        private void Awake()
        {
            _deltaTimer = new Timer(_timeBetweenDeltaUpdates);
        }

        /// <summary>
        /// Sets delta and driver details if needed.
        /// </summary>
        protected override void SetData(DriverData d1Data, DriverData d2Data)
        {
            _deltaTimer.Time += Time.deltaTime;
            int deltaIndex = d2Data.LapData.carPosition - 1;

            //Not showing correct driver info -> fix that
            if (_driver1ID != d1Data.ID || _driver2ID != d2Data.ID)
            {
                _driver1ID = d1Data.ID;
                _driver2ID = d2Data.ID;
                SetVisuals(d1Data, d2Data);

                UpdateDelta(deltaIndex);
            }

            if (_deltaTimer.Expired())
                UpdateDelta(deltaIndex);
        }


        /// <summary>
        /// Sets the delta.
        /// </summary>
        void UpdateDelta(int index)
        {
            _deltaTimer.Reset();

            DriverTemplate template = GameManager.TimingScreenManager.GetDriverTemplate(index, out bool status);
            //Data not ready to read yet
            if (!status)
                return;

            //We don't want to set anything if the delta isn't yet correct
            DriverTimeState state = template.TimeState;
            if (state != DriverTimeState.Starting && state != DriverTimeState.Pit && state != DriverTimeState.Pit_Area && state != DriverTimeState.Lapped)
            {
                float delta = template.DeltaToCarInFront;

                _deltaText.text = F1Utility.GetDeltaString(delta);

                //Color
                _deltaText.color = delta > _lastDelta ? _slowerColor : _fasterColor;

                _lastDelta = delta;
            }
            else
            {
                _deltaText.color = _startingDeltaColor;
                _deltaText.text = _defaultDeltaString;
            }
        }

        /// <summary>
        /// Sets all visuals, only done when changing subjects to spectate.
        /// </summary>
        protected override void SetVisuals(DriverData d1Data, DriverData d2Data)
        {
            base.SetVisuals(d1Data, d2Data);
            _driver1CarImage.sprite = GameManager.ParticipantManager.GetCarSprite(d1Data.ParticipantData.team);
            _driver2CarImage.sprite = GameManager.ParticipantManager.GetCarSprite(d2Data.ParticipantData.team);
        }
    }
}