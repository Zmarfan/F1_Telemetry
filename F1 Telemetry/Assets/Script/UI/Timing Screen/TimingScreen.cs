using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimingScreen : MonoBehaviour
{
    [SerializeField, Range(0.01f, 5f)] float _flashColorDuration = 1.0f;
    [SerializeField] Color _movedUpColor = Color.green;
    [SerializeField] Color _movedDownColor = Color.red;
    [SerializeField] DriverTemplate[] _driverTemplates;

    static TimingScreen _singleton;

    //Reach driver position by their ID
    Dictionary<byte, int> _driverPosition;
    bool _initValues = true;

    private void Awake()
    {
        if (_singleton == null)
            Init();
        else
            Destroy(this.gameObject);
    }

    /// <summary>
    /// Assign correct number to each placement and create singleton
    /// </summary>
    void Init()
    {
        _singleton = this;
        for (int i = 0; i < _driverTemplates.Length; i++)
            _driverTemplates[i].Init(i + 1, _flashColorDuration);
    }

    /// <summary>
    /// Maps driver IDs to position on track. Used to compare old position to current
    /// </summary>
    void InitDrivers()
    {
        _driverPosition = new Dictionary<byte, int>();

        for (int i = 0; i < Participants.Data.ParticipantData.Length; i++)
        {
            //Values are now initiated
            _initValues = false;

            ParticipantData participantData = Participants.Data.ParticipantData[i];
            LapData lapData = Participants.Data.LapData[i];

            //Init everyone with gaining a position on start!
            //Flash green then to white
            //Only add valid drivers to the grid
            if (Participants.ValidIndex(i))
                _driverPosition.Add(participantData.driverID, lapData.carPosition + 1);

            //Only enable so many positions in time standing as there are active drivers
            //If they DNF/DSQ later they will only gray out, not be removed
            if (i >= Participants.ActiveDrivers)
                _driverTemplates[i].SetActive(false);
            else
                _driverTemplates[i].SetActive(true);
        }
    }

    //Updates standing
    private void Update()
    {
        //ONLY IN RACE!

        //Only update standings when data can be read safely and correctly
        if (Participants.ReadyToReadFrom)
        {
            if (_initValues)
                InitDrivers();
            else
                DoTimingScreen();
        }
    }

    /// <summary>
    /// Updates positions in standing and checks stability
    /// </summary>
    void DoTimingScreen()
    {
        //Loop through all drivers
        for (int i = 0; i < Participants.Data.LapData.Length; i++)
        {
            //Skip the drivers that have no valid index -> junk data
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
                int positionIndex = lapData.carPosition - 1; //Index in array is always one less than position

                _driverTemplates[positionIndex].SetInitials(participantData.driverInitial); //Set initals for that position

                //Change color wether driver GAINED or LOST to this position -> compare old position with this one
                _driverTemplates[positionIndex].UpdatePositionColor(_driverPosition[participantData.driverID], _movedUpColor, _movedDownColor);
                _driverTemplates[positionIndex].SetTeamColor(participantData.teamColor); //Set team color

                //save this position to compare in future
                _driverPosition[participantData.driverID] = lapData.carPosition;
            }
        }
    }
}
