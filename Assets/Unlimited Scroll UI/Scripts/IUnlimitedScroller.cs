using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnlimitedScrollUI
{
    public interface IUnlimitedScroller
    {
        /// <summary>
        /// Whether this scroller has initialized and generate cells.
        /// </summary>
        public bool Generated { get; }
        
        /// <summary>
        /// Total row count.
        /// </summary>
        public int RowCount { get; }
        
        /// <summary>
        /// The first visible row.
        /// </summary>
        public int FirstRow { get; }
        
        /// <summary>
        /// The last visible row.
        /// </summary>
        public int LastRow { get; }

        /// <summary>
        /// The first visible column.
        /// </summary>
        public int FirstCol { get; }

        /// <summary>
        /// The last visible column.
        /// </summary>
        public int LastCol { get; }

        public float ContentHeight { get; }

        public float ContentWidth { get; }

        public float ViewportHeight { get; }

        public float ViewportWidth { get; }
        
        /// <summary>
        /// The calculated real number of cells per row.
        /// </summary>
        public int CellPerRow { get; }

        /// <summary>
        /// Call this function to initialize and generate cells.
        /// </summary>
        /// <param name="newCell">The cell game object.</param>
        /// <param name="newTotalCount">The total cell count you want to generate.</param>
        public void Generate(GameObject newCell, int newTotalCount);
    }
}
