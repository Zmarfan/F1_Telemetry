using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;

namespace FileExplorer
{
    public delegate void ClosedFileExplorerDelegate();

    public class FileExplorer : MonoBehaviour
    {
        [Header("Settings")]

        [SerializeField] string _fileNamePre = "File Name: ";
        [SerializeField] string _fileTypePre = "File Type: ";
        [SerializeField] string _directoryTypeName = "Directory";
        [SerializeField] StartDirectories _startFolder;

        [Header("Drop")]

        [SerializeField] Button _selectButton;
        [SerializeField] Text _fileNameText;
        [SerializeField] Text _fileTypeText;
        [SerializeField] InputField _inputfield;
        [SerializeField] Transform _iconArea;
        [SerializeField] GameObject _iconPrefab;
        [SerializeField] FileTypeToSprite[] _fileTypeToSpriteList;

        public event PressedIcon OpenFile;
        public event ClosedFileExplorerDelegate ClosedFileExplorer;

        //List of all items currently showing
        List<Icon> _items = new List<Icon>();

        Dictionary<FileTypes, Sprite> _fileTypeToSprite = new Dictionary<FileTypes, Sprite>();
        Dictionary<string, FileTypes> _supportedFileTypes = new Dictionary<string, FileTypes>()
        {
            { string.Empty, FileTypes.Directory },
            { ".txt", FileTypes.txt },
            { ".png", FileTypes.png },
        };

        string _filePath;
        string _currentlySelectedItemName = string.Empty;

        string _selectedName = string.Empty;
        string _selectedExtension = string.Empty;
        string _selectedFilePath = string.Empty;

        private void Awake()
        {
            //Center the file explorer
            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            //Initilize dictionary of filetype to sprite for icon retrival
            for (int i = 0; i < _fileTypeToSpriteList.Length; i++)
                _fileTypeToSprite.Add(_fileTypeToSpriteList[i].type, _fileTypeToSpriteList[i].sprite);

            Init();
        }

        /// <summary>
        /// Search and find from default path
        /// </summary>
        public void Init()
        {
            GoSpecificPlace((int)_startFolder);
        }

        /// <summary>
        /// Called when selecting item -> Sets and display item info
        /// </summary>
        void Selected(string name, string extension, string filePath)
        {
            _selectButton.interactable = true;

            //Save data for selecting directory via button
            _selectedName = name;
            _selectedExtension = extension;
            _selectedFilePath = filePath;

            //Show selected data details
            _fileNameText.text = _fileNamePre + name;
            _fileTypeText.text = extension == string.Empty ? _fileTypePre + _directoryTypeName : _fileTypePre + extension;
        }

        /// <summary>
        /// Called when double clicking an item
        /// </summary>
        void Open(string name, string extension, string filePath)
        {
            //It's a directory -> open filepath
            if (extension == string.Empty)
            {
                _inputfield.text = filePath;
                PressedGo();
            }
            //It's a file
            else
            {
                //send event further out and let them deal with it
                OpenFile?.Invoke(name, extension, filePath);
            }
            
        }

        /// <summary>
        /// Loads in files and directories from current filepath
        /// </summary>
        public string LoadFilesAndDirectories(string filePath)
        {
            DirectoryInfo fileList;
            try
            {
                fileList = new DirectoryInfo(filePath);
                FileInfo[] files = fileList.GetFiles();
                DirectoryInfo[] directories = fileList.GetDirectories();

                //Clear file info
                _fileNameText.text = string.Empty;
                _fileTypeText.text = string.Empty;

                //clear selected data
                _selectedName = null;
                _selectedExtension = null;
                _selectedFilePath = null;

                _selectButton.interactable = false;

                //Remove all previous icons
                for (int i = 0; i < _items.Count; i++)
                {
                    _items[i].ClickedIcon -= Selected;
                    _items[i].DoubleClickedIcon -= Open;
                    Destroy(_items[i].gameObject);
                }
                _items.Clear();

                //First show all directories
                for (int i = 0; i < directories.Length; i++)
                    SpawnItem(directories[i].Name, directories[i].Extension, directories[i].FullName);

                //Then show all files
                for (int i = 0; i < files.Length; i++)
                    SpawnItem(files[i].Name, files[i].Extension, files[i].FullName);

                //The current filepath is valid -> save it
                return filePath;
            }
            catch (Exception e)
            {
                //Do nothing -> wrong input
                Debug.Log(e);
                //This filepath is invalid, disregard
                return _filePath;
            }
        }

