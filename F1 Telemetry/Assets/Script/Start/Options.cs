using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using F1_Data_Management;

namespace F1_Options
{
    public class Options : MonoBehaviour
    {
        [SerializeField] Color _openTabColor;
        [SerializeField] Color _closedTabColor;
        //Indexed through OptionTabs enum
        [SerializeField] ColorSettings _colorSettings;
        [SerializeField] OptionTab[] _optionTabs;
        [SerializeField] GameObject[] _optionTabsArea;

        OptionTabs _currentTabOpen = OptionTabs.None;

        #region Start

        private void Awake()
        {
            CloseAllTabsOpenOne(null);
            _colorSettings.Init();
        }

        #endregion

        #region OptionData

        /// <summary>
        /// Creates OptionData from current settings to send to main application
        /// </summary>
        public OptionData OptionData
        {
            get { return new OptionData() { teamColorData = _colorSettings.GetData }; }
        }

        #endregion

        #region Tabs

        /// <summary>
        /// Open area for specified area and close all other -> change color on option tabs
        /// </summary>
        /// <param name="optionTabsIndex"></param>
        public void OpenTab(int optionTabsIndex)
        {
            OptionTabs tab = (OptionTabs)optionTabsIndex;

            //Tab already open -> close all
            if (_currentTabOpen == tab)
            {
                _currentTabOpen = OptionTabs.None;
                CloseAllTabsOpenOne(null);
            }
            else
            {
                _currentTabOpen = tab;
                CloseAllTabsOpenOne(_optionTabs[(int)tab]);
            }
        }

        /// <summary>
        /// Change all option tabs to closed except the exception which is opened
        /// </summary>
        /// <param name="exception">What tab should be opened (null if none)</param>
        void CloseAllTabsOpenOne(OptionTab exception)
        {
            for (int i = 0; i < _optionTabs.Length; i++)
                SetTabActivation(_optionTabs[i], _optionTabsArea[i], _optionTabs[i] == exception);
        }

        /// <summary>
        /// Opens or closes specific tab and it's corresponding tab area
        /// </summary>
        /// <param name="tab">Tab script</param>
        /// <param name="tabArea">Setting area for this tab</param>
        /// <param name="open">Opens or closes</param>
        void SetTabActivation(OptionTab tab, GameObject tabArea, bool open)
        {
            tab.SetDarkenColor(open ? _openTabColor : _closedTabColor);
            tabArea.SetActive(open);
        }

        public enum OptionTabs
        {
            General,
            Controls,
            Color,
            None
        }

        #endregion
    }

    /// <summary>
    /// Package full of data from options
    /// </summary>
    public struct OptionData
    {
        public Dictionary<Team, TeamColorData> teamColorData;
    }
}