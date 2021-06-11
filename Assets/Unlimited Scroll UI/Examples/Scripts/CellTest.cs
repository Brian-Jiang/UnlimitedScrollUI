using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellTest : MonoBehaviour {
    public TextMeshProUGUI text;
    public GameObject popup;

    public void SetText(int index) {
        text.text = $"{index}";
    }

    public void OpenPopup(int index) {
        GetComponent<Button>().onClick.AddListener(() => {
            var instance = Instantiate(popup, GameObject.Find("Canvas").transform);
            instance.GetComponent<Popup>().text.text = $"You just clicked the cell {index}!";
        });
    }
}
