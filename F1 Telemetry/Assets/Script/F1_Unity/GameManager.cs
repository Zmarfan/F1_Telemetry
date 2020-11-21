using F1_Data_Management;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    GameManager _singleton;

    private void Awake()
    {
        if (_singleton == null)
            _singleton = this;
        else
            Destroy(this.gameObject);

        StartListeningOnGame();
    }

    /// <summary>
    /// Starts process of listening for Packets sent from F1 2020. The packets will be read and processed.
    /// <para>Access read data from Participants in F1_Data_Management namespace</para>
    /// </summary>
    void StartListeningOnGame()
    {
        UdpReceiver.StartListening();
    }

    /// <summary>
    /// Stops process of listening for Packets sent from F1 2020. Also resets all Data storage.
    /// </summary>
    void StopListeningOnGame()
    {
        UdpReceiver.StopListening();
        PacketManager.Reset();
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
        PacketManager.ReadCollectedPackets();
    }
}
