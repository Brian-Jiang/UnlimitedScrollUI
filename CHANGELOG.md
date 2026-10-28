## 1.5.1
- Fix cells shifting by one spacing after cell jump (fast fling or `JumpTo`) when spacing is not zero.
- Fix divide-by-zero in `GridUnlimitedScroller` when `cellPerRow` is 0 or `matchContentWidth` fits no cell.
- `GridUnlimitedScroller.cellPerRow` is no longer overwritten when `matchContentWidth` is set.
- Scrollers now react to viewport size changes (rotation, resolution, split-screen).
- `RegularCell` animations now use unscaled time.
- `RegularCell.onGenerated` is marked obsolete.

## 1.5.0
- API change: delete `iCell.OnGenerated` and add `onGenerate` action callback to `IUnlimitedScroller.Generate`
  so that its easier to pass data to cell.

## 1.4.0
- Add jump to cell API to all scrollers