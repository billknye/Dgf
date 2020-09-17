using System.Collections.Generic;

namespace Dgf.Framework.States
{
    public class DisplayItem
    {
        public string Text { get; set; }

        public string Classes { get; set; }

        public string ImageUri { get; set; }

        public DisplayItem()
        {

        }

        public static DisplayItem Create(string text, string classes = null)
        {
            return new DisplayItem
            {
                Text = text,
                Classes = classes
            };
        }

        public static DisplayItem CreateWithImage(string text, string imageUri, string classes = null)
        {
            return new DisplayItem
            {
                Text = text,
                ImageUri = imageUri,
                Classes = classes
            };
        }
    }
}
