using System.Linq;
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
                if (originalText == value)
                    return;

                originalText = value;
                base.text = GetFixedText(originalText);
                havePropertiesChanged = true;
            }
        }

        public virtual bool PreserveNumbers
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

        public bool PreserveTashkeel
        {
            get { return preserveTashkeel; }
            set
            {
                if (preserveTashkeel == value)
                    return;

                preserveTashkeel = value;
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

        [SerializeField] protected bool preserveNumbers;
        [SerializeField] protected bool farsi = true;
        [SerializeField] protected bool preserveTashkeel;
        [SerializeField] protected string originalText;
        [SerializeField] protected bool fixTags;

        protected RTLSupport support;

        protected override void Awake()
        {
            base.Awake();
            support = new RTLSupport();
            UpdateSupport();
        }

        protected virtual void Update()
        {
            if (havePropertiesChanged)
            {
                if(support == null)
                    support = new RTLSupport();

                UpdateSupport();
                base.text = GetFixedText(originalText);
            }
        }

        protected virtual void UpdateSupport()
        {
            if(support == null)
                support = new RTLSupport();
            
            support.Farsi = farsi;
            support.PreserveNumbers = preserveNumbers;
            support.PreserveTashkeel = preserveTashkeel;
            support.FixTags = fixTags;
        }

        public virtual string GetFixedText(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            if(support == null)
                support = new RTLSupport();

            input = support.FixRTL(input);
            input = input.Reverse().ToArray().ArrayToString();
            isRightToLeftText = true;
            
            return input;
        }
    }
}