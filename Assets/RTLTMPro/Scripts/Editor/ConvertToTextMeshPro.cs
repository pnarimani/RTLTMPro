using System;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace RTLTMPro
{
    public class ConvertToTextMeshPro
    {
        [MenuItem("CONTEXT/Text/Convert to RTLTMPro")]
        public static void ConvertLegacyText(MenuCommand command)
        {
            var text = (Text)command.context;
            Convert(text);
        }

        private static void Convert(Text from)
        {
            var gameObject = from.gameObject;

            var font = from.font;
            var fontSize = from.fontSize;
            var color = from.color;
            var alignment = from.alignment;
            var text = from.text;
            var fontStyle = from.fontStyle;
            var lineSpacing = from.lineSpacing;
            var supportRichText = from.supportRichText;
            var resizeTextMaxSize = from.resizeTextMaxSize;
            var resizeTextMinSize = from.resizeTextMinSize;
            var resizeTextForBestFit = from.resizeTextForBestFit;
            var enabled = from.enabled;
            var maskable = from.maskable;
            var material = from.material;
            var raycastTarget = from.raycastTarget;
            var raycastPadding = from.raycastPadding;

            Undo.DestroyObjectImmediate(from);
            var to = Undo.AddComponent<RTLTextMeshPro>(gameObject);

            EditorGUI.BeginChangeCheck();

            to.text  = text;
            to.color = color;
            to.alignment = alignment switch
            {
                TextAnchor.UpperLeft    => TextAlignmentOptions.TopLeft,
                TextAnchor.UpperCenter  => TextAlignmentOptions.Top,
                TextAnchor.UpperRight   => TextAlignmentOptions.TopRight,
                TextAnchor.MiddleLeft   => TextAlignmentOptions.Left,
                TextAnchor.MiddleCenter => TextAlignmentOptions.Center,
                TextAnchor.MiddleRight  => TextAlignmentOptions.Right,
                TextAnchor.LowerLeft    => TextAlignmentOptions.BottomLeft,
                TextAnchor.LowerCenter  => TextAlignmentOptions.Bottom,
                TextAnchor.LowerRight   => TextAlignmentOptions.BottomRight,
                _                       => throw new ArgumentOutOfRangeException()
            };

            var possibleFonts = AssetDatabase.FindAssets($"t:{nameof(TMP_FontAsset)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<TMP_FontAsset>);

            bool IsMatchingFont(TMP_FontAsset f)
            {
                //We need to use reflection because the font property is null when population mode is set to static
                const BindingFlags flags = BindingFlags.Instance | BindingFlags.GetField | BindingFlags.Public | BindingFlags.NonPublic;
                var fontSourceFile = typeof(TMP_FontAsset)
                    .GetField("m_SourceFontFile_EditorRef", flags)
                    !.GetValue(f) as Font;

                return fontSourceFile == font;
            }

            to.font     = possibleFonts.FirstOrDefault(IsMatchingFont);
            to.fontSize = fontSize;

            to.fontStyle = fontStyle switch
            {
                FontStyle.Normal        => FontStyles.Normal,
                FontStyle.Bold          => FontStyles.Bold,
                FontStyle.Italic        => FontStyles.Italic,
                FontStyle.BoldAndItalic => FontStyles.Bold | FontStyles.Italic,
                _                       => throw new ArgumentOutOfRangeException()
            };

            to.lineSpacing      = lineSpacing;
            to.richText         = supportRichText;
            to.fontSizeMax      = resizeTextMaxSize;
            to.fontSizeMin      = resizeTextMinSize;
            to.enableAutoSizing = resizeTextForBestFit;
            to.enabled          = enabled;
            to.maskable         = maskable;
            to.material         = material;
            to.raycastTarget    = raycastTarget;
            to.raycastPadding   = raycastPadding;

            if (EditorGUI.EndChangeCheck())
                Undo.RecordObject(to, "Copy from old text");
        }

        [MenuItem("CONTEXT/TextMeshProUGUI/Convert to RTLTMPro", true)]
        public static bool ConvertTextMeshProUGUIValidate(MenuCommand command) 
            => command.context is TextMeshProUGUI and not RTLTextMeshPro;

        [MenuItem("CONTEXT/TextMeshProUGUI/Convert to RTLTMPro")]
        public static void ConvertTextMeshProUGUI(MenuCommand command)
        {
            var text = (TextMeshProUGUI)command.context;
            Convert(text);
        }
        
        /// <summary>
        /// Unfortunately I'm not sure how to do GUID swapping, so references to the text component will break, but at least it should make the transition much safer and quicker
        /// </summary>
        /// <param name="from"></param>
        private static void Convert(TextMeshProUGUI from)
        {
            var gameObject = from.gameObject;

            var margins = from.margin;
            var textPreprocessor = from.textPreprocessor;
            var text = from.text;
            var autoSizeTextContainer = from.autoSizeTextContainer;
            var font = from.font;
            var color = from.color;
            var alignment = from.alignment;
            var fontStyle = from.fontStyle;
            var lineSpacing = from.lineSpacing;
            var richText = from.richText;
            var fontSize = from.fontSize;
            var fontSizeMax = from.fontSizeMax;
            var fontSizeMin = from.fontSizeMin;
            var enableAutoSizing = from.enableAutoSizing;
            var enabled = from.enabled;
            var maskable = from.maskable; 
            var raycastTarget = from.raycastTarget;
            var raycastPadding = from.raycastPadding;
            var fontSharedMaterial = from.fontSharedMaterial;
            var fontSharedMaterials = from.fontSharedMaterials;
            var enableVertexGradient = from.enableVertexGradient;
            var colorGradient = from.colorGradient;
            var colorGradientPreset = from.colorGradientPreset;
            var spriteAsset = from.spriteAsset;
            var tintAllSprites = from.tintAllSprites;
            var styleSheet = from.styleSheet;
            var textStyle = from.textStyle;
            var overrideColorTags = from.overrideColorTags;
            var fontWeight = from.fontWeight;
            var characterSpacing = from.characterSpacing;
            var wordSpacing = from.wordSpacing;
            var lineSpacingAdjustment = from.lineSpacingAdjustment;
            var paragraphSpacing = from.paragraphSpacing;
            var characterWidthAdjustment = from.characterWidthAdjustment;
            var enableWordWrapping = from.enableWordWrapping;
            var wordWrappingRatios = from.wordWrappingRatios;
            var overflowMode = from.overflowMode;
            var linkedTextComponent = from.linkedTextComponent;
            
            Undo.DestroyObjectImmediate(from);
            var to = Undo.AddComponent<RTLTextMeshPro>(gameObject);
            
            EditorGUI.BeginChangeCheck();
            
            to.UpdateText();
            
            //Initialize the canvas renderer
            _ = to.canvasRenderer;

            //Initialize the mesh using reflection
            var mesh           = new Mesh
            {
                hideFlags = HideFlags.HideAndDontSave,
                name      = "TextMeshPro UI Mesh"
            };
            
            typeof(TMP_Text)
                .GetField("m_mesh", BindingFlags.Instance | BindingFlags.SetField | BindingFlags.NonPublic)
                !.SetValue(to, mesh);
            
            to.GetTextInfo(to.text);
            // rtlText.UpdateFontAsset();
            // rtlText.UpdateVertexData();
            _ = to.GetPreferredValues();



            to.margin                   = margins;
            to.fontSharedMaterials      = fontSharedMaterials;
            to.fontSharedMaterial       = fontSharedMaterial;
            to.textPreprocessor         = textPreprocessor;
            to.text                     = text;
            to.autoSizeTextContainer    = autoSizeTextContainer;
            to.font                     = font;
            to.color                    = color;
            to.alignment                = alignment;
            to.fontStyle                = fontStyle;
            to.lineSpacing              = lineSpacing;
            to.richText                 = richText;
            to.fontSize                 = fontSize;
            to.fontSizeMax              = fontSizeMax;
            to.fontSizeMin              = fontSizeMin;
            to.enableAutoSizing         = enableAutoSizing;
            to.enabled                  = enabled;
            to.maskable                 = maskable;
            to.raycastTarget            = raycastTarget;
            to.raycastPadding           = raycastPadding;
            to.enableVertexGradient     = enableVertexGradient;
            to.colorGradient            = colorGradient;
            to.colorGradientPreset      = colorGradientPreset;
            to.spriteAsset              = spriteAsset;
            to.tintAllSprites           = tintAllSprites;
            to.styleSheet               = styleSheet;
            to.textStyle                = textStyle;
            to.overrideColorTags        = overrideColorTags;
            to.fontWeight               = fontWeight;
            to.characterSpacing         = characterSpacing;
            to.wordSpacing              = wordSpacing;
            to.lineSpacingAdjustment    = lineSpacingAdjustment;
            to.paragraphSpacing         = paragraphSpacing;
            to.characterWidthAdjustment = characterWidthAdjustment;
            to.enableWordWrapping       = enableWordWrapping;
            to.wordWrappingRatios       = wordWrappingRatios;
            to.overflowMode             = overflowMode;
            to.linkedTextComponent      = linkedTextComponent;


            if (EditorGUI.EndChangeCheck())
                Undo.RecordObject(to, "Copy from old text");
        }
    }
}