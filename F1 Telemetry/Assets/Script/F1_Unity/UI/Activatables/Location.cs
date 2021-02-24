using UnityEngine;
using UnityEngine.UI;
using F1_Data_Management;

namespace F1_Unity
{
    public class Location : MonoBehaviour
    {
        [SerializeField] Text _stringText;
        [SerializeField] Image _flagImage;

        void OnEnable()
        {
            Session sessionData = GameManager.F1Info.ReadSession(out bool status);
            if (status)
            {
                _stringText.text = GameManager.FlagManager.GetGrandPrixString(sessionData.Track);
                _flagImage.sprite = GameManager.FlagManager.GetFlagByTrack(sessionData.Track);
            }
            else
                gameObject.SetActive(false);
        }
    }
}