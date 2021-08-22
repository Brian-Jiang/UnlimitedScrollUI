using UnityEditor;
using UnityEditor.UI;

namespace UnlimitedScrollUI.Editor {
    [CustomEditor(typeof(GridUnlimitedScroller), true)]
    [CanEditMultipleObjects]
    public class GridUnlimitedScrollerEditor : GridLayoutGroupEditor {
        private SerializedProperty matchContentWidth;
        private SerializedProperty cellPerRow;
        private SerializedProperty scrollRect;

        protected override void OnEnable() {
            base.OnEnable();

            matchContentWidth = serializedObject.FindProperty("matchContentWidth");
            cellPerRow = serializedObject.FindProperty("cellPerRow");
            scrollRect = serializedObject.FindProperty("scrollRect");
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(matchContentWidth, true);
            if (!matchContentWidth.boolValue) {
                EditorGUILayout.PropertyField(cellPerRow, true);
            }

            EditorGUILayout.PropertyField(scrollRect, true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
