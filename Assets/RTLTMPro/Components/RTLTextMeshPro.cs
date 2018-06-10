using TMPro;
using UnityEngine;

namespace RTLTMPro
{
    [ExecuteInEditMode]
    public class RTLTextMeshPro : TextMeshProUGUI
    {
        // ReSharper disable once InconsistentNaming
        public new string text
        {
            get { return GetFixedText(originalText); }
            set
            {
                base.text = value;
                havePropertiesChanged = true;
            }
        }

        public bool PreserveNumbers
        {
            get { return preserveNumbers; }
            set
            {
                preserveNumbers = value;
                havePropertiesChanged = true;
            }
        }

        protected bool AutofixMultiline
        {
            get { return autofixMultiline; }
            set
            {
                autofixMultiline = value;
                havePropertiesChanged = true;
            }
        }

        public string GetFixedText(string input)
        {
            input = input.Replace('ی', 'ي');
            input = autofixMultiline
                ? input.FixRTL(this, preserveTashkil, preserveNumbers)
                : input.FixRTL(preserveNumbers, preserveTashkil);
            input = input.Replace('ي', 'ى');
            input = input.Replace('ﻲ', 'ﻰ');
            
            return input;
        }

        private void Update()
        {
            if (havePropertiesChanged)
            {
                base.text = GetFixedText(originalText);
            }
        }

        [SerializeField] protected bool preserveNumbers = true;
        [SerializeField] protected bool autofixMultiline = true;
        [SerializeField] protected bool preserveTashkil;
        [SerializeField] protected string originalText;
    }
}