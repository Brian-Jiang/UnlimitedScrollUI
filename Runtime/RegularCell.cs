using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace UnlimitedScrollUI {
    /// <summary>
    /// The animation type for regular cell.
    /// </summary>
    public enum AnimationType {
        None,
        Fade,
        Scale,
        FadeAndScale
    }

    /// <summary>
    /// Will be called when the cell generate.
    /// </summary>
    [Serializable]
    public class GenerateEvent : UnityEvent<int> { }
    
    /// <summary>
    /// Will be called when the cell become visible in the viewport.
    /// </summary>
    [Serializable]
    public class BecomeVisibleEvent : UnityEvent<ScrollerPanelSide> { }
    
    /// <summary>
    /// Will be called when the cell become invisible in the viewport.
    /// </summary>
    [Serializable]
    public class BecomeInvisibleEvent : UnityEvent<ScrollerPanelSide> { }
    

    /// <summary>
    /// Regular cell that you can use to quickly setup your cell.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class RegularCell : MonoBehaviour, ICell {
        /// <summary>
        /// What kind of animation you want.
        /// </summary>
        [Tooltip("What kind of animation you want.")]
        public AnimationType animationType;

        /// <summary>
        /// How long is the animation.
        /// </summary>
        [Tooltip("How long is the animation.")] [Range(0f, 1f)]
        public float animInterval;

        /// <summary>
        /// Fade from this value if the animation has fading.
        /// </summary>
        [Tooltip("Fade from this value if the animation has fading.")] [Range(0f, 1f)]
        public float fadeFrom;

        /// <summary>
        /// Scale from this value if the animation has scaling.
        /// </summary>
        [Tooltip("Scale from this value if the animation has scaling.")] [Range(0f, 1f)]
        public float scaleFrom;

        /// <summary>
        /// Will be called when the cell generate. Add listeners that take an int parameter which represent the index
        /// of that cell.
        /// </summary>
        public GenerateEvent onGenerated;
        
        /// <summary>
        /// Will be called when the cell become visible in the viewport.
        /// </summary>
        public BecomeVisibleEvent onBecomeVisible;
        
        /// <summary>
        /// Will be called when the cell become invisible in the viewport.
        /// </summary>
        public BecomeInvisibleEvent onBecomeInvisible;

        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;

        /// <inheritdoc cref="ICell.OnBecomeVisible"/>
        public void OnBecomeVisible(ScrollerPanelSide side) {
            onBecomeVisible.Invoke(side);
            
            if (animationType == AnimationType.None) return;

            canvasGroup = GetComponent<CanvasGroup>();
            rectTransform = GetComponent<RectTransform>();
            canvasGroup.alpha = animationType == AnimationType.Scale ? 1f : fadeFrom;
            StartCoroutine(PlayAnimIn());
        }

        /// <inheritdoc cref="ICell.OnBecomeInvisible"/>
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