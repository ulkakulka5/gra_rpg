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

/// @class Page1
/// @brief Główna mapa gry.
/// @details Obsługuje ruch postaci, kolizje oraz przejścia między lokacjami.
public partial class Page1 : ContentPage
{
    /// @brief Krok ruchu postaci.
    double step = 10;

    /// @brief Początkowa pozycja X podczas przeciągania.
    double startX;

    /// @brief Początkowa pozycja Y podczas przeciągania.
    double startY;

    /// @brief Flaga blokująca wielokrotne przejścia między stronami.
    bool isNavigating = false;

    /// @brief Lista obszarów kolizyjnych.
    List<Rect> blockedAreas = new List<Rect>();

    /// @brief Konstruktor strony.
    /// @details Inicjalizuje obszary kolizji oraz zdarzenia systemowe.

    public Page1()
    {
        InitializeComponent();

       
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
    /// @brief Inicjalizacja obsługi klawiatury.
    /// @event Loaded
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
    /// @brief Obsługa klawiszy strzałek.
    /// @param e Zdarzenie klawiatury.
    /// @details Przesuwa postać "rozia".
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
    /// @brief Obsługa przeciągania postaci.
    /// @param sender Obiekt (Image).
    /// @param e Dane gestu.
    /// @details Pozwala przesuwać postać myszką.
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

    /// @brief Ustawia początkową pozycję postaci.
    protected override void OnAppearing()
    {
        base.OnAppearing();
        rozia.TranslationX = 1087;
        rozia.TranslationY = 308;
    }

    /// @brief Przesuwa postać.
    /// @param character Obiekt postaci.
    /// @param newX Nowa pozycja X.
    /// @param newY Nowa pozycja Y.
    /// @param checkCollision Czy sprawdzać kolizję.
    /// @async
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

    /// @brief Sprawdza kolizję postaci.
    /// @param x Pozycja X.
    /// @param y Pozycja Y.
    /// @param width Szerokość.
    /// @param height Wysokość.
    /// @return True jeśli zablokowane.
    /// @async
    /// @details Obsługuje również przejścia między stronami (drzwi).
    private async Task<bool> IsBlocked(double x, double y, double width, double height)
    {
        Rect characterRect = new Rect(x, y, width, height);

        var door1 = new Rect(139, 323, 64, 87);
        var door2 = new Rect(500, 830, 15, 15);
        var door3 = new Rect(0, 565, 50, 123);

        if (characterRect.IntersectsWith(door1) && !isNavigating)
        {
            isNavigating = true;
            await Shell.Current.GoToAsync(nameof(Page2));
            return false;
        }
        if(characterRect.IntersectsWith(door2) && !isNavigating)
        {
            isNavigating = true;
            await Shell.Current.GoToAsync(nameof(Page3));
            return false;
        }

        foreach (var area in blockedAreas)
        {
            if (characterRect.IntersectsWith(area))
                return true;
        }

        if (characterRect.IntersectsWith(door3) && !isNavigating)
        {
            isNavigating = true;
            await Shell.Current.GoToAsync(nameof(Page4));
            return false;
        }
        return false;
    }
}
