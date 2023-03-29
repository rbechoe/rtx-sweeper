<a name="readme-top"></a>



[![MIT License][license-shield]][license-url]
[![LinkedIn][linkedin-shield]][linkedin-url]



<!-- PROJECT LOGO -->
<br />
<div align="center">
  <a href="https://github.com/othneildrew/Best-README-Template">
    <img src="github resources/RTX Sweeper Logo.png" alt="Logo" width="80" height="80">
  </a>

  <h3 align="center">RTX Sweeper</h3>

  <p align="center">
    RTX Sweeper is essentially Minesweeper on steroids.
    <br />
    <a href="https://store.steampowered.com/app/1196300/RTX_Sweeper/"><strong>Steam Store (up-to-date) »</strong></a>
    <br />
    <br />
    <a href="https://www.ravibechoe.codes/rtx-sweeper">Project Info</a>
    ·
    <a href="https://www.raaforitme.com/rtx-sweeper">Promotional Page</a>
    ·
    <a href="https://ploopploop.itch.io/rtx-sweeper">Version 1.0 (deprecated)</a>
  </p>
</div>



<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#technical-section">Technical Section</a>
      <ul>
        <li><a href="#eventsystem">EventSystem</a></li>
      </ul>
    </li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgments">Acknowledgments</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## About The Project

