using UnityEngine;
using System.Runtime.InteropServices;
using System;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

namespace F1_Unity
{
    public class WidgetManager : MonoBehaviour
    {
        [SerializeField] Camera _camera;
        [SerializeField] EventSystem _eventSystem;
        [SerializeField] GraphicRaycaster _raycaster;

        const int GWL_EXSTYLE = -20;
        const uint WS_EX_LAYERED = 0x00080000;
        const uint WS_EX_TRANSPARENT = 0x00000020;
        const uint LWA_COLORKEY = 0x00000001;
        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

        //Creates a messageBox through windows API
        [DllImport("user32.dll")]
        public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

        //Returns pointer to the active window
        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        //Sets attributes for a window -> used to make it clickthrough
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

        //Used to make our application stay on top
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        //Used to make application interactable
        [DllImport("user32.dll")]
        static extern int SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);

        private struct MARGINS
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cxTopHeight;
            public int cxBottomHeight;
        }

        //Used to make it transparent
        [DllImport("Dwmapi.dll")]
        private static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

        IntPtr _handleWindow;

        private void Update()
        {
            #if !UNITY_EDITOR
            PointerEventData pointerEventData = new PointerEventData(_eventSystem);
            pointerEventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();

            _raycaster.Raycast(pointerEventData, results);

            SetClickThrough(results.Count == 0); 
            #endif    
        }

        /// <summary>
        /// Where is mouse on screen
        /// </summary>
        Vector3 GetMouseWorldPosition()
        {
            Vector3 worldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            worldPosition.z = 0f;
            return worldPosition;
        }

        /// <summary>
        /// If mouse is over fully transparent -> clickthrough
        /// </summary>
        /// <param name="clickThrough"></param>
        void SetClickThrough(bool clickThrough)
        {
            if (clickThrough)
                SetWindowLong(_handleWindow, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);
            else
                SetWindowLong(_handleWindow, GWL_EXSTYLE, WS_EX_LAYERED);
        }

        private void Start()
        {
            #if !UNITY_EDITOR
            StartTransparentApplication();
            #endif
        }

        /// <summary>
        /// Transforms application window to widget style transparent
        /// </summary>
        void StartTransparentApplication()
        {
            _handleWindow = GetActiveWindow();

            //-1 in margins create transparency
            MARGINS margins = new MARGINS { cxLeftWidth = -1 };
            DwmExtendFrameIntoClientArea(_handleWindow, ref margins);

            SetWindowLong(_handleWindow, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);

            SetWindowPos(_handleWindow, HWND_TOPMOST, 0, 0, 0, 0, 0);
        }
    }
}