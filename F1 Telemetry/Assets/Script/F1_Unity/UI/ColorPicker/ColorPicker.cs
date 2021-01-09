using UnityEngine;
using UnityEngine.UI;

namespace F1_Unity
{
    /// <summary>
    /// Allows user to choose specific color, sends out event when closing and/or saving
    /// </summary>
    public class ColorPicker : MonoBehaviour
    {
        [SerializeField, Range(0, 1000)] int _textureLength;

        [SerializeField] Color _testColor;
        [SerializeField] Image _previewImage;
        [SerializeField] FilterMode _filterMode;

        [SerializeField] Image _hueImage;
        [SerializeField] Image _saturationImage;
        [SerializeField] Image _lightnessImage;

        [SerializeField] Slider _hueSlider;
        [SerializeField] Slider _saturationSlider;
        [SerializeField] Slider _lightnessSlider;

        HSLColor _hslColor;

        private void Awake()
        {
            Init(_testColor);
        }

        /// <summary>
        /// Initilized textures and _hslVector from sliders
        /// </summary>
        public void Init(Color startColor)
        {
            _hslColor = HSLColor.HSLFromRGB(startColor);
            _hueSlider.value = _hslColor.h;
            _saturationSlider.value = _hslColor.s;
            _lightnessSlider.value = _hslColor.l;

            UpdateImages();
            UpdatePreview();
        }

        /// <summary>
        /// Update _hslVector from slider value change indicated by which slider
        /// </summary>
        /// <param name="sliderType">What slider</param>
        public void SliderChange(int sliderType)
        {
            HSLType type = (HSLType)sliderType;

            switch (type)
            {
                case HSLType.Hue:        { _hslColor.h = _hueSlider.value;        break; }
                case HSLType.Saturation: { _hslColor.s = _saturationSlider.value; break; }
                case HSLType.lightness:  { _hslColor.l = _lightnessSlider.value;  break; }
                default: throw new System.Exception("This HSLType is not implemented yet: " + type);
            }

            UpdateImages();
            UpdatePreview();
        }

        /// <summary>
        /// Updates all textures from current _hslVector
        /// </summary>
        void UpdateImages()
        {
            SetSliderImage(HSLType.Hue, _hueImage);
            SetSliderImage(HSLType.Saturation, _saturationImage);
            SetSliderImage(HSLType.lightness, _lightnessImage);
        }

        /// <summary>
        /// Sets the color of the preview based on current HSL values
        /// </summary>
        void UpdatePreview()
        {
            _previewImage.color = _hslColor.RGBColor();
        }

        /// <summary>
        /// Generates a sprite from _hslVector based on hue, saturation or lightness
        /// </summary>
        void SetSliderImage(HSLType type, Image modifyImage)
        {
            Texture2D texture = new Texture2D(_textureLength, 1);
            Color[] colorArray = GetColorArray(type);
            texture.SetPixels(colorArray);
            texture.filterMode = _filterMode;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.Apply();
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            modifyImage.sprite = sprite;
        }

        /// <summary>
        /// Gets a colorArray _textureLength x 1 based on HSLType
        /// </summary>
        Color[] GetColorArray(HSLType type)
        {
            Color[] colorArray = new Color[_textureLength];

            for (int i = 0; i < _textureLength; i++)
            {
                float point = i / (float)_textureLength;
                Color color;
                switch (type)
                {
                    case HSLType.Hue:        { color = HSLColor.RGBColor(point, _hslColor.s, _hslColor.l); break; }
                    case HSLType.Saturation: { color = HSLColor.RGBColor(_hslColor.h, point, _hslColor.l); break; }
                    case HSLType.lightness:  { color = HSLColor.RGBColor(_hslColor.h, _hslColor.s, point); break; }
                    default: throw new System.Exception("This HSLType is not implemented yet: " + type);
                }
                colorArray[i] = color;
            }

            return colorArray;
        }

        enum HSLType
        {
            Hue,
            Saturation,
            lightness
        }
    }
}