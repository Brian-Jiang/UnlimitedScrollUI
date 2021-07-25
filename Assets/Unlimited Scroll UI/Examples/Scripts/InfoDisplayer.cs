using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnlimitedScrollUI.Example
{
    public class InfoDisplayer : MonoBehaviour {
        public Text cellCount;
        public Text infoDisplay;

        private int totalCell;
        
        public static InfoDisplayer instance;
        
        private void Awake() {
            if (instance == null) {
                instance = this;
            }

            totalCell = 0;
        }

        public void UpdateCellCount(int count) {
            totalCell += count;
            cellCount.text = $"Total cell: {totalCell}";
        }

        public void UpdateTextDisplay(string text) {
            infoDisplay.text = text;
        }
    }
}
