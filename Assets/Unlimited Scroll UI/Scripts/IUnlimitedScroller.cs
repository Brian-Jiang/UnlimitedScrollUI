using UnityEngine;

namespace UnlimitedScrollUI
{
    public interface IUnlimitedScroller
    {
        /// <summary>
        /// Whether this scroller has initialized and generate cells.
        /// </summary>
        bool Generated { get; }
        
        /// <summary>
        /// Total row count.
        /// </summary>
        int RowCount { get; }
        
        /// <summary>
        /// The first visible row.
        /// </summary>
        int FirstRow { get; }
        
        /// <summary>
        /// The last visible row.
        /// </summary>
        int LastRow { get; }

        /// <summary>
        /// The first visible column.
        /// </summary>
        int FirstCol { get; }

        /// <summary>
        /// The last visible column.
        /// </summary>
        int LastCol { get; }

        float ContentHeight { get; }

        float ContentWidth { get; }

        float ViewportHeight { get; }

        float ViewportWidth { get; }
        
        /// <summary>
        /// The calculated real number of cells per row.
        /// </summary>
        int CellPerRow { get; }

        /// <summary>
        /// Call this function to initialize and generate cells.
        /// </summary>
        /// <param name="newCell">The cell game object.</param>
        /// <param name="newTotalCount">The total cell count you want to generate.</param>
        void Generate(GameObject newCell, int newTotalCount);
    }
}
