using System.Collections;
using UnityEngine;

namespace UnlimitedScrollUI.Example {
    public class ScrollerTest : MonoBehaviour {
        public GameObject cell;
        public bool autoGenerate;
        public int totalCount = 33;

        private IUnlimitedScroller unlimitedScroller;

        public void Generate() {
            unlimitedScroller.Generate(cell, totalCount, OnCellGenerate);
        }

        private void Start() {
            unlimitedScroller = GetComponent<IUnlimitedScroller>();
            // Wait until the scroller size was set by other layout controllers.
            if (autoGenerate) {
                StartCoroutine(DelayGenerate());
            }
        }

        private IEnumerator DelayGenerate() {
            yield return new WaitForEndOfFrame();
            Generate();
        }

        private static void OnCellGenerate(int index, ICell iCell) {
            var cellTest = ((Component)iCell).GetComponent<CellTest>();
            if (cellTest == null) return;

            cellTest.SetText(index);
            cellTest.SetupPopup(index);
            cellTest.GetInfoDisplayer();
        }
    }
}