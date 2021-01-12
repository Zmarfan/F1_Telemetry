using F1_Data_Management;
using UnityEngine;
using System.Collections.Generic;

namespace F1_Options
{
    /// <summary>
    /// Recieves list of Teams paired with colors and set out settings for them
    /// </summary>
    public class TeamColorArea : MonoBehaviour
    {
        [Header("Settings")]

        [SerializeField] List<TeamColorPair> testList;

        [SerializeField] GameObject teamColorOptionPrefab;

        [Header("Drop")]

        [SerializeField] Transform _spawnArea;

        //TEST ONLY
        private void Awake()
        {
            SetUpTeams(testList);
        }
        //TEST ONLY



        /// <summary>
        /// Instantiate color option for each team and set up event listeners on all of them
        /// </summary>
        public void SetUpTeams(List<TeamColorPair> teamColorList)
        {
            for (int i = 0; i < teamColorList.Count; i++)
                SpawnTeamColorOption(teamColorList[i]);
        }

        /// <summary>
        /// Spawns a team color option and sets up listener to its event
        /// </summary>
        /// <param name="teamColorPair"></param>
        void SpawnTeamColorOption(TeamColorPair teamColorPair)
        {
            GameObject obj = Instantiate(teamColorOptionPrefab, _spawnArea) as GameObject;
            TeamColorOption script = obj.GetComponent<TeamColorOption>();
            script.Init(teamColorPair);
            script.NewColor += SaveTeamColor;
        }

        #region Listener methods

        /// <summary>
        /// The user has selected a new color for a team -> save this color to this team
        /// </summary>
        /// <param name="teamColorPair"></param>
        void SaveTeamColor(TeamColorPair teamColorPair)
        {
            //DO SOME SAVING
        }

        #endregion
    }

    /// <summary>
    /// Pair of team and it's team color
    /// </summary>
    [System.Serializable]
    public struct TeamColorPair
    {
        public Team team;
        public Color currentColor;
        public Color DefaultColor { get; private set; }

        public TeamColorPair(Team team, Color currentColor, Color defaultColor)
        {
            this.team = team;
            this.currentColor = currentColor;
            this.DefaultColor = defaultColor;
        }
    }
}