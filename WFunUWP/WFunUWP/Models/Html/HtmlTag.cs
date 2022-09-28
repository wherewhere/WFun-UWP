﻿using System.Collections.Generic;

namespace WFunUWP.Models.Html
{
    internal sealed class HtmlTag
    {
        public string Name { get; set; }
        public TagType TagType { get; set; }
        public int StartIndex { get; set; }
        public int Length { get; set; }
        public Dictionary<string, string> Attributes { get; set; }

        public HtmlTag()
        {
            Attributes = new Dictionary<string, string>();
        }

        public override string ToString()
        {
            return $"{Name}, {TagType}";
        }
    }

    internal enum TagType
    {
        Open,
        Close,
        AutoClose
    }
}
