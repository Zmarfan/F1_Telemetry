using UnityEngine;
/// <summary>
/// The Starting point for program
/// </summary>
public class DreamCommentator : MonoBehaviour
{
    [Header("Settings")]

    [SerializeField] string _participantFileExtension = ".txt";

    [Header("Drop")]

    [SerializeField] GameObject _main;
    [SerializeField] Transform _fileExplorerHolder;
    [SerializeField] GameObject _fileExplorerPrefab;


    FileExplorer.FileExplorer _currentFileExplorer = null;
    string _participantFilePath = string.Empty;

    /// <summary>
    /// Player pressed Quit
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

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

    #region Participant Location

    /// <summary>
    /// Opens file explorer to select participant text file location
    /// </summary>
    public void ParticipantLocation()
    {
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
            _participantFilePath = filePath;
            _currentFileExplorer.OpenFile -= ParticipantLocationOpen;
            _currentFileExplorer.ClosedFileExplorer -= ClosedFileExplorer;
            Destroy(_currentFileExplorer.gameObject);

            //SetParticipantData(_participantFilePath);
        }
    }

    #endregion

    /// <summary>
    /// enable or disable main window.
    /// </summary>
    void Show(bool status)
    {
        _main.SetActive(status);
    }
}
