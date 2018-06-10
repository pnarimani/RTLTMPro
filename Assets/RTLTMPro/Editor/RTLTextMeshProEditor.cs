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
        private SerializedProperty fixNumbersProp;
        private SerializedProperty preserveTashkil;
        private bool changed;
        private bool foldout;
        private GUIStyle fixNumberStyle;


        private new void OnEnable()
        {
            base.OnEnable();
            foldout = true;
            textProp = serializedObject.FindProperty("m_text");
            fixNumbersProp = serializedObject.FindProperty("fixNumbers");
            preserveTashkil = serializedObject.FindProperty("preserveTashkil");
            originalTextProp = serializedObject.FindProperty("originalText");
            havePropertiesChangedProp = serializedObject.FindProperty("m_havePropertiesChanged");
            inputSourceProp = serializedObject.FindProperty("m_inputSource");
            isInputPasingRequiredProp = serializedObject.FindProperty("m_isInputParsingRequired");

            // Collapse the normal normal input field from parent
            // Commented out because we it collapses everything globaly
            //var nestedFoldout = typeof(TMP_UiEditorPanel).GetNestedType("m_foldout", BindingFlags.Static | BindingFlags.NonPublic);
            //var textInputField = nestedFoldout.GetField("textInput");
            //textInputField.SetValue(null, false);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

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
            fixNumbersProp.boolValue = GUILayout.Toggle(fixNumbersProp.boolValue, new GUIContent("Fix Numbers"));
            preserveTashkil.boolValue = GUILayout.Toggle(preserveTashkil.boolValue, new GUIContent("Preserve Tashkil"));
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

            GUI.Label(new Rect(rect.width - 125, rect.y + 4, 125, 24), "<i>Fix Numbers</i>", fixNumberStyle);
            fixNumbersProp.boolValue = EditorGUI.Toggle(new Rect(rect.width - 10, rect.y + 3, 20, 24), GUIContent.none, fixNumbersProp.boolValue);

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

            if (changed)
            {
                textProp.stringValue = ((RTLTextMeshPro)target).GetFixedText(originalTextProp.stringValue);
                havePropertiesChangedProp.boolValue = true;
                changed = false;
                EditorUtility.SetDirty(target);
            }

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }
}
