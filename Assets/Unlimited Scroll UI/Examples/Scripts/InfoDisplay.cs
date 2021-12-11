using UnityEngine;
using UnityEngine.UI;

namespace UnlimitedScrollUI.Example {
    public class InfoDisplay : MonoBehaviour {
        public InputField sizeInput;
        public Button confirmBtn;
        
        public Text cellCount;
        public Text visibleDisplay;
        public Text invisibleDisplay;

        public GameObject content;
        
        private IUnlimitedScroller unlimitedScroller;
        private int totalCell;

        private void Awake() {
            totalCell = 0;
            unlimitedScroller = content.GetComponent<IUnlimitedScroller>();
            confirmBtn.onClick.AddListener(() => unlimitedScroller.SetCacheSize(uint.Parse(sizeInput.text)));
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
