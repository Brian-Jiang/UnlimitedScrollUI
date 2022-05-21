using UnityEngine;

namespace UnlimitedScrollUI {
    /// <summary>
    /// The side of the scroller panel. Used for certain events.
    /// </summary>
    public enum ScrollerPanelSide {
        NoSide,
        Top,
        Bottom,
        Left,
        Right
    }

    public enum Alignment {
        Left, Center, Right
    }

    internal struct Cell {
        public int number;
        public GameObject go;
    }

    internal struct Padding {
        public int top, bottom, left, right;
    }

    /// <summary>
    /// Jump to cell method.
    /// </summary>
    public enum JumpToMethod {
        /// <summary>
        /// Scroll until the cell is visible.
        /// </summary>
        OnScreen,
        
        /// <summary>
        /// Scroll until the cell is on the center.
        /// </summary>
        Center
    }
}