[![Product Name Screen Shot][product-screenshot]](https://example.com)

This documentation serves mainly as a description about this project for my portfolio to ensure that the important files are seen. 
RTX Sweeper has been a passion project of mine since August 2021. I have spend countless hours since then working on this game. 
My main purpose was to put newly attained knowledge to good use, but eventually it evolved to a fun game on its own. 
I wanted to challenge myself to publish a full fledged game with DLC's and support to see how far my knowledge can take me. 
I also wanted to try out new things such as making my game eligible as a <a href="https://www.igdb.com/games/rtx-sweeper">streaming option</a> as well as a <a href="https://www.speedrun.com/rtx_sweeper">speedrun option</a>.

<p align="right">(<a href="#readme-top">back to top</a>)</p>



### Built With

Software used in order to create RTX Sweeper. Asset store packs are listed at <a href="#readme-acknowledgments">acknowledgments</a>.

* [![Unity3D 2021.1][Unity.com]][Unity-url]
* [![Visual Studio Community 2022][VS.com]][VS-url]

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- TECHNICAL SECTION -->
## Technical Section

In RTX Sweeper I have made a lot of systems in order to ensure that the game functions properly. Due to this I make use of interfaces, abstract classes, plenty of design patterns, and I have written my own EventSystem.

<p align="right">(<a href="#readme-top">back to top</a>)</p>



### EventSystem

My EventSystem is a simple yet effective system that gives me full control over when something happens. This is inspired by the <a href="https://en.wikipedia.org/wiki/Publish–subscribe_pattern">PubSub</a> design pattern.
The <a href="https://github.com/rbechoe/rtx-sweeper/blob/main/sweeper%20project/Assets/Scripts/Main/EventSystem.cs">EventSytem</a> works with Listeners, Invokers, and EventTypes. 
```csharp
public static class EventSystem
{
    public static Dictionary<EventType, Action> eventCollection = new Dictionary<EventType, Action>();
    public static Dictionary<EventType, Action<object>> eventCollectionParam = new Dictionary<EventType, Action<object>>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    public static void FillDictionary()
    {
        eventCollection.Clear();
        eventCollectionParam.Clear();

        foreach (EventType type in Enum.GetValues(typeof(EventType)))
        {
            if (!eventCollection.ContainsKey(type))
            {
                eventCollection.Add(type, Empty);
            }
            
            if (!eventCollectionParam.ContainsKey(type))
            {
                eventCollectionParam.Add(type, Empty);
            }
        }
    }

    private static void Empty() { }

    private static void Empty(object value) { }
}
```
The EventSystem has dictionaries with, as the keys, each possible event type. These get filled with an empty value. This is required in order to make this work so that the subscribers can listen.

<b>Listeners</b></br>
Methods subscribe to a specific event type, with or without a parameter. Whenever that event gets invoked, the method listening to it will be executed.
```csharp
private void OnEnable()
{
    EventSystem.eventCollection[EventType.UPDATE_BGM] += ApplyBGMSettings; // has no parameter
    EventSystem.eventCollectionParam[EventType.UPDATE_BGM] += ApplyBGMSettings;
}
```
To prevent reference exceptions they also need to be removed whenever the listener becomes disabled. It is essentially memory management that you are doing.
```csharp
private void OnDisable()
{
    EventSystem.eventCollection[EventType.UPDATE_BGM] -= ApplyBGMSettings; // has no parameter
    EventSystem.eventCollectionParam[EventType.UPDATE_BGM] -= ApplyBGMSettings;
}
```
There are 2 types of listeners for now. One without a paremeter.
```csharp
private void ApplyBGMSettings()
{
    foreach(AudioSource bgm in bgmSources)
    {
        bgm.volume = someValue;
    }
}
```
And one with. For this one it is important that the parameter is of type <b>object</b>. This ensures a generic setup for the Eventsystem. In order to make the parameter usable it has to be casted to a specic type, such as a float in this sample.
```csharp
private void ApplyBGMSettings(object value)
{
    foreach(AudioSource bgm in bgmSources)
    {
        bgm.volume = (float)value;
    }
}
```

<b>Invokers</b></br>
At specific points in the game I need to invoke an event type, for example when the game finishes. At this point everything listening to it will be executed. When you click on a bomb you will trigger the GameOver event type. This will ensure that you see the right window and that the timers and interactables stop functioning.
There are 2 ways to invoke an event type, without parameters it looks like this.
```csharp
public void UpdateBGMVolume()
{
    EventSystem.eventCollectionParam[EventType.UPDATE_BGM]();
}
```
With a parameter it looks like this. All listeners will receive the value from the parameter. Do note that listeners with a parameter are in a different dictionary than those without. Sometimes they have to listen to the same event, but they can choose to do nothing with the given parameter.
```csharp
public void UpdateBGMVolume()
{
    EventSystem.eventCollectionParam[EventType.UPDATE_BGM](BGMSlider.value);
}
```

<b>EventTypes</b></br>
An event type is simply just an entry in an enum. Each entry describes what it does and has a comment about whether it passes a parameter or not. 
```csharp
public enum EventType
{
    // gameplay related
    BOMB_UPDATE,        // passes int
    UPDATE_TIME,        // passes float
    START_POS,          // passes vector3
    PLANT_FLAG,         // passes vector3[]
    ADD_EMPTY,          // passes gameobject
    COUNT_BOMBS,        // passes nothing
    // gameflow related
    PREPARE_GAME,       // passes nothing
    RANDOM_GRID,        // passes nothing
}
```

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- ROADMAP -->
## Roadmap

- [x] Version 1.0: full base game released on Steam
- [x] Version 1.1: Fanatics update
- [x] Version 1.12: General update
- [x] DLC: Cosmetics
- [x] Version 1.3: Upping the standards
- [ ] Version 1.5: The Anomaly arrives
- [ ] Version 1.55: Huge Cosmetics update
- [ ] Version 1.6: Environmental puzzle improvements
- [ ] Version 1.65: Settings
    - [ ] Proper language support
    - [ ] Graphic options (for both with and without ray tracing)
- [ ] Version 1.7: Tropical Island expansion
- [ ] DLC: Multiplayer
    - [ ] 1 vs 1
    - [ ] Battle Royale
- [ ] Version 1.75: Multiplayer update

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE` for more information.

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- CONTACT -->
## Contact

Ravi Bechoe - [Portfolio](https://www.ravibechoe.codes) - rbechoe@outlook.com

Project Link: [https://github.com/rbechoe/rtx-sweeper](https://github.com/rbechoe/rtx-sweeper)

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- ACKNOWLEDGMENTS -->
<a name="readme-acknowledgments"></a>
## Acknowledgments

<b>Friends that helped</b><br>
* [Kris de Haas's Assets & Ideas](https://k4b4448.artstation.com)
* [Nathalie Verduin's UI](https://nathalieverduinart.jimdo.com)
* [Geoffrey Hendrikx's Localisation](http://www.geoffreyhendrikx.com/)
* [Yasmin van Rooijen's Animals](https://www.artstation.com/yasminvanrooijen)
* [Caspar Gelderman's Caustics](https://caspargelderman.games)

<b>Asset Store stuff</b><br>
* [SineVFX](https://assetstore.unity.com/packages/vfx/particles/spells/top-down-effects-191455)
* [Sabri Ayes](https://assetstore.unity.com/packages/3d/props/weapons/free-missile-72692)
* [IGIIID](https://assetstore.unity.com/packages/3d/vehicles/air/super-spitfire-53217)
* [CH Assets](https://assetstore.unity.com/packages/3d/environments/landscapes/lowpoly-style-ultra-pack-108275)
* [D.K.V. studio](https://assetstore.unity.com/packages/2d/textures-materials/water/simple-water-shader-hdrp-207454)
* [Avionx](https://assetstore.unity.com/packages/2d/textures-materials/sky/skybox-series-free-103633)
* [Wild Mage Games](https://assetstore.unity.com/packages/tools/utilities/ultimate-screenshot-creator-82008)

<b>Other</b><br>
* [Steamworks](https://partner.steamgames.com)
* [Othneil Drew's readme template](https://github.com/othneildrew/Best-README-Template)

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[license-shield]: https://img.shields.io/github/license/othneildrew/Best-README-Template.svg?style=for-the-badge
[license-url]: https://github.com/rbechoe/rtx-sweeper/blob/main/LICENSE
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=for-the-badge&logo=linkedin&color=0072b1
[linkedin-url]: https://www.linkedin.com/in/ravibechoe/
[product-screenshot]: https://github.com/rbechoe/rtx-sweeper/blob/main/github%20resources/RTX%20Screenshot%20Menu.jpg
[Unity.com]: https://img.shields.io/badge/Unity3D-35495E?style=for-the-badge&logo=unity&logoColor=ffffff
[Unity-url]: https://unity.com
[VS.com]: https://img.shields.io/badge/visual%20studio-35495E?style=for-the-badge&logo=visualstudio&logoColor=b65fcf
[VS-url]: https://visualstudio.microsoft.com/vs/community/
