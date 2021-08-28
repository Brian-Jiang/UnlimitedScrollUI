using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnlimitedScrollUI {
    public class HorizontalUnlimitedScroller : HorizontalLayoutGroup, IUnlimitedScroller {
        /// <summary>
        /// <inheritdoc cref="IUnlimitedScroller.Generated"/>
        /// </summary>
        public bool Generated { get; private set; }

        /// <summary>
        /// <inheritdoc cref="IUnlimitedScroller.RowCount"/>
        /// There is only one row for horizontal layout.
        /// </summary>
        public int RowCount => 1;

        /// <summary>
        /// <inheritdoc cref="IUnlimitedScroller.FirstRow"/>
        /// Always equals 0 since there is only one row.
        /// </summary>
        public int FirstRow => 0;

        /// <summary>
        /// <inheritdoc cref="IUnlimitedScroller.LastRow"/>
        /// Always equals 0 since there is only one row.
        /// </summary>
        public int LastRow => 0;

        /// <summary>
        /// <inheritdoc cref="IUnlimitedScroller.FirstCol"/>
        /// </summary>
        public int FirstCol {
            get {
                var col = (int)((-contentTrans.anchoredPosition.x - offsetPadding.left) / (cellX + spacingX));
                return Mathf.Clamp(col, 0, CellPerRow - 1);
            }
        }

        /// <summary>
        /// <inheritdoc cref="IUnlimitedScroller.LastCol"/>
        /// </summary>
        public int LastCol {
            get {
                var col = (int)((-contentTrans.anchoredPosition.x + ViewportWidth - offsetPadding.left) /
                                (cellX + spacingX));
                return Mathf.Clamp(col, 0, CellPerRow - 1);
            }
        }

        /// <summary>
        /// <inheritdoc cref="IUnlimitedScroller.ContentHeight"/>
        /// </summary>
        public float ContentHeight {
            get => contentTrans.rect.height;
            private set => contentTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value);
        }

        /// <summary>
        /// <inheritdoc cref="IUnlimitedScroller.ContentWidth"/>
        /// </summary>
        public float ContentWidth {
            get => contentTrans.rect.width;
            private set => contentTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value);
        }

        /// <summary>
        /// <inheritdoc cref="IUnlimitedScroller.ViewportHeight"/>
        /// </summary>
        public float ViewportHeight => scrollerRectTransform.rect.height;
        
        /// <summary>
        /// <inheritdoc cref="IUnlimitedScroller.ViewportWidth"/>
        /// </summary>
        public float ViewportWidth => scrollerRectTransform.rect.width;

        /// <summary>
        /// <inheritdoc cref="IUnlimitedScroller.CellPerRow"/>
        /// </summary>
        public int CellPerRow => totalCount;
        
        /// <summary>
        /// The <c>ScrollRect</c> component on ScrollView.
        /// </summary>
        [Tooltip("The ScrollRect component on ScrollView.")]
        public ScrollRect scrollRect;
        
        private RectTransform contentTrans;
        private LayoutGroup layoutGroup;
        private RectTransform scrollerRectTransform;
        
        private float cellX;
        private float cellY;
        private float spacingX;
        private float spacingY;
        private Padding offsetPadding;

        private int totalCount;
        private GameObject storedElement;
        private List<Cell> currentElements;

        private int currentFirstRow;
        private int currentLastRow;
        private int currentFirstCol;
        private int currentLastCol;

        /// <summary>
        /// <inheritdoc cref="IUnlimitedScroller.Generate"/>
        /// </summary>
        /// <param name="newCell"><inheritdoc cref="IUnlimitedScroller.Generate"/></param>
        /// <param name="newTotalCount"><inheritdoc cref="IUnlimitedScroller.Generate"/></param>
        public void Generate(GameObject newCell, int newTotalCount) {
            layoutGroup = GetComponent<LayoutGroup>();
            Generated = true;
            storedElement = newCell;
            totalCount = newTotalCount;
            scrollRect.onValueChanged.AddListener(OnScroll);
            InitParams();

            GenerateAllCells();
        }

        private void InitParams() {
            var rect = storedElement.GetComponent<RectTransform>().rect;
            scrollerRectTransform = scrollRect.GetComponent<RectTransform>();
            contentTrans = GetComponent<RectTransform>();
            cellX = rect.width;
            cellY = rect.height;
            spacingX = ((HorizontalOrVerticalLayoutGroup)layoutGroup).spacing;
            spacingY = 0f;

            currentElements = new List<Cell>();
            offsetPadding = new Padding() {
                top = layoutGroup.padding.top,
                bottom = layoutGroup.padding.bottom,
                left = layoutGroup.padding.left,
                right = layoutGroup.padding.right
            };
            ContentHeight = cellY * RowCount + spacingY * (RowCount - 1) + offsetPadding.top + offsetPadding.bottom;
            ContentWidth = cellX * CellPerRow + spacingX * (CellPerRow - 1) + offsetPadding.left + offsetPadding.right;
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
            var cell = new Cell() { go = instance, number = index };
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

            layoutGroup.padding.left = offsetPadding.left + (currentFirstCol == 0
                ? 0
                : (int)(currentFirstCol * cellX + (currentFirstCol - 1) * spacingX));
            layoutGroup.padding.right = offsetPadding.right + (int)((CellPerRow - LastCol - 1) * (cellX + spacingX));
            layoutGroup.padding.top = offsetPadding.top + (currentFirstRow == 0
                ? 0
                : (int)(currentFirstRow * cellY + (currentFirstRow - 1) * spacingY));
            layoutGroup.padding.bottom = offsetPadding.bottom + (int)((RowCount - LastRow - 1) * (cellY + spacingY));
            for (var r = currentFirstRow; r <= currentLastRow; ++r) {
                for (var c = currentFirstCol; c <= currentLastCol; ++c) {
                    var index = GetCellIndex(r, c);
                    if (index >= totalCount) continue;
                    GenerateCell(index, ScrollerPanelSide.NoSide);
                }
            }
        }

        private void GenerateRow(int row, bool onTop) {
            var firstCol = currentFirstCol;
            var lastCol = currentLastCol;

            var indexEnd = GetCellIndex(row, lastCol);
            indexEnd = indexEnd >= totalCount ? totalCount - 1 : indexEnd;
            for (var i = GetCellIndex(row, firstCol); i <= indexEnd; ++i) {
                GenerateCell(i, onTop ? ScrollerPanelSide.Top : ScrollerPanelSide.Bottom);
            }

            if (onTop) layoutGroup.padding.top -= (int)(cellY + spacingY);
            else layoutGroup.padding.bottom -= (int)(cellY + spacingY);
        }

        private void GenerateCol(int col, bool onLeft) {
            var firstRow = currentFirstRow;
            var lastRow = currentLastRow;

            for (var i = firstRow; i <= lastRow; i++) {
                var index = GetCellIndex(i, col);
                if (index >= totalCount) continue;
                GenerateCell(index, onLeft ? ScrollerPanelSide.Left : ScrollerPanelSide.Right);
            }

            if (onLeft) layoutGroup.padding.left -= (int)(cellX + spacingX);
            else layoutGroup.padding.right -= (int)(cellX + spacingX);
        }

        private void DestroyRow(int row, bool onTop) {
            var firstCol = currentFirstCol;
            var lastCol = currentLastCol;

            var indexEnd = GetCellIndex(row, lastCol);
            indexEnd = indexEnd >= totalCount ? totalCount - 1 : indexEnd;
            for (var i = GetCellIndex(row, firstCol); i <= indexEnd; ++i) {
                DestroyCell(i, onTop ? ScrollerPanelSide.Top : ScrollerPanelSide.Bottom);
            }

            if (onTop) layoutGroup.padding.top += (int)(cellY + spacingY);
            else layoutGroup.padding.bottom += (int)(cellY + spacingY);
        }

        private void DestroyCol(int col, bool onLeft) {
            var firstRow = currentFirstRow;
            var lastRow = currentLastRow;

            for (var i = firstRow; i <= lastRow; i++) {
                var index = GetCellIndex(i, col);
                if (index >= totalCount) continue;
                DestroyCell(index, onLeft ? ScrollerPanelSide.Left : ScrollerPanelSide.Right);
            }

            if (onLeft) layoutGroup.padding.left += (int)(cellX + spacingX);
            else layoutGroup.padding.right += (int)(cellX + spacingX);
        }

        private void OnScroll(Vector2 position) {
            if (!Generated) return;

            if (LastCol < currentFirstCol || FirstCol > currentLastCol || FirstRow > currentLastRow ||
                LastRow < currentFirstRow) {
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
