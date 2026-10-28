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
        /// Deprecated. Scrollers have not invoked this since 1.5.0. Initialize cells in the <c>onGenerate</c>
        /// callback passed to <see cref="IUnlimitedScroller.Generate"/> instead.
        /// </summary>
        [Obsolete("Not invoked by scrollers since 1.5.0. Initialize cells in the onGenerate callback passed to " +
                  "IUnlimitedScroller.Generate.")]
        [Tooltip("Deprecated: not invoked by scrollers. Use the onGenerate callback of IUnlimitedScroller.Generate.")]
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
        private Coroutine animCoroutine;

        private void Awake() {
            canvasGroup = GetComponent<CanvasGroup>();
            rectTransform = GetComponent<RectTransform>();
        }

        private void OnDisable() {
            // Unity stops coroutines on disable (e.g. the cell moving into the inactive cache), so don't leave the
            // cell half animated.
            if (animCoroutine == null) return;

            animCoroutine = null;
            ApplyAnim(1f);
        }

        /// <inheritdoc cref="ICell.OnBecomeVisible"/>
        public void OnBecomeVisible(ScrollerPanelSide side) {
            onBecomeVisible.Invoke(side);

            // An inactive cell can't run the animation; OnDisable already left it in its final state.
            if (animationType == AnimationType.None || !isActiveAndEnabled) return;

            if (animCoroutine != null) {
                StopCoroutine(animCoroutine);
                animCoroutine = null;
            }

            if (animInterval <= 0f) {
                ApplyAnim(1f);
                return;
            }

            animCoroutine = StartCoroutine(PlayAnimIn());
        }

        /// <inheritdoc cref="ICell.OnBecomeInvisible"/>
        public void OnBecomeInvisible(ScrollerPanelSide side) {
            onBecomeInvisible.Invoke(side);
        }

        private IEnumerator PlayAnimIn() {
            var t = 0f;
            ApplyAnim(t);
            while (t < 1f) {
                yield return null;

                // Unscaled so cells still appear while the game is paused with timeScale = 0.
                t = Mathf.MoveTowards(t, 1f, Time.unscaledDeltaTime / animInterval);
                ApplyAnim(t);
            }

            animCoroutine = null;
        }

        private void ApplyAnim(float t) {
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
        }
    }
}