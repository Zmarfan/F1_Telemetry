using F1_Data_Management;
using UnityEngine;
using FileExplorer;
using System.Collections.Generic;
using System.Linq;

namespace F1_Unity
{
    public class GameManager : MonoBehaviour
    {
        //Used only to init the managers
        [SerializeField] ParticipantManager _participantManagerScript;
        [SerializeField] DriverDataManager _driverDataManagerScript;

        GameManager _singleton;
        public static F1Info F1Info { get; private set; } = new F1Info();

        private void Awake()
        {
            if (_singleton == null)
            {
                _singleton = this;
                StartListeningOnGame();
            }
            else
                Destroy(this.gameObject);
        }

        /// <summary>
        /// Called from Start window to init GameManager before it's activated
        /// </summary>
        public void Init(List<DreamCommentator.ParticipantData> data, List<ParticipantManager.NumberSpriteStruct> portraitSprites)
        {
            _participantManagerScript.Init(data.Select(item => item.numberName).ToList(), portraitSprites);
            _driverDataManagerScript.Init(data.Select(item => item.championshipEntry).ToList());
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