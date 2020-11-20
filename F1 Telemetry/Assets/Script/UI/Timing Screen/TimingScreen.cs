using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimingScreen : MonoBehaviour
{
    [SerializeField, Range(0.01f, 5f)] float _flashColorDuration = 1.0f; 
    [SerializeField] DriverTemplate[] _driverTemplates;

    static TimingScreen _singleton;

    //Reach driver position by their ID
    Dictionary<byte, int> _driverPosition;

    private void Awake()
    {
        if (_singleton == null)
            Init();
        else
            Destroy(this.gameObject);
    }

    void Init()
    {
        _singleton = this;
        for (int i = 0; i < _driverTemplates.Length; i++)
            _driverTemplates[i].Init(i + 1, _flashColorDuration);
    }

    void InitDrivers()
    {
        _driverPosition = new Dictionary<byte, int>();

        for (int i = 0; i < Participants.Data.ParticipantData.Length; i++)
        {
            ParticipantData participantData = Participants.Data.ParticipantData[i];
            LapData lapData = Participants.Data.LapData[i];
            //Init everyone with gaining a position on start!
            //Flash green then to white
            if (Participants.ValidIndex(i))
                _driverPosition.Add(participantData.driverID, lapData.carPosition + 1);

            if (i >= Participants.ActiveDrivers)
                _driverTemplates[i].SetActive(false);
            else
                _driverTemplates[i].SetActive(true);
        }
    }

    private void Update()
    {
        //ONLY IN RACE!
        if (Participants.ReadyToReadFrom)
        {
            if (_driverPosition == null)
                InitDrivers();
            else
                DoTimingScreen();
        }
    }

    void DoTimingScreen()
    {
        //Loop through all drivers
        for (int i = 0; i < Participants.Data.LapData.Length; i++)
        {
            if (!Participants.ValidIndex(i))
                continue;

            LapData lapData = Participants.Data.LapData[i];
            ParticipantData participantData = Participants.Data.ParticipantData[i];

            //If this driver doesn't exist, it has just joined the session, recalculate everything!
            if (!_driverPosition.ContainsKey(participantData.driverID))
                InitDrivers();

            //Drivers position has changed! Update!
            if (_driverPosition[participantData.driverID] != lapData.carPosition)
            {
                int positionIndex = lapData.carPosition - 1;
                //Set initals for that position
                _driverTemplates[positionIndex].SetInitials(participantData.driverInitial);
                //Change color wether driver GAINED or LOST to this position -> compare old position with this one
                _driverTemplates[positionIndex].UpdatePositionColor(_driverPosition[participantData.driverID]);
                //Set team color
                _driverTemplates[positionIndex].SetTeamColor(participantData.teamColor);

                //save this position to compare in future
                _driverPosition[participantData.driverID] = lapData.carPosition;
            }
        }
    }
}
