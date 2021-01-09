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
        [SerializeField] ColorPickerState _colorState = ColorPickerState.HSL;

        [SerializeField] Image _bar0Image;
        [SerializeField] Image _bar1Image;
        [SerializeField] Image _bar2Image;

        [SerializeField] Slider _bar0Slider;
        [SerializeField] Slider _bar1Slider;
        [SerializeField] Slider _bar2Slider;


        HSLColor _hslColor;
        HSVColor _hsvColor;
        Color _rgbColor;

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
            _hsvColor = HSVColor.HSVFromRGB(startColor);
            _rgbColor = startColor;

            UpdateSliderValues();
            UpdateImages();
            UpdatePreview();
        }

        /// <summary>
        /// Sets the value for the sliders
        /// </summary>
        void UpdateSliderValues()
        {
            SetBarValuesFromState(out float bar_0, out float bar_1, out float bar_2);

            _bar0Slider.value = bar_0;
            _bar1Slider.value = bar_1;
            _bar2Slider.value = bar_2;
        }

        /// <summary>
        /// Changes state for color picker (HSL -> HSV -> RGB)
        /// </summary>
        public void ChangeState()
        {
            switch (_colorState)
            {
                case ColorPickerState.HSL:
                    {
                        _hsvColor = _hslColor.HSVColor();
                        _rgbColor = _hslColor.RGBColor();
                        break;
                    }
                case ColorPickerState.HSV:
                    {
                        _hslColor = _hsvColor.HSLColor();
                        _rgbColor = _hsvColor.RGBColor();
                        break;
                    }
                case ColorPickerState.RGB:
                    {
                        _hslColor = HSLColor.HSLFromRGB(_rgbColor);
                        _hsvColor = HSVColor.HSVFromRGB(_rgbColor);
                        break;
                    }
                default: throw new System.Exception("This ColorState is not implemented yet: " + _colorState);
            }

            _colorState = (ColorPickerState)(((int)_colorState + 1) % (int)ColorPickerState.Length);

            UpdateSliderValues();
            UpdateImages();
            UpdatePreview();
        }

        /// <summary>
        /// Update _hslVector from slider value change indicated by which slider
        /// </summary>
        /// <param name="sliderType">What slider</param>
        public void SliderChange()
        {
            switch (_colorState)
            {
                case ColorPickerState.HSL:
                    {
                        _hslColor.h = _bar0Slider.value;
                        _hslColor.s = _bar1Slider.value;
                        _hslColor.l = _bar2Slider.value;
                        break;
                    }
                case ColorPickerState.HSV:
                    {
                        _hsvColor.h = _bar0Slider.value;
                        _hsvColor.s = _bar1Slider.value;
                        _hsvColor.v = _bar2Slider.value;
                        break;
                    }
                case ColorPickerState.RGB:
                    {
                        _rgbColor.r = _bar0Slider.value;
                        _rgbColor.g = _bar1Slider.value;
                        _rgbColor.b = _bar2Slider.value;
                        break;
                    }
                default: throw new System.Exception("This ColorState is not implemented yet: " + _colorState);
            }

            UpdateImages();
            UpdatePreview();
        }

        /// <summary>
        /// Updates all textures from current _hslVector
        /// </summary>
        void UpdateImages()
        {
            SetSliderImage(ColorBarType.Bar_0, _bar0Image);
            SetSliderImage(ColorBarType.Bar_1, _bar1Image);
            SetSliderImage(ColorBarType.Bar_2, _bar2Image);
        }

        /// <summary>
        /// Sets the color of the preview based on current HSL values
        /// </summary>
        void UpdatePreview()
        {
            switch (_colorState)
            {
                case ColorPickerState.HSL: _previewImage.color = _hslColor.RGBColor(); break;
                case ColorPickerState.HSV: _previewImage.color = _hsvColor.RGBColor(); break;
                case ColorPickerState.RGB: _previewImage.color = _rgbColor;            break;
                default: throw new System.Exception("This ColorState is not implemented yet: " + _colorState);
            }
        }

        /// <summary>
        /// Generates a sprite from _hslVector based on hue, saturation or lightness
        /// </summary>
        void SetSliderImage(ColorBarType type, Image modifyImage)
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
        Color[] GetColorArray(ColorBarType type)
        {
            Color[] colorArray = new Color[_textureLength];

            for (int i = 0; i < _textureLength; i++)
            {
                float point = i / (float)_textureLength;
                Color color = GetColor(type, point);
                colorArray[i] = color;
            }

            return colorArray;
        }

        /// <summary>
        /// Gets RGB color depending on ColorPickerState and for which bar it interpolates
        /// </summary>
        /// <param name="type">Which bar?</param>
        /// <param name="point">point value to replace that bars stored value</param>
        /// <returns></returns>
        Color GetColor(ColorBarType type, float point)
        {
            switch (type)
            {
                case ColorBarType.Bar_0:
                    {
                        switch (_colorState)
                        {
                            case ColorPickerState.HSL: return HSLColor.RGBColor(point, _hslColor.s, _hslColor.l);
                            case ColorPickerState.HSV: return HSVColor.RGBColor(point, _hsvColor.s, _hsvColor.v);
                            case ColorPickerState.RGB: return new Color(point, _rgbColor.g, _rgbColor.b);
                            default: throw new System.Exception("This ColorState is not implemented yet: " + _colorState);
                        }
                    }
                case ColorBarType.Bar_1:
                    {
                        switch (_colorState)
                        {
                            case ColorPickerState.HSL: return HSLColor.RGBColor(_hslColor.h, point, _hslColor.l);
                            case ColorPickerState.HSV: return HSVColor.RGBColor(_hsvColor.h, point, _hsvColor.v);
                            case ColorPickerState.RGB: return new Color(_rgbColor.r, point, _rgbColor.b);
                            default: throw new System.Exception("This ColorState is not implemented yet: " + _colorState);
                        }
                    }
                case ColorBarType.Bar_2:
                    {
                        switch (_colorState)
                        {
                            case ColorPickerState.HSL: return HSLColor.RGBColor(_hslColor.h, _hslColor.s, point);
                            case ColorPickerState.HSV: return HSVColor.RGBColor(_hsvColor.h, _hsvColor.s, point);
                            case ColorPickerState.RGB: return new Color(_rgbColor.r, _rgbColor.g, point);
                            default: throw new System.Exception("This ColorState is not implemented yet: " + _colorState);
                        }
                    }
                default: throw new System.Exception("This HSLType is not implemented yet: " + type);
            }
        }

        /// <summary>
        /// Sets bar values from hsl, hsv or rgb depending on state
        /// </summary>
        public void SetBarValuesFromState(out float bar_0, out float bar_1, out float bar_2)
        {
            switch (_colorState)
            {
                case ColorPickerState.HSL: { bar_0 = _hslColor.h; bar_1 = _hslColor.s; bar_2 = _hslColor.l; break; }
                case ColorPickerState.HSV: { bar_0 = _hsvColor.h; bar_1 = _hsvColor.s; bar_2 = _hsvColor.v; break; }
                case ColorPickerState.RGB: { bar_0 = _rgbColor.r; bar_1 = _rgbColor.g; bar_2 = _rgbColor.b; break; }
                default: throw new System.Exception("This ColorState is not implemented yet: " + _colorState);
            }
        }

        enum ColorPickerState
        {
            HSL,
            HSV,
            RGB,
            Length
        }

        /// <summary>
        /// What bar are this refering to in HSL, HSV and RGB
        /// </summary>
        enum ColorBarType
        {
            /// <summary>
            /// Hue or Red
            /// </summary>
            Bar_0,
            /// <summary>
            /// Saturation or Green
            /// </summary>
            Bar_1,
            /// <summary>
            /// Lightness, value or blue
            /// </summary>
            Bar_2
        }
    }
}