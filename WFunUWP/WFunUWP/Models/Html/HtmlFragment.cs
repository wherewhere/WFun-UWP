﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace WFunUWP.Models.Html
{
    public abstract class HtmlFragment
    {
        public string Name { get; set; }
        public HtmlFragment Parent { get; set; }
        public List<HtmlFragment> Fragments { get; } = new List<HtmlFragment>();

        public HtmlNode AsNode()
        {
            return this as HtmlNode;
        }

        public HtmlText AsText()
        {
            return this as HtmlText;
        }

        public IEnumerable<HtmlFragment> Descendants()
        {
            foreach (HtmlFragment descendant in Fragments)
            {
                yield return descendant;
                foreach (HtmlFragment innerDescendant in descendant.Descendants())
                {
                    yield return innerDescendant;
                }
            }
        }

        public HtmlFragment Ascendant(string name)
        {
            return Ascendants().FirstOrDefault(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<HtmlFragment> Ascendants()
        {
            HtmlFragment parent = this.Parent;
            while (parent != null)
            {
                yield return parent;
                parent = parent.Parent;
            }
        }

        internal HtmlNode AddNode(HtmlTag tag)
        {
            HtmlNode node = new HtmlNode(tag);
            Fragments.Add(node);

            return node;
        }

        internal void TryToAddText(HtmlText text)
        {
            if (text != null)
            {
                Fragments.Add(text);
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
