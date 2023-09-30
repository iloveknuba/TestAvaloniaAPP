using Avalonia.Media.Imaging;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive;
using System.Threading.Tasks;
using TestAvaloniaAPP.Data;
using TestAvaloniaAPP.Data.Interfaces;
using TestAvaloniaAPP.Data.Repositories;

namespace TestAvaloniaAPP.ViewModels;

public class MainViewModel : ViewModelBase, INotifyPropertyChanged
{
    public string Greeting => "Welcome to Avalonia!";

    private PlaylistRepo _playlistRepo;

    private string _playlisttitle;
    public string PlaylistTitle
    {
        get { return _playlisttitle; }
        set { _playlisttitle = value; }
    }

    private Playlist _playlist;
    public Playlist Playlist
    {
        get => _playlist;
        set
        {
            if (_playlist != value)
            {
                _playlist = value;
               OnPropertyChanged(nameof(Playlist));
            }
        }
    }

    private string _displayText;
    public string DisplayText
    {
        get { return _displayText; }
        set
        {
            if (_displayText != value)
            {
                _displayText = value;  
               OnPropertyChanged(nameof(DisplayText));
            }
        }
    }


   
    public Task<Bitmap?> ImageFromWebsite
    {
        
        get
        {
            return
                ImageHelper.LoadFromWeb(new Uri(Playlist.Avatar));
        }
    } 

  

    public async void CheckPath(object path)
    {
        PlaylistRepo playlist = new PlaylistRepo();
        Playlist = playlist.GetWebPlaylist(path.ToString());
        
    }
    public MainViewModel()
    {
        
        // Playlist = playlist.GetPlaylist("https://music.amazon.com/playlists/B01M11SBC8") ;




    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
