using UnityEditor;
using UnityEditor.UI;

namespace UnlimitedScrollUI.Editor {
    [CustomEditor(typeof(HorizontalUnlimitedScroller), true)]
    [CanEditMultipleObjects]
    public class HorizontalUnlimitedScrollerEditor : HorizontalOrVerticalLayoutGroupEditor {
        private SerializedProperty cacheSize;
        private SerializedProperty scrollRect;

        protected override void OnEnable() {
            base.OnEnable();

            cacheSize = serializedObject.FindProperty("cacheSize");
            scrollRect = serializedObject.FindProperty("scrollRect");
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            
            EditorGUILayout.PropertyField(cacheSize, true);
            EditorGUILayout.PropertyField(scrollRect, true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
