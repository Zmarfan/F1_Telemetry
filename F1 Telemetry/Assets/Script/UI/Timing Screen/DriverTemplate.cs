using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DriverTemplate : MonoBehaviour
{
    [SerializeField] Image _positionImage;
    [SerializeField] Transform _fastestLap;
    [SerializeField] Text _positionText;
    [SerializeField] Image _teamColorImage;
    [SerializeField] Text _initialsText;
    [SerializeField] Text _timeText;

    //The position of this template
    int _position = 0;
    Timer _colorTimer;
    bool _resetColor = false;

    private void Update()
    {
        if (_colorTimer != null && _resetColor)
        {
            _colorTimer.Time += Time.deltaTime;

            if (_colorTimer.Expired())
            {
                _resetColor = false;
                _colorTimer.Reset();
                _positionImage.color = Color.white;
            }
        }
    }

    /// <summary>
    /// Called when first creating the template. Sets position based on index.
    /// </summary>
    public void Init(int initPosition, float colorDuration)
    {
        _position = initPosition;
        _colorTimer = new Timer(colorDuration);
        _positionText.text = _position.ToString();
    }

    public void SetActive(bool state)
    {
        transform.gameObject.SetActive(state);
    }

    /// <summary>
    /// Activate/Deactivate fastest lap symbol next to player in timestandings
    /// </summary>
    public void SetFastestLap(bool state)
    {
        _fastestLap.gameObject.SetActive(state);
    }

    public void UpdatePositionColor(int oldPosition)
    {
        Color color = oldPosition < _position ? Color.red : Color.green;
        _positionImage.color = color;
        _resetColor = true;
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
