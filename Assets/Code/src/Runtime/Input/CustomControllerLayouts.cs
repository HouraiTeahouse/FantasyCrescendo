using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.InputSystem.Editor;
#endif

namespace HouraiTeahouse.FantasyCrescendo {

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class CustomControllerLayouts : MonoBehaviour {

  static CustomControllerLayouts() {
    RegisterControllerLayouts();
  }

  [RuntimeInitializeOnLoadMethod]
  static void RegisterControllerLayouts() {
    InputSystem.RegisterLayout(@"
{
    ""name"" : ""Mayflash GameCube Controller Adapter"",
    ""extend"" : ""Gamepad"",
    ""format"" : ""HID"",
    ""device"" : { ""interface"" : ""HID"", ""product"" : ""Mayflash.*GameCube.*"" },
    ""controls"" : [
        { ""name"" : ""leftShoulder"", ""offset"" : 1, ""bit"" : 4 },
        { ""name"" : ""rightShoulder"", ""offset"" : 1, ""bit"" : 5 },
        { ""name"" : ""buttonSouth"", ""offset"" : 1, ""bit"" : 1 },
        { ""name"" : ""buttonEast"", ""offset"" : 1, ""bit"" : 2 },
        { ""name"" : ""buttonWest"", ""offset"" : 1, ""bit"" : 0 },
        { ""name"" : ""buttonNorth"", ""offset"" : 1, ""bit"" : 3 },
        { ""name"" : ""dpad"", ""offset"" : 9 },
        { ""name"" : ""dpad/up"", ""offset"" : 0, ""bit"" : 8 },
        { ""name"" : ""dpad/down"", ""offset"" : 0, ""bit"" : 9 },
        { ""name"" : ""dpad/left"", ""offset"" : 0, ""bit"" : 10 },
        { ""name"" : ""dpad/right"", ""offset"" : 0, ""bit"" : 11 },
        { ""name"" : ""start"", ""offset"" : 2, ""bit"" : 1 },
        { ""name"" : ""leftTrigger"", ""offset"" : 7, ""format"" : ""BYTE"" },
        { ""name"" : ""rightTrigger"", ""offset"" : 8, ""format"" : ""BYTE"" },
        { ""name"" : ""leftStick"", ""offset"" : 3, ""format"" : ""VC2S"" },
        { ""name"" : ""leftStick/x"", ""offset"" : 0, ""format"" : ""BYTE"",  ""parameters"" : ""normalize,normalizeMin=-0,normalizeZero=0.5,normalizeMax=1"" },
        { ""name"" : ""leftStick/y"", ""offset"" : 1, ""format"" : ""BYTE"",  ""parameters"" : ""normalize,normalizeMin=-0,normalizeZero=0.5,normalizeMax=1,invert"" },
        { ""name"" : ""rightStick"", ""offset"" : 5, ""format"" : ""VC2S"" },
        { ""name"" : ""rightStick/y"", ""offset"" : 0, ""format"" : ""BYTE"",  ""parameters"" : ""normalize,normalizeMin=-0,normalizeZero=0.5,normalizeMax=1,invert""},
        { ""name"" : ""rightStick/x"", ""offset"" : 1, ""format"" : ""BYTE"",   ""parameters"" : ""normalize,normalizeMin=-0,normalizeZero=0.5,normalizeMax=1"" }
    ]
}
    ");
  }

}

}