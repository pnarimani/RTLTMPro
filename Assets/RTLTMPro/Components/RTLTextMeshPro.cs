using TMPro;
using UnityEngine;

namespace RTLTMPro
{
    public class RTLTextMeshPro : TextMeshProUGUI
    {
        // ReSharper disable once InconsistentNaming
        public new string text
        {
            get
            {
                return GetFixedText(originalText);
            }
            set
            {
                originalText = value;

                // Fill base props with fixed values
                m_text = text;
                base.text = text;
            }
        }

        public bool PreserveNumbers
        {
            get { return preserveNumbers; }
            set { preserveNumbers = value; }
        }

        public string GetFixedText(string input)
        {
            input = input.Replace('ی', 'ي');
            input = RTLSupport.Fix(input, preserveTashkil, !preserveNumbers);
            input = input.Replace('ي', 'ى');
            input = input.Replace('ﻲ', 'ﻰ');
            return input;
        }

        [SerializeField] protected bool preserveNumbers = true;
        [SerializeField] protected bool preserveTashkil;
        [SerializeField] protected string originalText;
    }
}
