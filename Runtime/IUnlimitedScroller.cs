using System;
using UnityEngine;
// ReSharper disable UnusedMemberInSuper.Global

namespace UnlimitedScrollUI
{
    public interface IUnlimitedScroller
    {
        /// <summary>
        /// Whether this scroller has initialized.
        /// </summary>
        bool Initialized { get; }
        
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

        /// <summary>
        /// The height of the content.
        /// </summary>
        float ContentHeight { get; }

        /// <summary>
        /// The width of the content.
        /// </summary>
        float ContentWidth { get; }

        /// <summary>
        /// The height of the viewport.
        /// </summary>
        float ViewportHeight { get; }

        /// <summary>
        /// The width of the viewport.
        /// </summary>
        float ViewportWidth { get; }
        
        /// <summary>
        /// The calculated real number of cells per row or columns count.
        /// </summary>
        int CellPerRow { get; }

        /// <summary>
        /// Call this function to initialize and generate cells.
        /// </summary>
        /// <param name="newCell">The cell game object.</param>
        /// <param name="newTotalCount">The total cell count you want to generate.</param>
        /// <param name="onGenerate">The delegate when the cell is generated, you can initialize cell data here.</param>
        void Generate(GameObject newCell, int newTotalCount, Action<int, ICell> onGenerate);

        /// <summary>
        /// Call this function to jump to cell at index.
        /// </summary>
        /// <param name="index">The index of the cell.</param>
        /// <param name="method">The jump method.</param>
        void JumpTo(uint index, JumpToMethod method);

        /// <summary>
        /// Set a new caching size. If smaller than current cache size, cache will be
        /// trimmed to the new size
        /// </summary>
        /// <param name="newSize">New cache size.</param>
        void SetCacheSize(uint newSize);

        /// <summary>
        /// Clear all cells including cache.
        /// </summary>
        void Clear();

        /// <summary>
        /// Clear all cached cells. This will not change the cache size.
        /// </summary>
        void ClearCache();
    }
}
