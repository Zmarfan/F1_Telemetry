using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using F1_Data_Management;
using F1_Unity;
using UnityEngine.UI;

namespace F1_Options
{
    public class TrackText : MonoBehaviour
    {
        [SerializeField] Text _trackNameText;
        [SerializeField] Text _currentText;
        [SerializeField] Image _flagImage;

        public void Init(StringTrackStruct data)
        {
            _trackNameText.text = ConvertEnumToString.Convert<Track>(data.track);
            _currentText.text = data.text;
            _flagImage.sprite = data.flag;
        }
    }
}