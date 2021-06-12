using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour {
    public Text text;
    public Button btn;

    private void Start() {
        btn.onClick.AddListener(() => Destroy(gameObject));
    }
}
