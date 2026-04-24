using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;


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

    /// @brief pierwsza pozycja strzałki.
    Rect strzalkaBox1 = new Rect(800, 450, 50, 50);
    Rect strzalkaBox2 = new Rect(150, 460, 50, 50);

    int licznikStrzalek = 0;

    private Grid blackScreen;

    /// @brief Konstruktor strony.
    /// @details Inicjalizuje obszary kolizji oraz zdarzenia systemowe.
    /// 



    public Page1()
    {
        InitializeComponent();


        blockedAreas.Add(new Rect(163, 535, 457, 157));
        blockedAreas.Add(new Rect(33, 152, 270, 168));
        blockedAreas.Add(new Rect(306, 196, 144, 127));
        blockedAreas.Add(new Rect(1000, 535, 239, 167));
        blockedAreas.Add(new Rect(0, 0, 1280, 161));
        blockedAreas.Add(new Rect(993, 142, 237, 151));

        blackScreen = new Grid
        {
            BackgroundColor = Colors.Black,
            Opacity = 0,
            InputTransparent = true
        };

        MainGrid.Children.Add(blackScreen); // MainGrid = twój główny Grid
    




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
    /// @details Przesuwa postać "jozio".
    private void Content_KeyDown(object sender, KeyRoutedEventArgs e)
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
            MoveCharacter(jozio, newX, newY, true);
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
            MoveCharacter(rozia, newX, newY, true);
        }
    
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

    /// @brief Ustawia początkową pozycję postaci i strzałki.
    protected async override void OnAppearing()
    {
        base.OnAppearing();
        if (Levels.Level == 1)
        {
            jozio.TranslationX = 1087;
            jozio.TranslationY = 308;
            strzalka.TranslationX = 800;
            strzalka.TranslationY = 450;
            strzalka.IsVisible = true;
            //await Task.Delay(1000);
            //polecenie1.IsVisible = true;
        }

        if (Levels.Level == 1.5)
        {
            jozio.TranslationX = 700;
            jozio.TranslationY = 720;
            strzalka.IsVisible = false;

        }

        if (Levels.Level == 1.8)
        {
            jozio.TranslationX = 40;
            jozio.TranslationY = 480;
            strzalka.IsVisible = false;
        }

        if (Levels.Level == 2)
        {
            jozio.TranslationX = 139;
            jozio.TranslationY = 323;
            strzalka.IsVisible = false;
            //polecenie1.IsVisible = false;
            await Task.Delay(1000);
            strzalka.IsVisible = true;
            strzalka.TranslationX = 150;
            strzalka.TranslationY = 460;
            strzalka.Rotation = 90;
        }
        if (Levels.Level == 2.5)
        {
            jozio.TranslationX = 40;
            jozio.TranslationY = 480;
            strzalka.IsVisible = false;
        }
        if (Levels.Level == 3)
        {
            jozio.IsVisible = true;
            jozio.TranslationX = 700;
            jozio.TranslationY = 720;
            strzalka.IsVisible = false;
            //polecenie1.IsVisible = false;
            await Task.Delay(1000);
            await FadeBlack();

        }
        if (Levels.Level == 3.5)
        {
            jozio.IsVisible = false;
            rozia.IsVisible = true;
            rozia.TranslationX = 700;
            rozia.TranslationY = 720;
            strzalka.IsVisible = false;

        }

        if (Levels.Level == 3.8)
        {
            jozio.IsVisible = false;
            rozia.IsVisible = true;
            rozia.TranslationX = 40;
            rozia.TranslationY = 480;
            strzalka.IsVisible = false;
        }

        if (Levels.Level == 4.5)
        {
            jozio.IsVisible = false;
            rozia.IsVisible = true;
            rozia.TranslationX = 700;
            rozia.TranslationY = 720;
            strzalka.IsVisible = false;
        }
        if (Levels.Level == 4.8)
        {
            jozio.IsVisible = false;
            rozia.IsVisible = true;
            rozia.TranslationX = 40;
            rozia.TranslationY = 480;
            strzalka.IsVisible = false;
        }
        if(Levels.Level == 5)
        {
            jozio.IsVisible = false;
            rozia.IsVisible = false;
            strzalka.IsVisible = false;
        }
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


    async Task FadeBlack()
    {
        blackScreen.IsVisible = true;
        await blackScreen.FadeTo(1, 400);

        jozio.IsVisible = false;
        rozia.IsVisible = true;
        rozia.TranslationX = 1087;
        rozia.TranslationY = 308;
        jozio.IsVisible = false;
        await Task.Delay(1000);

        await blackScreen.FadeTo(0, 400);
        blackScreen.IsVisible = false;
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
        var door2 = new Rect(700, 800, 5, 5);
        var door3 = new Rect(0, 480, 50, 123);
        var door4 = new Rect(1100, 700, 30, 30);
        

        if (characterRect.IntersectsWith(strzalkaBox1) && !isNavigating && Levels.Level == 1)
        {
            if (licznikStrzalek == 0)
            {
                strzalkaBox1 = new Rect(600, 500, 50, 50);
                strzalka.TranslationX = 600;
                strzalka.TranslationY = 500;
                licznikStrzalek = 1;
            }
            else if (licznikStrzalek == 1)
            {
                strzalkaBox1 = new Rect(400, 500, 50, 50);
                strzalka.TranslationX = 400;
                strzalka.TranslationY = 500;
                licznikStrzalek = 2;
            }
            else if (licznikStrzalek == 2)
            {
                strzalkaBox1 = new Rect(150, 460, 50, 50);
                strzalka.TranslationX = 150;
                strzalka.TranslationY = 460;
                licznikStrzalek = 3;
                strzalka.Rotation = 0;
            }
            else if (licznikStrzalek == 3)
            {
                strzalka.IsVisible = false;
            }
        }
        else if (characterRect.IntersectsWith(strzalkaBox2) && !isNavigating && Levels.Level == 2)
        {
            if (licznikStrzalek == 0)
            {
                strzalkaBox2 = new Rect(400, 500, 50, 50);
                strzalka.TranslationX = 400;
                strzalka.TranslationY = 500;
                licznikStrzalek = 1;
            }
            else if (licznikStrzalek == 1)
            {
                strzalkaBox2 = new Rect(600, 500, 50, 50);
                strzalka.TranslationX = 600;
                strzalka.TranslationY = 500;
                licznikStrzalek = 2;
            }
            else if (licznikStrzalek == 2)
            {
                strzalkaBox2 = new Rect(700, 550, 50, 50);
                strzalka.TranslationX = 700;
                strzalka.TranslationY = 550;
                licznikStrzalek = 3;
                strzalka.Rotation = 180;
            }
            else if (licznikStrzalek == 3)
            {
                strzalkaBox2 = new Rect(700, 750, 50, 50);
                strzalka.TranslationX = 700;
                strzalka.TranslationY = 750;
                licznikStrzalek = 3;
                strzalka.Rotation = 180;
            }
            else if (licznikStrzalek == 4)
            {
                strzalka.IsVisible = false;
            }
        }


        if (characterRect.IntersectsWith(door1) && !isNavigating && Levels.Level < 2)
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
        if(characterRect.IntersectsWith(door4) && !isNavigating && Levels.Level >= 4 )
        {
            isNavigating = true;
            await Shell.Current.GoToAsync(nameof(Page6));
            return false;
        }
        return false;
    }
}
    