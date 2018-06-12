using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;

namespace RTLTMPro
{
    [CustomEditor(typeof(RTLTextMeshPro)), CanEditMultipleObjects]
    public class RTLTextMeshProEditor : TMP_UiEditorPanel
    {
        private static readonly string[] UIStateLabel = { "\t- <i>Click to expand</i> -", "\t- <i>Click to collapse</i> -" };
        private SerializedProperty originalTextProp;
        private SerializedProperty textProp;
        private SerializedProperty havePropertiesChangedProp;
        private SerializedProperty inputSourceProp;
        private SerializedProperty isInputPasingRequiredProp;
        private SerializedProperty preserveNumbersProp;
        private SerializedProperty farsiProp;
        private SerializedProperty preserveTashkeelProp;
        private SerializedProperty fixTagsProp;
        private bool changed;
        private bool foldout;
        private GUIStyle fixNumberStyle;
        private RTLTextMeshPro tmpro;

        private new void OnEnable()
        {
            base.OnEnable();
            foldout = true;
            textProp = serializedObject.FindProperty("m_text");
            preserveNumbersProp = serializedObject.FindProperty("preserveNumbers");
            farsiProp = serializedObject.FindProperty("farsi");
            preserveTashkeelProp = serializedObject.FindProperty("preserveTashkeel");
            fixTagsProp = serializedObject.FindProperty("fixTags");
            originalTextProp = serializedObject.FindProperty("originalText");
            havePropertiesChangedProp = serializedObject.FindProperty("m_havePropertiesChanged");
            inputSourceProp = serializedObject.FindProperty("m_inputSource");
            isInputPasingRequiredProp = serializedObject.FindProperty("m_isInputParsingRequired");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            tmpro = (RTLTextMeshPro)target;

            // Copy Default GUI Toggle Style
            if (fixNumberStyle == null)
            {
                fixNumberStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 12,
                    normal = { textColor = TMP_UIStyleManager.Section_Label.normal.textColor },
                    richText = true
                };
            }

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUI.BeginChangeCheck();
            farsiProp.boolValue = GUILayout.Toggle(farsiProp.boolValue, new GUIContent("Farsi"));
            preserveNumbersProp.boolValue = GUILayout.Toggle(preserveNumbersProp.boolValue, new GUIContent("Preserve Numbers"));
            preserveTashkeelProp.boolValue = GUILayout.Toggle(preserveTashkeelProp.boolValue, new GUIContent("Preserve Tashkeel"));

            if (tmpro.richText)
                fixTagsProp.boolValue = GUILayout.Toggle(fixTagsProp.boolValue, new GUIContent("FixTags"));

            if (GUILayout.Button("Re-Fix"))
                changed = true;

            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                changed = true;
            }



            Rect rect = EditorGUILayout.GetControlRect(false, 25);
            rect.y += 2;

            GUI.Label(rect, "<b>RTL TEXT INPUT BOX</b>" + (foldout ? UIStateLabel[1] : UIStateLabel[0]), TMP_UIStyleManager.Section_Label);
            if (GUI.Button(new Rect(rect.x, rect.y, rect.width - 150, rect.height), GUIContent.none, GUI.skin.label))
                foldout = !foldout;

            if (foldout)
            {
                EditorGUI.BeginChangeCheck();
                originalTextProp.stringValue = EditorGUILayout.TextArea(originalTextProp.stringValue, TMP_UIStyleManager.TextAreaBoxEditor, GUILayout.Height(125), GUILayout.ExpandWidth(true));

                if (EditorGUI.EndChangeCheck())
                {
                    inputSourceProp.enumValueIndex = 0;
                    isInputPasingRequiredProp.boolValue = true;
                    changed = true;
                }
            }

            serializedObject.ApplyModifiedProperties();

            if (changed)
            {
                textProp.stringValue = tmpro.GetFixedText(originalTextProp.stringValue);
                havePropertiesChangedProp.boolValue = true;
                changed = false;
                EditorUtility.SetDirty(target);
            }

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }
}
