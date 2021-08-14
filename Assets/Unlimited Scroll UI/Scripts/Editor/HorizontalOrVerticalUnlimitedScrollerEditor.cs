using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace UnlimitedScrollUI
{
    [CustomEditor(typeof(HorizontalUnlimitedScroller), true)]
    [CanEditMultipleObjects]
    public class HorizontalOrVerticalUnlimitedScrollerEditor : HorizontalOrVerticalLayoutGroupEditor
    {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            
            // Debug.Log("on inspector");
        }
    }
}
