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


    private string _avatarUrl;
    public string AvatarUrl
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
            try
            {
                OnPropertyChanged(nameof(AvatarUrl));
                return
                    ImageHelper.LoadFromWeb(new Uri(AvatarUrl));
            }
            catch { return null; }
        }
        
    } 

  public MainViewModel()
    {
        AvatarUrl = "data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAAALAAAAAABAAEAAAIBRAA7";
    }

    public async void CheckPath(object path)
    {
        PlaylistRepo playlist = new PlaylistRepo();
        Playlist = playlist.GetWebPlaylist(path.ToString());
        AvatarUrl = Playlist.Avatar;
        OnPropertyChanged(nameof(ImageFromWebsite));
    }
    
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
