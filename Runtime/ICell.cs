namespace UnlimitedScrollUI {
    /// <summary>
    /// <para>If you want to have your own script for cell, implement this interface.</para>
    /// </summary>
    public interface ICell {
        /// <summary>
        /// Called when the cell become visible.
        /// </summary>
        /// <param name="side">The side that this cell become visible. For example, <c>side = ScrollerPanelSide.Right</c> means
        /// that the player is dragging the panel to left so that this cell appears from right.</param>
        void OnBecomeVisible(ScrollerPanelSide side);
        
        /// <summary>
        /// Called when the cell become invisible.
        /// </summary>
        /// <param name="side">The side that this cell become invisible. For example, <c>side = ScrollerPanelSide.Right</c> means
        /// that the player is dragging the panel to right so that this cell disappears to right.</param>
        void OnBecomeInvisible(ScrollerPanelSide side);
    }
}
