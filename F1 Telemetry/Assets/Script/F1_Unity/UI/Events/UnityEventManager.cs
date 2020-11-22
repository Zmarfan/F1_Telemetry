using UnityEngine;
using F1_Data_Management;
using F1_Unity;

public class UnityEventManager : MonoBehaviour
{
    [SerializeField] Transform _canvas;
    [SerializeField] GameObject _fastestLapPrefab;

    private void OnEnable()
    {
        EventManager.FastestLapEvent += FastestLapEvent;
    }

    private void OnDisable()
    {    
        EventManager.FastestLapEvent -= FastestLapEvent;
    }

    /// <summary>
    /// Spawns EventPrefab and init with Packet.
    /// </summary>
    void SpawnEventPrefab(GameObject prefab, Packet packet)
    {
        GameObject obj = Instantiate(prefab, Vector3.zero, Quaternion.identity, _canvas) as GameObject;
        obj.GetComponent<EventBase>().Init(packet);
    }

    /// <summary>
    /// Called when fastest lap event occour. Spawns fastest lap prefab.
    /// </summary>
    void FastestLapEvent(Packet fastestLapPacket)
    {
        SpawnEventPrefab(_fastestLapPrefab, fastestLapPacket);
    }
}
