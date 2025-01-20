using UnityEngine;
using System.Collections.Generic;

namespace RTLTMPro
{
    public static class RichTextFixer
    {
        public enum TagType
        {
            None,
            Opening,
            Closing,
            SelfContained,
        }

        public struct Tag
        {
            public int Start;
            public int End;
            public int HashCode;
            public TagType Type;

            public Tag(int start, int end, TagType type, int hashCode)
            {
                Type = type;
                Start = start;
                End = end;
                HashCode = hashCode;
            }
        }

        /// <summary>
        ///     Fixes rich text tags in input string and returns the result.
        /// </summary>
        public static void Fix(FastStringBuilder text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                FindTag(text, i, out Tag tag);

                // If we couldn't find a tag, end the process
                if (tag.Type == TagType.None)
                {
                    break;
                }

                text.Reverse(tag.Start, tag.End - tag.Start + 1);

                i = tag.End;
            }
        }

        // The structure of the tag:
        // Opening: <tag-name> or <tag-name="value">
        // Closing: </tag-name>
        public static void CorrectTagOrder(FastStringBuilder str)
        {
            if (str.Length <= 0)
            {
                return;
            }

            List<FastStringBuilder> substrings = new List<FastStringBuilder>();
            List<int> closingTags = new List<int>();
            List<int> openingTags = new List<int>();

            int count = 0;
            int index = -1;
            for (int i = 0; i < str.Length; i++)
            {
                // TODO: precheck?
                if (count > 0 && 
                    (str.Get(i) == '<' && str.Get(i+1) != ' ' ||
                    str.Get(i) == '>' ||
                    i == str.Length - 1))
                {
                    if (str.Get(i) == '>')
                    {
                        count++;

                        // Tag name is empty -> It's not a tag 
                        if (count < 3)
                        {
                            continue;
                        }
                    }
                    else if (str.Get(i) == '<')
                    {
                        i--;
                    }

                    // Cache new substring
                    FastStringBuilder substr = new FastStringBuilder(count);
                    str.Substring(substr, i - count + 1, count);
                    substrings.Add(substr);
                    
                    // Mark indexes of closing tags
                    index++;
                    if (substr.Get(0) == '<')
                    {
                        if (substr.Get(1) == '/')
                        {
                            closingTags.Add(index);
                        }
                        else
                        {
                            openingTags.Add(index);
                        }
                    }

                    // Reset
                    count = 0;
                    continue;
                }
                
                count++;
            }

            // Nothing to be fixed
            if (closingTags.Count <= 0 && openingTags.Count <= 0)
            {
                return;
            }

            // Make pairs and correct order
            for (int i = 0; i < closingTags.Count; i++)
            {
                int closingIndex = closingTags[i];
                var targetClosing = substrings[closingIndex];

                // Get the first 3 letters of the closing tag as the compared type
                int tagType = 0;
                for (int k = 2; k < 5; k++)
                {
                    if (k < targetClosing.Length)
                    {
                        tagType += targetClosing.Get(k);
                    }
                    else
                    {
                        break;
                    }
                }

                for (int j = 0; j < openingTags.Count; j++)
                {
                    var openingIndex = openingTags[j];
                    if (openingIndex <= closingIndex)
                    {
                        continue;
                    }

                    var comparedOpening = substrings[openingIndex];

                    // Get the first 3 letters of the opening tag as the compared type
                    int comparedTagType = 0;
                    for (int k = 1; k < 4; k++)
                    {
                        if (k < comparedOpening.Length)
                        {
                            comparedTagType += comparedOpening.Get(k);
                        }
                        else
                        {
                            break;
                        }
                    }

                    // Find the closest openign tag with the same type
                    if (comparedOpening.Get(0) == '<' && comparedTagType == tagType)
                    {
                        // Swap the positions
                        substrings[closingIndex] = comparedOpening;
                        substrings[openingIndex] = targetClosing;
                        
                        openingTags[j] = -1;
                    }
                }
            }

            if (openingTags.Count != closingTags.Count)
            {
                List<FastStringBuilder> header = new List<FastStringBuilder>();
                for(int i = 0; i < openingTags.Count; i++)
                {
                    if (openingTags[i] != -1)
                    {
                        int tagIndex = openingTags[i];
                        var tag = substrings[tagIndex].ToString();
                        header.Add(new FastStringBuilder(tag));

                        // Get the tag name
                        for (int j = 1; j < tag.Length; j++)
                        {
                            if (tag[j] == '>' || tag[j] == '=')
                            {
                                tag = tag.Substring(1, j-1);
                                string newTag = string.Format("</{0}>", tag);
                                substrings[tagIndex].SetValue(newTag);
                            }
                        }
                    }
                }

                for (int i = 0; i < header.Count; i++)
                {
                    substrings.Insert(0, header[i]);
                }
            }

            str.Join(str, substrings.ToArray());
        }

        public static void FindTag(
            FastStringBuilder str,
            int start,
            out Tag tag)
        {
            for (int i = start; i < str.Length;)
            {
                if (str.Get(i) != '<')
                {
                    i++;
                    continue;
                }
                
                bool calculateHashCode = true;
                tag.HashCode = 0;
                for (int j = i + 1; j < str.Length; j++)
                {
                    int jChar = str.Get(j);
                    if (calculateHashCode)
                    {
                        if (Char32Utils.IsLetter(jChar))
                        {
                            unchecked
                            {
                                if (tag.HashCode == 0)
                                {
                                    tag.HashCode = jChar.GetHashCode();
                                }
                                else
                                {
                                    tag.HashCode = (tag.HashCode * 397) ^ jChar.GetHashCode();
                                }
                            }
                        }
                        else if (tag.HashCode != 0)
                        {
                            // We have computed the hash code. Now we reached a non letter character. We need to stop
                            calculateHashCode = false;
                        }
                    }

                    // Rich text tag cannot contain < or start with space
                    if ((j == i + 1 && jChar == ' ') || jChar == '<')
                    {
                        break;
                    }

                    if (jChar == '>')
                    {
                        // Check if the tag is closing, opening or self contained

                        tag.Start = i;
                        tag.End = j;

                        if (str.Get(j - 1) == '/')
                        {
                            // This is self contained.
                            tag.Type = TagType.SelfContained;
                        }
                        else if (str.Get(i + 1) == '/')
                        {
                            // This is closing
                            tag.Type = TagType.Closing;
                        }
                        else
                        {
                            tag.Type = TagType.Opening;
                        }

                        return;
                    }
                }

                i++;
            }

            tag.Start = 0;
            tag.End = 0;
            tag.Type = TagType.None;
            tag.HashCode = 0;
        }
    }
}