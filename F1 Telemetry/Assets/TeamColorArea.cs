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

        [SerializeField] GameObject teamColorOptionPrefab;

        [Header("Drop")]

        [SerializeField] Transform _spawnArea;

        List<TeamColorOption> _areaOptions = new List<TeamColorOption>();

        /// <summary>
        /// Instantiate color option for each team and set up event listeners on all of them
        /// </summary>
        public void SetUpTeams(List<TeamColorData> teamColorList)
        {
            for (int i = 0; i < teamColorList.Count; i++)
                SpawnTeamColorOption(teamColorList[i]);
        }

        /// <summary>
        /// Return a list of all this area's options teamcolor data. Used to save the modified data
        /// </summary>
        /// <returns></returns>
        public List<TeamColorData> GetAreaColorData()
        {
            List<TeamColorData> list = new List<TeamColorData>();
            for (int i = 0; i < _areaOptions.Count; i++)
                list.Add(_areaOptions[i].TeamColor);
            return list;
        }

        /// <summary>
        /// Spawns a team color option and sets up listener to its event
        /// </summary>
        /// <param name="teamColorPair"></param>
        void SpawnTeamColorOption(TeamColorData teamColorPair)
        {
            GameObject obj = Instantiate(teamColorOptionPrefab, _spawnArea) as GameObject;
            TeamColorOption script = obj.GetComponent<TeamColorOption>();
            script.Init(teamColorPair);
            _areaOptions.Add(script);
        }
    }
}