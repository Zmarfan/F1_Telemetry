using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace FileExplorer
{
    public delegate void PressedIcon(string name, string extension, string filePath);

    public class Icon : MonoBehaviour
    {
        [SerializeField, Range(0.01f, 5f)] float _doubleClickWindow = 0.5f;
        [SerializeField] Color _notSupportedColor;
        [SerializeField] Text _nameText;
        [SerializeField] Image _spriteImage;
        [SerializeField] Image _buttonImage;
        [SerializeField] Button _button;

        public event PressedIcon ClickedIcon;
        public event PressedIcon DoubleClickedIcon;

        bool _clickedWillDoubleClick = false;
        string _name; 
        string _extension;
        string _filePath;

        public void SetValues(string name, string extension, string filePath, Sprite sprite, bool supported)
        {
            _name = name;
            _extension = extension;
            _filePath = filePath;

            _spriteImage.sprite = sprite;
            _buttonImage.sprite = sprite;
            _nameText.text = name;

            //Don't allow usage of item if it's not supported
            if (!supported)
            {
                _spriteImage.color = _notSupportedColor;
                _button.enabled = false;
                _buttonImage.enabled = false;
            }
        }

        /// <summary>
        /// Gets called when icon is clicked -> Invoke event that it was clicked
        /// </summary>
        public void ClickedIconButton()
        {
            //Double click -> open
            if (_clickedWillDoubleClick)
                DoubleClickedIcon?.Invoke(_name, _extension, _filePath);
            //Single click -> select
            else
            {
                ClickedIcon?.Invoke(_name, _extension, _filePath);
                StartCoroutine(Clicked());
            }
        }

        /// <summary>
        /// Keeps track of timing window for double click
        /// </summary>
        IEnumerator Clicked()
        {
            _clickedWillDoubleClick = true;
            yield return new WaitForSecondsRealtime(_doubleClickWindow);
            _clickedWillDoubleClick = false;
        }
    }
}