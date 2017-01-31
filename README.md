# DTCommandPalette
Command palette for Unity - run methods, open scenes, and more!

### To install:
Clone the git repository and add the entire folder to your Unity project.

### Supported Versions:
Tested on Unity 5.4. It will probably work on other similar versions..

### Features:
Open..

`%t (Cmd-T on Mac / Control-T on Windows)`
* Open Scenes
* Select GameObjects in the Scene
* Open Prefabs into a sandbox scene (requires [PrefabSandbox](https://github.com/DarrenTsung/DTPrefabSandbox))

![CommandPaletteScreenshot](CommandPaletteScreenshot.png)

Command Palette

`%#m (Cmd-Shift-M on Mac / Control-Shift-M on Windows)`
* Run commands (any method with [MethodCommand] attribute can be run)

![CommandPaletteShowoff](CommandPaletteShowoff.gif)

Example (C#):
```csharp
using DTCommandPalette;
using UnityEngine;

public static class ExampleClass {
  [MethodCommand]
  public static void DeletePlayerPrefs() {
    PlayerPrefs.DeleteAll();
  }
}

public class ExampleMonoBehaviour : MonoBehaviour {
  [SerializeField] private Image _image;

  // This method command will only appear if
  // the currently selected GameObject has an
  // ExampleMonoBehaviour on it
  [MethodCommand]
  public void TurnYellow() {
    _image.color = Color.yellow;
  }
}
```
