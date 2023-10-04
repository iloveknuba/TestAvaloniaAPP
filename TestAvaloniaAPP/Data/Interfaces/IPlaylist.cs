using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAvaloniaAPP.Data.Interfaces
{
    public interface IPlaylist
    {
        Playlist GetWebPlaylist(string url);
       
    }
}
