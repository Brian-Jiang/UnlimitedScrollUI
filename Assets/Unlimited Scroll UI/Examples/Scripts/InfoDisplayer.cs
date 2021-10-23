using UnityEngine;
using UnityEngine.UI;

namespace UnlimitedScrollUI.Example {
    public class InfoDisplayer : MonoBehaviour {
        public InputField sizeInput;
        public Button confirmBtn;
        
        public Text cellCount;
        public Text visibleDisplay;
        public Text invisibleDisplay;

        public GridUnlimitedScroller gridUnlimitedScroller;

        private int totalCell;

        public static InfoDisplayer instance;

        private void Awake() {
            if (instance == null) {
                instance = this;
            }

            totalCell = 0;
            confirmBtn.onClick.AddListener(() => gridUnlimitedScroller.SetCacheSize(uint.Parse(sizeInput.text)));
        }

        public void UpdateCellCount(int count) {
            totalCell += count;
            cellCount.text = $"Total cell: {totalCell}";
        }

        public void UpdateVisibleDisplay(string text) {
            visibleDisplay.text = text;
        }

        public void UpdateInvisibleDisplay(string text) {
            invisibleDisplay.text = text;
        }
    }
}
