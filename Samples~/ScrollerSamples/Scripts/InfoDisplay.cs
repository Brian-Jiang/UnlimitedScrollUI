using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UnlimitedScrollUI.Example {
    public class InfoDisplay : MonoBehaviour {
        public InputField sizeInput;
        [FormerlySerializedAs("confirmBtn")]
        public Button sizeConfirmBtn;

        public InputField jumpInput;
        public Button centerJumpConfirmBtn;
        public Button onScreenJumpConfirmBtn;
        
        public Text cellCount;
        public Text visibleDisplay;
        public Text invisibleDisplay;

        public GameObject content;
        
        private IUnlimitedScroller unlimitedScroller;
        private int totalCell;

        private void Awake() {
            totalCell = 0;
            unlimitedScroller = content.GetComponent<IUnlimitedScroller>();
            sizeConfirmBtn.onClick.AddListener(() => unlimitedScroller.SetCacheSize(uint.Parse(sizeInput.text)));
            centerJumpConfirmBtn.onClick.AddListener(() => unlimitedScroller.JumpTo(uint.Parse(jumpInput.text), JumpToMethod.Center));
            onScreenJumpConfirmBtn.onClick.AddListener(() => unlimitedScroller.JumpTo(uint.Parse(jumpInput.text), JumpToMethod.OnScreen));
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
