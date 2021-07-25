using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace UnlimitedScrollUI {
    public enum AnimationType {
        None,
        Fade,
        Scale,
        FadeAndScale
    }

    [Serializable]
    public class GenerateEvent : UnityEvent<int> { }
    
    [Serializable]
    public class BecomeVisibleEvent : UnityEvent<ScrollerPanelSide> { }
    
    [Serializable]
    public class BecomeInvisibleEvent : UnityEvent<ScrollerPanelSide> { }
    

    [RequireComponent(typeof(CanvasGroup))]
    public class RegularCell : MonoBehaviour, ICell {
        public AnimationType animationType;
        [Range(0f, 1f)] public float animInterval;
        [Range(0f, 1f)] public float fadeFrom;
        [Range(0f, 1f)] public float scaleFrom;

        public GenerateEvent onGenerated;
        public BecomeVisibleEvent onBecomeVisible;
        public BecomeInvisibleEvent onBecomeInvisible;

        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;

        public void OnGenerated(int index) {
            onGenerated.Invoke(index);
            if (animationType == AnimationType.None) return;

            canvasGroup = GetComponent<CanvasGroup>();
            rectTransform = GetComponent<RectTransform>();
            canvasGroup.alpha = animationType == AnimationType.Scale ? 1f : fadeFrom;
            StartCoroutine(PlayAnimIn());
        }

        public void OnBecomeVisible(ScrollerPanelSide side) {
            onBecomeVisible.Invoke(side);
        }

        public void OnBecomeInvisible(ScrollerPanelSide side) {
            onBecomeInvisible.Invoke(side);
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

                switch (animationType) {
                    case AnimationType.None:
                        break;
                    case AnimationType.Fade:
                        canvasGroup.alpha = Mathf.Lerp(fadeFrom, 1f, t);
                        break;
                    case AnimationType.Scale:
                        rectTransform.localScale = Vector3.one * Mathf.Lerp(scaleFrom, 1f, t);
                        break;
                    case AnimationType.FadeAndScale:
                        canvasGroup.alpha = Mathf.Lerp(fadeFrom, 1f, t);
                        rectTransform.localScale = Vector3.one * Mathf.Lerp(scaleFrom, 1f, t);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                yield return null;
            }
        }
    }
}