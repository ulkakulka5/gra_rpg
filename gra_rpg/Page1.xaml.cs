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

public partial class Page1 : ContentPage
{
    double step = 10;
    double startX;
    double startY;

    List<Rect> blockedAreas = new List<Rect>();
    List<Rect> openAreas = new List<Rect>();

    public Page1()
    {
        InitializeComponent();

        // przyk³adowe przeszkody
        blockedAreas.Add(new Rect(163, 535, 457, 157));
        blockedAreas.Add(new Rect(33, 152, 270, 168));
        blockedAreas.Add(new Rect(306, 196, 144, 127));
        blockedAreas.Add(new Rect(1000, 535, 239, 167));
        blockedAreas.Add(new Rect(0, 0, 1280, 161));
        blockedAreas.Add(new Rect(993, 142, 237, 151));

        


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
        double newX = rozia.TranslationX;
        double newY = rozia.TranslationY;

        switch (e.Key)
        {
            case VirtualKey.Left:  newX -= step; break;
            case VirtualKey.Right: newX += step; break;
            case VirtualKey.Up:    newY -= step; break;
            case VirtualKey.Down:  newY += step; break;
        }

        // TYLKO rozia ma kolizjê
        MoveCharacter(rozia, newX, newY, true);
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

        rozia_mouse.TranslationX = 400;
        rozia_mouse.TranslationY = 200;
        rozia.TranslationX = 1087;
        rozia.TranslationY = 308;
    }
    private async void MoveCharacter(Image character, double newX, double newY, bool checkCollision)
    {
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
        var door1 = new Rect(139, 323, 64, 87);

        if (characterRect.IntersectsWith(door1))
        {
            await Shell.Current.GoToAsync(nameof(Page2));
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