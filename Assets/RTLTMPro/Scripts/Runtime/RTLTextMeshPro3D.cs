//#define RTL_OVERRIDE

using System.Linq;
using TMPro;
using UnityEngine;

namespace RTLTMPro
{
    [ExecuteInEditMode]
    public class RTLTextMeshPro3D : TextMeshPro
    {
        // ReSharper disable once InconsistentNaming
#if RTL_OVERRIDE
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

        protected bool ForceFix
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

        [SerializeField]
        protected bool preserveNumbers;

        [SerializeField]
        protected bool farsi = true;

        [SerializeField]
        [TextArea(3, 10)]
        protected string originalText;

        [SerializeField]
        protected bool fixTags = true;

        [SerializeField]
        protected bool forceFix;

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
                if (support == null)
                    support = new RTLSupport();

                UpdateSupport();
                UpdateText();
            }
        }

        public virtual void UpdateText()
        {
            if (support == null)
                support = new RTLSupport();

            if (originalText == null)
                originalText = "";

            if (ForceFix == false && support.IsRTLInput(originalText) == false)
            {
                isRightToLeftText = false;
                base.text = originalText;
            }
            else
            {
                isRightToLeftText = true;
                base.text = GetFixedText(originalText);
            }

            havePropertiesChanged = true;
        }

        protected virtual void UpdateSupport()
        {
            if (support == null)
                support = new RTLSupport();

            support.Farsi = farsi;
            support.PreserveNumbers = preserveNumbers;
            support.FixTextTags = fixTags;
        }

        public virtual string GetFixedText(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            if (support == null)
                support = new RTLSupport();

            input = support.FixRTL(input);
            input = input.Reverse().ToArray().ArrayToString();

            return input;
        }
    }
}