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
    [SerializeField] GameObject _penaltyPrefab;
    [SerializeField] GameObject _retirePrefab;
    [SerializeField] GameObject _teamMateInPitPrefab;

    //Events that are waiting to be triggered but can't activate yet
    Queue<EventBase> _waitingEvents = new Queue<EventBase>();
    bool _eventPlaying = false;

    private void OnEnable()
    {
        GameManager.F1Info.FastestLapEvent += FastestLapEvent;
        GameManager.F1Info.DRSEnabledEvent += DRSEnabledEvent;
        GameManager.F1Info.DRSDisabledEvent += DRSDisabledEvent;
        GameManager.F1Info.ChequeredFlagEvent += ChequeredFlagEvent;
        GameManager.F1Info.PenaltyEvent += PenaltyEvent;
        GameManager.F1Info.RetirementEvent += RetirementEvent;
        GameManager.F1Info.TeamMateInPitsEvent += TeamMateInPitEvent;
    }

    private void OnDisable()
    {
        GameManager.F1Info.FastestLapEvent -= FastestLapEvent;
        GameManager.F1Info.DRSEnabledEvent -= DRSEnabledEvent;
        GameManager.F1Info.DRSDisabledEvent -= DRSDisabledEvent;
        GameManager.F1Info.ChequeredFlagEvent -= ChequeredFlagEvent;
        GameManager.F1Info.PenaltyEvent -= PenaltyEvent;
        GameManager.F1Info.RetirementEvent -= RetirementEvent;
        GameManager.F1Info.TeamMateInPitsEvent -= TeamMateInPitEvent;
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

    /// <summary>
    /// Called when Retirement event occour. Spawns retirement prefab.
    /// </summary>
    void RetirementEvent(Packet packet)
    {
        EventBase thisEvent = SpawnEventPrefab(_retirePrefab, packet);
        AddToEventQueue(thisEvent);
    }

    /// <summary>
    /// Called when Team mate in pit event occour. Spawns Team mate in pit prefab.
    /// </summary>
    void TeamMateInPitEvent(Packet packet)
    {
        EventBase thisEvent = SpawnEventPrefab(_teamMateInPitPrefab, packet);
        AddToEventQueue(thisEvent);
    }

    /// <summary>
    /// Called when Penalty event occour. Spawns penalty prefab.
    /// </summary>
    void PenaltyEvent(Packet packet)
    {
        PenaltyEventPacket penaltyPacket = (PenaltyEventPacket)packet;
        PenaltyType t = penaltyPacket.PenaltyType;

        //No need to react to all these kinds of penalties
        if (t == PenaltyType.Penalty_Reminder || t == PenaltyType.Warning || t == PenaltyType.This_And_Next_Lap_Invalidated || t == PenaltyType.This_And_Next_Lap_Invalidated_Without_Reason ||
            t == PenaltyType.This_And_Previous_Lap_Invalidated || t == PenaltyType.This_And_Previous_Lap_Invalidated_Without_Reason || t == PenaltyType.This_Lap_Invalidated ||
            t == PenaltyType.This_Lap_Invalidated_Without_Reason)
            return;

        EventBase thisEvent = SpawnEventPrefab(_penaltyPrefab, packet);
        AddToEventQueue(thisEvent);
    }

    #endregion
}
