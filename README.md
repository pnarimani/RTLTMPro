# RTL Text Mesh Pro
This plugin adds Right-to-left language support to "TextMeshPro" Unity plugin. 
You need to have `TextMeshPro` plugin in your project. You can install TMPro via `Package Manager`.

Currently Arabic, Farsi and Hebrew are supported. If you find an issue with one of these languages or want support for another RTL language, open an issue.

# Attention
Looking for maintainers. Send email to sorencoder@gmail.com

# Features
### Farsi, Arabic and Hebrew
![Persian Text](Screenshots/Persian-Text.PNG)
![Arabic Text](Screenshots/Arabic-Text.PNG)
![Hebrew Text](Screenshots/Hebrew-Text.PNG)

### Realtime RTL Text
You don't need to convert, copy and paste texts. Start writing and texts will be converted right away.  
  
![Preview](Screenshots/Realtime.gif)

### Rich Text
All `Text Mesh Pro`'s tags are available in `RTL Text Mesh Pro`
  
![Rich Text Preview](Screenshots/Rich%20Text.PNG)

### RTL InputField (See [known issues](#known-issues))
Realtime InputField is supported.  
  
![Input Field Preview](Screenshots/InputField.gif)  

### RTL Dropdown (See [known issues](#known-issues))
  
![Dropdown Preview](Screenshots/Dropdown.gif)

### Multiline
Yes, This plugin has no problem with multiline RTL texts.
  
![Multiline Preview](Screenshots/Multiline.PNG)

### AutoSize
Auto Font Size is fully supported.  
  
![AutoSize Preview](Screenshots/AutoSize.gif)

### English, Farsi and Arabic digits are supported
  
![Numbers Preview](Screenshots/Numbers.PNG)

### Arabic Tashkeel
Arabic tashkeel are supported.  
  
![Tashkeel Preview](Screenshots/Arabic%20Text.PNG)  

### Zero-Width No-Joiner character support
You can insert Zero-Width No-Joiner character with Ctrl+Shift+2 hotkey.  
  
![ZWNJ Preview](Screenshots/zwnj.PNG)  

# Installation
* You need to have `TextMeshPro` plugin in your project. You can install TMPro via `Package Manager`. DO NOT Install Text Mesh Pro from Asset Store.
### from .unitypackage file
Download the latest unitypackage file from the [releases](https://github.com/sorencoder/RTLTMPro/releases) section and import it into your project from "Assets -> Import Package -> Custom Package..." menu in Unity.

### from OpenUPM
[hk1ll3r](https://github.com/hk1ll3r/) maintains a package manager version of this repo on [OpenUPM](https://openupm.com/packages/com.nosuchstudio.rtltmpro/).

In Project Settings window, add OpenUPM as a scoped registry or if you have already added it, add the new scope to it.

![ProjectSettings](Screenshots/ProjectSettings.PNG)

Then in Package Manager window, change scope to *My Registries*. Select "RTL Text Mesh Pro" package and press *Install*.

![PackageManager](Screenshots/PackageManager.PNG)

The sample scenes and demo resources (fonts, shaders, etc.) are included in the package as a .unitypackage file. You need to import those into your Assets folder to use them. From the project window navigate to the package folder and double click "RTLTMPRo-demo-resources" file to import these assets into your project.

![Project](Screenshots/Project.PNG)

# How To Use
* Open one of the range files in `Assets/RTLTMPro/Ranges/` folder using your favorite text editor.
  * RTL Letters are in `LetterRanges.txt` file
  * English, Arabic and Farsi numbers are in `NumberRanges.txt` file
  * Arabic tashil are in `TashkilRanges.txt` file.
* Make sure you have copied ranges that you want to use
* Open `Window/TextMeshPro/Font Asset Creator` window.
* Assign your font in `Font Source` field (Your font must support RTL characters)
* Set `Character Set` to `Unicode Range`
* Paste copied ranges inside  `Character Sequence (Hex)`
* Press `Generate Font Atlas` button and wait for it to generate the atlas file.
* Press `Save TextMeshPro Font Asset` and save the asset.
* Use `GameObject/UI/* - RTLTMP` menu to create RTL UI elements. (Alternatively you can replace `Text Mesh Pro UGUI` components with `RTL Text Mesh Pro`)
* Assign your font asset `Font Asset` property in `RTL Text Mesh Pro` component 
* Enter text in `RTL TEXT INPUT BOX` secion.
  
## Usage Description
### Farsi
When checked, English numbers will be converted to Farsi numbers.
When unchecked, English numbers will be converted to Arabic numbers.  

### Preserve Numbers
When checked numbers will not be converted.  

### Force Fix
RTL Text Mesh Pro does not fix texts that start with English characters. 
Checking this checkbox forces RTL TextMeshPro to fix the text even when it starts with English character. 
**Multiline English texts will have problem on components that have `ForceFix` checked.**  

### Fix Tags
When checked, RTL Text Mesh Pro will try to fix rich text tags.  

# Known Issues
* **Fixed in latest version. For older versions follow the steps below**
  We need to override the `text` property of `TextMeshProUGUI`. But the `text` property is not defined `virtual`. You need to manually make the property virtual.  
  * Open `TMP_Text.cs` from TextMeshPro source code
  * add virtual keyword to text property.  
  ![Text](Screenshots/TextProperty.PNG)
  * Open `RTLTextMeshPro.cs` and uncomment the top line where it says `//#define RTL_OVERRIDE`
  * Now you can use InputFields and Dropdowns.
  
# Contribution
All contributions are welcomed. Just make sure you follow the project's code style.  

Contact: sorencoder@gmail.com
