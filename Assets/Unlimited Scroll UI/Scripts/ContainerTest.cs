using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerTest : MonoBehaviour {
    public GameObject element;
    public int totalCount = 33;
    
    private UnlimitedScroller container;

    private void Start() {
        container = GetComponent<UnlimitedScroller>();
        container.Generate(element, totalCount);
    }
}
