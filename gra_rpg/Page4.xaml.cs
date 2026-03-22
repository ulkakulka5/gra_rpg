using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System;
using Microsoft.Maui.Graphics;

#if WINDOWS
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml.Input;
using Windows.System;
#endif
namespace gra_rpg;

public partial class Page4 : ContentPage
{
    double step = 10;
    double startX;
    double startY;
    bool isNavigating = false;

    List<Rect> blockedAreas = new List<Rect>();
    public Page4()
	{
		InitializeComponent();
        // blockedAreas.Add(new Rect(2013, 1196, 541, 468));


        blockedAreas.Add(new Rect(0, 0, 860, 200));
        blockedAreas.Add(new Rect(0, 200, 680, 50));
        blockedAreas.Add(new Rect(730, 200, 130, 50));

        // Ściany boczne i przeszkody wokół domu
        blockedAreas.Add(new Rect(0, 250, 260, 200));
        blockedAreas.Add(new Rect(810, 250, 50, 200));

        // Dolne partie muru (z przerwą na przejście)
        blockedAreas.Add(new Rect(0, 430, 600, 50));
        blockedAreas.Add(new Rect(700, 430, 170, 50));

        // Jezioro (prawy dół)
        blockedAreas.Add(new Rect(990, 580, 290, 300));



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

        // TYLKO rozia ma kolizjê
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

        // rozia_mouse.TranslationX = 400;
        //rozia_mouse.TranslationY = 200;
        jozio.TranslationX = 1200;
        jozio.TranslationY = 500;
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

        double maxX = MainGrid.Width - character.Width;
        double maxY = MainGrid.Height - character.Height;

        newX = Math.Max(0, Math.Min(newX, maxX));
        newY = Math.Max(0, Math.Min(newY, maxY));

        if (!checkCollision || !await IsBlocked(newX, newY, character.Width, character.Height))
        {
            character.TranslationX = newX;
            character.TranslationY = newY;
        }
    }

    private async Task<bool> IsBlocked(double x, double y, double width, double height)
    {
        Rect characterRect = new Rect(x, y, width, height);

        var door1 = new Rect(680, 200, 50, 55);

        if (characterRect.IntersectsWith(door1) && !isNavigating)
        {
            isNavigating = true;
            await Shell.Current.GoToAsync(nameof(Page5));
            return false;
        }

        foreach (var area in blockedAreas)
        {
            if (characterRect.IntersectsWith(area))
                return true;
        }

      return false;
    }
}

