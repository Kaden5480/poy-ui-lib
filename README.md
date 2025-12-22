# poy-ui-lib
![Code size](https://img.shields.io/github/languages/code-size/Kaden5480/poy-ui-lib?color=5c85d6)
![Open issues](https://img.shields.io/github/issues/Kaden5480/poy-ui-lib?color=d65c5c)
![License](https://img.shields.io/github/license/Kaden5480/poy-ui-lib?color=a35cd6)

A UI library for
[Peaks of Yore](https://store.steampowered.com/app/2236070/).

# Overview
- [Installing](#installing)
- [Building from source](#building-from-source)
    - [Dotnet](#dotnet-build)
    - [Visual Studio](#visual-studio-build)
    - [Build configuration](#build-configuration)
- [Credits](#credits)

# Installing
### BepInEx
If you haven't installed BepInEx yet, follow the install instructions here:
- [Windows](https://github.com/Kaden5480/modloader-instructions#bepinex-windows)
- [Linux](https://github.com/Kaden5480/modloader-instructions#bepinex-linux)

### UILib
- Download the latest release
[here](https://github.com/Kaden5480/poy-ui-lib/releases).
- The compressed zip will contain a `plugins` directory.
- Copy the files in `plugins` to `BepInEx/plugins` in your game directory.

# Building from source
Whichever approach you use for building from source, the resulting
plugin/mod can be found in `bin/`.

The following configurations are supported:
- Debug
- Release

Keep in mind, `res/uilib.bundle` is not pushed with the repo.
So you would have to make your own.

## Dotnet build
To build with dotnet, run the following command, replacing
<configuration> with the desired value:
```sh
dotnet build -c <configuration>
```

## Visual Studio build
To build with Visual Studio, open UILib.sln and build by pressing ctrl + shift + b,
or by selecting Build -> Build Solution.

## Build configuration
The following can be configured:
- The path Peaks of Yore is installed at
- Whether the mod should automatically install on build
- Whether integration with [Mod Menu](https://github.com/Kaden5480/poy-mod-menu) should be enabled

Note that all of these properties are optional.

The configuration file must be in the root of this repository and must be called "Config.props".
```xml
<Project>
  <PropertyGroup>
    <!-- For example, if peaks is installed under F: -->
    <GamePath>F:\Games\Peaks of Yore</GamePath>

    <!-- Add this option if you want to install after building -->
    <InstallAfterBuild>true</InstallAfterBuild>

    <!-- Add this option if you want to disable mod menu integration
        This is useful for bootstrapping UILib -->
    <ModMenu>false</ModMenu>
  </PropertyGroup>
</Project>
```

# Credits
- Roman Antique font: https://www.dafont.com/roman-antique.font
- [Givo](https://gitlab.com/givowo): fixing input field fonts, suggesting themes, suggesting toggling interactions with overlays/windows (like the steam overlay)
- [Argore](https://github.com/ArgoreOfficial/): suggesting bounds checks for window/component focus and shaders for generating the color picker graphics
