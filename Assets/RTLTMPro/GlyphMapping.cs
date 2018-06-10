namespace RTLTMPro
{
    /// <summary>
    ///     Data Structure for glyph conversion
    /// </summary>
    public class GlyphMapping
    {
        public int From { get; set; }
        public int To { get; set; }

        public GlyphMapping(int from, int to)
        {
            From = from;
            To = to;
        }
    }
}