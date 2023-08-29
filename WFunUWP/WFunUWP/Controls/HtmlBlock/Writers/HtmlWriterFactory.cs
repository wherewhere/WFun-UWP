using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WFunUWP.Controls.Writers
{
    internal class HtmlWriterFactory
    {
        private static List<HtmlWriter> _writers;

        public static HtmlWriter Find(HtmlNode fragment)
        {
            EnsureWriters();

            return _writers.FirstOrDefault(w => w.Match(fragment)); ;
        }

        private static void EnsureWriters()
        {
            if (_writers == null)
            {
                _writers = new List<HtmlWriter>();
                _writers.AddRange(ScanWriters());
            }
        }

        private static IEnumerable<HtmlWriter> ScanWriters()
        {
            return typeof(HtmlWriter)
                        .GetTypeInfo()
                        .Assembly.DefinedTypes
                                    .Where(t => typeof(HtmlWriter).GetTypeInfo().IsAssignableFrom(t) && !t.IsAbstract)
                                    .Select(t => Activator.CreateInstance(t.AsType()))
                                    .Cast<HtmlWriter>()
                                    .ToList();
        }
    }
}
