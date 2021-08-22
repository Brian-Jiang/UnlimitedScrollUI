using UnityEditor;
using UnityEditor.UI;

namespace UnlimitedScrollUI.Editor {
    [CustomEditor(typeof(HorizontalUnlimitedScroller), true)]
    [CanEditMultipleObjects]
    public class HorizontalUnlimitedScrollerEditor : HorizontalOrVerticalLayoutGroupEditor {
        private SerializedProperty scrollRect;

        protected override void OnEnable() {
            base.OnEnable();

            scrollRect = serializedObject.FindProperty("scrollRect");
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(scrollRect, true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
