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
    /// Called when fastest lap event occour. Spawns fastest lap prefab.
    /// </summary>
    void FastestLapEvent(Packet fastestLapPacket)
    {
        GameObject obj = Instantiate(_fastestLapPrefab, Vector3.zero, Quaternion.identity, _canvas) as GameObject;
        obj.GetComponent<EventBase>().Init(fastestLapPacket);
    }
}
