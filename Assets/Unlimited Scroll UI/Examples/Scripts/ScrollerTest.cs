using UnityEngine;
using UnlimitedScrollUI;

public class ScrollerTest : MonoBehaviour {
    public GameObject cell;
    public int totalCount = 33;
    
    private IUnlimitedScroller unlimitedScroller;

    private void Start() {
        unlimitedScroller = GetComponent<IUnlimitedScroller>();
        unlimitedScroller.Generate(cell, totalCount);
    }
}
