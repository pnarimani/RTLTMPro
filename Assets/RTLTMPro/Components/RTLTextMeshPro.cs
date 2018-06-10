using TMPro;
using UnityEngine;

namespace RTLTMPro
{
    [ExecuteInEditMode]
    public class RTLTextMeshPro : TextMeshProUGUI
    {
        // ReSharper disable once InconsistentNaming
        public override string text
        {
            get { return base.text; }
            set
            {
                originalText = value;
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

        [SerializeField] protected bool preserveNumbers = true;
        [SerializeField] protected bool farsiNumbers = true;
        [SerializeField] protected bool preserveTashkeel;
        [SerializeField] protected string originalText;

        public string GetFixedText(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            input = input.Replace('ی', 'ي');
            input = input.FixRTL(preserveNumbers, farsiNumbers, preserveTashkeel);
            input = ReverseText(input);
            isRightToLeftText = true;
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

        private static string ReverseText(string source)
        {
            char[] split = { '\n' };
            string[] paragraphs = source.Split(split);
            string result = "";
            foreach (string paragraph in paragraphs)
            {
                char[] output = new char[paragraph.Length];
                for (int i = 0; i < paragraph.Length; i++)
                {
                    output[(output.Length - 1) - i] = paragraph[i];
                }
                result += new string(output);
                result += "\n";
            }
            return result;
        }
    }
}