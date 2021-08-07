using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnlimitedScrollUI {
    /// <summary>
    /// The type of auto layout group you are using.
    /// </summary>
    public enum AutoLayoutType {
        Vertical,
        Horizontal,
        Grid
    }

    /// <summary>
    /// The side of the scroller panel. Used for certain events.
    /// </summary>
    public enum ScrollerPanelSide {
        NoSide, Top, Bottom, Left, Right
    }

    internal struct Cell {
        public int number;
        public GameObject go;
    }

    internal struct Padding {
        public int top, bottom, left, right;
    }

    /// <summary>
    /// Main class UnlimitedScroller that control the spawn and destroy of cells.
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    public class UnlimitedScroller : MonoBehaviour {
        /// <summary>
        /// Whether this scroller has initialized and generate cells.
        /// </summary>
        public bool Generated { get; private set; }

        /// <summary>
        /// Total row count.
        /// </summary>
        public int RowCount =>
            totalCount % CellPerRow == 0
                ? totalCount / CellPerRow
                : totalCount / CellPerRow + 1;

        /// <summary>
        /// The first visible row.
        /// </summary>
        public int FirstRow {
            get {
                if (layoutType == AutoLayoutType.Horizontal) return 0;

                var row = (int) ((contentTrans.anchoredPosition.y - offsetPadding.top) / (cellY + spacingY));
                return row < 0 ? 0 : row >= RowCount ? RowCount - 1 : row;
            }
        }

        /// <summary>
        /// The last visible row.
        /// </summary>
        public int LastRow {
            get {
                if (layoutType == AutoLayoutType.Horizontal) return 0;

                var row = (int) ((contentTrans.anchoredPosition.y + ViewportHeight - offsetPadding.top) /
                                 (cellY + spacingY));
                return row < 0 ? 0 : row >= RowCount ? RowCount - 1 : row;
            }
        }

        /// <summary>
        /// The first visible column.
        /// </summary>
        public int FirstCol {
            get {
                if (layoutType == AutoLayoutType.Vertical) return 0;

                var col = (int) ((-contentTrans.anchoredPosition.x - offsetPadding.left) / (cellX + spacingX));
                return col < 0 ? 0 : col >= CellPerRow ? CellPerRow - 1 : col;
            }
        }

        /// <summary>
        /// The last visible column.
        /// </summary>
        public int LastCol {
            get {
                if (layoutType == AutoLayoutType.Vertical) return 0;

                var col = (int) ((-contentTrans.anchoredPosition.x + ViewportWidth - offsetPadding.left) /
                                 (cellX + spacingX));
                return col < 0 ? 0 : col >= CellPerRow ? CellPerRow - 1 : col;
            }
        }

        public float ContentHeight {
            get => contentTrans.rect.height;
            private set => contentTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value);
        }

        public float ContentWidth {
            get => contentTrans.rect.width;
            private set => contentTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value);
        }

        public float ViewportHeight {
            get => GetComponent<RectTransform>().rect.height;
        }

        public float ViewportWidth {
            get => GetComponent<RectTransform>().rect.width;
        }

        /// <summary>
        /// The calculated real number of cells per row.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public int CellPerRow {
            get {
                switch (layoutType) {
                    case AutoLayoutType.Vertical:
                        return 1;
                    case AutoLayoutType.Horizontal:
                        return totalCount;
                    case AutoLayoutType.Grid:
                        return cellPerRow;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        /// <summary>
        /// Match cell per row to the width of Content. If set, cellPerRow will be ignored.
        /// </summary>
        [Tooltip("Match cell per row to the width of Content. If set, cellPerRow will be ignored.")]
        public bool matchContentWidth;
        
        /// <summary>
        /// Cell Count per row if not match Content width.
        /// </summary>
        [Tooltip("Cell Count per row if not match Content width.")]
        public int cellPerRow;

        /// <summary>
        /// The Content GameObject.
        /// </summary>
        [Tooltip("The Content GameObject.")]
        public RectTransform contentTrans;
        
        /// <summary>
        /// The layout group component.
        /// </summary>
        [Tooltip("The layout group component.")]
        public LayoutGroup layoutGroup;

        /// <summary>
        /// The type of auto layout group you are using. Please match with your auto layout group.
        /// </summary>
        [Tooltip("The type of auto layout group you are using. Please match with your auto layout group.")]
        public AutoLayoutType layoutType;
        // public int extraRowCount;

        private GameObject storedElement;

        private float cellX;
        private float cellY;
        private float spacingX;
        private float spacingY;
        private Padding offsetPadding;

        // private int emptyRows;

        private int totalCount;

        private List<Cell> currentElements;

        private ScrollRect scrollRect;

        private int currentFirstRow;
        private int currentLastRow;
        private int currentFirstCol;
        private int currentLastCol;

        /// <summary>
        /// Call this function to initialize and generate cells.
        /// </summary>
        /// <param name="newCell">The cell game object.</param>
        /// <param name="newTotalCount">The total cell count you want to generate.</param>
        public void Generate(GameObject newCell, int newTotalCount) {
            Generated = true;
            storedElement = newCell;
            totalCount = newTotalCount;
            scrollRect = GetComponent<ScrollRect>();
            scrollRect.onValueChanged.AddListener(OnScroll);
            InitParams();

            GenerateAllCells();
        }

        private void InitParams() {
            var rect = storedElement.GetComponent<RectTransform>().rect;
            switch (layoutType) {
                case AutoLayoutType.Vertical:
                    cellX = rect.width;
                    cellY = rect.height;
                    cellPerRow = 1;
                    spacingX = 0f;
                    spacingY = ((HorizontalOrVerticalLayoutGroup) layoutGroup).spacing;
                    break;
                case AutoLayoutType.Horizontal:
                    cellX = rect.width;
                    cellY = rect.height;
                    cellPerRow = totalCount;
                    spacingX = ((HorizontalOrVerticalLayoutGroup) layoutGroup).spacing;
                    spacingY = 0f;
                    break;
                case AutoLayoutType.Grid:
                    var gridLayoutGroup = (GridLayoutGroup) layoutGroup;
                    cellX = gridLayoutGroup.cellSize.x;
                    cellY = gridLayoutGroup.cellSize.y;
                    spacingX = gridLayoutGroup.spacing.x;
                    spacingY = gridLayoutGroup.spacing.y;
                    cellPerRow = matchContentWidth ? (int) (ContentWidth / cellX) : cellPerRow;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            currentElements = new List<Cell>();
            offsetPadding = new Padding() {
                top = layoutGroup.padding.top,
                bottom = layoutGroup.padding.bottom,
                left = layoutGroup.padding.left,
                right = layoutGroup.padding.right
            };
            ContentHeight = cellY * RowCount + spacingY * (RowCount - 1) + offsetPadding.top + offsetPadding.bottom;
            // print($"cellY: {cellY}, total: {totalCount}, ele/row: {CellPerRow}");
            ContentWidth = cellX * CellPerRow + spacingX * (CellPerRow - 1) + offsetPadding.left + offsetPadding.right;

            // emptyRows = 0;
        }

        private int GetCellIndex(int row, int col) {
            return CellPerRow * row + col;
        }

        private int GetFirstGreater(int index) {
            var start = 0;
            var end = currentElements.Count;
            while (start != end) {
                var middle = start + (end - start) / 2;
                if (currentElements[middle].number <= index) {
                    start = middle + 1;
                } else {
                    end = middle;
                }
            }

            return start;
        }

        private void GenerateCell(int index, ScrollerPanelSide side) {
            var order = GetFirstGreater(index);
            var instance = Instantiate(storedElement, contentTrans);
            instance.GetComponent<Transform>().SetSiblingIndex(order);
            var cell = new Cell() {go = instance, number = index};
            currentElements.Insert(order, cell);

            var iCell = instance.GetComponent<ICell>();
            iCell.OnGenerated(index);
            iCell.OnBecomeVisible(side);
        }

        private void DestroyCell(int index, ScrollerPanelSide side) {
            var order = GetFirstGreater(index - 1);
            var cell = currentElements[order];
            currentElements.RemoveAt(order);
            cell.go.GetComponent<ICell>().OnBecomeInvisible(side);
            DestroyImmediate(cell.go);
        }

        private void DestroyAllCells() {
            var total = currentElements.Count;
            for (var i = 0; i < total; i++) {
                var cell = currentElements[0];
                currentElements.RemoveAt(0);
                cell.go.GetComponent<ICell>().OnBecomeInvisible(ScrollerPanelSide.NoSide);
                DestroyImmediate(cell.go);
            }
        }

        private void GenerateAllCells() {
            currentFirstCol = FirstCol;
            currentLastCol = LastCol;
            currentFirstRow = FirstRow;
            currentLastRow = LastRow;

            // print(CellPerRow);
            // print($"first col: {currentFirstCol}, last col: {currentLastCol}");
            // print($"first row: {currentFirstRow}, last row: {currentLastRow}");

            layoutGroup.padding.left = offsetPadding.left + (currentFirstCol == 0
                ? 0
                : (int) (currentFirstCol * cellX + (currentFirstCol - 1) * spacingX));
            layoutGroup.padding.right = offsetPadding.right + (int) ((CellPerRow - LastCol - 1) * (cellX + spacingX));
            layoutGroup.padding.top = offsetPadding.top + (currentFirstRow == 0
                ? 0
                : (int) (currentFirstRow * cellY + (currentFirstRow - 1) * spacingY));
            layoutGroup.padding.bottom = offsetPadding.bottom + (int) ((RowCount - LastRow - 1) * (cellY + spacingY));
            for (var r = currentFirstRow; r <= currentLastRow; ++r) {
                for (var c = currentFirstCol; c <= currentLastCol; ++c) {
                    var index = GetCellIndex(r, c);
                    if (index >= totalCount) continue;
                    GenerateCell(index, ScrollerPanelSide.NoSide);
                }
            }
        }

        private void GenerateRow(int row, bool onTop) {
            // print($"generate row: {row}, ontop = {onTop}");
            // var firstCol = FirstCol;
            // var lastCol = LastCol;
            var firstCol = currentFirstCol;
            var lastCol = currentLastCol;

            var indexEnd = GetCellIndex(row, lastCol);
            indexEnd = indexEnd >= totalCount ? totalCount - 1 : indexEnd;
            for (var i = GetCellIndex(row, firstCol); i <= indexEnd; ++i) {
                GenerateCell(i, onTop ? ScrollerPanelSide.Top : ScrollerPanelSide.Bottom);
            }

            if (onTop) layoutGroup.padding.top -= (int) (cellY + spacingY);
            else layoutGroup.padding.bottom -= (int) (cellY + spacingY);
        }

        private void GenerateCol(int col, bool onLeft) {
            // print($"generate col: {col}, onLeft = {onLeft}");
            // var firstRow = FirstRow;
            // var lastRow = LastRow;
            var firstRow = currentFirstRow;
            var lastRow = currentLastRow;

            for (var i = firstRow; i <= lastRow; i++) {
                var index = GetCellIndex(i, col);
                if (index >= totalCount) continue;
                GenerateCell(index, onLeft ? ScrollerPanelSide.Left : ScrollerPanelSide.Right);
            }

            if (onLeft) layoutGroup.padding.left -= (int) (cellX + spacingX);
            else layoutGroup.padding.right -= (int) (cellX + spacingX);
        }

        private void DestroyRow(int row, bool onTop) {
            // print($"destroy row: {row}, ontop = {onTop}");
            // var firstCol = FirstCol;
            // var lastCol = LastCol;
            var firstCol = currentFirstCol;
            var lastCol = currentLastCol;

            var indexEnd = GetCellIndex(row, lastCol);
            indexEnd = indexEnd >= totalCount ? totalCount - 1 : indexEnd;
            for (var i = GetCellIndex(row, firstCol); i <= indexEnd; ++i) {
                DestroyCell(i, onTop ? ScrollerPanelSide.Top : ScrollerPanelSide.Bottom);
            }

            if (onTop) layoutGroup.padding.top += (int) (cellY + spacingY);
            else layoutGroup.padding.bottom += (int) (cellY + spacingY);
        }

        private void DestroyCol(int col, bool onLeft) {
            // print($"destroy col: {col}, onLeft = {onLeft}");
            // var firstRow = FirstRow;
            // var lastRow = LastRow;
            var firstRow = currentFirstRow;
            var lastRow = currentLastRow;

            for (var i = firstRow; i <= lastRow; i++) {
                // print($"row: {i}, col: {col}");
                var index = GetCellIndex(i, col);
                if (index >= totalCount) continue;
                DestroyCell(index, onLeft ? ScrollerPanelSide.Left : ScrollerPanelSide.Right);
            }

            if (onLeft) layoutGroup.padding.left += (int) (cellX + spacingX);
            else layoutGroup.padding.right += (int) (cellX + spacingX);
        }

        private void OnScroll(Vector2 position) {
            if (!Generated) return;
            
            if (LastCol < currentFirstCol || FirstCol > currentLastCol || FirstRow > currentLastRow ||
                LastRow < currentFirstRow) {
                // print("regenerate");
                // print($"current first col: {currentFirstCol}, last col: {currentLastCol}");
                // print($"current first row: {currentFirstRow}, last row: {currentLastRow}");
                // print($"first col: {FirstCol}, last col: {LastCol}");
                // print($"first row: {FirstRow}, last row: {LastRow}");
                DestroyAllCells();
                GenerateAllCells();
                return;
            }

            if (currentFirstCol > FirstCol) {
                // new left col
                for (var col = currentFirstCol - 1; col >= FirstCol; --col) {
                    GenerateCol(col, true);
                }

                currentFirstCol = FirstCol;
            }

            if (currentLastCol < LastCol) {
                // new right col
                for (var col = currentLastCol + 1; col <= LastCol; ++col) {
                    GenerateCol(col, false);
                }

                currentLastCol = LastCol;
            }

            if (currentFirstCol < FirstCol) {
                // left col invisible
                // print($"{currentFirstCol}, {FirstCol}");
                for (var col = currentFirstCol; col < FirstCol; ++col) {
                    DestroyCol(col, true);
                }

                currentFirstCol = FirstCol;
            }

            if (currentLastCol > LastCol) {
                // right col invisible
                for (var col = currentLastCol; col > LastCol; --col) {
                    DestroyCol(col, false);
                }

                currentLastCol = LastCol;
            }


            if (currentFirstRow > FirstRow) {
                // new top row
                for (var row = currentFirstRow - 1; row >= FirstRow; --row) {
                    GenerateRow(row, true);
                }

                currentFirstRow = FirstRow;
            }

            if (currentLastRow < LastRow) {
                // new bottom row
                for (var row = currentLastRow + 1; row <= LastRow; ++row) {
                    GenerateRow(row, false);
                }

                currentLastRow = LastRow;
            }

            if (currentFirstRow < FirstRow) {
                // top row invisible
                for (var row = currentFirstRow; row < FirstRow; ++row) {
                    DestroyRow(row, true);
                }

                currentFirstRow = FirstRow;
            }

            if (currentLastRow > LastRow) {
                // bottom row invisible
                for (var row = currentLastRow; row > LastRow; --row) {
                    DestroyRow(row, false);
                }

                currentLastRow = LastRow;
            }
        }
    }
}