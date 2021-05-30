using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AutoLayoutType {
    Vertical, Horizontal, Grid
}

internal struct Cell {
    public int number;
    public GameObject go;
}

[RequireComponent(typeof(ScrollRect))]
public class UnlimitedScroller : MonoBehaviour {
    public bool HasElements { get; private set; } = false;

    public int RowCount =>
        totalCount % ElementPerRow == 0
            ? totalCount / ElementPerRow
            : totalCount / ElementPerRow + 1;

    public int FirstRow {
        get {
            if (layoutType == AutoLayoutType.Horizontal) return 0;

            var row = (int) (contentTrans.anchoredPosition.y / cellY);
            return row < 0 ? 0 : row >= RowCount ? RowCount - 1 : row;
        }
    }

    public int LastRow {
        get {
            if (layoutType == AutoLayoutType.Horizontal) return 0;

            var row = (int) ((contentTrans.anchoredPosition.y + ViewportHeight) / cellY);
            return row < 0 ? 0 : row >= RowCount ? RowCount - 1 : row;
        }
    }

    public int FirstCol {
        get {
            if (layoutType == AutoLayoutType.Vertical) return 0;

            var col = (int) (-contentTrans.anchoredPosition.x / cellX);
            return col < 0 ? 0 : col >= ElementPerRow ? ElementPerRow - 1 : col;
        }
    }

