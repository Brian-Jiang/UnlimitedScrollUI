using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class RegularCell : MonoBehaviour, ICell {
    // private SlidingWindowElement element;
    public TextMeshProUGUI text;
    public float animInterval;
    
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    public void OnGenerated(int index) {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup.alpha = 0f;
        StartCoroutine(PlayAnimIn());
        // print($"generated, index = {index}");
        text.text = $"{index}";
    }

    private IEnumerator PlayAnimIn() {
        var t = 0f;
        var willFinish = false;
        while (!willFinish) {
            t = Mathf.MoveTowards(t, 1f, Time.deltaTime / animInterval);
            if (Mathf.Approximately(t, 1f)) {
                t = 1f;
                willFinish = true;
            }
            
            canvasGroup.alpha = t;
            rectTransform.localScale = Vector3.one * t;

            yield return null;
        }
        
    }
}
