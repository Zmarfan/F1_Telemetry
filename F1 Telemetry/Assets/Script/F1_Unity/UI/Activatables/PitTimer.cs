using UnityEngine;
using F1_Data_Management;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace F1_Unity
{
    public class PitTimer : MonoBehaviour, IActivatableReset
    {
        [SerializeField] GameObject _driverPitStopPrefab;
        [SerializeField] Transform _holder;
        [SerializeField] CanvasGroup _canvasGroup;

        /// <summary>
        /// Dictionary of all active driverPitStops currently activated indexed by their vehicle index
        /// </summary>
        Dictionary<int, DriverPitStop> _driverPitStopDictionary = new Dictionary<int, DriverPitStop>();

        #region Event handling

        private void OnEnable()
        {
            List<DriverPitStop> list = _driverPitStopDictionary.Values.ToList();
            for (int i = 0; i < list.Count; i++)
                list[i].DeadEvent += RemoveDriverPitStop;
        }

        private void OnDisable()
        {
            List<DriverPitStop> list = _driverPitStopDictionary.Values.ToList();
            for (int i = 0; i < list.Count; i++)
                list[i].DeadEvent -= RemoveDriverPitStop;
        }

        #endregion

        public void ClearActivatable()
        {
            List<DriverPitStop> list = _driverPitStopDictionary.Values.ToList();
            for (int i = 0; i < list.Count; i++)
                Destroy(list[i].gameObject);
            _driverPitStopDictionary = new Dictionary<int, DriverPitStop>();
        }

        private void Update()
        {
            if (GameManager.F1Info.ReadyToReadFrom)
                UpdatePitTimers();
            else
                Show(false);
        }

        /// <summary>
        /// Runs every frame if possible and sets all values.
        /// </summary>
        void UpdatePitTimers()
        {
            Show(true);

            //Looping over position
            for (int i = 1; i <= F1Info.MAX_AMOUNT_OF_CARS; i++)
            {
                DriverData data = GameManager.DriverDataManager.GetDriverFromPosition(i, out bool status);
                //Is in pits
                if (data.LapData.pitStatus != PitStatus.None)
                {
                    //Just entered pits
                    if (!_driverPitStopDictionary.ContainsKey(data.VehicleIndex) && data.LapData.resultStatus == ResultStatus.Active)
                        _driverPitStopDictionary.Add(data.VehicleIndex, SpawnNewDriverPitStop(data));
                }
            }
        }

        /// <summary>
        /// Spawns a new prefab for a driver in pit. Listen to event when it finishes.
        /// </summary>
        DriverPitStop SpawnNewDriverPitStop(DriverData driverData)
        {
            GameObject obj = Instantiate(_driverPitStopPrefab, Vector3.zero, Quaternion.identity, _holder) as GameObject;
            DriverPitStop script = obj.GetComponent<DriverPitStop>();
            script.Init(driverData);
            script.DeadEvent += RemoveDriverPitStop;
            return script;
        }

        /// <summary>
        /// Called when a driver making a pitstop is done -> removes it from dictionary
        /// </summary>
        void RemoveDriverPitStop(int vehicleIndex)
        {
            _driverPitStopDictionary.Remove(vehicleIndex);
        }

        /// <summary>
        /// Either hides or showes the activatble.
        /// </summary>
        void Show(bool status)
        {
            _canvasGroup.alpha = status ? 1.0f : 0.0f;
        }
    }
}