    public int LastCol {
        get {
            if (layoutType == AutoLayoutType.Vertical) return 0;

            var col = (int) ((-contentTrans.anchoredPosition.x + ViewportWidth) / cellX);
            return col < 0 ? 0 : col >= ElementPerRow ? ElementPerRow - 1 : col;
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

    public int ElementPerRow {
        get {
            switch (layoutType) {
                case AutoLayoutType.Vertical:
                    return 1;
                case AutoLayoutType.Horizontal:
                    return totalCount;
                case AutoLayoutType.Grid:
                    return elementPerRow;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public int elementPerRow;
    public bool matchContentWidth;

    public RectTransform contentTrans;
    public LayoutGroup layoutGroup;
    public AutoLayoutType layoutType;
    public int extraRowCount;

    private GameObject storedElement;


    // private int viewportVisibleCount;

    private float cellY;
    private float cellX;

    // private int emptyRows;

    private int totalCount;

    private List<Cell> currentElements;

    private ScrollRect scrollRect;

    private int currentFirstRow;
    private int currentLastRow;
    private int currentFirstCol;
    private int currentLastCol;

    public void Generate(GameObject newElement, int newTotalCount) {
        storedElement = newElement;
        totalCount = newTotalCount;
        InitParams();


        currentFirstCol = FirstCol;
        currentLastCol = LastCol;
        currentFirstRow = FirstRow;
        currentLastRow = LastRow;

        print(ElementPerRow);
        print($"first col: {currentFirstCol}, last col: {currentLastCol}");
        print($"first row: {currentFirstRow}, last row: {currentLastRow}");
        layoutGroup.padding.right = (int) ((ElementPerRow - LastCol - 1) * cellX);
        layoutGroup.padding.bottom = (int) ((RowCount - LastRow - 1) * cellY);
        for (var r = currentFirstRow; r <= currentLastRow; ++r) {
            for (var c = currentFirstCol; c <= currentLastCol; ++c) {
                GenerateCell(GetCellIndex(r, c));
            }
        }
    }

    private void InitParams() {
        var rect = storedElement.GetComponent<RectTransform>().rect;
        switch (layoutType) {
            case AutoLayoutType.Vertical:
                cellX = rect.width;
                cellY = rect.height;
                elementPerRow = 1;
                break;
            case AutoLayoutType.Horizontal:
                cellX = rect.width;
                cellY = rect.height;
                elementPerRow = totalCount;
                break;
            case AutoLayoutType.Grid:
                var gridLayoutGroup = (GridLayoutGroup) layoutGroup;
                cellX = gridLayoutGroup.cellSize.x;
                cellY = gridLayoutGroup.cellSize.y;
                elementPerRow = matchContentWidth ? (int) (ContentWidth / cellX) : elementPerRow;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        currentElements = new List<Cell>();
        ContentHeight = cellY * (
            totalCount % ElementPerRow == 0
                ? totalCount / ElementPerRow
                : totalCount / ElementPerRow + 1);
        print($"cellY: {cellY}, total: {totalCount}, ele/row: {ElementPerRow}");
        ContentWidth = cellX * ElementPerRow;
        // viewportVisibleCount = (int)(ViewportHeight / cellY + extraRowCount + 1) * elementPerRow;
        // emptyRows = 0;
    }

    private int GetCellIndex(int row, int col) {
        return ElementPerRow * row + col;
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

    private void GenerateCell(int index) {
        var order = GetFirstGreater(index);
        var instance = Instantiate(storedElement, contentTrans);
        instance.GetComponent<Transform>().SetSiblingIndex(order);
        var cell = new Cell() {go = instance, number = index};
        currentElements.Insert(order, cell);

        var slidingElement = instance.GetComponent<ICell>();
        slidingElement.OnGenerated(index);
    }

    private void DestroyCell(int index) {
        var order = GetFirstGreater(index - 1);
        var cell = currentElements[order];
        currentElements.RemoveAt(order);
        DestroyImmediate(cell.go);
    }

    private void DestroyAllCells() {
        var total = currentElements.Count;
        for (var i = 0; i < total; i++) {
            var cell = currentElements[0];
            currentElements.RemoveAt(0);
            DestroyImmediate(cell.go);
        }
    }

    private void GenerateAllCells() {
        currentFirstCol = FirstCol;
        currentLastCol = LastCol;
        currentFirstRow = FirstRow;
        currentLastRow = LastRow;

        // print(ElementPerRow);
        // print($"first col: {currentFirstCol}, last col: {currentLastCol}");
        // print($"first row: {currentFirstRow}, last row: {currentLastRow}");
        layoutGroup.padding.left = (int) (currentFirstCol * cellX);
        layoutGroup.padding.right = (int) ((ElementPerRow - LastCol - 1) * cellX);
        layoutGroup.padding.top = (int) (currentFirstRow * cellY);
        layoutGroup.padding.bottom = (int) ((RowCount - LastRow - 1) * cellY);
        for (var r = currentFirstRow; r <= currentLastRow; ++r) {
            for (var c = currentFirstCol; c <= currentLastCol; ++c) {
                GenerateCell(GetCellIndex(r, c));
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
        for (var i = GetCellIndex(row, firstCol); i <= indexEnd; ++i) {
            GenerateCell(i);
        }

        if (onTop) layoutGroup.padding.top -= (int) cellY;
        else layoutGroup.padding.bottom -= (int) cellY;
    }

    private void GenerateCol(int col, bool onLeft) {
        // print($"generate col: {col}, onLeft = {onLeft}");
        // var firstRow = FirstRow;
        // var lastRow = LastRow;
        var firstRow = currentFirstRow;
        var lastRow = currentLastRow;

        for (var i = firstRow; i <= lastRow; i++) {
            var index = GetCellIndex(i, col);
            GenerateCell(index);
        }

        if (onLeft) layoutGroup.padding.left -= (int) cellX;
        else layoutGroup.padding.right -= (int) cellX;
    }

    private void DestroyRow(int row, bool onTop) {
        // print($"destroy row: {row}, ontop = {onTop}");
        // var firstCol = FirstCol;
        // var lastCol = LastCol;
        var firstCol = currentFirstCol;
        var lastCol = currentLastCol;

        var indexEnd = GetCellIndex(row, lastCol);
        for (var i = GetCellIndex(row, firstCol); i <= indexEnd; ++i) {
            DestroyCell(i);
        }

        if (onTop) layoutGroup.padding.top += (int) cellY;
        else layoutGroup.padding.bottom += (int) cellY;
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
            DestroyCell(index);
        }

        if (onLeft) layoutGroup.padding.left += (int) cellX;
        else layoutGroup.padding.right += (int) cellX;
    }

    private void Start() {
        scrollRect = GetComponent<ScrollRect>();
        scrollRect.onValueChanged.AddListener(OnScroll);
    }

    private void OnScroll(Vector2 position) {
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
