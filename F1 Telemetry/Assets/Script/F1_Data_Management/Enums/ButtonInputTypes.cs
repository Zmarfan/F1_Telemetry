namespace F1_Data_Management
{
    /// <summary>
    /// Holds all enums but also their relative AND value to read from data
    /// </summary>
    public enum ButtonInputTypes
    {
        A_Button = 0x0001,
        Y_Button = 0x0002,
        B_Button = 0x0004,
        X_Button = 0x0008,

        D_Pad_Left = 0x0010,
        D_Pad_Right = 0x0020,
        D_Pad_Up = 0x0040,
        D_Pad_Down = 0x0080,

        Options_Or_Menu = 0x0100,
        LB = 0x0200,
        RB = 0x0400,
        LT = 0x0800,

        RT = 0x1000,
        Left_Stick_Click = 0x2000,
        Right_Stick_Click = 0x4000
    }
}