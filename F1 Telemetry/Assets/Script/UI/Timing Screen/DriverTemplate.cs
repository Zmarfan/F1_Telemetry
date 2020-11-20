using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DriverTemplate : MonoBehaviour
{
    [SerializeField] Transform _fastestLap;
    [SerializeField] Text _positionText;
    [SerializeField] Image _teamColorImage;
    [SerializeField] Text _initialsText;
    [SerializeField] Text _timeText;

    /// <summary>
    /// Activate/Deactivate fastest lap symbol next to player in timestandings
    /// </summary>
    public void SetFastestLap(bool state)
    {
        _fastestLap.gameObject.SetActive(state);
    }

    /// <summary>
    /// Sets the driver positionText to position
    /// </summary>
    public void SetPosition(int position)
    {
        _positionText.text = position.ToString();
    }

    /// <summary>
    /// Sets the team color to color
    /// </summary>
    public void SetTeamColor(F1_Telemetry.Color color)
    {
        _teamColorImage.color = new Color(color.r, color.g, color.b, color.a);
    }

    /// <summary>
    /// Sets the 3 letter initials for driver in time standings
    /// </summary>
    public void SetInitials(string initials)
    {
        _initialsText.text = initials;
    }

    /// <summary>
    /// Sets the string in time section: time or interval
    /// </summary>
    public void SetTimeText(string timeText)
    {
        _timeText.text = timeText;
    }
}
