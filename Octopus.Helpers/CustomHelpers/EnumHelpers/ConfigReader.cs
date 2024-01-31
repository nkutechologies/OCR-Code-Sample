using System;

namespace Octopus.Helpers.CustomHelpers.EnumHelpers
{
    public enum ConfigReader
    {
        [EnumText("ReadingSection")]
        ReadingSection
    }
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class EnumTextAttribute : Attribute
    {
        public string Text { get; }

        public EnumTextAttribute(string text)
        {
            Text = text;
        }
    }
}
