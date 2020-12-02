﻿using UnityEngine;
using UnityEngine.UI;
using F1_Data_Management;

namespace F1_Unity
{
    public class Location : MonoBehaviour
    {
        [SerializeField] Text _stringText;
        [SerializeField] Image _flagImage;

        private void Start()
        {
            Init();
        }

        /// <summary>
        /// Called on initiation to set values.
        /// </summary>
        public void Init()
        {
            Session sessionData = GameManager.F1Info.ReadSession(out bool status);
            if (status)
            {
                _stringText.text = FlagManager.GetGrandPrixString(sessionData.Track);
                _flagImage.sprite = FlagManager.GetFlagByTrack(sessionData.Track);
            }
            else
                gameObject.SetActive(false);
        }
    }
}