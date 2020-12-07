using F1_Data_Management;
using UnityEngine;
using UnityEngine.UI;

namespace F1_Unity
{
    public class CircuitInfo : MonoBehaviour
    {
        [SerializeField] Animator _animator;
        [SerializeField] string _animatorTrigger = "Trigger";
        [SerializeField] string _preTrackNameString = "Circuit Info - ";
        [SerializeField] Text _circuitInfoText;
        [SerializeField] Text _trackTypeText;
        [SerializeField] Text _fullThrottleText;
        [SerializeField] Text _topSpeedKMHText;
        [SerializeField] Text _topSpeedMPHText;
        [SerializeField] Text _downforceText;
        [SerializeField] Text _tyreWearText;

        void OnEnable()
        {
            Session sessionData = GameManager.F1Info.ReadSession(out bool status);
            if (status)
            {
                //_animator.SetTrigger(_animatorTrigger);
                _circuitInfoText.text = _preTrackNameString + ConvertEnumToString.Convert<Track>(sessionData.Track);

                //Get info about circuit depending on track
                FlagManager.CircuitInfoData data = FlagManager.GetCircuitInfoData(sessionData.Track);

                _trackTypeText.text = data.trackType.ToString().ToUpper();
                _fullThrottleText.text = data.fullThrottle.ToString("0.0").Replace(',', '.');
                _topSpeedKMHText.text = data.topSpeed.ToString();
                _topSpeedMPHText.text = ((ushort)(data.topSpeed * Constants.CONVERT_KMH_TO_MPH)).ToString();
                _downforceText.text = data.downforce.ToString().ToUpper();
                _tyreWearText.text = data.tyreWear.ToString().ToUpper();
            }
            else
                gameObject.SetActive(false);
        }
    }
}