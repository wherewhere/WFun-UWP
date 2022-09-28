﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WFunUWP.Models.Html
{
    public sealed class HtmlDocument : HtmlFragment
    {
        public HtmlNode Body
        {
            get
            {
                return Descendants()
                            .FirstOrDefault(d => d.Name == "body")?.AsNode();
            }
        }

        public HtmlDocument()
        {
            Name = "doc";
        }

        public static async Task<HtmlDocument> LoadAsync(string document)
        {
            return await Task.Run<HtmlDocument>(() =>
            {
                return Load(document);
            });
        }

        public static HtmlDocument Load(string document)
        {
            HtmlDocument root = new HtmlDocument();

            if (!string.IsNullOrWhiteSpace(document))
            {
                if (TagReader.Any(document))
                {
                    document = Clean(document);

                    TagReader reader = TagReader.Create(document);
                    AddFragments(reader, root, new Stack<string>());
                }
                else
                {
                    root.TryToAddText(new HtmlText
                    {
                        Content = document
                    });
                }
            }

            return root;
        }

        private static string AddFragments(TagReader reader, HtmlFragment parentFragment, Stack<string> openTagStack)
        {
            while (reader.Read())
            {
                parentFragment.TryToAddText(HtmlText.Create(reader.Document, reader.PreviousTag, reader.CurrentTag));

                if (reader.CurrentTag.TagType == TagType.Close && openTagStack.Any(t => t.Equals(reader.CurrentTag.Name, StringComparison.CurrentCultureIgnoreCase)))
                {
                    return reader.CurrentTag.Name;
                }

                HtmlNode node = parentFragment.AddNode(reader.CurrentTag);
                node.Parent = parentFragment;

                if (reader.CurrentTag.TagType == TagType.Open)
                {
                    openTagStack.Push(reader.CurrentTag.Name);

                    string lastClosed = AddFragments(reader, node, openTagStack);
                    if (lastClosed != openTagStack.Pop())
                    {
                        return lastClosed;
                    }
                }
            }
            parentFragment.TryToAddText(HtmlText.Create(reader.Document, reader.CurrentTag, null));
            return null;
        }

        private static string Clean(string doc)
        {
            doc = Regex.Replace(doc, @"\r\n\s+|\n\s+|\r\n|\n|\r", string.Empty, RegexOptions.Singleline);
            doc = Regex.Replace(doc, "<!--.*?-->", string.Empty);

            return doc;
        }
    }
}
