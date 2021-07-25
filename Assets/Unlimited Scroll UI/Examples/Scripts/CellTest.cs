using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnlimitedScrollUI.Example {
    public class CellTest : MonoBehaviour {
        public Text text;
        public GameObject popup;

        private int index;
        
        public void SetText(int newIndex) {
            index = newIndex;
            text.text = $"{index}";
        }

        public void SetupPopup(int newIndex) {
            index = newIndex;
            GetComponent<Button>().onClick.AddListener(() => {
                var instance = Instantiate(popup, GameObject.Find("Canvas").transform);
                instance.GetComponent<Popup>().text.text = $"You just clicked the cell {index}!";
            });
        }

        public void ChangeCount(int count) {
            InfoDisplayer.instance.UpdateCellCount(count);
        }

        public void DisplayVisibleText(ScrollerPanelSide side) {
            var sideName = Enum.GetName(typeof(ScrollerPanelSide), side);
            InfoDisplayer.instance.UpdateTextDisplay($"Cell {index} visible from {sideName}.");
        }
        
        public void DisplayInvisibleText(ScrollerPanelSide side) {
            var sideName = Enum.GetName(typeof(ScrollerPanelSide), side);
            InfoDisplayer.instance.UpdateTextDisplay($"Cell {index} invisible to {sideName}.");
        }
    }
}
