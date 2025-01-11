> [!IMPORTANT]
> CubeKit is a new era for the previous CrimsonUI, GlowUI and Toolkit options.
> The second version of CubeKit (formerly Crimson Toolkit) brings together all your favourite toolkits into one repository, with styles from GlowUI and Crimson and useful controls and utilities.

![GlowUI Economy](https://github.com/user-attachments/assets/ed1298b0-03b6-4d2d-bf7d-abe5fcc4c039)

<p align="center">
    <a href='https://nuget.org/packages/Riverside.Toolkit'>
      <img src='https://github.com/Rise-Software/Rise-Media-Player/assets/74561130/3d7edcaf-26d8-4453-a751-29b851721abd' alt='Get it from Microsoft' />
    </a>
    <a href='https://github.com/RiversideValley/Toolkit/releases/latest'>
      <img src='https://github.com/Rise-Software/Rise-Media-Player/assets/74561130/60deb402-0c8e-4579-80e6-69cb7b19cd43' alt='Get it from GitHub' />
    </a>
</p>

---

Introducing **CubeKit**, the **ultimate** toolkit for building modern apps on .NET. **CubeKit** is a set of **versatile, platform-agnostic APIs** that enable you to build apps for **multiple platforms** using .NET.
Using a custom multi-target solution called Alloy, it exposes APIs that are suited to the project you install the aggregate package on, allowing you to **build apps for UWP, WinUI and .NET Core.**

**CubeKit** is the new name for its various predecessors including **CrimsonUI** (now merged with GlowUI), **GlowUI** (GlowUI is still used to refer to the styles, but it is included as part of CubeKit as a whole), **Crimson Toolkit** and **`Riverside.Toolkit`**.
It is built using custom tooling to target all versions of .NET.

---

## Using CubeKit

> NuGet is a standard package manager for .NET applications which is built into Visual Studio. When you open solution in Visual Studio, choose the *Tools* menu > *NuGet Package Manager* > *Manage NuGet packages for solution…* 

You can install all recommended helpers and controls using the [aggregate package](https://nuget.org/packages/Riverside.Toolkit) which automatically installs all the appropriate helpers for your target platform (.NET Core, WinUI and UWP .NET 9).
You can see examples of using APIs on the [documentation](https://riversidevalley.github.io/Toolkit) page.

The root namespace for CubeKit is as follows:
- `Riverside.Toolkit.*`: Main CubeKit components and helpers
- `Riverside.GlowUI.*`: Related to styles and GlowUI
- `Riverside.Extensions.*`: Various collections of .NET helpers and rewrites

CubeKit also contains controls from WCT 7.x that didn't make it to 8.x, such as `DropShadowPanel`.

### Building from source

It's recommended to read [Sergio's blog post on UWP .NET 9](https://devblogs.microsoft.com/ifdef-windows/preview-uwp-support-for-dotnet-9-native-aot/) to make yourself familiar with UWP .NET 9 NativeAOT and the limitations.

#### 1. Prerequisites

- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) with the following individual components:
    - Windows SDK
    - UWP build tools
    - WinUI development workload
    - .NET SDK
    - [Preview Windows SDK bundle](https://aka.ms/preview-uwp-support-for-dotnet9-windows-sdk)
- Git for Windows

#### 2. Clone the repository

```ps
git clone https://github.com/RiversideValley/Toolkit
```

This will create a local copy of the repository.

#### 3. Build the project

To build CubeKit for development, open the `CubeKit.sln` item in Visual Studio. Right-click on the project you want to build and press 'Build'.

## Contributing

We're always looking for a helping hand, [look for open issues here](https://github.com/RiversideValley/Toolkit/issues) or create a [fork of the repo](https://github.com/RiversideValley/Toolkit/fork) to try or build new features.
Even just improving our docs and samples for existing components here, or adding new tests can be a huge help!

Check out our [documentation](https://riversidevalley.github.io/Toolkit) page to learn more about the project and how to contribute to it.

If you have a new idea for CubeKit, please write a [feature request](https://github.com/RiversideValley/Toolkit/issues/new?q=sort%3Aupdated-desc+is%3Aissue+is%3Aopen&template=feature_request.yml)! However if your idea is quite large and requires extra tracking, please [start a discussion](https://github.com/RiversideValley/Toolkit/discussions/new?category=ideas)! Any ideas for APIs, controls and styling suggestions are appreciated. No contribution is too big or too small.

---

| Package | Latest | Description |
|--------|--------|--------|
| `Riverside.Toolkit` | | The main aggregate package for CubeKit. |
| `Riverside.Toolkit.Flyouts` | | Flyouts manifest package from Fluent Flyouts 3 (obsolete) |
| `Riverside.Toolkit.Controls` | | Controls and user controls from CubeKit. |
| `Riverside.Toolkit.Converters` | | Converters from CubeKit to reduce need for writing them in your own apps (part of the [One Toolkit](https://github.com/RiversideValley/Toolkit/discussions/22) effort) |
| `Riverside.Toolkit.Helpers` | | A [collection of useful static helpers](https://riversidevalley.github.io/Toolkit/api/Riverside.Toolkit.Helpers.html) for building modern apps. Shared with the `Riverside.Extensions` package. |
| `Riverside.Toolkit.Icons` | | A collection of [resources for using fluent icons](https://riversidevalley.github.io/Toolkit/api/Riverside.Toolkit.Icons.html) in WinUI/UWP apps. It is obsolete now but still kept as it is used by GlowUI. |
| `Riverside.GlowUI` | | The GlowUI styles package. |
| `Riverside.GlowUI.Materials` | | Materials for use in GlowUI apps such as [Bloom](https://riversidevalley.github.io/Toolkit/api/Riverside.GlowUI.Materials.BloomView.html) and [Aurora](https://riversidevalley.github.io/Toolkit/api/Riverside.GlowUI.Materials.AuroraLite.html). |
| `Riverside.Extensions` | | This package previously contained runtime agnostic helpers that are now part of the main toolkit package. This namespace is now used for rewrites of other packages and helpers. |

---

CubeKit is used in many production apps, including **[United Sets](https://apps.microsoft.com/detail/9N7CWZ3L5RWL), [Protecc](https://apps.microsoft.com/detail/9PJX91M06TZS), [Clippy](https://apps.microsoft.com/detail/9NWK37S35V5T), [Flowboard](https://apps.microsoft.com/detail/9PB20HCH5XN2), [Darksky](https://apps.microsoft.com/detail/9NP22DTFSCTS) and way more!**

| United Sets | Protecc | Flowboard |
|--------|--------|--------|
| ![image](https://github.com/user-attachments/assets/a5925938-3b21-4848-842d-ca58aa574e08) | ![image](https://github.com/user-attachments/assets/23bde4df-0a92-43ea-9589-9c30b39c7f12) | ![image](https://github.com/user-attachments/assets/fd4c20bb-325b-422b-9ff9-d6f29a3c7b67) |

---

![GlowUI Economy](https://github.com/user-attachments/assets/f7603612-fc65-41d0-b169-eaca51434b42)
