using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnlimitedScrollUI {
    public class GridUnlimitedScroller : GridLayoutGroup, IUnlimitedScroller {
        #region Properties

        /// <inheritdoc cref="IUnlimitedScroller.Initialized"/>
        public bool Initialized { get; private set; }

        /// <inheritdoc cref="IUnlimitedScroller.Generated"/>
        public bool Generated { get; private set; }

        /// <inheritdoc cref="IUnlimitedScroller.RowCount"/>
        public int RowCount =>
            totalCount % CellPerRow == 0
                ? totalCount / CellPerRow
                : totalCount / CellPerRow + 1;

        /// <inheritdoc cref="IUnlimitedScroller.FirstRow"/>
        public int FirstRow {
            get {
                var row = (int)((contentTrans.anchoredPosition.y - offsetPadding.top) / (cellY + spacingY));
                return Mathf.Clamp(row, 0, RowCount - 1);
            }
        }

        /// <inheritdoc cref="IUnlimitedScroller.LastRow"/>
        public int LastRow {
            get {
                var row = (int)((contentTrans.anchoredPosition.y + ViewportHeight - offsetPadding.top) /
                                (cellY + spacingY));
                return Mathf.Clamp(row, 0, RowCount - 1);
            }
        }

        /// <inheritdoc cref="IUnlimitedScroller.FirstCol"/>
        public int FirstCol {
            get {
                var col = (int)((-contentTrans.anchoredPosition.x - offsetPadding.left) / (cellX + spacingX));
                return Mathf.Clamp(col, 0, CellPerRow - 1);
            }
        }

        /// <inheritdoc cref="IUnlimitedScroller.LastCol"/>
        public int LastCol {
            get {
                var col = (int)((-contentTrans.anchoredPosition.x + ViewportWidth - offsetPadding.left) /
                                (cellX + spacingX));
                return Mathf.Clamp(col, 0, CellPerRow - 1);
            }
        }

        /// <inheritdoc cref="IUnlimitedScroller.ContentHeight"/>
        public float ContentHeight {
            get => contentTrans.rect.height;
            private set => contentTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value);
        }

        /// <inheritdoc cref="IUnlimitedScroller.ContentWidth"/>
        public float ContentWidth {
            get => contentTrans.rect.width;
            private set => contentTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value);
        }

        /// <inheritdoc cref="IUnlimitedScroller.ViewportHeight"/>
        public float ViewportHeight => viewportRectTransform.rect.height;
        
        /// <inheritdoc cref="IUnlimitedScroller.ViewportWidth"/>
        public float ViewportWidth => viewportRectTransform.rect.width;
        
        /// <inheritdoc cref="IUnlimitedScroller.CellPerRow"/>
        public int CellPerRow => currentCellPerRow;

        #endregion

        #region Public Fields

        /// <summary>
        /// Match cell per row to the width of Content. If set, cellPerRow will be ignored.
        /// </summary>
        [Tooltip("Match cell per row to the width of Content. If set, cellPerRow will be ignored.")]
        public bool matchContentWidth;

        /// <summary>
        /// Cell Count per row if not match Content width.
        /// </summary>
        [Tooltip("Cell Count per row if not match Content width.")]
        [Min(1)]
        public int cellPerRow = 1;

        /// <summary>
        /// Max size of cached cells.
        /// </summary>
        [Tooltip("Max size of cached cells.")]
        public uint cacheSize;
        
        /// <summary>
        /// The <c>ScrollRect</c> component on ScrollView.
        /// </summary>
        [Tooltip("The ScrollRect component on ScrollView.")]
        public ScrollRect scrollRect;

        public Alignment horizontalAlignment;

        #endregion

        #region Private Fields

        private RectTransform contentTrans;
        private LayoutGroup layoutGroup;
        private RectTransform viewportRectTransform;
        
        private float cellX;
        private float cellY;
        private float spacingX;
        private float spacingY;
        private Padding offsetPadding;

        private int totalCount;
        private GameObject cellPrefab;
        private List<Cell> currentCells;

        private int currentFirstRow;
        private int currentLastRow;
        private int currentFirstCol;
        private int currentLastCol;

        private Action<int, ICell> onCellGenerate;

        private GameObject pendingDestroyGo;
        private LRUCache<int, GameObject> cachedCells;

        private int currentCellPerRow = 1;
        private bool contentStretchesWidth;
        private float initialContentWidth;
        private float initialViewportWidth;
        private bool originalHorizontal;
        private Vector2 lastViewportSize;

        #endregion

        /// <inheritdoc cref="IUnlimitedScroller.Generate"/>
        public void Generate(GameObject newCell, int newTotalCount, Action<int, ICell> onGenerate) {
            if (Generated) return;

            if (!Initialized) Initialize();
            cellPrefab = newCell;
            totalCount = newTotalCount;
            onCellGenerate = onGenerate;
            InitParams();
            Generated = true;
            
            if (totalCount <= 0) return;
            GenerateAllCells();
        }

        public void JumpTo(uint index, JumpToMethod method) {
            if (index >= totalCount) return;
            
            var cellRowCount = index / CellPerRow;
            var cellColCount = index % CellPerRow;
            float verticalPosition, horizontalPosition;
            switch (method) {
                case JumpToMethod.OnScreen:
                    if (cellRowCount >= FirstRow && cellRowCount <= LastRow && cellColCount >= FirstCol &&
                        cellColCount <= LastCol) return;

                    if (cellRowCount > LastRow) {
                        verticalPosition =
                            (offsetPadding.bottom + (RowCount - cellRowCount - 1) * cellSize.y +
                             (RowCount - cellRowCount - 1) * spacingY) /
                            (ContentHeight - ViewportHeight);
                    } else {
                        verticalPosition =
                            (offsetPadding.bottom + (RowCount - cellRowCount) * cellSize.y +
                                (RowCount - cellRowCount - 1) * spacingY - ViewportHeight) /
                            (ContentHeight - ViewportHeight);
                    }

                    if (cellColCount > LastCol) {
                        horizontalPosition =
                            (offsetPadding.left + (cellColCount + 1) * cellSize.x + cellColCount * spacingX -
                             ViewportWidth) / (ContentWidth - ViewportWidth);
                    } else {
                        horizontalPosition =
                            (offsetPadding.left + (cellColCount) * cellSize.x + cellColCount * spacingX) /
                            (ContentWidth - ViewportWidth);
                    }

                    if (ContentHeight > ViewportHeight && !(cellRowCount >= FirstRow && cellRowCount <= LastRow)) {
                        scrollRect.verticalNormalizedPosition = verticalPosition;
                    }

                    if (ContentWidth > ViewportWidth && !(cellColCount >= FirstCol && cellColCount <= LastCol)) {
                        scrollRect.horizontalNormalizedPosition = horizontalPosition;
                    }

                    return;
                case JumpToMethod.Center:
                    verticalPosition =
                        (offsetPadding.bottom + (RowCount - cellRowCount - 0.5f) * cellSize.y +
                            (RowCount - cellRowCount - 1) * spacingY - ViewportHeight / 2f) /
                        (ContentHeight - ViewportHeight);

                    horizontalPosition =
                        (offsetPadding.left + (cellColCount + 0.5f) * cellSize.x + cellColCount * spacingX -
                         ViewportWidth / 2f) / (ContentWidth - ViewportWidth);

                    if (ContentHeight > ViewportHeight) {
                        scrollRect.verticalNormalizedPosition = verticalPosition;
                    }

                    if (ContentWidth > ViewportWidth) {
                        scrollRect.horizontalNormalizedPosition = horizontalPosition;
                    }

                    return;
                default:
                    throw new ArgumentOutOfRangeException(nameof(method), method, null);
            }
        }

        /// <inheritdoc cref="IUnlimitedScroller.SetCacheSize"/>
        public void SetCacheSize(uint newSize) {
            cachedCells.SetCapacity(newSize);
        }

        /// <inheritdoc cref="IUnlimitedScroller.Clear"/>
        public void Clear() {
            if (!Generated) return;
            
            DestroyAllCells();
            ClearCache();
            Destroy(pendingDestroyGo);
            Generated = false;

            ContentHeight = 0f;
            ContentWidth = 0f;
            layoutGroup.padding.top = offsetPadding.top;
            layoutGroup.padding.bottom = offsetPadding.bottom;
            layoutGroup.padding.left = offsetPadding.left;
            layoutGroup.padding.right = offsetPadding.right;
        }

        /// <inheritdoc cref="IUnlimitedScroller.ClearCache"/>
        public void ClearCache() {
            cachedCells.Clear();
        }

        private void Initialize() {
            layoutGroup = GetComponent<LayoutGroup>();
            viewportRectTransform = scrollRect.viewport;
            contentTrans = GetComponent<RectTransform>();
            
            offsetPadding = new Padding {
                top = layoutGroup.padding.top,
                bottom = layoutGroup.padding.bottom,
                left = layoutGroup.padding.left,
                right = layoutGroup.padding.right
            };

            // Remember the content width before generating overrides it, so matchContentWidth still works after
            // Clear() and when the viewport is resized.
            contentStretchesWidth = !Mathf.Approximately(contentTrans.anchorMin.x, contentTrans.anchorMax.x);
            initialContentWidth = contentTrans.rect.width;
            initialViewportWidth = ViewportWidth;
            originalHorizontal = scrollRect.horizontal;

            scrollRect.onValueChanged.AddListener(OnScroll);

            Initialized = true;
        }

        private void InitParams() {
            var gridLayoutGroup = (GridLayoutGroup) layoutGroup;
            cellX = gridLayoutGroup.cellSize.x;
            cellY = gridLayoutGroup.cellSize.y;
            spacingX = gridLayoutGroup.spacing.x;
            spacingY = gridLayoutGroup.spacing.y;

            contentTrans.anchorMin = Vector2.up;
            contentTrans.anchorMax = Vector2.up;
            contentTrans.anchoredPosition = Vector2.zero;
            ApplyLayout();

            currentCells = new List<Cell>();

            pendingDestroyGo = new GameObject("[Cache Node]");
            pendingDestroyGo.transform.SetParent(transform);
            pendingDestroyGo.SetActive(false);

            cachedCells = new LRUCache<int, GameObject>((_, go) => Destroy(go), cacheSize);
            lastViewportSize = viewportRectTransform.rect.size;
        }

        private int CalculateCellPerRow() {
            if (!matchContentWidth) return Mathf.Max(1, cellPerRow);
            if (cellX + spacingX <= 0f) return 1;

            var availableWidth = contentStretchesWidth
                ? initialContentWidth + ViewportWidth - initialViewportWidth
                : initialContentWidth;
            var count = (int)((availableWidth - offsetPadding.left - offsetPadding.right + spacingX) / (cellX + spacingX));
            return Mathf.Max(1, count);
        }

        // Updates cell per row, content size and horizontal alignment. Keeps the vertical scroll position.
        private void ApplyLayout() {
            currentCellPerRow = CalculateCellPerRow();

            ContentHeight = cellY * RowCount + spacingY * (RowCount - 1) + offsetPadding.top + offsetPadding.bottom;
            ContentWidth = cellX * CellPerRow + spacingX * (CellPerRow - 1) + offsetPadding.left + offsetPadding.right;

            float contentX;
            switch (horizontalAlignment) {
                case Alignment.Left:
                    contentX = 0f;
                    break;
                case Alignment.Center:
                    contentX = (ViewportWidth - ContentWidth) / 2f;
                    break;
                case Alignment.Right:
                    contentX = ViewportWidth - ContentWidth;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            scrollRect.horizontal = originalHorizontal &&
                                    (horizontalAlignment == Alignment.Left || ContentWidth >= ViewportWidth);
            contentTrans.anchoredPosition = new Vector2(contentX, contentTrans.anchoredPosition.y);
        }

        private int GetCellIndex(int row, int col) {
            return CellPerRow * row + col;
        }

        private int GetFirstGreater(int index) {
            var start = 0;
            var end = currentCells.Count;
            while (start != end) {
                var middle = start + (end - start) / 2;
                if (currentCells[middle].number <= index) {
                    start = middle + 1;
                } else {
                    end = middle;
                }
            }

            return start;
        }

        private void GenerateCell(int index, ScrollerPanelSide side) {
            ICell iCell;
            if (cachedCells.TryGet(index, out var instance)) {
                instance.transform.SetParent(contentTrans);
                cachedCells.Remove(index);
                
                iCell = instance.GetComponent<ICell>();
            } else {
                instance = Instantiate(cellPrefab, contentTrans);
                instance.name = cellPrefab.name + "_" + index;
                
                iCell = instance.GetComponent<ICell>();
                onCellGenerate?.Invoke(index, iCell);
            }

            var order = GetFirstGreater(index);
            instance.transform.SetSiblingIndex(order);
            var cell = new Cell { go = instance, number = index };
            currentCells.Insert(order, cell);

            iCell.OnBecomeVisible(side);
        }
        
        private void GenerateAllCells() {
            currentFirstCol = FirstCol;
            currentLastCol = LastCol;
            currentFirstRow = FirstRow;
            currentLastRow = LastRow;

            UpdatePadding();
            for (var r = currentFirstRow; r <= currentLastRow; ++r) {
                for (var c = currentFirstCol; c <= currentLastCol; ++c) {
                    var index = GetCellIndex(r, c);
                    if (index >= totalCount) continue;
                    GenerateCell(index, ScrollerPanelSide.NoSide);
                }
            }
        }

        private void DestroyCell(int index, ScrollerPanelSide side) {
            var order = GetFirstGreater(index - 1);
            var cell = currentCells[order];
            currentCells.RemoveAt(order);
            cell.go.GetComponent<ICell>().OnBecomeInvisible(side);
            cell.go.transform.SetParent(pendingDestroyGo.transform);
            cachedCells.Add(index, cell.go);
        }

        private void DestroyAllCells() {
            var total = currentCells.Count;
            for (var i = 0; i < total; i++) {
                var cell = currentCells[0];
                currentCells.RemoveAt(0);
                cell.go.GetComponent<ICell>().OnBecomeInvisible(ScrollerPanelSide.NoSide);
                Destroy(cell.go);
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
        }

        private void GenerateCol(int col, bool onLeft) {
            var firstRow = currentFirstRow;
            var lastRow = currentLastRow;

            for (var i = firstRow; i <= lastRow; i++) {
                var index = GetCellIndex(i, col);
                if (index >= totalCount) continue;
                GenerateCell(index, onLeft ? ScrollerPanelSide.Left : ScrollerPanelSide.Right);
            }
        }

        private void DestroyRow(int row, bool onTop) {
            var firstCol = currentFirstCol;
            var lastCol = currentLastCol;

            var indexEnd = GetCellIndex(row, lastCol);
            indexEnd = indexEnd >= totalCount ? totalCount - 1 : indexEnd;
            for (var i = GetCellIndex(row, firstCol); i <= indexEnd; ++i) {
                DestroyCell(i, onTop ? ScrollerPanelSide.Top : ScrollerPanelSide.Bottom);
            }
        }

        private void DestroyCol(int col, bool onLeft) {
            var firstRow = currentFirstRow;
            var lastRow = currentLastRow;

            for (var i = firstRow; i <= lastRow; i++) {
                var index = GetCellIndex(i, col);
                if (index >= totalCount) continue;
                DestroyCell(index, onLeft ? ScrollerPanelSide.Left : ScrollerPanelSide.Right);
            }
        }

        // Padding is always derived from the visible range so that full rebuilds and row-by-row updates agree.
        private void UpdatePadding() {
            layoutGroup.padding.left = offsetPadding.left + Mathf.RoundToInt(currentFirstCol * (cellX + spacingX));
            layoutGroup.padding.right = offsetPadding.right +
                                        Mathf.RoundToInt((CellPerRow - currentLastCol - 1) * (cellX + spacingX));
            layoutGroup.padding.top = offsetPadding.top + Mathf.RoundToInt(currentFirstRow * (cellY + spacingY));
            layoutGroup.padding.bottom = offsetPadding.bottom +
                                         Mathf.RoundToInt((RowCount - currentLastRow - 1) * (cellY + spacingY));
            SetDirty();
        }

        private void LateUpdate() {
            if (!Generated) return;

            var viewportSize = viewportRectTransform.rect.size;
            if (viewportSize == lastViewportSize) return;

            lastViewportSize = viewportSize;
            OnViewportResized();
        }

        private void OnViewportResized() {
            if (totalCount <= 0) {
                ApplyLayout();
                return;
            }

            var oldCellPerRow = CellPerRow;
            var firstVisibleIndex = GetCellIndex(FirstRow, FirstCol);
            ApplyLayout();

            if (CellPerRow == oldCellPerRow) {
                UpdateVisibleCells();
                return;
            }

            // Cells re-flow into a different number of columns. Keep the first visible cell's row at the top.
            var row = firstVisibleIndex / CellPerRow;
            var y = Mathf.Clamp(offsetPadding.top + row * (cellY + spacingY), 0f,
                Mathf.Max(0f, ContentHeight - ViewportHeight));
            contentTrans.anchoredPosition = new Vector2(contentTrans.anchoredPosition.x, y);
            DestroyAllCells();
            GenerateAllCells();
        }

        private void OnScroll(Vector2 position) {
            UpdateVisibleCells();
        }

        private void UpdateVisibleCells() {
            if (!Generated || totalCount <= 0) return;

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

            UpdatePadding();
        }
    }
}
