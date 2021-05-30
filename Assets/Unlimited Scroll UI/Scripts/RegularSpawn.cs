using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegularSpawn : MonoBehaviour
{
    public GameObject element;
    public int totalCount = 33;
    public RectTransform parent;

    // private UnlimitedScroller container;

    private void Start() {
        parent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 100f * (totalCount / 10f));
        for (int i = 0; i < totalCount; i++) {
            var go = Instantiate(element, parent);
            go.GetComponent<RegularCell>().OnGenerated(i);
        }
    }
}
