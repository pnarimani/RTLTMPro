using TMPro;
using UnityEngine;

namespace RTLTMPro
{
    [ExecuteInEditMode]
    public class RTLTextMeshPro : TextMeshProUGUI
    {
        // ReSharper disable once InconsistentNaming
#if TMP_VERSION_2_1_0_OR_NEWER
        public override string text
#else
        public new string text
#endif
        {
            get { return base.text; }
            set
            {
                if (originalText == value)
                    return;

                originalText = value;

                UpdateText();
            }
        }

        public string OriginalText
        {
            get { return originalText; }
        }

        public bool PreserveNumbers
        {
            get { return preserveNumbers; }
            set
            {
                if (preserveNumbers == value)
                    return;

                preserveNumbers = value;
                havePropertiesChanged = true;
            }
        }

        public bool Farsi
        {
            get { return farsi; }
            set
            {
                if (farsi == value)
                    return;

                farsi = value;
                havePropertiesChanged = true;
            }
        }

        public bool FixTags
        {
            get { return fixTags; }
            set
            {
                if (fixTags == value)
                    return;

                fixTags = value;
                havePropertiesChanged = true;
            }
        }

        public bool ForceFix
        {
            get { return forceFix; }
            set
            {
                if (forceFix == value)
                    return;

                forceFix = value;
                havePropertiesChanged = true;
            }
        }

        [SerializeField] protected bool preserveNumbers;

        [SerializeField] protected bool farsi = true;

        [SerializeField][TextArea(3, 10)] protected string originalText;

        [SerializeField] protected bool fixTags = true;

        [SerializeField] protected bool forceFix;

        protected readonly FastStringBuilder finalText = new FastStringBuilder(RTLSupport.DefaultBufferSize);
        protected FastStringBuilder original_untaged = new FastStringBuilder(RTLSupport.DefaultBufferSize);
        protected FastStringBuilder original_untaged_fixed = new FastStringBuilder(RTLSupport.DefaultBufferSize);

        protected void Update()
        {
            if (havePropertiesChanged)
            {
                UpdateText();
            }
        }

        public void UpdateText()
        {
            if (originalText == null)
                originalText = "";

            if (ForceFix == false && TextUtils.IsRTLInput(originalText) == false)
            {
                isRightToLeftText = false;
                base.text = originalText;
            }
            else
            {
                // Get The Untaged version of the text
                original_untaged.Clear();
                RichTextFixer.GetUntaged(originalText, original_untaged);
                original_untaged_fixed.Clear();
                RTLSupport.FixRTL(original_untaged.ToString(), original_untaged_fixed, farsi, fixTags, preserveNumbers);
                // original_untaged_fixed.Reverse();

                isRightToLeftText = true;
                base.text = GetFixedText(originalText);
            }

            havePropertiesChanged = true;
        }
        public void add_tag(string tag, string seprator, int index)
        {
            FastStringBuilder text = new FastStringBuilder(RTLSupport.DefaultBufferSize);
            text.SetValue(originalText);

            string taged_value = tag.Replace(seprator, text.get_str(index));
            text.Remove(index, 1);
            text.Insert(index, taged_value);

            originalText = text.ToString();
            UpdateText();
        }
        private string GetFixedText(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            finalText.Clear();
            // RTLSupport.FixRTL(input, finalText, farsi, fixTags, preserveNumbers);
            // RTLFix with the untaged version for converting the taged text.
            RTLSupport.FixRTL(input, finalText, original_untaged_fixed, farsi, fixTags, preserveNumbers);
            finalText.Reverse();
            return finalText.ToString();
        }
    }
}