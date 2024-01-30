using System.Reflection;
using TMPro;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;

#if TMP_VERSION_2_1_0_OR_NEWER
using TMP_UiEditorPanel = TMPro.EditorUtilities.TMP_EditorPanelUI;
#else
using TMP_UiEditorPanel = TMPro.EditorUtilities.TMP_UiEditorPanel;
#endif

namespace RTLTMPro
{
    [CustomEditor(typeof(RTLTextMeshPro)), CanEditMultipleObjects]
    public class RTLTextMeshProEditor : TMP_UiEditorPanel
    {
        static readonly GUIContent k_RtlToggleLabel = new GUIContent("Enable RTL Editor", "Reverses text direction and allows right to left editing.");
        static readonly GUIContent k_StyleLabel = new GUIContent("Text Style", "The style from a style sheet to be applied to the text.");

        FieldInfo m_inputSourceField;
        FieldInfo m_TextStyleField;
        
        RTLTextMeshPro rTLTextMeshPro;

        private SerializedProperty enableRTLProp;
        private SerializedProperty originalTextProp;
        private SerializedProperty preserveNumbersProp;
        private SerializedProperty farsiProp;
        private SerializedProperty fixTagsProp;
        private SerializedProperty forceFixProp;

        private bool foldout;
        private RTLTextMeshPro tmpro;

        protected override void OnEnable()
        {
            rTLTextMeshPro = (RTLTextMeshPro)target;
            base.OnEnable();
            foldout = true;

            m_inputSourceField = m_TextComponent.GetType().GetField("m_TextComponent", BindingFlags.Instance | BindingFlags.NonPublic);
            m_TextStyleField = m_TextComponent.GetType().GetField("m_TextStyle", BindingFlags.Instance | BindingFlags.NonPublic);

            enableRTLProp = serializedObject.FindProperty("enalbeRTL");
            preserveNumbersProp = serializedObject.FindProperty("preserveNumbers");
            farsiProp = serializedObject.FindProperty("farsi");
            fixTagsProp = serializedObject.FindProperty("fixTags");
            forceFixProp = serializedObject.FindProperty("forceFix");
            originalTextProp = serializedObject.FindProperty("originalText");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            tmpro = (RTLTextMeshPro)target;

            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(originalTextProp, new GUIContent("RTL Text Input Box"));

            ListenForZeroWidthNoJoiner();

            if (EditorGUI.EndChangeCheck())
                OnChanged();

            serializedObject.ApplyModifiedProperties();

            // Make sure Multi selection only includes TMP Text objects.
            if (IsMixSelectionTypes())
                return;

            serializedObject.Update();

            DrawTextInput();

            DrawMainSettings();

            DrawExtraSettings();

            EditorGUILayout.Space();

            if (serializedObject.ApplyModifiedProperties() || m_HavePropertiesChanged)
            {
                m_TextComponent.havePropertiesChanged = true;
                m_HavePropertiesChanged = false;
                EditorUtility.SetDirty(target);
            }

            foldout = EditorGUILayout.Foldout(foldout, "RTL Settings", TMP_UIStyleManager.boldFoldout);
            if (foldout)
            {
                DrawOptions();

                if (GUILayout.Button("Re-Fix"))
                    m_HavePropertiesChanged = true;

                if (EditorGUI.EndChangeCheck())
                    m_HavePropertiesChanged = true;
            }

            if (m_HavePropertiesChanged)
                OnChanged();

            serializedObject.ApplyModifiedProperties();
        }

        protected void OnChanged()
        {
            tmpro.UpdateText();
            m_HavePropertiesChanged = false;
            m_TextComponent.havePropertiesChanged = true;
            m_TextComponent.ComputeMarginSize();
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
            var editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);

            bool shortcutPressed = (Event.current.modifiers & EventModifiers.Control) != 0 &&
                                   (Event.current.modifiers & EventModifiers.Shift) != 0 &&
                                   Event.current.type == EventType.KeyUp &&
                                   Event.current.keyCode == KeyCode.Alpha2;

            if (!shortcutPressed) return;

            originalTextProp.stringValue = originalTextProp.stringValue.Insert(editor.cursorIndex, ((char)SpecialCharacters.ZeroWidthNoJoiner).ToString());
            editor.selectIndex++;
            editor.cursorIndex++;
            Event.current.Use();
            Repaint();
        }

        protected new void DrawTextInput()
        {
            EditorGUILayout.Space();

            Rect rect = EditorGUILayout.GetControlRect(false, 22);
            GUI.Label(rect, new GUIContent("<b>Text Input</b>"), TMP_UIStyleManager.sectionHeader);

            EditorGUI.indentLevel = 0;

            // If the text component is linked, disable the text input box.
            if (m_ParentLinkedTextComponentProp.objectReferenceValue != null)
            {
                EditorGUILayout.HelpBox("The Text Input Box is disabled due to this text component being linked to another.", MessageType.Info);
            }
            else
            {
                // Display RTL Toggle
                float labelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 110f;

                enableRTLProp.boolValue = EditorGUI.Toggle(new Rect(rect.width - 120, rect.y + 3, 130, 20), k_RtlToggleLabel, enableRTLProp.boolValue);

                EditorGUIUtility.labelWidth = labelWidth;

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_TextProp, GUIContent.none);

                // Need to also compare string content due to issue related to scroll bar drag handle
                if (EditorGUI.EndChangeCheck() && m_TextProp.stringValue != m_TextComponent.text)
                {
                    m_inputSourceField.SetValue(m_TextComponent, 0);
                    m_HavePropertiesChanged = true;
                }

                if (enableRTLProp.boolValue)
                {
                    GUILayout.Label("RTL Text Input");

                    EditorGUI.BeginChangeCheck();
                    rTLTextMeshPro.rtlText = EditorGUILayout.TextArea(rTLTextMeshPro.rtlText, TMP_UIStyleManager.wrappingTextArea, GUILayout.Height(EditorGUI.GetPropertyHeight(m_TextProp) - EditorGUIUtility.singleLineHeight), GUILayout.ExpandWidth(true));

                    if (EditorGUI.EndChangeCheck())
                    {
                        m_TextProp.stringValue = rTLTextMeshPro.rtlText;
                    }
                }

                // TEXT STYLE
                if (m_StyleNames != null)
                {
                    rect = EditorGUILayout.GetControlRect(false, 17);

                    EditorGUI.BeginProperty(rect, k_StyleLabel, m_TextStyleHashCodeProp);

                    m_TextStyleIndexLookup.TryGetValue(m_TextStyleHashCodeProp.intValue, out m_StyleSelectionIndex);

                    EditorGUI.BeginChangeCheck();
                    m_StyleSelectionIndex = EditorGUI.Popup(rect, k_StyleLabel, m_StyleSelectionIndex, m_StyleNames);
                    if (EditorGUI.EndChangeCheck())
                    {
                        m_TextStyleHashCodeProp.intValue = m_Styles[m_StyleSelectionIndex].hashCode;
                        m_TextStyleField.SetValue(m_TextComponent, m_Styles[m_StyleSelectionIndex]);
                        m_HavePropertiesChanged = true;
                    }

                    EditorGUI.EndProperty();
                }
            }
        }
    }
}