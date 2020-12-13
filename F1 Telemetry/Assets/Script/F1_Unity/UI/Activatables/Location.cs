using UnityEngine;
using UnityEngine.UI;
using F1_Data_Management;

namespace F1_Unity
{
    public class Location : MonoBehaviour
    {
        //[SerializeField] Animator _animator;
        //[SerializeField] string _animatorTrigger = "Trigger";
        [SerializeField] Text _stringText;
        [SerializeField] Image _flagImage;

        void OnEnable()
        {
            Session sessionData = GameManager.F1Info.ReadSession(out bool status);
            if (status)
            {
                //_animator.SetTrigger(_animatorTrigger);

                _stringText.text = FlagManager.GetGrandPrixString(sessionData.Track);
                _flagImage.sprite = FlagManager.GetFlagByTrack(sessionData.Track);
            }
            else
                gameObject.SetActive(false);
        }
    }
}