using System.Collections.Generic;
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

        [SerializeField] [TextArea(3, 10)] protected string originalText;

        [SerializeField] protected bool fixTags = true;

        [SerializeField] protected bool forceFix;

        protected readonly FastStringBuilder finalText = new FastStringBuilder(RTLSupport.DefaultBufferSize);

        readonly List<string> currentArabicWordBuffer = new List<string>();

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

            if (!ForceFix && !TextUtils.IsRTLInput(originalText))
            {
                isRightToLeftText = false;

                string final = "";
                string[] words = originalText.Split(' ');

                //preprocess
                int idx = -1;
                int wIdx = 0;
                foreach (string w in words) {
                    if (TextUtils.IsRTLInput(w)) {
                        if (idx == -1)
                            idx = wIdx;

                        currentArabicWordBuffer.Add(w);
                    }
                    else if (idx > -1) {
                        //we had a word buffer already
                        currentArabicWordBuffer.Reverse();
                        for (int i = idx; i < wIdx; i++) {
                            words[i] = currentArabicWordBuffer[i - idx];
                        }

                        currentArabicWordBuffer.Clear();
                        idx = -1;
                    }

                    wIdx++;
                }

                if (idx > -1) {
                    currentArabicWordBuffer.Reverse();
                    for (int i = idx; i < wIdx; i++) {
                        words[i] = currentArabicWordBuffer[i - idx];
                    }

                    currentArabicWordBuffer.Clear();
                }

                foreach (string w in words) {
                    string txt = TextUtils.IsRTLInput(w) ? GetFixedText(w, false) : w;
                    final += txt + ' ';
                }

                if (words.Length > 0)
                    final = final.Remove(final.Length - 1);

                base.text = final;
            } else
            {
                isRightToLeftText = true;
                base.text = GetFixedText(originalText);
            }

            havePropertiesChanged = true;
        }

        private string GetFixedText(string input, bool reverse = true)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            finalText.Clear();
            RTLSupport.FixRTL(input, finalText, farsi, fixTags, preserveNumbers);
            if (reverse)
                finalText.Reverse();

            return finalText.ToString();
        }
    }
}