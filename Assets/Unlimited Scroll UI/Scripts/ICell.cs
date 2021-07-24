namespace UnlimitedScrollUI {
    /// <summary>
    /// <para>If you want to have your own script for cell, implement this interface.</para>
    /// </summary>
    public interface ICell { 
        void OnGenerated(int index);
        void OnBecomeInvisible(ScrollerPanelSide side);
    }
}
