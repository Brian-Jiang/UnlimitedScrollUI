using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollerTest : MonoBehaviour {
    public GameObject cell;
    public int totalCount = 33;
    
    private UnlimitedScroller unlimitedScroller;

    private void Start() {
        unlimitedScroller = GetComponent<UnlimitedScroller>();
        unlimitedScroller.Generate(cell, totalCount);
    }
}
