using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Plugin.Maui.Audio;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#if WINDOWS
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml.Input;
using Windows.System;
#endif

namespace gra_rpg;

public partial class Page5 : ContentPage
{
    double step = 10;
    double startX;
    double startY;
    bool isNavigating = false;
    IAudioPlayer? player;

    int collectedMirrors = 0;
    List<Image> activeMirrors = new List<Image>();

    
    List<Rect> blockedAreas = new List<Rect>();

    public Page5()
    {
        InitializeComponent();

        
        activeMirrors.Add(mirror1);
        activeMirrors.Add(mirror2);
        activeMirrors.Add(mirror3);
        activeMirrors.Add(mirror4);
        activeMirrors.Add(mirror5);
        activeMirrors.Add(mirror6);

        // --- ŚCIANY LABIRYNTU --- // Lewa strona i początek
        blockedAreas.Add(new Rect(0, 0, 100, 832));
        blockedAreas.Add(new Rect(0, 200, 180, 30));
        blockedAreas.Add(new Rect(0, 320, 180, 30));
        blockedAreas.Add(new Rect(230, 600, 60, 200));
        blockedAreas.Add(new Rect(230, 590, 160, 30));
        blockedAreas.Add(new Rect(360, 590, 30, 100));
        blockedAreas.Add(new Rect(440, 470, 60, 240));

        // Środek i dolne partie
        blockedAreas.Add(new Rect(490, 370, 60, 120));
        blockedAreas.Add(new Rect(580, 590, 30, 120));
        blockedAreas.Add(new Rect(490, 690, 120, 20));
        blockedAreas.Add(new Rect(530, 370, 90, 20));
        blockedAreas.Add(new Rect(230, 360, 150, 20));
        blockedAreas.Add(new Rect(230, 520, 150, 20));
        blockedAreas.Add(new Rect(230, 360, 60, 160));
        blockedAreas.Add(new Rect(360, 360, 20, 50));
        blockedAreas.Add(new Rect(360, 470, 20, 50));
        blockedAreas.Add(new Rect(660, 470, 50, 20));
        blockedAreas.Add(new Rect(660, 360, 180, 20));
        blockedAreas.Add(new Rect(660, 360, 50, 120));
        blockedAreas.Add(new Rect(790, 360, 50, 320));
        blockedAreas.Add(new Rect(790, 670, 180, 20));
        blockedAreas.Add(new Rect(940, 590, 70, 100));
        blockedAreas.Add(new Rect(940, 360, 80, 170));
        blockedAreas.Add(new Rect(960, 360, 120, 20));
        blockedAreas.Add(new Rect(660, 590, 50, 200));
        blockedAreas.Add(new Rect(660, 589, 100, 20));

        // Zawiłości w górnej części
        blockedAreas.Add(new Rect(230, 40, 60, 260));
        blockedAreas.Add(new Rect(240, 270, 140, 30));
        blockedAreas.Add(new Rect(370, 250, 20, 50));
        blockedAreas.Add(new Rect(390, 40, 20, 120));
        blockedAreas.Add(new Rect(270, 40, 120, 20));
        blockedAreas.Add(new Rect(660, 0, 60, 90));
        blockedAreas.Add(new Rect(450, 0, 60, 210));
        blockedAreas.Add(new Rect(600, 160, 20, 50));
        blockedAreas.Add(new Rect(370, 250, 210, 20));
        blockedAreas.Add(new Rect(450, 190, 150, 20));
        blockedAreas.Add(new Rect(660, 300, 150, 20));

        // Prawa strona
        blockedAreas.Add(new Rect(980, 0, 50, 290));
        blockedAreas.Add(new Rect(820, 0, 60, 120));
        blockedAreas.Add(new Rect(660, 170, 60, 150));
        blockedAreas.Add(new Rect(980, 270, 100, 20));
        blockedAreas.Add(new Rect(1150, 0, 140, 320));
        blockedAreas.Add(new Rect(790, 200, 60, 120));
        blockedAreas.Add(new Rect(1150, 370, 140, 430));

        // Dolna granica mapy
        blockedAreas.Add(new Rect(660, 790, 640, 20));
        blockedAreas.Add(new Rect(0, 790, 520, 20));

#if WINDOWS
        Loaded += OnLoaded;
#endif
    }

#if WINDOWS
    private void OnLoaded(object sender, EventArgs e)
    {
        var mauiWindow = Application.Current!.Windows[0];
        var winuiWindow = mauiWindow.Handler?.PlatformView as Microsoft.UI.Xaml.Window;

        if (winuiWindow != null)
        {
            var content = winuiWindow.Content as Microsoft.UI.Xaml.UIElement;
            if (content != null)
            {
                content.KeyDown += Content_KeyDown;
                content.Focus(Microsoft.UI.Xaml.FocusState.Programmatic);
            }
        }
    }

