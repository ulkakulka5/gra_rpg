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

/// @class Page5
/// @brief Labirynt z mechaniką zbierania przedmiotów.
/// @details Gracz porusza się po labiryncie, unika przeszkód i zbiera lustra.
/// Po zebraniu wszystkich elementów następuje powrót do poprzedniej lokacji.
public partial class Page5 : ContentPage
{
    /// @brief Krok ruchu postaci.
    double step = 10;

    /// @brief Początkowa pozycja X przy przeciąganiu.
    double startX;

    /// @brief Początkowa pozycja Y przy przeciąganiu.
    double startY;

    /// @brief Flaga zapobiegająca wielokrotnej nawigacji.
    bool isNavigating = false;

    /// @brief Odtwarzacz audio dla efektów dźwiękowych.
    IAudioPlayer? player;

    /// @brief Liczba zebranych luster.
    int collectedMirrors = 0;

    /// @brief Lista aktywnych luster na mapie.
    List<Image> activeMirrors = new List<Image>();

    /// @brief Lista obszarów kolizyjnych (ściany labiryntu).
    List<Rect> blockedAreas = new List<Rect>();

    /// @brief Konstruktor strony.
    /// @details Inicjalizuje elementy oraz tworzy labirynt poprzez definiowanie kolizji.
    public Page5()
    {
        InitializeComponent();

        /// @brief Dodanie wszystkich luster do listy aktywnych obiektów.
        activeMirrors.Add(mirror1);
        activeMirrors.Add(mirror2);
        activeMirrors.Add(mirror3);
        activeMirrors.Add(mirror4);
        activeMirrors.Add(mirror5);
        activeMirrors.Add(mirror6);

        /// @brief Definicja ścian labiryntu (kolizje)
        /// @details Każdy Rect reprezentuje fragment ściany.
        blockedAreas.Add(new Rect(0, 0, 100, 832));
        // ... (pozostałe Recty bez zmian)

#if WINDOWS
        Loaded += OnLoaded;
#endif
    }

#if WINDOWS
    /// @brief Inicjalizacja obsługi klawiatury.
    /// @event Loaded
    /// @details Podpina sterowanie postacią za pomocą klawiszy.
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

    /// @brief Obsługa klawiatury (ruch postaci).
    /// @param sender Źródło zdarzenia.
    /// @param e Argumenty klawiatury.
    /// @details Sterowanie postacią "jozio".
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

    /// @brief Obsługa przeciągania postaci.
    /// @param sender Obiekt przeciągany.
    /// @param e Dane gestu.
    /// @details Pozwala poruszać postać myszką (bez kolizji).
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

    /// @brief Wywoływane przy pojawieniu się strony.
    /// @details Ustawia początkową pozycję gracza.
    protected override void OnAppearing()
    {
        base.OnAppearing();

        jozio.TranslationX = 560;
        jozio.TranslationY = 730;
    }

    /// @brief Przesuwa postać po labiryncie.
    /// @param character Postać.
    /// @param newX Nowa pozycja X.
    /// @param newY Nowa pozycja Y.
    /// @param checkCollision Czy sprawdzać kolizje.
    /// @async
    /// @details Sprawdza granice mapy oraz kolizje.
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

            /// @brief Sprawdzanie zbierania przedmiotów.
            if (character == jozio)
            {
                CheckMirrorCollection();
            }
        }
    }

    /// @brief Sprawdza zbieranie luster.
    /// @async
    /// @details
    /// - wykrywa kolizję z lustrem
    /// - odtwarza dźwięk
    /// - aktualizuje licznik
    /// - kończy poziom po zebraniu wszystkich
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

                    /// @brief Aktualizacja UI licznika.
                    MirrorCounter.Source = $"lustro_licznik{collectedMirrors}.png";

                    /// @brief Warunek ukończenia poziomu.
                    if (collectedMirrors >= 6)
                    {
                        await Shell.Current.GoToAsync(nameof(Page4));
                    }
                }
            }
        }
    }

    /// @brief Sprawdza kolizje ze ścianami.
    /// @param x Pozycja X.
    /// @param y Pozycja Y.
    /// @param width Szerokość postaci.
    /// @param height Wysokość postaci.
    /// @return True jeśli ruch zablokowany.
    /// @async
    private async Task<bool> IsBlocked(double x, double y, double width, double height)
    {
        Rect characterRect = new Rect(x, y, width, height);

        foreach (var area in blockedAreas)
        {
            if (characterRect.IntersectsWith(area))
                return true;
        }

        return false;
    }

    /// @brief Odtwarza dźwięk zebrania lustra.
    /// @async
    /// @details Ładuje plik "mirror.mp3" i odtwarza efekt.
    async void Play()
    {
        var audioManager = AudioManager.Current;

        var stream = await FileSystem.OpenAppPackageFileAsync("mirror.mp3");

        player = audioManager.CreatePlayer(stream);

        player.Loop = false;
        player.Play();
    }
}