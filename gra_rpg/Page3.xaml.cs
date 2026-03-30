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

/// @class Page3
/// @brief Lokacja specjalna z animacją lisa.
/// @details Strona umożliwia poruszanie postacią oraz interakcję z obiektem (rzeka),
/// która uruchamia animację i przenosi gracza z powrotem do mapy głównej.
public partial class Page3 : ContentPage
{
    /// @brief Krok ruchu postaci.
    double step = 10;

    /// @brief Początkowa pozycja X przy przeciąganiu.
    double startX;

    /// @brief Początkowa pozycja Y przy przeciąganiu.
    double startY;

    /// @brief Flaga zapobiegająca wielokrotnej nawigacji.
    bool isNavigating = false;

    /// @brief Lista obszarów kolizyjnych.
    /// @details Obecnie pusta, ale przygotowana do rozbudowy.
    List<Rect> blockedAreas = new List<Rect>();

    /// @brief Konstruktor strony.
    /// @details Inicjalizuje komponenty i przypisuje zdarzenia dla systemu Windows.
    public Page3()
    {
        InitializeComponent();

#if WINDOWS
        Loaded += OnLoaded;
#endif
    }

#if WINDOWS
    /// @brief Inicjalizacja obsługi klawiatury.
    /// @param sender Obiekt wywołujący zdarzenie.
    /// @param e Argumenty zdarzenia.
    /// @event Loaded
    /// @details Podpina obsługę klawiszy strzałek do sterowania postacią.
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
    /// @param sender Źródło zdarzenia.
    /// @param e Argumenty klawiatury.
    /// @details Przesuwa postać "jozio" po mapie.
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

        MoveCharacter(jozio, newX, newY, true);
    }
#endif

    /// @brief Obsługa przeciągania obiektu.
    /// @param sender Obiekt przeciągany (Image).
    /// @param e Dane gestu.
    /// @details Pozwala użytkownikowi przesuwać postać myszką.
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

            MoveCharacter(obrazek, newX, newY, true);
        }
    }

    /// @brief Wywoływane przy pojawieniu się strony.
    /// @details Ustawia początkowe pozycje elementów sceny (postać, lis, obiekty).
    protected override void OnAppearing()
    {
        base.OnAppearing();

        kamien.TranslationX = 400;
        kamien.TranslationY = 200;

        jozio.TranslationX = 570;
        jozio.TranslationY = 50;

        strzalka.TranslationX = 900;
        strzalka.TranslationY = 195;

        lisek.TranslationX = 700;
        lisek.TranslationY = 300;
    }

    /// @brief Przesuwa postać po ekranie.
    /// @param character Obiekt postaci.
    /// @param newX Nowa pozycja X.
    /// @param newY Nowa pozycja Y.
    /// @param checkCollision Czy sprawdzać kolizje.
    /// @async
    /// @details Ogranicza ruch do granic ekranu i sprawdza kolizje.
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

    /// @brief Sprawdza kolizję postaci z otoczeniem.
    /// @param x Pozycja X.
    /// @param y Pozycja Y.
    /// @param width Szerokość postaci.
    /// @param height Wysokość postaci.
    /// @return True jeśli ruch jest zablokowany.
    /// @async
    /// @details
    /// - Wykrywa wejście do rzeki
    /// - Uruchamia animację lisa
    /// - Przenosi gracza do Page1
    private async Task<bool> IsBlocked(double x, double y, double width, double height)
    {
        Rect characterRect = new Rect(x, y, width, height);

        /// @brief Obszar rzeki (trigger zdarzenia).
        var river = new Rect(1250, 191, 20, 20);

        if (characterRect.IntersectsWith(river) && !isNavigating)
        {
            isNavigating = true;

            /// @brief Uruchomienie animacji lisa.
            lisek.IsAnimationPlaying = true;

            await lisek.TranslateTo(1200, 191, 3000);
            await lisek.TranslateTo(1230, 191, 2000);

            lisek.IsAnimationPlaying = false;
            lisek.IsVisible = false;

            /// @brief Opóźnienie przed zmianą sceny.
            await Task.Delay(1000);

            await Shell.Current.GoToAsync(nameof(Page1));
            return false;
        }

        /// @brief Sprawdzanie standardowych kolizji.
        foreach (var area in blockedAreas)
        {
            if (characterRect.IntersectsWith(area))
                return true;
        }

        return false;
    }
}