    private void Content_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        double newX = jozio.TranslationX;
        double newY = jozio.TranslationY;

        switch (e.Key)
        {
            case VirtualKey.Left:  newX -= step; break;
            case VirtualKey.Right: newX += step; break;
            case VirtualKey.Up:    newY -= step; break;
            case VirtualKey.Down:  newY += step; break;
        }

        // Ruch dotyczy tylko Józia
        MoveCharacter(jozio, newX, newY, true);
    }
#endif

    private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        var obrazek = sender as Image;
        if (obrazek == null) return;

        if (e.StatusType == GestureStatus.Started)
        {
            startX = obrazek.TranslationX;
            startY = obrazek.TranslationY;
        }
        else if (e.StatusType == GestureStatus.Running)
        {
            double newX = startX + e.TotalX;
            double newY = startY + e.TotalY;

            MoveCharacter(obrazek, newX, newY, false);
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        
        jozio.TranslationX = 560;
        jozio.TranslationY = 730;
    }

    private async void MoveCharacter(Image character, double newX, double newY, bool checkCollision)
    {
        if (isNavigating)
            return;

        if (MainGrid.Width <= 0 || MainGrid.Height <= 0)
        {
            character.TranslationX = newX;
            character.TranslationY = newY;
            return;
        }

        double maxX = MainGrid.Width - character.WidthRequest;
        double maxY = MainGrid.Height - character.HeightRequest;

        newX = Math.Max(0, Math.Min(newX, maxX));
        newY = Math.Max(0, Math.Min(newY, maxY));

        
        double charWidth = character.WidthRequest > 0 ? character.WidthRequest : 30;
        double charHeight = character.HeightRequest > 0 ? character.HeightRequest : 30;

        if (!checkCollision || !await IsBlocked(newX, newY, charWidth, charHeight))
        {
            character.TranslationX = newX;
            character.TranslationY = newY;

            
            if (character == jozio)
            {
                CheckMirrorCollection();
            }
        }
    }

    private async void CheckMirrorCollection()
    {
       
        double charWidth = jozio.WidthRequest > 0 ? jozio.WidthRequest : 30;
        double charHeight = jozio.HeightRequest > 0 ? jozio.HeightRequest : 30;
        Rect jozioRect = new Rect(jozio.TranslationX, jozio.TranslationY, charWidth, charHeight);

        foreach (var mirror in activeMirrors)
        {
            if (mirror.IsVisible)
            {
               
                Rect mirrorRect = new Rect(mirror.Margin.Left, mirror.Margin.Top, mirror.WidthRequest, mirror.WidthRequest);

                if (jozioRect.IntersectsWith(mirrorRect))
                {
                    Play();
                    mirror.IsVisible = false;
                    collectedMirrors++;

                   
                    MirrorCounter.Source = $"lustro_licznik{collectedMirrors}.png";

                    if (collectedMirrors >= 6)
                    {
                       
                        await Shell.Current.GoToAsync(nameof(Page4));
                    }
                }
            }
        }
    }

    private async Task<bool> IsBlocked(double x, double y, double width, double height)
    {
        Rect characterRect = new Rect(x, y, width, height);

        
        // var door1 = new Rect(139, 323, 64, 87);
        // if (characterRect.IntersectsWith(door1) && !isNavigating) 

        foreach (var area in blockedAreas)
        {
            if (characterRect.IntersectsWith(area))
                return true;
        }

        return false;
    }

    async void Play()
    {
        var audioManager = AudioManager.Current;

        var stream = await FileSystem.OpenAppPackageFileAsync("mirror.mp3");

        player = audioManager.CreatePlayer(stream);

        player.Loop = false;
        player.Play();
    }
}