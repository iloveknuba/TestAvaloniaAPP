using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAvaloniaAPP.Data
{
    public class NodeComparer : IEqualityComparer<HtmlNode>
    {
       
            public bool Equals(HtmlNode x, HtmlNode y)
            {
                // Порівнюємо два HtmlNode за атрибутами, в цьому випадку за атрибутом "id"
                return x.Attributes["primary-text"]?.Value == y.Attributes["primary-text"]?.Value;
            }

            public int GetHashCode(HtmlNode obj)
            {
                // Використовуємо значення атрибута "id" як хеш-код
                return obj.Attributes["primary-text"]?.Value.GetHashCode() ?? 0;
            }
        
    }
}
