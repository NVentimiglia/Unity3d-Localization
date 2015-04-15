#Foundation Localization
3/24/2015 | Nicholas Ventimiglia | AvariceOnline.com

The localization service provides translation of UI elements and strings on demand.

- It supports csv format
- It partitions languages by folder : ie : Resources/Localization/English
- It supports multiple files : ie : Resources/Localization/English/LobbyStrings.txt
- Translate strings by asking for the key. LocalizationService.Instance.Get("key");
- Automatic update of strings using the [Localized] annotation
- Yandex translator built in (Like google translate)
- TextBinder supports uGUI
- Supports Unity3d 5


### Platforms
Desktop, Webplayer, Android, iOS, Windows Store

##Setup
- FullSerializer is required. Located in plugin folder. (https://github.com/jacobdufault/fullserializer)
- LocalizationService Scriptable object located in /Resources/
- Place CSV formatted Language files inside /Resources/Localization/{LanguageName}/{filename}.txt

####Example Csv File

````
Eamples.ExampleString1, Hello Friend
Eamples.ExampleString2, "Hello, Will you End Me ?"
Eamples.ExampleString3, My Cat is pretty lol.
````

##Yandex Translator
Editor window is located under Tools/Foundation/Yandex Translator. Add your API key (free) (https://api.yandex.com/key/form.xml?service=trnsl), select the languages you want to support and press the magic button.

##Use


####uGUI Text
Just slap the LocalizedText monobehaviour on the text field, select the file you want and select the key you want. Your ui text element will now be translated.
    
    

####Code Behind
````
    /// <summary>
    /// Example of how to localize your code behind
    /// </summary>
    [Localized("Eamples.ExampleString1")]
    public static string ExampleString = "Hello Friend";


    public void Awake()
    {
        //Localizes the example string
        LocalizationService.Instance.Localize(this);

        //alt way of getting strings
        var s = LocalizationService.Instance.Get("Eamples.ExampleString1");

        // auto magical string updates
        var s2 = ExampleString;
    }
````
    
####Changing the language
````
    /// <summary>
    /// Example of how to change the language
    /// </summary>
    public void RandomLanguage()
    {
        var languages = LocalizationService.Instance.Languages;
        
        LocalizationService.Instance.Language = Random(languages);
    }
````

    
####Language Change Events
````
        private void Awake()
        {
            LocalizationService.OnLanguageChanged += OnLocalization;
        }

        private void OnDestroy()
        {
            LocalizationService.OnLanguageChanged -= OnLocalization;
        }

        public void OnLocalization(LocalizationService localization)
        {
            GetComponent<Text>().text = localization.GetFromFile(File, Key, label.text);
        }
````

## More

Part of the Unity3d Foundation toolkit. A collection of utilities for making high quality data driven games. http://unity3dFoundation.com

- [**Tasks**](https://github.com/NVentimiglia/Unity3d-Async-Task) : An async task library for doing background work or extending coroutines with return results.
- [**Messenger**](https://github.com/NVentimiglia/Unity3d-Event-Messenger) : Listener pattern. A message broker for relaying events in a loosely coupled way. Supports auto subscription via the [Subscribe] annotation.
- [**Terminal**](https://github.com/NVentimiglia/Unity3d-uGUI-Terminal): A in game terminal for debugging !
- [**Injector**](https://github.com/NVentimiglia/Unity3d-Service-Injector): Service Injector for resolving services and other components. Supports auto injection using the [Inject] annotation
- [**DataBinding**](https://github.com/NVentimiglia/Unity3d-Databinding-Mvvm-Mvc) : For MVVM / MVC style databinding. Supports the new uGUI ui library.
- [**Localization**](https://github.com/NVentimiglia/Unity3d-Localization)   : Supports in editor translation, multiple files and automatic translation of scripts using the [Localized] annotation.
- **Cloud** : Parse-like storage and account services using a ASP.NET MVC back end. Need to authenticate your users? Reset passwords with branded emails? Save high scores or character data in a database? Maybe write your own authoritative back end? This is it.
- **Lobby** : The ultimate example scene. Everything you need to deploy for a game, minus the actual game play.

## Donations
[I accept donations via papal. Your money is an objective measure of my self esteem.](https://www.paypal.com/us/cgi-bin/webscr?cmd=_send-money&nav=1&email=nick@simplesys.us)
