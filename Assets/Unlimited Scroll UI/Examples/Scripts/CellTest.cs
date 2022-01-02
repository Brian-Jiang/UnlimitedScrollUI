using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UnlimitedScrollUI.Example {
    public class CellTest : MonoBehaviour {
        public Text text;
        public Image bg;
        public GameObject popup;
        
        private InfoDisplay infoDisplay;
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

        public void GetInfoDisplayer() {
            infoDisplay = FindObjectOfType<InfoDisplay>();
        }

        public void ChangeCount(int count) {
            if (!infoDisplay) return;
            
            infoDisplay.UpdateCellCount(count);
        }

        public void DisplayVisibleText(ScrollerPanelSide side) {
            if (!infoDisplay) return;
            
            var sideName = Enum.GetName(typeof(ScrollerPanelSide), side);
            infoDisplay.UpdateVisibleDisplay($"Cell {index} visible from {sideName}.");
        }
        
        public void DisplayInvisibleText(ScrollerPanelSide side) {
            if (!infoDisplay) return;
            
            var sideName = Enum.GetName(typeof(ScrollerPanelSide), side);
            infoDisplay.UpdateInvisibleDisplay($"Cell {index} invisible to {sideName}.");
        }

        public void Shine(params object[] args) {
            var color = (Color) args[0];
            StartCoroutine(Shine(color));
        }

        private IEnumerator Shine(Color color) {
            var currentColor = bg.color;
            bg.color = color;
            var alpha = 0f;
            const float speed = 2f;
            while (true) {
                alpha += speed * Time.deltaTime;
                bg.color = Color.Lerp(color, currentColor, alpha);
                if (alpha >= 1f) {
                    yield break;
                }
                yield return null;
            }
            
        }
    }
}
