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

    internal struct Cell {
        public int number;
        public GameObject go;
    }

    internal struct Padding {
        public int top, bottom, left, right;
    }
}