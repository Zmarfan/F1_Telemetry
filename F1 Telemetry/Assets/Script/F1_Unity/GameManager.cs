using F1_Data_Management;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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
