using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System;
using Microsoft.Maui.Graphics;
using System.Threading.Tasks;

#if WINDOWS
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml.Input;
using Windows.System;
#endif

namespace gra_rpg;

/// @class Page4
/// @brief Lokacja z domem i jeziorem.
/// @details Strona zawiera rozbudowany system kolizji (ściany, przeszkody, jezioro)
/// oraz przejścia do innych lokacji (Page5 i powrót do Page1).
public partial class Page4 : ContentPage
{
    /// @brief Krok ruchu postaci.
    double step = 10;

    /// @brief Początkowa pozycja X przy przeciąganiu.
    double startX;

    /// @brief Początkowa pozycja Y przy przeciąganiu.
    double startY;

    /// @brief Flaga blokująca wielokrotne przejścia między stronami.
    bool isNavigating = false;

    /// @brief Lista obszarów kolizyjnych.
    /// @details Zawiera ściany, przeszkody oraz jezioro.
    List<Rect> blockedAreas = new List<Rect>();

    /// @brief pierwsza pozycja strzałki.
    Rect strzalkaBox1 = new Rect(1000, 500, 50, 50);
    Rect strzalkaBox2 = new Rect(800, 500, 50, 50);

    /// @brief Licznik strzałek, używany do sterowania ruchem strzałki na mapie.
    int licznikStrzalek = 0;

    /// @brief Konstruktor strony.
    /// @details Inicjalizuje komponenty oraz definiuje obszary kolizji.
    public Page4()
    {
        InitializeComponent();

        /// @brief Górne ściany
        blockedAreas.Add(new Rect(0, 0, 860, 200));
        blockedAreas.Add(new Rect(0, 200, 680, 50));
        blockedAreas.Add(new Rect(730, 200, 130, 50));

        /// @brief Ściany boczne wokół domu
        blockedAreas.Add(new Rect(0, 250, 260, 200));
        blockedAreas.Add(new Rect(810, 250, 50, 200));

        /// @brief Dolna część muru (z przejściem)
        blockedAreas.Add(new Rect(0, 430, 600, 50));
        blockedAreas.Add(new Rect(700, 430, 170, 50));

        /// @brief Jezioro (obszar niedostępny)
        blockedAreas.Add(new Rect(990, 580, 290, 300));

#if WINDOWS
        Loaded += OnLoaded;
#endif
    }

#if WINDOWS
    /// @brief Inicjalizacja obsługi klawiatury.
    /// @param sender Obiekt wywołujący zdarzenie.
    /// @param e Argumenty zdarzenia.
    /// @event Loaded
    /// @details Podpina zdarzenie KeyDown do sterowania postacią.
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

    /// @brief Obsługa klawiatury (strzałki).
    /// @param sender Źródło zdarzenia.
    /// @param e Argumenty klawiatury.
    /// @details Przesuwa postać "jozio".
    private async void Content_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if(Levels.Level < 3){

            double newX = jozio.TranslationX;
            double newY = jozio.TranslationY;

            switch (e.Key)
            {
                case VirtualKey.Left:  newX -= step; break;
                case VirtualKey.Right: newX += step; break;
                case VirtualKey.Up:    newY -= step; break;
                case VirtualKey.Down:  newY += step; break;
            }

            // TYLKO jozio ma kolizjê
            await MoveCharacter(jozio, newX, newY, true);
        }
        else if(Levels.Level >= 3)
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
            await MoveCharacter(rozia, newX, newY, true);
        }
    }
