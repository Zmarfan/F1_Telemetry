using F1_Data_Management;
using UnityEngine;
using System;
using FileExplorer;
using System.Collections.Generic;

namespace F1_Options
{
    /// <summary>
    /// Holds and is the main manager of all color settings
    /// </summary>
    public class ColorSettings : MonoBehaviour
    {
        [Header("Settings")]

        [SerializeField] TeamColorAreaStruct[] _teamColorAreas;
        [SerializeField] GameObject _headerPrefab;
        [SerializeField] GameObject _teamColorAreaPrefab;

        [Header("Drop")]

        [SerializeField] Transform _contentParent;

        [Header("Storage")]

        [SerializeField] TeamColorData[] _teamColors;

        Dictionary<Team, TeamColorData> _teamColorsDictionary = new Dictionary<Team, TeamColorData>();
        //Reference to check for changes when saving
        List<TeamColorArea> _teamColorAreasInUse = new List<TeamColorArea>();

        public void Init()
        {
            LoadFromMemory();
            SetupDictionary();
            SpawnWorkArea();
        }

        /// <summary>
        /// Getter for user modified data concerning coloring. 
        /// </summary>
        public Dictionary<Team, TeamColorData> GetData { get { return _teamColorsDictionary; } }

        #region Saving & Loading

        /// <summary>
        /// Runs when user presses save icon -> Save all modified color values
        /// </summary>
        public void Save()
        {
            //Loop through all areas of options to get all data
            for (int i = 0; i < _teamColorAreasInUse.Count; i++)
            {
                List<TeamColorData> data = _teamColorAreasInUse[i].GetAreaColorData();
                for (int j = 0; j < data.Count; j++)
                {
                    //Update dictionary value (this will be sent to the game)
                    _teamColorsDictionary[data[j].team] = data[j];
                    //Save to memory
                    SaveTeamData(data[j]);
                }
            }
        }

        /// <summary>
        /// Saves teamcolordata to memory by using team enum as save location
        /// </summary>
        /// <param name="data"></param>
        void SaveTeamData(TeamColorData data)
        {
            SaveSystem.Save(data.team.ToString(), data.currentColor);
        }

        /// <summary>
        /// Load in all player selected colors for teams from storage and set to current color
        /// </summary>
        void LoadFromMemory()
        {
            Array array = Enum.GetValues(typeof(Team));
            for (int i = 0; i < array.Length; i++)
            {
                object data = SaveSystem.Load(array.GetValue(i).ToString());
                if (data != null)
                {
                    Color color = (Color)data;
                    _teamColors[i].currentColor = color;
                }
            }
        }

        #endregion

        /// <summary>
        /// Initilizes dictionary to be able to bigO(1) access of team colors given a team
        /// </summary>
        void SetupDictionary()
        {
            for (int i = 0; i < _teamColors.Length; i++)
                _teamColorsDictionary.Add(_teamColors[i].team, _teamColors[i]);
        }

        #region Spawning

        /// <summary>
        /// Spawns a prefab on specified place hierchally
        /// </summary>
        GameObject SpawnGameObject(GameObject prefab, Transform parent)
        {
            return Instantiate(prefab, parent) as GameObject;
        }

        /// <summary>
        /// Creates the visual area for player to be able to pick colors for teams
        /// Also saves reference to all interactions to pick up changes on save
        /// </summary>
        void SpawnWorkArea()
        {
            for (int i = 0; i < _teamColorAreas.Length; i++)
            {
                //Spawn header
                SpawnGameObject(_headerPrefab, _contentParent).GetComponent<ColorSettingHeader>().SetHeaderText(_teamColorAreas[i].headerName);

                TeamColorArea area = SpawnGameObject(_teamColorAreaPrefab, _contentParent).GetComponent<TeamColorArea>();
                _teamColorAreasInUse.Add(area);

                //Get Color data for each team
                List<TeamColorData> data = new List<TeamColorData>();

                for (int j = 0; j < _teamColorAreas[i].teams.Length; j++)
                    data.Add(_teamColorsDictionary[_teamColorAreas[i].teams[j]]);

                area.SetUpTeams(data);
            }
        }

        #endregion

        /// <summary>
        /// Used to create the visual interface for changing colors -> has header and teams for this area
        /// </summary>
        [Serializable]
        public struct TeamColorAreaStruct
        {
            public string headerName;
            public Team[] teams;
        }
    }

    /// <summary>
    /// Pair of team and it's team color
    /// </summary>
    [Serializable]
    public struct TeamColorData
    {
        public Team team;
        public Color currentColor;
        public Color defaultColor;

        public TeamColorData(Team team, Color currentColor, Color defaultColor)
        {
            this.team = team;
            this.currentColor = currentColor;
            this.defaultColor = defaultColor;
        }
    }
}