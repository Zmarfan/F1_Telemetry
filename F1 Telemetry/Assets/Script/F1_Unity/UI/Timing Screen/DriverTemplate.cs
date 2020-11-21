using UnityEngine;
using UnityEngine.UI;

namespace F1_Unity
{
    public class DriverTemplate : MonoBehaviour
    {
        [SerializeField] Image _positionImage;  //The white image under position number -> flashes red/green during overtakes
        [SerializeField] Transform _fastestLap; //transform for fastest lap image, deactivated on default
        [SerializeField] Text _positionText;    //Only used on init on Awake, never changes
        [SerializeField] Image _teamColorImage;
        [SerializeField] Text _initialsText;
        [SerializeField] Text _timeText;        //Time text, different depending on interval/to leader -> not yet implemented


        int _position = 0;         //The position of this template -> static never changes
        Timer _colorTimer;         //Timer for how long the positionImage shall flash
        bool _resetColor = false;

        private void Update()
        {
            UpdateColor();
        }

        /// <summary>
        /// Makes sure color goes back to white after set time
        /// </summary>
        void UpdateColor()
        {
            //colorTimer needs to be created from TimingScreen before we can use it
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

        /// <summary>
        /// Only called on start of a race
        /// </summary>
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

        /// <summary>
        /// Called when a overtake just happened. Depending on if driver went up or down the color differ
        /// </summary>
        /// <param name="oldPosition"></param>
        public void UpdatePositionColor(int oldPosition, Color movedUpColor, Color movedDownColor)
        {
            Color color = oldPosition < _position ? movedDownColor : movedUpColor;
            _positionImage.color = color;
            _resetColor = true;
        }

        /// <summary>
        /// Sets the team color to color
        /// </summary>
        public void SetTeamColor(F1_Data_Management.Color color)
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
}
