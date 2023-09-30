using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAvaloniaAPP.Data
{
    public class Playlist
    {
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string Description { get; set; }
        public IEnumerable<Song> Songs { get; set; }

       
    }
}
