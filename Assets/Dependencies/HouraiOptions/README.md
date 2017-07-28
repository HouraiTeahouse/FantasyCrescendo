# HouraiOptions

A Unity3D library for creating, managing, and persisting game settings and 
options in a straightfoward, code-first manner.

The values for these are stored in PlayerPrefs in an unencrypted form. The 
expectation is that these values make no signifigant impact on the game.

* [Installation](#installation)
* [Setup](#setup)
* [Extensions](#extensions)
* [Using the Library](#using-the-library)
  * [Creating Options](#creating-options)
  * [Reading/Setting Option Values](#reading-setting-option-values)
  * [Listening for Changes](#listening-for-changes)

# Installation
0. Install the following dependencies:
  * [HouraiLib](https://github.com/HouraiTeahouse/HouraiLib)
0. Install the latest compatible *.unitypackage from the 
   [releases page](https://github.com/HouraiTeahouse/HouraiOptions/releases)
   for the target Unity3D version.
0. (Optional): Alternatively simply load the files from the git repository
   instead of unitypackage.

## Setup
The only requirement to set up is to create a GameObject with a `OptionsManager`
component. 

It is strongly recommended to mark the GameObject the OptionsManager is on
as `DontDestroyOnLoad` so that it persists across scene loads.

## Extensions

* [HouraiOptions.UI](https://github.com/HouraiTeahouse/HouraiOptions.UI) - 
  Automatic UI generation for 

## Using the Library

### Creating Options
```csharp
// Denote a class as an option category with the OptionCategory attribute
[OptionCategory("Audio Options")]
public class AudioOptions {
    
    // Denote a single option with the Option attribute
    // Only valid on properties, does not work on fields.
    // Can work on private fields.
    [Option]
    public float MasterVolume { get; set; }

    // Declare a human readable name for the property.
    // If none is specified, the property's name will be used instead.
    [Option("BGM Volume")]
    public float MusicVolume { get; set; }

    // A default value can be supplied
    [Option("SFX Volume", DefaultValue=0.5f)]
    public float EffectVolume { get; set; }

}

// Leaving the name empty will use the class name as the category name
[OptionCategory]
public class SomeOptions {

    // Strings are valid
    [Option("Test Option 1")]
    public string OptionString { get; set; }

    // Booleans are valid
    [Option("Test Option 2")]
    public bool OptionBool { get; set; }

    // Integers are valid
    [Option("Test Option 3")]
    public int OptionInt { get; set; }

    public void RunSomething() {
        // Functions and other class elements will be ignored.
    }
}
```

### Reading/Setting Option Values
```csharp
// Getting reference to the OptionsManager instance:
// Note: if there does not exist a OptionManager in the loaded scene(s), this will return null.
OptionsManager optionsManager = OptionManager.Instance;

// Getting the options instance for a type: 
// (The value is a plain old C# object of the specfied type)
AudioOptions audioOptions = optionsManager.Get<AudioOptions>();

// Read the properties like any other object.
float masterVolume = audioOptions.MasterVolume;

// Set the properties like any other object.
// Note: Setting option values like this will not automatically save.
audioOptions.MasterVolume = Random.value;

// Save changes made to options objects:
optionsManager.SaveAllChanges();

// Revert changes made to options object (loads values from PlayerPrefs)
optionsManager.RevertAllChanges();
```

### Getting OptionMetadata
```csharp
// Get metadata for a type:
CategoryInfo audioCategory = OptionManager.Instance.GetInfo<AudioOptions>();

audioCategory.Name         // Get the category's human readable name (System.String)
audioCategory.Instance     // Get the object instance for the category (System.Object)
audioCategory.Type         // Get the class type of the category (System.Type)
audioCategory.Options      // Get all options under the category (IEnumerable<OptionInfo>)

// Get option metadata:
OptionInfo optionInfo = audioCategory.GetInfo("MasterVolume");

optionInfo.Name            // Get the option's human readable name (System.String)
optionInfo.PropertyInfo    // Get the option's PropertyInfo (System.Reflection.PropertyInfo)
optionInfo.CategoryInfo    // Get the option's Category (CategoryInfo)
optionInfo.Attribute       // Get the option's option attribute (OptionAttribute)
optionInfo.Key             // Get the option's PlayerPrefs key (System.String)
```

### Listening for Changes
```csharp
// Get the OptionInfo for the option:
OptionInfo volume = OptionManager.Instance.GetInfo<AudioOptions>().GetInfo("MasterVolume");

// Create a callback:
// before: The prior value.
// after: the new value.
Action<float, float> callback = (before, after) => {
    Debug.Log(string.Format("Before: {0}, After: {1}", before, after));
};

// These callbacks will be executed each time the property's value changes.

// Add a listener for it to the option
// Parameter type must match the actual type of the property. Will throw an error otherwise.
volume.AddListener<float>(callback);

// Remove a listener from thet option:
// Parameter type must match the actual type of the property. Will throw an error otherwise.
volume.RemoveListener<float>(callback);
```