#endif

    /// @brief Obsługa przeciągania postaci.
    /// @param sender Obiekt przeciągany.
    /// @param e Dane gestu.
    /// @details Pozwala przesuwać postać myszką (bez kolizji).
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
    /// @details Ustawia początkową pozycję postaci.
    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (Levels.Level < 3)
        {
            rozia.IsVisible = false;
            jozio.TranslationX = 1200;
            jozio.TranslationY = 500;
        }
        else if (Levels.Level == 3)
        {
            rozia.IsVisible = true;
            jozio.IsVisible = false;
            rozia.TranslationX = 1200;
            rozia.TranslationY = 500;
            strzalka.IsVisible = true;
            strzalka.TranslationX = 1000;
            strzalka.TranslationY = 500;
        }
        else if (Levels.Level == 4)
        {
            rozia.IsVisible = true;
            jozio.IsVisible = false;
            rozia.TranslationX = 680;
            rozia.TranslationY = 200;
            strzalka.IsVisible = true;
            strzalka.TranslationX = 800;
            strzalka.TranslationY = 500;
            strzalka.Rotation = 90;
        }   
        else if (Levels.Level > 3)
        {
            rozia.IsVisible = true;
            jozio.IsVisible = false;
            rozia.TranslationX = 1200;
            rozia.TranslationY = 500;
        }
    }

    /// @brief Przesuwa postać.
    /// @param character Obiekt postaci.
    /// @param newX Nowa pozycja X.
    /// @param newY Nowa pozycja Y.
    /// @param checkCollision Czy sprawdzać kolizje.
    /// @async
    /// @details Ogranicza ruch do granic mapy oraz sprawdza kolizje.
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

        double maxX = MainGrid.Width - character.Width;
        double maxY = MainGrid.Height - character.Height;

        newX = Math.Max(0, Math.Min(newX, maxX));
        newY = Math.Max(0, Math.Min(newY, maxY));

        if (!checkCollision || !await IsBlocked(newX, newY, character.Width, character.Height))
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

    /// @brief Sprawdza kolizje oraz przejścia między lokacjami.
    /// @param x Pozycja X.
    /// @param y Pozycja Y.
    /// @param width Szerokość postaci.
    /// @param height Wysokość postaci.
    /// @return True jeśli ruch zablokowany.
    /// @async
    /// @details
    /// Obsługuje:
    /// - wejście do domu (Page5)
    /// - powrót do mapy (Page1)
    /// - kolizje ze ścianami i jeziorem
    private async Task<bool> IsBlocked(double x, double y, double width, double height)
    {
        Rect characterRect = new Rect(x, y, width, height);

        /// @brief Drzwi do domu.
        var door1 = new Rect(680, 200, 5, 5);

        /// @brief Wyjście z lokacji.
        var door2 = new Rect(1250, 500, 5, 5);

        if (characterRect.IntersectsWith(strzalkaBox1) && !isNavigating && Levels.Level == 3)
        {
            if (licznikStrzalek == 0)
            {
                strzalkaBox1 = new Rect(1000, 500, 50, 50);
                strzalka.TranslationX = 1000;
                strzalka.TranslationY = 500;
                licznikStrzalek = 1;
            }
            else if (licznikStrzalek == 1)
            {
                strzalkaBox1 = new Rect(800, 500, 50, 50);
                strzalka.TranslationX = 800;
                strzalka.TranslationY = 500;
                licznikStrzalek = 2;
            }
            else if (licznikStrzalek == 2)
            {
                strzalkaBox1 = new Rect(600, 500, 50, 50);
                strzalka.TranslationX = 600;
                strzalka.TranslationY = 500;
                licznikStrzalek = 3;
                strzalka.Rotation = 0;
            }
            else if (licznikStrzalek == 3)
            {
                strzalkaBox1 = new Rect(690, 300, 50, 50);
                strzalka.TranslationX = 690;
                strzalka.TranslationY = 300;
                licznikStrzalek = 4;
                strzalka.Rotation = 0;
            }
            else if (licznikStrzalek == 4)
            {
                strzalka.IsVisible = false;
            }
        }
        if (characterRect.IntersectsWith(strzalkaBox2) && !isNavigating && Levels.Level == 4)
        {
            if (licznikStrzalek == 0)
            {
                strzalkaBox2 = new Rect(1000, 500, 50, 50);
                strzalka.TranslationX = 1000;
                strzalka.TranslationY = 500;
                licznikStrzalek = 1;
                strzalka.Rotation = 90;
            }
            
            else if (licznikStrzalek == 1)
            {
                strzalka.IsVisible = false;
            }
        }

        if (characterRect.IntersectsWith(door1) && !isNavigating && (Levels.Level == 3 || Levels.Level == 3.5))
        {
            isNavigating = true;
            await Shell.Current.GoToAsync(nameof(Page5));
            return false;
        }

        if (characterRect.IntersectsWith(door2) && !isNavigating)
        {
            isNavigating = true;
            if(Levels.Level >= 1 && Levels.Level < 2)
            {
                Levels.Level = 1.8;
            }
            else if (Levels.Level == 2)
            {
                Levels.Level = 2.5;
            }
            else if (Levels.Level == 3 || Levels.Level == 3.5 || Levels.Level == 3.8)
            {
                Levels.Level = 3.8;
            }
            else if (Levels.Level == 4   || Levels.Level == 4.5)
            {
                Levels.Level = 4.8;
            }
                await Shell.Current.GoToAsync(nameof(Page1));
            return false;
        }

        /// @brief Sprawdzanie kolizji z przeszkodami.
        foreach (var area in blockedAreas)
        {
            if (characterRect.IntersectsWith(area))
                return true;
        }

        return false;
    }
}