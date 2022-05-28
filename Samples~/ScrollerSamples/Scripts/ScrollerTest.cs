using System.Collections;
using UnityEngine;

namespace UnlimitedScrollUI.Example {
    public class ScrollerTest : MonoBehaviour {
        public GameObject cell;
        public bool autoGenerate;
        public int totalCount = 33;

        private IUnlimitedScroller unlimitedScroller;

        public void Generate() {
            unlimitedScroller.Generate(cell, totalCount, (index, iCell) => {
                var regularCell = iCell as RegularCell;
                if (regularCell != null) regularCell.onGenerated?.Invoke(index);
            });
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
            unlimitedScroller.Generate(cell, totalCount, (index, iCell) => {
                var regularCell = iCell as RegularCell;
                if (regularCell != null) regularCell.onGenerated?.Invoke(index);
            });
        }
    }
}