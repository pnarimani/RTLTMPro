using System.Collections.Generic;

namespace RTLTMPro
{
    public static class RichTextFixer
    {
        private readonly struct Tag
        {
            public readonly int Start;
            public readonly int End;

            public Tag(int start, int end)
            {
                Start = start;
                End = end;
            }
        }

        private static readonly List<Tag> ClosedTags = new List<Tag>(64);
        private static readonly List<int> ClosedTagsHash = new List<int>(64);

        /// <summary>
        ///     Fixes rich text tags in input string and returns the result.
        /// </summary>
        public static void Fix(FastStringBuilder text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                FindTag(text, i, out int tagStart, out int tagEnd, out int tagType, out int hashCode);

                // If we couldn't find a tag, end the process
                if (tagType == 0)
                {
                    break;
                }

                switch (tagType)
                {
                    case 1: // Opening tag
                        {
                            Tag closingTag = default;

                            // Search and find the closing tag for this
                            bool foundClosingTag = false;
                            for (int j = ClosedTagsHash.Count - 1; j >= 0; j--)
                            {
                                if (ClosedTagsHash[j] == hashCode)
                                {
                                    closingTag = ClosedTags[j];
                                    foundClosingTag = true;
                                    ClosedTags.RemoveAt(j);
                                    ClosedTagsHash.RemoveAt(j);
                                    break;
                                }
                            }

                            if (foundClosingTag)
                            {
                                // NOTE: order of execution is important here

                                int openingTagLength = tagEnd - tagStart + 1;
                                int closingTagLength = closingTag.End - closingTag.Start + 1;

                                text.Reverse(tagStart, openingTagLength);
                                text.Reverse(closingTag.Start, closingTagLength);
                            } else
                            {
                                text.Reverse(tagStart, tagEnd - tagStart + 1);
                            }

                            break;
                        }
                    case 2: // Closing tag
                        {
                            ClosedTags.Add(new Tag(tagStart, tagEnd));
                            ClosedTagsHash.Add(hashCode);
                            break;
                        }
                    case 3: // Self contained tag
                        {
                            text.Reverse(tagStart, tagEnd - tagStart + 1);
                            break;
                        }
                }

                i = tagEnd;
            }
        }

        public static void FindTag(
            FastStringBuilder str,
            int start,
            out int tagStart,
            out int tagEnd,
            out int tagType,
            out int hashCode)
        {
            for (int i = start; i < str.Length;)
            {
                if (str.Get(i) != '<')
                {
                    i++;
                    continue;
                }

                bool calculateHashCode = true;
                hashCode = 0;
                for (int j = i + 1; j < str.Length; j++)
                {
                    char jChar = str.Get(j);

                    if (calculateHashCode)
                    {
                        if (char.IsLetter(jChar))
                        {
                            unchecked
                            {
                                if (hashCode == 0)
                                {
                                    hashCode = jChar.GetHashCode();
                                } else
                                {
                                    hashCode = (hashCode * 397) ^ jChar.GetHashCode();
                                }
                            }
                        } else if (hashCode != 0)
                        {
                            // We have computed the hash code. Now we reached a non letter character. We need to stop
                            calculateHashCode = false;
                        }
                    }

                    // Rich text tag cannot contain RTL chars
                    if (TextUtils.IsRTLCharacter(jChar) || jChar == ' ')
                    {
                        break;
                    }

                    if (jChar == '>')
                    {
                        // Check if the tag is closing, opening or self contained

                        tagStart = i;
                        tagEnd = j;

                        if (str.Get(j - 1) == '/')
                        {
                            // This is self contained.
                            tagType = 3;
                            return;
                        }

                        if (str.Get(i + 1) == '/')
                        {
                            // This is closing
                            tagType = 2;
                            return;
                        }

                        tagType = 1;
                        return;
                    }
                }

                i++;
            }

            tagStart = 0;
            tagEnd = 0;
            tagType = 0;
            hashCode = 0;
        }
    }
}