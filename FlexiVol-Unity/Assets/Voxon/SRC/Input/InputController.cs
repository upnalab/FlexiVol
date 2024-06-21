using System.IO;
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Voxon
{
    /// <summary>  
    ///  Keycodings for Keyboard Input
    ///  </summary>
    ///  <remarks>
    ///  Default keybinding file name is default.json (clearing the file name without entering a new will load this file)
    ///  Default return value from key or button functions is Keys._ or Buttons._ .
    /// </remarks>
    public enum Keys
    {
        _ = 0x00,
        Escape = 0x01,
        _1 = 0x02,
        _2 = 0x03,
        _3 = 0x04,
        _4 = 0x05,
        _5 = 0x06,
        _6 = 0x07,
        _7 = 0x08,
        _8 = 0x09,
        _9 = 0x0A,
        _0 = 0x0B,

        A = 0x1E,
        B = 0x30,
        C = 0x2E,
        D = 0x20,
        E = 0x12,
        F = 0x21,
        G = 0x22,
        H = 0x23,
        I = 0x17,
        J = 0x24,
        K = 0x25,
        L = 0x26,
        M = 0x32,
        N = 0x31,
        O = 0x18,
        P = 0x19,
        Q = 0x10,
        R = 0x13,
        S = 0x1F,
        T = 0x14,
        U = 0x16,
        V = 0x2F,
        W = 0x11,
        X = 0x2D,
        Y = 0x15,
        Z = 0x2C,

        Alt_Left = 0x38,
        Alt_Right = 0xB8,
        Backspace = 0x0E,
        CapsLock = 0x3A,
        Comma = 0x33,
        Control_Left = 0x1D,
        Control_Right = 0x9D,
        Delete = 0xD3,
        Divide = 0x35,
        Dot = 0x34,
        End = 0xCF,
        Enter = 0x1C,
        Equals = 0x0D,
        Home = 0xC7,
        Insert = 0xD2,
        Minus = 0x0C,
        NumLock = 0x45,
        PageDown = 0xD1,
        PageUp = 0xC9,
        Pause = 0xC5,
        PrintScreen = 0xB7,
        SecondaryAction = 0xDD,
        SemiColon = 0x27,
        ScrollLock = 0x46,
        Shift_Left = 0x2A,
        Shift_Right = 0x36,
        SingleQuote = 0x28,
        Space = 0x39,
        SquareBracket_Open = 0x1A,
        SquareBracket_Close = 0x1B,
        Tab = 0x0F,
        Tilde = 0x29,
        //BackSlash = 0x2B, (Owned by VX1)
    
        F1 = 0x3B,
        F2 = 0x3C,
        F3 = 0x3D,
        F4 = 0x3E,
        F5 = 0x3F,
        F6 = 0x40,
        F7 = 0x41,
        F8 = 0x42,
        F9 = 0x43,
        F10 = 0x44,
        F11 = 0x57,
        F12 = 0x58,

        NUMPAD_Divide = 0xB5,
        NUMPAD_Multiply = 0x37,
        NUMPAD_Minus = 0x4A,
        NUMPAD_Plus = 0x4E,
        NUMPAD_Enter = 0x9C,

        NUMPAD_0 = 0x52,
        NUMPAD_1 = 0x4F,
        NUMPAD_2 = 0x50,
        NUMPAD_3 = 0x51,
        NUMPAD_4 = 0x4B,
        NUMPAD_5 = 0x4C,
        NUMPAD_6 = 0x4D,
        NUMPAD_7 = 0x47,
        NUMPAD_8 = 0x48,
        NUMPAD_9 = 0x49,
        NUMPAD_Dot = 0x53,
        
        ARROW_Up = 0xC8,
        ARROW_Down = 0xD0,
        ARROW_Left = 0xCB,
        ARROW_Right = 0xCD

    };

    /// <summary>  
    ///  Keycoding for XBox controller buttons
    ///  </summary>
    public enum Buttons
    {
        _,
        DPad_Up = 0x0001,
        DPad_Down = 0x0002,
        DPad_Left = 0x0004,
        DPad_Right = 0x0008,
        Start = 0x0010,
        Back = 0x0020,
        Left_Thumb = 0x0040,
        Right_Thumb = 0x0080,
        Left_Shoulder = 0x0100,
        Right_Shoulder = 0x0200,
        A = 0x1000,
        B = 0x2000,
        X = 0x4000,
        Y = 0x8000
    };

    /// <summary>  
    ///  Keycoding for XBox controller Axis
    ///  </summary>
    public enum Axis
    {
        _,
        LeftTrigger = 1,
        RightTrigger = 2,
        LeftStickX = 3,
        LeftStickY = 4,
        RightStickX = 5,
        RightStickY = 6
    }

    public enum Mouse_Button
    {
        _,
        Mouse_1 = 0x1,
        Mouse_2 = 0x2,
        Mouse_3 = 0x4
    }
    
    public enum SpaceNav_Button
    {
        _,
        SpaceNav_1 = 0x1,
        SpaceNav_2 = 0x2,
    }

    public struct MousePosition
    {
        public float X;
        public float Y;
        public float Z;

        public MousePosition(Vector3 pos)
        {
            X = pos.x / 100;
            Y = pos.y / 100;
            Z = pos.z / 100;
        }
        public MousePosition(float[] pos)
        {
            X = pos[0] / 100;
            Y = pos[1] / 100;
            Z = pos[2] / 100;
        }
    }

    [System.Serializable] public class KeyBindings : SerializableDictionary<string, Keys> { public KeyBindings():base(new System.Collections.Generic.StaticStringComparer()) { } }
    [System.Serializable] public class MouseBindings : SerializableDictionary<string, Mouse_Button> { public MouseBindings() : base(new System.Collections.Generic.StaticStringComparer()) { } }
    [System.Serializable] public class SpaceNavBindings : SerializableDictionary<string, SpaceNav_Button> { public SpaceNavBindings() : base(new System.Collections.Generic.StaticStringComparer()) { } }
    [System.Serializable] public class ButtonBindings : SerializableDictionary<string, Buttons> { public ButtonBindings() : base(new System.Collections.Generic.StaticStringComparer()) { } }
    [System.Serializable] public class AxisBindings : SerializableDictionary<string, Axis> { public AxisBindings() : base(new System.Collections.Generic.StaticStringComparer()) { } }

    /// <summary>  
    ///  Input Controller handles keybindings, Allows Saving and Loading of keybinding.json files
    ///  </summary>
    public class InputController : Singleton<InputController>
    {
        [Header("Load / Save")] public string filename = "default.json";

        [FormerlySerializedAs("Keyboard")] [Header("Keyboard")]
        public KeyBindings keyboard = new KeyBindings();

        [FormerlySerializedAs("Mouse")] [Header("Mouse")]
        public MouseBindings mouse = new MouseBindings();

        [Header("SpaceNav")] 
        public SpaceNavBindings spacenav = new SpaceNavBindings();

        [FormerlySerializedAs("J1Buttons")] [Header("Joy 1")]
        public ButtonBindings j1Buttons = new ButtonBindings();

        [FormerlySerializedAs("J1Axis")] public AxisBindings j1Axis = new AxisBindings();

        [FormerlySerializedAs("J2Buttons")] [Header("Joy 2")]
        public ButtonBindings j2Buttons = new ButtonBindings();

        [FormerlySerializedAs("J2Axis")] public AxisBindings j2Axis = new AxisBindings();

        [FormerlySerializedAs("J3Buttons")] [Header("Joy 3")]
        public ButtonBindings j3Buttons = new ButtonBindings();

        [FormerlySerializedAs("J3Axis")] public AxisBindings j3Axis = new AxisBindings();

        [FormerlySerializedAs("J4Buttons")] [Header("Joy 4")]
        public ButtonBindings j4Buttons = new ButtonBindings();

        [FormerlySerializedAs("J4Axis")] public AxisBindings j4Axis = new AxisBindings();


        // Use this for initialization
        private void Start()
        {
            LoadData();
        }

        public static void LoadData()
        {
            try {
                if (Instance.filename == null) { return; }
                if (PlayerPrefs.GetInt("Voxon_VXProcessReportingLevel")  >= (int)VXProcessReportLevel.Processes) Debug.Log($"InputController.cs - Loaded: {Instance.filename} containing input bindings for the scene.");
            } 
            catch  (Exception e)
            {
             Debug.LogError(" Error couldn't Load Data from Input Controller");
             Debug.LogError(e.Message);

            }
            string filePath = Path.Combine(Application.streamingAssetsPath, Instance.filename);
            if (File.Exists(filePath))
            {
                // Read the JSON from the file into a string
                string dataAsJson = File.ReadAllText(filePath);
                // Pass the JSON to JSON Utility, and tell it to make a gameobject from it
                var loaded = JsonUtility.FromJson<InputData>(dataAsJson);

                loaded.To_IC();
            }
            else
            {
                if (PlayerPrefs.GetInt("Voxon_VXProcessReportingLevel")  >= (int)VXProcessReportLevel.General)  Debug.Log("InputController.cs - Cannot load input data. Creating: " + filePath);
                VXProcess.add_log_line("Cannot load input data. Creating: " + filePath);
                if (!Directory.Exists(Application.streamingAssetsPath))
                {
                    var file = new FileInfo(filePath);
                    file.Directory?.Create();
                }

                SaveData();
            }
        }

        public static void SaveData()
        {
            var save = new InputData();
            save.From_IC();

            string dataAsJson = JsonUtility.ToJson(save, true);
            string filePath = Path.Combine(Application.streamingAssetsPath, Instance.filename);

            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                var file = new FileInfo(filePath);
                file.Directory?.Create();
            }

            File.WriteAllText(filePath, dataAsJson);
        }

        public static Keys GetKey(string key)
        {
            if (Instance.keyboard.ContainsKey(key))
            {
                return Instance.keyboard[key];
            }
            else
            {
                if (PlayerPrefs.GetInt("Voxon_VXProcessReportingLevel")  >= (int)VXProcessReportLevel.General) Debug.Log($"InputController.cs / Inputer Manager - Does not contain this string: {key} Add input in Voxon -> Input Manager");
                if (PlayerPrefs.GetInt("Voxon_VXProcessReportingLevel")  >= (int)VXProcessReportLevel.General) VXProcess.add_log_line($"Input Manager does not contain this string: {key}");
                return Keys._;
            }
        }

        public static Buttons GetButton(string button, int joystick)
        {
            switch (joystick)
            {
                case 0:
                    return Instance.j1Buttons.ContainsKey(button) ? Instance.j1Buttons[button] : Buttons._;

                case 1:
                    return Instance.j2Buttons.ContainsKey(button) ? Instance.j2Buttons[button] : Buttons._;

                case 2:
                    return Instance.j3Buttons.ContainsKey(button) ? Instance.j3Buttons[button] : Buttons._;

                case 3:
                    return Instance.j4Buttons.ContainsKey(button) ? Instance.j4Buttons[button] : Buttons._;
            }

            return Buttons._;
        }

        public static Axis GetAxis(string axis, int joystick)
        {
            switch (joystick)
            {
                case 0:
                    return Instance.j1Axis.ContainsKey(axis) ? Instance.j1Axis[axis] : Axis._;

                case 1:
                    return Instance.j2Axis.ContainsKey(axis) ? Instance.j2Axis[axis] : Axis._;

                case 2:
                    return Instance.j3Axis.ContainsKey(axis) ? Instance.j3Axis[axis] : Axis._;

                case 3:
                    return Instance.j4Axis.ContainsKey(axis) ? Instance.j4Axis[axis] : Axis._;

            }

            return Axis._;
        }

        public static Mouse_Button GetMouseButton(string key)
        {
            if (Instance.mouse.ContainsKey(key))
            {
                return Instance.mouse[key];
            }
            else
            {
                if (PlayerPrefs.GetInt("Voxon_VXProcessReportingLevel")  >= (int)VXProcessReportLevel.General) Debug.Log($"InputController.cs / Inputer Manager - Does not contain this string: {key} Add input in Voxon -> Input Manager");
                return Mouse_Button._;
            }
        }

        public static SpaceNav_Button GetSpaceNavButton(string key)
        {
            if (Instance.spacenav.ContainsKey(key))
            {
                return Instance.spacenav[key];
            }
            else
            {
                if (PlayerPrefs.GetInt("Voxon_VXProcessReportingLevel")  >= (int)VXProcessReportLevel.General) Debug.Log($"InputController.cs / Inputer Manager - Does not contain this string: {key} Add input in Voxon -> Input Manager");
                return SpaceNav_Button._;
            }
        }
    

    [ExecuteInEditMode]
        private void OnEnable()
        {
            if (filename == "")
            {
                filename = "default.json";
            }
        }

        private void OnValidate()
        {
            if (filename == "")
            {
                filename = "default.json";
            }
        }
    }
}