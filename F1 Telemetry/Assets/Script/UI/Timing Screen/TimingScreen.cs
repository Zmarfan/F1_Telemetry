using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimingScreen : MonoBehaviour
{
    [SerializeField] DriverTemplate[] _driverPositions;

    static TimingScreen _singleton;

    private void Awake()
    {
        if (_singleton == null)
            _singleton = this;
        else
            Destroy(this.gameObject);
    }

    //private void Update()
    //{
    //    if (Participants.Data.ParticipantData != null && Participants.Data.LapData != null)
    //    {
    //        for (int i = 0; i < Participants.Data.LapData.Length; i++)
    //        {
    //            LapData lapData = Participants.Data.LapData[i];
    //            ParticipantData participantData = Participants.Data.ParticipantData[i];
    //            //If carPosition is 0 it means it's junk data and to be dismissed
    //            if (lapData.carPosition > 0)
    //            {
    //                int positionIndex = lapData.carPosition - 1;
    //                _driverPositions[positionIndex].SetInitials(participantData.driverInitial);
    //                _driverPositions[positionIndex].SetPosition(positionIndex + 1);
    //                _driverPositions[positionIndex].SetTeamColor(participantData.teamColor);
    //                _driverPositions[positionIndex].SetTimeText("skrrt");
    //            }
    //        }
    //    }
    //}
}
