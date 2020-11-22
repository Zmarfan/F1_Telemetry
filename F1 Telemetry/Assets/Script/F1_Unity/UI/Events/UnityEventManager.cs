using UnityEngine;
using F1_Data_Management;
using F1_Unity;
using System.Collections.Generic;

public class UnityEventManager : MonoBehaviour
{
    [SerializeField] Transform _canvas;
    [SerializeField] GameObject _fastestLapPrefab;
    [SerializeField] GameObject _drsEnabledPrefab;
    [SerializeField] GameObject _drsDisabledPrefab;
    [SerializeField] GameObject _chequeredFlagPrefab;

    //Events that are waiting to be triggered but can't activate yet
    Queue<EventBase> _waitingEvents = new Queue<EventBase>();
    bool _eventPlaying = false;

    private void OnEnable()
    {
        EventManager.FastestLapEvent += FastestLapEvent;
        EventManager.DRSEnabledEvent += DRSEnabledEvent;
        EventManager.DRSDisabledEvent += DRSDisabledEvent;
        EventManager.ChequeredFlagEvent += ChequeredFlagEvent;
    }

    private void OnDisable()
    {    
        EventManager.FastestLapEvent -= FastestLapEvent;
        EventManager.DRSEnabledEvent -= DRSEnabledEvent;
        EventManager.DRSDisabledEvent -= DRSDisabledEvent;
        EventManager.ChequeredFlagEvent -= ChequeredFlagEvent;
    }

    /// <summary>
    /// Spawns EventPrefab and init with Packet.
    /// </summary>
    EventBase SpawnEventPrefab(GameObject prefab, Packet packet)
    {
        GameObject obj = Instantiate(prefab, Vector3.zero, Quaternion.identity, _canvas) as GameObject;
        EventBase thisEvent = obj.GetComponent<EventBase>();
        thisEvent.Init(packet);
        return thisEvent;
        
    }

    /// <summary>
    /// Adds event to queue and if the queue was empty -> Begin this event!
    /// </summary>
    void AddToEventQueue(EventBase thisEvent)
    {
        if (!_eventPlaying && _waitingEvents.Count == 0)
            BeginEvent(thisEvent);
        else
            _waitingEvents.Enqueue(thisEvent);
    }

    /// <summary>
    /// Invoked when a playing event ends. Checks if queue isn't empty. If it's not -> Begin next event in queue
    /// </summary>
    void CheckEventQueue()
    {
        _eventPlaying = false;
        if (_waitingEvents.Count > 0)
        {
            EventBase thisEvent = _waitingEvents.Dequeue();
            BeginEvent(thisEvent);
        }
    }

    /// <summary>
    /// Begin event and subscribe to it's ending event.
    /// </summary>
    void BeginEvent(EventBase thisEvent)
    {
        _eventPlaying = true;
        thisEvent.EventOver += CheckEventQueue;
        thisEvent.Begin();
    }

    #region DifferentEvents

    /// <summary>
    /// Called when fastest lap event occour. Spawns fastest lap prefab.
    /// </summary>
    void FastestLapEvent(Packet packet)
    {
        EventBase thisEvent = SpawnEventPrefab(_fastestLapPrefab, packet);
        AddToEventQueue(thisEvent);
    }

    /// <summary>
    /// Called when DRS enabled event occour. Spawns DRS enabled prefab.
    /// </summary>
    void DRSEnabledEvent(Packet packet)
    {
        EventBase thisEvent = SpawnEventPrefab(_drsEnabledPrefab, packet);
        AddToEventQueue(thisEvent);
    }

    /// <summary>
    /// Called when DRS disabled event occour. Spawns DRS disabled prefab.
    /// </summary>
    void DRSDisabledEvent(Packet packet)
    {
        EventBase thisEvent = SpawnEventPrefab(_drsDisabledPrefab, packet);
        AddToEventQueue(thisEvent);
    }

    /// <summary>
    /// Called when Chequered flag event occour (Leader is about to cross finish line). Spawns chequered flag prefab.
    /// </summary>
    void ChequeredFlagEvent(Packet packet)
    {
        EventBase thisEvent = SpawnEventPrefab(_chequeredFlagPrefab, packet);
        AddToEventQueue(thisEvent);
    }

    #endregion
}
