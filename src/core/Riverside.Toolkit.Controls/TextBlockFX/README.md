![TextBlockFX](https://github.com/user-attachments/assets/918cc291-ce89-41c0-aa60-26905ca874c5)

# `Riverside.Toolkit.Controls.TextBlockFX`

A `TextBlock` control which animates the text with customizable effects.
`TextBlockFX` generates difference results for attached effect to animate the text when its content changes by using its built-in diffing algorithm.

https://user-images.githubusercontent.com/8193074/147348037-efe70068-d188-4a26-a23a-c94e2b03ede9.mp4

**Write your own effect**:

- Define a new effect class and implement the `ITextEffect` interface.
- Draw the text by using Win2D API.

---

> `TextBlockFX` is taken from [@validvoid](https://github.com/validvoid)'s own [`TextBlockFX.Win2D.UWP`](https://nuget.org/packages/TextBlockFX.Win2D.UWP) package, but modified to work across frameworks in CubeKit.