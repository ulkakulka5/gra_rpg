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
    /// @details Sterowanie postacią "rozia".
    private async void Content_KeyDown(object sender, KeyRoutedEventArgs e)
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

        await MoveCharacter(rozia, newX, newY, true);
    }
#endif

    /// @brief Obsługa przeciągania postaci.
    /// @param sender Obiekt przeciągany.
    /// @param e Dane gestu.
    /// @details Pozwala poruszać postać myszką (bez kolizji).
    private async void OnPanUpdated(object sender, PanUpdatedEventArgs e)
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

            await MoveCharacter(obrazek, newX, newY, false);
        }
    }

    /// @brief Wywoływane przy pojawieniu się strony.
    /// @details Ustawia początkową pozycję gracza.
    protected override void OnAppearing()
    {
        base.OnAppearing();

        rozia.TranslationX = 560;
        rozia.TranslationY = 730;
    }

    /// @brief Przesuwa postać po labiryncie.
    /// @param character Postać.
    /// @param newX Nowa pozycja X.
    /// @param newY Nowa pozycja Y.
    /// @param checkCollision Czy sprawdzać kolizje.
    /// @async
    /// @details Sprawdza granice mapy oraz kolizje.
    private async Task MoveCharacter(Image character, double newX, double newY, bool checkCollision)
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
            // Animate movement for smoother walking. Duration scales with distance.
            double dx = newX - character.TranslationX;
            double dy = newY - character.TranslationY;
            double dist = Math.Sqrt(dx * dx + dy * dy);
            // duration between 60ms and 350ms depending on distance
            uint duration = (uint)Math.Max(60, Math.Min(350, (int)(dist * 4)));

            try
            {
                await character.TranslateTo(newX, newY, duration, Easing.CubicOut);
            }
            catch
            {
                // If animation is canceled or fails, fallback to direct placement
                character.TranslationX = newX;
                character.TranslationY = newY;
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
        double charWidth = rozia.WidthRequest > 0 ? rozia.WidthRequest : 30;
        double charHeight = rozia.HeightRequest > 0 ? rozia.HeightRequest : 30;

        Rect roziaRect = new Rect(rozia.TranslationX, rozia.TranslationY, charWidth, charHeight);

        foreach (var mirror in activeMirrors)
        {
            if (mirror.IsVisible)
            {
                Rect mirrorRect = new Rect(mirror.Margin.Left, mirror.Margin.Top, mirror.WidthRequest, mirror.WidthRequest);

                if (roziaRect.IntersectsWith(mirrorRect))
                {
                    Play();

                    mirror.IsVisible = false;
                    collectedMirrors++;

                    /// @brief Aktualizacja UI licznika.
                    MirrorCounter.Source = $"lustro_licznik{collectedMirrors}.png";

                    /// @brief Warunek ukończenia poziomu.
                    if (collectedMirrors >= 6)
                    {
                        Levels.Level = 4;
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