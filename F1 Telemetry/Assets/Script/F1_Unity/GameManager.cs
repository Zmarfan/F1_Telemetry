using F1_Data_Management;
using UnityEngine;
using RawInput;
using System.Collections.Generic;
using System.Linq;
using F1_Options;

namespace F1_Unity
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] GameObject _lockInputSymbol;
        [SerializeField] Key _lockInputKey;

        //Used only to init the managers
        [SerializeField] ParticipantManager _participantManagerScript;
        [SerializeField] DriverDataManager _driverDataManagerScript;
        [SerializeField] LapManager _lapManagerScript;
        [SerializeField] F1Utility _F1UtilityScript;
        [SerializeField] FlagManager _flagManagerScript;
        [SerializeField] InputManager _inputManager;
        [SerializeField] ActivationManager _activationManager;

        [SerializeField] TimingScreen _raceTimingScreen;

        static GameManager _singleton;
        public static F1Info F1Info { get; private set; } = new F1Info();

        /// <summary>
        /// Holds all raceNumber to raceDriver correlations. Team, car and portrait sprites. And tyre compound sprites
        /// </summary>
        public static ParticipantManager ParticipantManager { get { return _singleton._participantManagerScript; } }
        /// <summary>
        /// Holds all lap data for each driver.
        /// </summary>
        public static LapManager LapManager { get { return _singleton._lapManagerScript; } }
        /// <summary>
        /// Common methods for handling F1 data. Team color and delta formatting
        /// </summary>
        public static F1Utility F1Utility { get { return _singleton._F1UtilityScript; } }
        /// <summary>
        /// Holds sprites and different data depending on track and nationality. Also hold weather sprites.
        /// </summary>
        public static FlagManager FlagManager { get { return _singleton._flagManagerScript; } }
        /// <summary>
        /// Holds data for all drivers based on position on track. Also holds championship standing for all drivers.
        /// </summary>
        /// </summary>
        /// </summary>
        public static DriverDataManager DriverDataManager { get { return _singleton._driverDataManagerScript; } }
        /// <summary>
        /// Way to read input from user. Subscribe to correct event.
        /// </summary>
        public static InputManager InputManager { get { return _singleton._inputManager; } }
        /// <summary>
        /// RawInputSystem that allows subscribtion and read of inputs on a low level (application not in focus). Use InputManager instead for reading input
        /// </summary>
        public static RawInputSystem RawInputSystem { get; private set; } 

        private void OnEnable()
        {
            RawInputSystem.BeginListening();
            F1Info.SessionStartedEvent += SessionStarted;
            F1Info.SessionEndedEvent += SessionEnded;
        }

        private void OnDisable()
        {
            RawInputSystem.StopListening();
            F1Info.SessionStartedEvent -= SessionStarted;
            F1Info.SessionEndedEvent -= SessionEnded;
        }

        /// <summary>
        /// Called when a session is started -> clear out old data (visuals)
        /// </summary>
        void SessionStarted(Packet packet)
        {
            //clear old stored data
            LapManager.Reset();
            _raceTimingScreen.CompleteReset();
        }

        /// <summary>
        /// Called when session is ended -> Clear out stored data (not visuals)
        /// </summary>
        /// <param name="packet"></param>
        void SessionEnded(Packet packet)
        {
            F1Info.Clear();
            _activationManager.ClearData();
        }

        private void OnApplicationQuit()
        {
            RawInputSystem.StopListening();
        }

        /// <summary>
        /// Called from Start window to init GameManager before it's activated
        /// </summary>
        public void Init(DreamCommentator.DreamCommentatorInitPackage package)
        {
            _singleton = this;

            RawInputSystem = new RawInputSystem(_singleton._lockInputKey);

            _participantManagerScript.Init(package.participantData.Select(item => item.numberName).ToList(), package.portraitData);
            _driverDataManagerScript.Init(package.participantData.Select(item => item.championshipEntry).ToList());
            _F1UtilityScript.SetTeamColors(package.optionData.teamColorData);

            StartListeningOnGame();
        }

        /// <summary>
        /// Starts process of listening for Packets sent from F1 2020. The packets will be read and processed.
        /// <para>Access read data from Participants in F1_Data_Management namespace</para>
        /// </summary>
        void StartListeningOnGame()
        {
            F1Info.Start();
        }

        /// <summary>
        /// Stops process of listening for Packets sent from F1 2020. Also resets all Data storage.
        /// </summary>
        void StopListeningOnGame()
        {
            F1Info.Reset();
        }

        private void Update()
        {
            ReadCollectPackets();
            _singleton._lockInputSymbol.SetActive(RawInputSystem.InputLock);
        }

        /// <summary>
        /// Process all the packets read since last frame. Will store processed data in Participants.
        /// </summary>
        void ReadCollectPackets()
        {
            F1Info.ReadCollectedPackets();
        }
    }
}