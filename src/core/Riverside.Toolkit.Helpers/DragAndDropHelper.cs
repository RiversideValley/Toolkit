using Windows.ApplicationModel.DataTransfer;

namespace Riverside.Toolkit.Helpers;

/// <summary>
/// Helper methods for adding seamless drag &amp; drop support to different <see cref="ListViewBase"/>
/// </summary>
/// <typeparam name="T">The type of the dragged &amp; dropped items</typeparam>
public class DragDropHelper<T>
{
    /// <summary> ID strings for data package</summary>
    private const string Item = "CubeActionItem";
    private const string Source = "CubeActionList";

    /// <summary>
    /// Invoked when dragging starts
    /// Set data package ID's
    /// </summary>
    public void DragItemsStarting(object sender, DragItemsStartingEventArgs e)
    {
        e.Data.Properties[Item] = e.Items[0];
        e.Data.Properties[Source] = sender as ListViewBase;
    }

    /// <summary>
    /// Invoked when item drags over <see cref="ListViewBase"/>
    /// </summary>
    public void DragOver(object sender, DragEventArgs e)
    {
        e.AcceptedOperation = DataPackageOperation.Move;
        e.DragUIOverride.IsGlyphVisible = false;
        e.Handled = true;
    }

    /// <summary>
    /// Occurs when item drops on <see cref="ListViewBase"/>
    /// </summary>
    /// <param name="sender">The <see cref="ListViewBase"/> to drop to</param>
    /// <param name="e">The <see cref="DragEventArgs"/></param>
    public void Drop(object sender, DragEventArgs e)
    {
        DragOperationDeferral Deferral = e.GetDeferral(); // Create a deferral to process item
        var TargetList = sender as ListViewBase; // Target listview which we are dropping to
        var SourceList = e.DataView.Properties[Source] as ListViewBase; // Source listview where item is from
        var SourceItems = SourceList.ItemsSource as ObservableCollection<T>;
        var TargetItems = TargetList.ItemsSource as ObservableCollection<T>;
        var DragItem = (T)e.DataView.Properties[Item];

        // Position of cursor
        Windows.Foundation.Point pt = e.GetPosition(TargetList);
        // Get index of where item was dropped in list
        int? DropIndex = (
            from index in Enumerable.Range(0, TargetList.Items.Count) // Index range from 0 to number of items
            let Element = TargetList.ContainerFromIndex(index) as UIElement // Get item
            let posElement = Element.TransformToVisual(TargetList).TransformPoint(default) // Item point position
            let size = Element.ActualSize // Item size
            let IsMoreThanTopLeft = pt.X >= posElement.X && pt.Y >= posElement.Y // Overflowing check for grid
            let IsLessThanBottomRight = pt.X <= posElement.X + size.X && pt.Y <= posElement.Y + size.Y // Overflowing check for grid
            where IsMoreThanTopLeft && IsLessThanBottomRight
            select (int?)index // Retrieve index with data
        ).FirstOrDefault(); // Return the first result

        try
        {
            _ = SourceItems.Remove(DragItem); // Remove item from original list
        }
        catch { /* Ignore lists that have groups */}
        TargetItems.Insert(DropIndex.HasValue ? (int)DropIndex : 0, DragItem); // Add item to new list

        e.AcceptedOperation = DataPackageOperation.Move; // Set operation as moved
        Deferral.Complete(); // Set deferral to complete so framework can continue processing
        e.Handled = true; // Set drop operation as completed
    }
}