        /// <summary>
        /// Instantiate file or directory to list and set it's values properly
        /// </summary>
        /// <param name="name">File name</param>
        /// <param name="extension">File type</param>
        /// <param name="filePath">The full path to this item</param>
        void SpawnItem(string name, string extension, string filePath)
        {
            try
            {
                //if it's a directory -> check if it's accessable -> if not don't show it
                DirectoryInfo[] directories;
                if (extension == string.Empty)
                    directories = new DirectoryInfo(filePath).GetDirectories();

                //Valid directories and files get through here
                FileTypes fileType = GetSupportedFileType(extension);
                GameObject item = Instantiate(_iconPrefab, Vector3.zero, Quaternion.identity, _iconArea) as GameObject;
                Icon script = item.GetComponent<Icon>();
                script.SetValues(name, extension, filePath, _fileTypeToSprite[fileType], fileType != FileTypes.Unknown);
                //Listen to clicked event
                script.ClickedIcon += Selected;
                script.DoubleClickedIcon += Open;
                _items.Add(script);
            }
            //Can't access directory -> don't display it
            catch {}  
        }

        /// <summary>
        /// Checks if the item being added is supported, returns true if it is and what type of item it is
        /// </summary>
        FileTypes GetSupportedFileType(string extenstion)
        {
            if (_supportedFileTypes.ContainsKey(extenstion))
                return _supportedFileTypes[extenstion];
            return FileTypes.Unknown;
        }

        /// <summary>
        /// Called when user presses close button to close file explorer
        /// </summary>
        public void Close()
        {
            //Call out that it's destroyed
            ClosedFileExplorer?.Invoke();
            Destroy(this.gameObject);
        }

        /// <summary>
        /// Called when user presses button for select -> used to open selected file or !directory!
        /// </summary>
        public void Select()
        {
            OpenFile?.Invoke(_selectedName, _selectedExtension, _selectedFilePath);
        }

        /// <summary>
        /// Move one directory backwards if possible
        /// </summary>
        public void Back()
        {
            string currentInput = _filePath;
            StringBuilder builder = new StringBuilder(currentInput);
            int lastSlashIndex = currentInput.LastIndexOf('\\');
            //Remove last slash if any
            if (lastSlashIndex == builder.Length - 1 && lastSlashIndex != -1)
                builder.Remove(lastSlashIndex, 1);

            int nextSlashEnding = currentInput.LastIndexOf('\\');
            //There exist a new slash -> remove everything after that
            if (nextSlashEnding != -1)
                builder.Remove(nextSlashEnding, builder.Length - nextSlashEnding);

            _inputfield.text = builder.ToString();
            PressedGo();
        }

        /// <summary>
        /// A search is commenced -> load url 
        /// </summary>
        public void PressedGo()
        {
            string currentFilePath = _inputfield.text;
            _filePath = LoadFilesAndDirectories(currentFilePath);
        }

        /// <summary>
        /// Search for specific pre chosen folder -> used by quick buttons in interface
        /// </summary>
        public void GoSpecificPlace(int startDirectoryIndex)
        {
            Environment.SpecialFolder specialFolder;

            switch ((StartDirectories)startDirectoryIndex)
            {
                case StartDirectories.dekstop:    specialFolder = Environment.SpecialFolder.DesktopDirectory; break;
                case StartDirectories.documents:  specialFolder = Environment.SpecialFolder.MyDocuments; break;
                case StartDirectories.favorites: specialFolder = Environment.SpecialFolder.ProgramFiles; break;
                default: specialFolder = Environment.SpecialFolder.MyDocuments; break;
            }
            _inputfield.text = Environment.GetFolderPath(specialFolder);
            PressedGo();
        }

        #region Structs and Enums

        /// <summary>
        /// Supported filetypes
        /// </summary>
        public enum FileTypes
        {
            Directory,
            txt,
            png,
            Unknown
        }

        /// <summary>
        /// Used to navigate with quick buttons from interface
        /// </summary>
        [Serializable]
        public enum StartDirectories
        {
            dekstop,
            documents,
            favorites,
        }

        /// <summary>
        /// Used to initilize dictionary for filetype sprites
        /// </summary>
        [Serializable]
        public struct FileTypeToSprite
        {
            public FileTypes type;
            public Sprite sprite;
        }

        #endregion
    }
}