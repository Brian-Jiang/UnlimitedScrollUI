using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CellTest : MonoBehaviour {
    public TextMeshProUGUI text;

    public void SetText(int index) {
        text.text = $"{index}";
    }

}
