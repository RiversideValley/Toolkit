# CubeKit

Introducing **CubeKit**, the **ultimate** toolkit for building modern apps on .NET. **CubeKit** is a set of **versatile, platform-agnostic APIs** that enable you to build apps for **multiple platforms** using .NET.
Using a custom multi-target solution called Alloy, it exposes APIs that are suited to the project you install the aggregate package on, allowing you to **build apps for UWP, WinUI and .NET Core.**

**CubeKit** is the new name for its various predecessors including **CrimsonUI** (now merged with GlowUI), **GlowUI** (GlowUI is still used to refer to the styles, but it is included as part of CubeKit as a whole), **Crimson Toolkit** and **`Riverside.Toolkit`**.
It is built using custom tooling to target all versions of .NET.

---

> NuGet is a standard package manager for .NET applications which is built into Visual Studio. When you open solution in Visual Studio, choose the *Tools* menu > *NuGet Package Manager* > *Manage NuGet packages for solution…*

You can install all recommended helpers and controls using the [aggregate package](https://nuget.org/packages/Riverside.Toolkit) which automatically installs all the appropriate helpers for your target platform (.NET Core, WinUI and UWP .NET 9).
You can see examples of using APIs on the [documentation](https://riversidevalley.github.io/Toolkit) page.

The root namespace for CubeKit is as follows:
- `Riverside.Toolkit.*`: Main CubeKit components and helpers
- `Riverside.GlowUI.*`: Related to styles and GlowUI
- `Riverside.Extensions.*`: Various collections of .NET helpers and rewrites

CubeKit also contains controls from WCT 7.x that didn't make it to 8.x, such as `DropShadowPanel`.
