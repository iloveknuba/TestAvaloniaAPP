using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAvaloniaAPP.Data
{
    public class NodeComparer : IEqualityComparer<Song>
    {
       
            public bool Equals(Song x, Song y)
            {
                // Порівнюємо два HtmlNode за атрибутами, в цьому випадку за атрибутом "id"
                return x.SongName == y.SongName;
            }

            public int GetHashCode(Song obj)
            {
                // Використовуємо значення атрибута "id" як хеш-код
                return obj.SongName?.GetHashCode() ?? 0;
            }
        
    }
}
