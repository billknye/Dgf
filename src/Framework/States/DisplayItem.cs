using System.Collections.Generic;

namespace Dgf.Framework.States
{
    public class DisplayItem
    {
        /// <summary>
        /// Text to be displayed
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Classes applied to the text elements to allow for styling rules
        /// </summary>
        public string Classes { get; set; }

        /// <summary>
        /// Uri of the image to show with the text
        /// </summary>
        public string ImageUri { get; set; }

        /// <summary>
        /// Text used to describe the text, used as a title or expanded description
        /// </summary>
        public string DescriptiveText { get; set; }

        public DisplayItem()
        {

        }

        public static DisplayItem Create(string text, string classes = null, string description = null)
        {
            return new DisplayItem
            {
                Text = text,
                Classes = classes,
                DescriptiveText = description
            };
        }

        public static DisplayItem CreateWithImage(string text, string imageUri, string classes = null, string description = null)
        {
            return new DisplayItem
            {
                Text = text,
                ImageUri = imageUri,
                Classes = classes,
                DescriptiveText = description
            };
        }

        public static implicit operator DisplayItem(string text)
        {
            return Create(text);
        }
    }
}
