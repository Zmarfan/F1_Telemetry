using UnityEngine;
using F1_Data_Management;

namespace F1_Unity
{
    /// <summary>
    /// State machine controller for different timing screen depending on session
    /// </summary>
    public class TimingScreenManager : MonoBehaviour
    {
        [SerializeField] Transform _spawnContainer;
        [SerializeField] GameObject _qTimingScreenPrefab;
        [SerializeField] GameObject _raceTimingScreenPrefab;

        /// <summary>
        /// Base controller of timingscreen that can be any timing screen
        /// </summary>
        TimingScreenBase _currentTimingScreen;

        private void Update()
        {
            //Update current timing screen once per frame
            if (ActiveTimingScreen)
                _currentTimingScreen.UpdateTimingScreen();
        }

        #region Public methods (GameManager)

        /// <summary>
        /// Completely resets to original prefab state -> used when changing between sessions that are the same session type to clear data
        /// </summary>
        public void CompleteReset()
        {
            if (ActiveTimingScreen)
                _currentTimingScreen.CompleteReset();
        }

        /// <summary>
        /// What new state the timing screen should enter
        /// Remove previous timing screen and spawn in new correct one
        /// </summary>
        public void SetMode(TimingScreenType type)
        {
            //Remove previous timing screen
            if (ActiveTimingScreen)
                Destroy(_currentTimingScreen.gameObject);

            switch (type)
            {
                case TimingScreenType.Qualifying: { SpawnTimingScreen(_qTimingScreenPrefab);     break; }
                case TimingScreenType.Race:       { SpawnTimingScreen(_raceTimingScreenPrefab);  break; }
                case TimingScreenType.One_Shot_Q: { SpawnTimingScreen(_qTimingScreenPrefab); Debug.LogWarning("One shot Q timing screen doesn't exist"); break; }
                case TimingScreenType.Time_Trial: { SpawnTimingScreen(_qTimingScreenPrefab); Debug.LogWarning("Time_Trial timing screen doesn't exist"); break; }
                default:
                    throw new System.Exception("There is no current implementation to handle this session: " + type);
            }
        }

        #endregion

        #region Public Methods (Usage)

        /// <summary>
        /// Hides or shows the current timing screen (Still active in background)
        /// </summary>
        public void SetActive(bool status)
        {
            if (ActiveTimingScreen)
                _currentTimingScreen.SetActive(status);
        }

        /// <summary>
        /// Get the driver template for specific index (Used to read delta for each driver)
        /// </summary>
        /// <param name="index">Position - 1</param>
        /// <returns>Access to a driver's delta, state and info</returns>
        public TimingScreenEntry GetDriverTemplate(int index, out bool status)
        {
            if (index >= 0 && index < F1Info.MAX_AMOUNT_OF_CARS)
            {
                status = ActiveTimingScreen;

                return ActiveTimingScreen ? _currentTimingScreen.GetDriverTemplate(index) : null;
            }
            else
                throw new System.Exception(index + " is not a valid index to access a driver template!");
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Spawns in a new timing screen of prefered variant and sets it to current timing screen to work from
        /// </summary>
        void SpawnTimingScreen(GameObject prefab)
        {
            GameObject obj = Instantiate(prefab, _spawnContainer) as GameObject;
            _currentTimingScreen = obj.GetComponent<TimingScreenBase>();
        }

        /// <summary>
        /// Indicates if timing scren is null or not (true if not)
        /// </summary>
        bool ActiveTimingScreen
        {
            get { return _currentTimingScreen != null; }
        }

        #endregion

        /// <summary>
        /// Different states timing screen can be in
        /// </summary>
        public enum TimingScreenType
        {
            Qualifying,
            Race,
            One_Shot_Q,
            Time_Trial,
        }
    }
}