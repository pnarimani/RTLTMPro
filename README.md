# RTL Text Mesh Pro
This plugin adds Right-to-left language support to "Text Mesh Pro" Unity plugin.  
This project used [ArabicSupprt](https://www.assetstore.unity3d.com/en/#!/content/2674) for Unity Asset. [ArabicSupprt for Unity now on Github.](https://github.com/Konash/arabic-support-unity)

# Features
### Realtime RTL Text
You don't need to convert, copy and paste texts. Start writing and texts will be converted right away.  
  
![Preview](Screenshots/Realtime.gif)

### Rich Text
All `Text Mesh Pro`'s tags are available in `RTL Text Mesh Pro`
  
![Rich Text Preview](Screenshots/Rich%20Text.PNG)

### RTL InputField (See [known issues](#known-issues))
Realtime InputField is supported.  
  
![Input Field Preview](Screenshots/InputField.gif)

### Multiline
Yes, This plugin has no problem with multiline RTL texts.
  
![Multiline Preview](Screenshots/Multiline.PNG)

### AutoSize
Auto Font Size is fully supported.  
  
![AutoSize Preview](Screenshots/AutoSize.gif)

### English, Farsi and Arabic digits are supported
  
![Numbers Preview](Screenshots/Numbers.PNG)

### Arabic Tashkeel
Arabic tashkeel are supported. Also you can turn them off or on for every `RTL Text Mesh Pro` object.
  
![Tashkeel Preview](Screenshots/Arabic%20Text.PNG)

# How To Use
* Import latest unitypcakage to your project.
* Use `GameObject->UI->* - RTLTMP` menu to create RTL UI elements.
  Alternatively you can replace 

# Known Issues
* Multiline has issues with English text.
* InputField (and anything that was designed to work with TextMeshProUGUI script) will not work unless you do these steps:
  * Open `TMP_Text.cs` from TextMeshPro source code
  * add virtual keyword to text property.  
  ![Text](Screenshots/TextProperty.PNG)
  * Open `RTLTextMeshPro.cs` and uncomment the top line where it says `//#define RTL_OVERRIDE`
  * Now you can use InputFields (and Dropdowns in future)
  
# Contribution
All contributions are welcomed. Just make sure you follow the project's code style.  

