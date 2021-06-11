using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour {
    public TextMeshProUGUI text;
    public Button btn;

    private void Start() {
        btn.onClick.AddListener(() => Destroy(gameObject));
    }
}
