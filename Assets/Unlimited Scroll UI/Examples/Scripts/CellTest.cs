using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellTest : MonoBehaviour {
    public Text text;
    public GameObject popup;

    public void SetText(int index) {
        text.text = $"{index}";
    }

    public void SetupPopup(int index) {
        GetComponent<Button>().onClick.AddListener(() => {
            var instance = Instantiate(popup, GameObject.Find("Canvas").transform);
            instance.GetComponent<Popup>().text.text = $"You just clicked the cell {index}!";
        });
    }
}
