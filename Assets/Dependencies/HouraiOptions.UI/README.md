# HouaiOptions.UI

An extension package for 
[HouraiOptions](https://github.com/HouraiTeahouse/HouraiOptions) that provides
automatic UI construction for options created by HouraiOptions.

Currently the only supported UI framework is uGUI, Unity3D's built in UI framework.

# Installation
0. Install the following dependencies:
  * [HouraiLib](https://github.com/HouraiTeahouse/HouraiLib)
  * [HouraiOptions](https://github.com/HouraiTeahouse/HouraiOptions)
0. Install the latest compatible *.unitypackage from the 
   [releases page](https://github.com/HouraiTeahouse/HouraiOptions.UI/releases)
   for the target Unity3D version.
0. (Optional): Alternatively simply load the files from the git repository
   instead of unitypackage.

# Usage

```csharp
// See HouraiOptioons for information about other attributes
[OptionCategory("Audio Options")]
public class SomeOptions {

    // Non-annotated options will be exposed using the default 
    [Option]
    public float OptionFloat1 { get; set; }

    // Change the drawer by providing an alternative drawer attribute
    // These alternative versions 
    [Option, Slider(0f, 1f)]
    public float OptionFloat2 { get; set; }

}
```

## Supported Controls

 * **Toggle** - A simple on/off click. Works with `bool`.
 * **IntField** - A standard text entry box for integers. Default for `int`.
 * **StringField** - A standard text entry box for strings. Default for `string`.
 * **FloatField** - A standard text entry box for floating point values. Default for `float`.
 * **Slider** - A draggable slider between two extremes. Works with `int` and `float`.
 * **Dropdown** - A dropdown to select between different values. Works with `string`.
 * **ResolutionDropdown** - A dropdown for selecting from available. Works with `string`.
