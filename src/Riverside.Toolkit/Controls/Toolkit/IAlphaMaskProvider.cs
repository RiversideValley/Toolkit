namespace Riverside.Toolkit.Controls.Toolkit
{
    /// <summary>
    /// Any user control can implement this interface to provide a custom alpha mask to it's parent <see cref="UserControls.DropShadowPanel"/>
    /// </summary>
    public interface IAlphaMaskProvider
    {
        /// <summary>
        /// Gets a value indicating whether the AlphaMask needs to be retrieved after the element has loaded.
        /// </summary>
        bool WaitUntilLoaded { get; }

        /// <summary>
        /// This method should return the appropriate alpha mask to be used in the shadow of this control
        /// </summary>
        /// <returns>The alpha mask as a composition brush</returns>
        CompositionBrush GetAlphaMask();
    }
}
