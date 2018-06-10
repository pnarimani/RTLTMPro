namespace RTLTMPro
{
    public class TashkeelLocation
    {
        public char Tashkeel { get; set; }
        public int Position { get; set; }

        public TashkeelLocation(char tashkeel, int position)
        {
            Tashkeel = tashkeel;
            Position = position;
        }
    }
}