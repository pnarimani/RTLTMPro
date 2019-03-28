using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;

namespace RTLTMPro
{
    [CustomEditor(typeof(RTLTextMeshPro)), CanEditMultipleObjects]
    public class RTLTextMeshProEditor : TMP_UiEditorPanel
    {
        private SerializedProperty originalTextProp;
        private SerializedProperty havePropertiesChangedProp;
        private SerializedProperty inputSourceProp;
        private SerializedProperty isInputPasingRequiredProp;
        private SerializedProperty preserveNumbersProp;
        private SerializedProperty farsiProp;
        private SerializedProperty fixTagsProp;
        private SerializedProperty forceFixProp;

        private bool changed;
        private bool foldout;
        private RTLTextMeshPro tmpro;

        private new void OnEnable()
        {
            base.OnEnable();
            foldout = true;
            preserveNumbersProp = serializedObject.FindProperty("preserveNumbers");
            farsiProp = serializedObject.FindProperty("farsi");
            fixTagsProp = serializedObject.FindProperty("fixTags");
            forceFixProp = serializedObject.FindProperty("forceFix");
            originalTextProp = serializedObject.FindProperty("originalText");
            havePropertiesChangedProp = serializedObject.FindProperty("m_havePropertiesChanged");
            inputSourceProp = serializedObject.FindProperty("m_inputSource");
            isInputPasingRequiredProp = serializedObject.FindProperty("m_isInputParsingRequired");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            tmpro = (RTLTextMeshPro) target;
            
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(originalTextProp, new GUIContent("RTL Text Input Box"));

            ListenForZeroWidthNoJoiner();

            if (EditorGUI.EndChangeCheck())
            {
                inputSourceProp.enumValueIndex = 0;
                isInputPasingRequiredProp.boolValue = true;
                changed = true;
            }

            if (changed)
                OnChanged();

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();

            foldout = EditorGUILayout.Foldout(foldout, "RTL Settings", TMP_UIStyleManager.boldFoldout);
            if (foldout)
            {
                DrawOptions();

                if (GUILayout.Button("Re-Fix"))
                    changed = true;

                if (EditorGUI.EndChangeCheck())
                {
                    changed = true;
                }
            }

            if(changed)
                OnChanged();

            serializedObject.ApplyModifiedProperties();
        }

        protected void OnChanged()
        {
            tmpro.UpdateText();
            havePropertiesChangedProp.boolValue = true;
            changed = false;
            EditorUtility.SetDirty(target);
        }

        protected virtual void DrawOptions()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            farsiProp.boolValue = GUILayout.Toggle(farsiProp.boolValue, new GUIContent("Farsi"));
            forceFixProp.boolValue = GUILayout.Toggle(forceFixProp.boolValue, new GUIContent("Force Fix"));
            preserveNumbersProp.boolValue = GUILayout.Toggle(preserveNumbersProp.boolValue, new GUIContent("Preserve Numbers"));

            if (tmpro.richText)
                fixTagsProp.boolValue = GUILayout.Toggle(fixTagsProp.boolValue, new GUIContent("FixTags"));

            EditorGUILayout.EndHorizontal();
        }

        protected virtual void ListenForZeroWidthNoJoiner()
        {
            var editor = (TextEditor) GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);

            bool shortcutPressed = (Event.current.modifiers & EventModifiers.Control) != 0 &&
                                   (Event.current.modifiers & EventModifiers.Shift) != 0 &&
                                   Event.current.type == EventType.KeyUp &&
                                   Event.current.keyCode == KeyCode.Alpha2;

            if (!shortcutPressed) return;

            originalTextProp.stringValue = originalTextProp.stringValue.Insert(editor.cursorIndex, ((char) GeneralLetters.ZeroWidthNoJoiner).ToString());
            editor.selectIndex++;
            editor.cursorIndex++;
            Event.current.Use();
            Repaint();
        }
    }
}