using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FileExplorer;
using F1_Unity;

/// <summary>
/// The Starting point for program
/// </summary>
public class DreamCommentator : MonoBehaviour
{
    static readonly int PARTICIPANT_DATA_NUMBER_INDEX = 0;
    static readonly int PARTICIPANT_DATA_NAME_INDEX = 1;
    static readonly int PARTICIPANT_DATA_INITIAL_INDEX = 2;

    [Header("Settings")]

    [SerializeField] string _participantFileExtension = ".txt";
    [SerializeField] string _noValidData = "No Valid Data!";
    [SerializeField] string _validData = "Valid Data!";
    [SerializeField] Color _validDataColor = Color.green;
    [SerializeField] Color _invalidDataColor = Color.red;

    [Header("Drop")]

    [SerializeField] Button _startButton;
    [SerializeField] Text _participantDataStatusText;
    [SerializeField] GameObject _main;
    [SerializeField] Transform _fileExplorerHolder;
    [SerializeField] GameObject _fileExplorerPrefab;

    FileExplorer.FileExplorer _currentFileExplorer = null;

    //Data about participant that is sent to GameManager when starting game
    List<ParticipantManager.NumberNameStruct> _participantData;

    private void Awake()
    {
        LoadInAllData();
    }

    /// <summary>
    /// Attempt to load in data from memory
    /// </summary>
    void LoadInAllData()
    {
        //ParticipantData
        string participantFilePath = SaveSystem.LoadFilePath(SaveTypes.ParticipantData);
        if (participantFilePath != null)
            LoadInParticipantData(participantFilePath);
        //Portraits
        //string portraitFilePath = SaveSystem.LoadFilePath(SaveTypes.Portrait);
        //if (portraitFilePath != null)
        //    LoadInPortraitData(portraitFilePath);
    }

    #region Misc

    /// <summary>
    /// Checks if data is available for a safe start of the application
    /// </summary>
    void AttemptToReadyStart()
    {
        //Enable start button if data is available
        if (_participantData != null)
            _startButton.interactable = true;
    }

    /// <summary>
    /// Player pressed Quit
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// enable or disable main window.
    /// </summary>
    void Show(bool status)
    {
        _main.SetActive(status);
    }

    #endregion

    #region FileExploring

    /// <summary>
    /// Spawns a file explorer and return the script on it
    /// </summary>
    FileExplorer.FileExplorer SpawnFileExplorer()
    {
        //Hide the main window while file exploring
        Show(false);
        GameObject obj = Instantiate(_fileExplorerPrefab, Vector3.zero, Quaternion.identity, _fileExplorerHolder) as GameObject;
        return obj.GetComponent<FileExplorer.FileExplorer>();
    }

    /// <summary>
    /// Called when file explorer closes via it's close button
    /// </summary>
    void ClosedFileExplorer()
    {
        Show(true);
    }

    #endregion

    #region Participant Data Handling

    /// <summary>
    /// Attempts to read text file from filepath -> if possible -> send data to correct place, if not -> lock
    /// </summary>
    void LoadInParticipantData(string filePath)
    {
        try
        {
            string stringData = File.ReadAllText(filePath);
            string[] lines = stringData.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            //The finished data set
            var data = new List<ParticipantManager.NumberNameStruct>();

            //Loop over all lines
            for (int i = 0; i < lines.Length; i++)
            {
                //Skip blank lines
                if (lines[i] == string.Empty)
                    continue;

                string[] parts = lines[i].Split(new[] { "+" }, StringSplitOptions.None);
                //Loop over all parts of data for this driver
                for (int j = 0; j < parts.Length; j++)
                {
                    //Read in actual data for 1 driver
                    var addData = new ParticipantManager.NumberNameStruct();
                    addData.raceNumber = byte.Parse(parts[PARTICIPANT_DATA_NUMBER_INDEX]);
                    addData.name = parts[PARTICIPANT_DATA_NAME_INDEX];
                    addData.initals = parts[PARTICIPANT_DATA_INITIAL_INDEX];

                    //Add this driver data in data storage
                    data.Add(addData);
                }
            }
            //Valid data!
            HandleValidParticipantData(data, filePath);
        }
        //No tries to see if the data is as expected, is it not it will come here and get cleaned up! Expect clean data.
        catch
        {
            HandleInvalidParticipantData();
        }
    }

    /// <summary>
    /// Called when valid ParticipantData is aquired
    /// </summary>
    void HandleValidParticipantData(List<ParticipantManager.NumberNameStruct> data, string filePath)
    {
        _participantData = data;
        //Save this filepath for future use as it gave valid data
        SaveSystem.SaveFilePath(filePath, SaveTypes.ParticipantData);
        _participantDataStatusText.text = _validData;
        _participantDataStatusText.color = _validDataColor;
        AttemptToReadyStart();
    }

    /// <summary>
    /// Called when attempted reach of ParticipantData failed
    /// </summary>
    void HandleInvalidParticipantData()
    {
        _participantData = null;
        //Don't allow start of game if data isn't available
        _startButton.interactable = false;
        _participantDataStatusText.text = _noValidData;
        _participantDataStatusText.color = _invalidDataColor;
        Debug.LogWarning("Error with file reading!");
    }

    #endregion

    #region Participant File Exploring

    /// <summary>
    /// Opens file explorer to select participant text file location
    /// </summary>
    public void ParticipantLocation()
    {
        //Starting Point for reading participant data manually!
        _currentFileExplorer = SpawnFileExplorer();
        _currentFileExplorer.OpenFile += ParticipantLocationOpen;
        _currentFileExplorer.ClosedFileExplorer += ClosedFileExplorer;
    }

    /// <summary>
    /// Called when an item is selected in file explorer when looking for participant location
    /// </summary>
    /// <param name="name">Name of file</param>
    /// <param name="extension">Type of file</param>
    /// <param name="filePath">Filepath to that file</param>
    void ParticipantLocationOpen(string name, string extension, string filePath)
    {
        //It's a text file -> read in
        if (extension == _participantFileExtension)
        {
            Show(true);
            string participantFilePath = filePath;
            _currentFileExplorer.OpenFile -= ParticipantLocationOpen;
            _currentFileExplorer.ClosedFileExplorer -= ClosedFileExplorer;
            Destroy(_currentFileExplorer.gameObject);

            LoadInParticipantData(participantFilePath);
        }
    }

    #endregion

    #region Portrait File Exploring

    /// <summary>
    /// Opens file explorer to select portraits directory location
    /// </summary>
    public void PortraitLocation()
    {
        //Starting Point for reading participant data manually!
        _currentFileExplorer = SpawnFileExplorer();
        _currentFileExplorer.OpenFile += PortraitLocationOpen;
        _currentFileExplorer.ClosedFileExplorer += ClosedFileExplorer;
    }

    /// <summary>
    /// Called when a directory is selected in file explorer when looking for portrait directory location
    /// </summary>
    /// <param name="name">Name of directory</param>
    /// <param name="extension">Type of file</param>
    /// <param name="filePath">Filepath to that directory</param>
    void PortraitLocationOpen(string name, string extension, string filePath)
    {
        //It's a directory -> read in
        if (extension == string.Empty)
        {
            Show(true);
            string portraitFilePath = filePath;
            _currentFileExplorer.OpenFile -= PortraitLocationOpen;
            _currentFileExplorer.ClosedFileExplorer -= ClosedFileExplorer;
            Destroy(_currentFileExplorer.gameObject);

            //LoadInPortraitData(portraitFilePath);
        }
    }

    #endregion
}
