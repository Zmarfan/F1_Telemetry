using UnityEngine.UI;
using UnityEngine;
using F1_Data_Management;

namespace F1_Unity
{
    public abstract class ActivatableBase : MonoBehaviour
    {
        [Header("Drop")]

        [SerializeField] protected CanvasGroup _canvasGroup;

        [Header("Left Driver")]

        [SerializeField] protected Text _driver1PositionText;
        [SerializeField] protected Text _driver1NameText;
        [SerializeField] protected Text _driver1NumberText;
        [SerializeField] protected Image _driver1TeamImage;
        [SerializeField] protected Image _driver1PortraitImage;
        [SerializeField] protected Image _driver1TeamStripeImage;

        [Header("Right Driver")]

        [SerializeField] protected Text _driver2PositionText;
        [SerializeField] protected Text _driver2NameText;
        [SerializeField] protected Text _driver2NumberText;
        [SerializeField] protected Image _driver2TeamImage;
        [SerializeField] protected Image _driver2PortraitImage;
        [SerializeField] protected Image _driver2TeamStripeImage;

        //Used to check if it needs to update anything but Delta
        protected byte _driver1ID = byte.MaxValue;
        protected byte _driver2ID = byte.MaxValue;

        protected virtual void Update()
        {
            if (GameManager.F1Info.ReadyToReadFrom)
                UpdateActivatable();
            else
                Show(false);
        }

        /// <summary>
        /// Called every frame to update visuals for activatable
        /// </summary>
        protected virtual void UpdateActivatable()
        {
            bool status1 = false;
            bool status2 = false;
            DriverData d2Data = GameManager.F1Info.ReadSpectatingCarData(out status1);

            if (status1)
            {
                //Getting driverData of driver in front of spectating car
                if (d2Data.LapData.carPosition - 2 >= 0)
                {
                    DriverData d1Data = DriverDataManager.GetDriverFromPosition(d2Data.LapData.carPosition - 1, out status2);
                    SetData(d1Data, d2Data);
                }
                //It's the leader
                else
                {
                    //Read car behind spectator if he is leading
                    DriverData d1Data = DriverDataManager.GetDriverFromPosition(d2Data.LapData.carPosition + 1, out status2);
                    SetData(d2Data, d1Data);
                }
            }

            if (status1 && status2)
                Show(true);
            else
                Show(false);
        }

        /// <summary>
        /// If there is no valid data to look at -> Don't show any
        /// </summary>
        protected virtual void Show(bool active)
        {
            _canvasGroup.alpha = active ? 1.0f : 0.0f;
        }

        protected abstract void SetData(DriverData d1Data, DriverData d2Data);

        /// <summary>
        /// Sets all visuals, only done when changing subjects to spectate.
        /// </summary>
        protected virtual void SetVisuals(DriverData d1Data, DriverData d2Data)
        {
            _driver1PositionText.text = d1Data.LapData.carPosition.ToString();
            _driver1NameText.text = ParticipantManager.GetNameFromNumber(d1Data.RaceNumber).ToUpper();
            _driver1NumberText.text = "<i>" + d1Data.RaceNumber + "</i>";
            _driver1TeamImage.sprite = ParticipantManager.GetTeamSprite(d1Data.ParticipantData.team);
            _driver1PortraitImage.sprite = ParticipantManager.GetPortraitFromNumber(d1Data.RaceNumber);
            _driver1TeamStripeImage.color = TeamColor.GetColorByTeam(d1Data.ParticipantData.team);

            _driver2PositionText.text = d2Data.LapData.carPosition.ToString();
            _driver2NameText.text = ParticipantManager.GetNameFromNumber(d2Data.RaceNumber).ToUpper();
            _driver2NumberText.text = "<i>" + d2Data.RaceNumber + "</i>";
            _driver2TeamImage.sprite = ParticipantManager.GetTeamSprite(d2Data.ParticipantData.team);
            _driver2PortraitImage.sprite = ParticipantManager.GetPortraitFromNumber(d2Data.RaceNumber);
            _driver2TeamStripeImage.color = TeamColor.GetColorByTeam(d2Data.ParticipantData.team);
        }
    }
}