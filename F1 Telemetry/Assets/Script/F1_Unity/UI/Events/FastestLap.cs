using F1_Data_Management;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;

namespace F1_Unity
{
    /// <summary>
    /// Display fastest lap information
    /// </summary>
    public class FastestLap : MonoBehaviour
    {
        static readonly int MINUTE = 60;

        [Header("Settings")]

        [SerializeField, Range(0.01f, 10f)] float _showTime = 8f;
        [SerializeField, Range(0.01f, 10f)] float _fadeTime = 0.5f;

        [Header("Drop")]

        [SerializeField] CanvasGroup _canvasGroup;
        [SerializeField] Text _firstNameText;
        [SerializeField] Text _SecondNameText;
        [SerializeField] Text _oneNameText;       //Off by default
        [SerializeField] Text _time;
        [SerializeField] Image _teamImage;

        [SerializeField] Sprite[] _teamSprites; //They are in same order as Team enum -> last one is for everything other than 10 main teams

        Timer _fadeTimer;
        bool _fadingIn = true;
        Timer _showTimer;

        private void Awake()
        {
            _fadeTimer = new Timer(_fadeTime);
            _showTimer = new Timer(_showTime);
        }

        /// <summary>
        /// Init fastest lap with correct settings. time in seconds is converted to display format
        /// </summary>
        public void Init(string fullName, float time, Team team)
        {
            //Correct position relative to anchor
            GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

            InitTime(time);
            InitName(fullName);

            //Team is in sprite list (official F1 team)
            if ((int)team < _teamSprites.Length - 1)
                _teamImage.sprite = _teamSprites[(int)team];
            //Set to default F1 sprite if other team
            else
                _teamImage.sprite = _teamSprites[_teamSprites.Length - 1];
        }

        /// <summary>
        /// Converts time in seconds to display format and sets it
        /// </summary>
        void InitTime(float timeInSeconds)
        {
            TimeSpan span = TimeSpan.FromSeconds(timeInSeconds);
            StringBuilder showText = new StringBuilder();
            showText.Append(span.Minutes);
            showText.Append('.');
            showText.Append(span.Seconds);
            showText.Append('.');
            showText.Append(span.Milliseconds);
            _time.text = showText.ToString();
        }

        /// <summary>
        /// Breaks up and sets the name to be shown in fastest lap
        /// </summary>
        void InitName(string fullName)
        {
            string[] nameParts = fullName.Split();
            string firstName = nameParts[0];
            string lastName = string.Empty;

            //Create lastname if possible, if not -> single name only!
            if (nameParts.Length > 1)
            {
                for (int i = 1; i < nameParts.Length; i++)
                    lastName += nameParts[i];
                //Display first name then lastname
                _firstNameText.text = firstName;
                _SecondNameText.text = lastName.ToUpper();
            }
            //Display fullname as one in the middle
            else
            {
                _firstNameText.enabled = false;
                _SecondNameText.enabled = false;
                _oneNameText.enabled = true;
                _oneNameText.text = fullName.ToUpper();
            }
        }

        private void Update()
        {
            Showing();
        }

        /// <summary>
        /// Controls how fastest lap fades in and out and then removes
        /// </summary>
        void Showing()
        {
            _fadeTimer.Time += Time.deltaTime;

            if (!_fadeTimer.Expired())
            {
                //Fade in from black
                if (_fadingIn)
                    _canvasGroup.alpha = _fadeTimer.Ratio();
                //Fade out to black
                else
                    _canvasGroup.alpha = 1.0f - _fadeTimer.Ratio();
            }
            //Destroy when faded out
            else if (!_fadingIn)
                Destroy(this.gameObject);
            //Have up for a bit of time
            else
            {
                _showTimer.Time += Time.deltaTime;

                if (_showTimer.Expired())
                {
                    _fadingIn = false;
                    _fadeTimer.Reset();
                }
            }
        }
    }
}
