> [!IMPORTANT]
> Major infrastructure changes are happening to Crimson, `Riverside.Toolkit`, GlowUI and CubeKit and lots of new changes are expected to come to the new 2.0 version of the toolkit. Stay tuned in [Developer Sanctuary](https://dsc.gg/devsanx) for more info!

# 🧰 Crimson

#### The better way to style your apps.

---

### 🤔 What is Crimson?

Crimson is the best way to style your WinUI apps with pretty styles and useful controls that allow for creating professional but appealing apps that will delight users.

> CrimsonUI is a dynamic UI library designed for UWP and Windows App SDK. Rooted in WinUI and influenced by FireCube's GlowUI, CrimsonUI seamlessly blends default WinUI elements with glowing, shiny components. Immerse yourself in stunning animations, ensuring a very smooth and beautiful user experience. Elevate your app's visual appeal with the elegance of CrimsonUI.

## 🎨 `Crimson.UI`

<!--
> [!NOTE]
> Requires `CommunityToolkit.WinUI.UI.Controls` for WinAppSdk and `CommunityToolkit.UWP.UI.Controls` for UWP support.
-->

Crimson UI is the original, primary component of the Crimson project and contains styles and themes for building resplendent, glowing apps.
Originally based on [@FireCube](https://github.com/firecubestudios)'s GlowUI, CrimsonUI provides the consumer with shiny, glowing UI elements that makes crafting a beautiful UI satisfying.

In order to install CrimsonUI, add the [CrimsonUI  NuGet package](https://nuget.org/packages/Crimson) to your app. This will allow you to make the best use of its controls and update it easily.
Symbols for CrimsonUI are also available on the NuGet Gallery server and are generated on build.

In order to consume CrimsonUI in your app, add the following line to your WinUI3 or UWP app's `App.xaml` file:
```xaml
   <Application.Resources>
       <ResourceDictionary>
           <ResourceDictionary.MergedDictionaries>
               <ResourceDictionary Source="ms-appx:///Crimson/Styles.xaml" />
           </ResourceDictionary.MergedDictionaries>
       </ResourceDictionary>
   </Application.Resources>
```

### 🔳 Easily blending in

> CrimsonUI controls are designed to integrate flawlessly with Windows applications, providing a seamless user interface experience. They feature rest brushes that are meticulously crafted to match the standard WinUI controls, ensuring a consistent and harmonious visual integration. This design philosophy allows for a cohesive user experience, where the CrimsonUI elements blend naturally with the native components of the Windows environment. The controls are built with attention to detail, aiming to enhance the aesthetics and functionality of applications while maintaining a familiar look and feel that users trust. With CrimsonUI, developers can elevate their application's interface, making it not only visually appealing but also intuitively usable, leveraging the robustness of Windows' native UI elements.

### 🌠 Simply beautiful

> CrimsonUI’s design leverages the visual appeal of glowing panels and accent color gradients to create a dynamic 3D appearance, reminiscent of a neon glow. This effect adds depth and a touch of modernity to the user interface, ensuring that applications not only function well but also stand out with a contemporary, eye-catching aesthetic. The use of gradients and light effects in CrimsonUI controls helps to bring interfaces to life, making them more engaging and visually stimulating for users.

## 🧰 `Crimson.Toolkit`

This repository also contains the Crimson Toolkit, a large toolkit containing various helpers, controls and other libraries, similar to the Microsoft [Community Toolkit](https://github.com/CommunityToolkit). It was merged from the original `Ivirius.UI.Controls` library and `Riverside.Toolkit`, and is published on NuGet under the latter's name.

The Crimson Toolkit is the newer part of the Crimson ecosystem. It contains tools such as helpers, controls, branding assets and various other useful tools to build pixel-perfect apps.
