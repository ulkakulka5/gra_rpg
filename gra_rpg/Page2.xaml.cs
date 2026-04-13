using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System;
using Microsoft.Maui.Graphics;
using System.Threading;
using Microsoft.Maui.Dispatching;

using Plugin.Maui.Audio;




#if WINDOWS
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml.Input;
using Windows.System;
#endif


namespace gra_rpg;

/// @class Page2
/// @brief Scena ze skrzyni¹ i zagadk¹.
/// @details Gracz mo¿e otworzyæ skrzyniê wpisuj¹c poprawny kod.
public partial class Page2 : ContentPage
{
    /// @brief Krok ruchu.
    double step = 10;

    /// @brief Flaga nawigacji.
    bool isNavigating = false;

    /// @brief Odtwarzacz audio.
    IAudioPlayer? player;

    /// @brief Konstruktor strony.
    public Page2()
    {
        InitializeComponent();
    
#if WINDOWS
        Loaded += OnLoaded;
#endif
    }

#if WINDOWS
    /// @brief Inicjalizacja obs³ugi klawiatury.
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
                content.PointerPressed += Content_PointerPressed;
                content.Focus(Microsoft.UI.Xaml.FocusState.Programmatic);
            }
        }
    }
    /// @brief Obs³uga klawiszy strza³ek.
    /// @param e Zdarzenie klawiatury.
    /// @details Przesuwa postaæ "jozia".
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

        
    }
    private void Content_PointerPressed(object? sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        var point = e.GetCurrentPoint((Microsoft.UI.Xaml.UIElement)sender);
        double mouseX = point.Position.X;
        double mouseY = point.Position.Y;

        Point mouse = new Point(mouseX, mouseY);
        }

     /// @brief Obs³uga klikniêcia skrzyni.
    /// @event Click
    /// @details Wyœwietla panel wpisywania kodu.
    private void OnChestClicked(object sender, EventArgs e)
    {

        ChestCode.IsVisible = true;
        Closed.IsVisible = true;
        CodeEntry1.Text = "";
        CodeEntry2.Text = "";
        CodeEntry3.Text = "";
        CodeEntry1.Focus();

    }

#endif


    /// @brief Wywo³ywane przy pojawieniu siê strony.
    /// @details Ustawia pozycje elementów UI.
    protected override void OnAppearing()
    {
        base.OnAppearing();


        CodeEntry1.TranslationX = 12;
        CodeEntry2.TranslationX = 12;
        CodeEntry3.TranslationX = 12;
        CodeEntry1.TranslationY = -5;
        CodeEntry2.TranslationY = -5;
        CodeEntry3.TranslationY = -5;
        Closed.TranslationX = 5;
        Opened.TranslationX = 5;
        Chest_close.TranslationX = 500; 
        Chest_close.TranslationY = 100;
        Chest_open.TranslationX = 500;
        Chest_open.TranslationY = 100;
        Chleb.TranslationX = 490;
        Chleb.TranslationY = 80;
        Inventory.TranslationX = -580;
        Inventory.TranslationY = -220;

        jozio.TranslationX = 549;
        jozio.TranslationY = 270;

    }
    /*private async  void OnCodeSubmit(object sender, EventArgs e)
    {

        
    }*/

    /// @brief Obs³uga zmiany pierwszego pola kodu.
    private void CodeEntry1_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!string.IsNullOrEmpty(CodeEntry1.Text) && CodeEntry1.Text.Length == 1)
        {
            CodeEntry2.Focus();
        }
    }

    /// @brief Obs³uga zmiany drugiego pola kodu.
    private void CodeEntry2_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!string.IsNullOrEmpty(CodeEntry2.Text) && CodeEntry2.Text.Length == 1)
        {
            CodeEntry3.Focus();
        }
    }

    /// @brief Obs³uga trzeciego pola kodu.
    /// @details Sprawdza poprawnoœæ kodu (158).
    /// @async
    private async void CodeEntry3_TextChanged(object sender, TextChangedEventArgs e)
    {
        string first = CodeEntry1.Text;
        string second = CodeEntry2.Text;
        string third = CodeEntry3.Text;
        string code = first + second + third;

        if (code == "158")
        {
            PlayOK();
            Closed.IsVisible = false;
            Opened.IsVisible = true;
            await Task.Delay(800);
            ChestCode.IsVisible = false;
            Opened.IsVisible = false;
            Chest_close.IsVisible = false;
            Chest_close.IsEnabled = false;
            Chest_open.IsVisible = true;
            Chleb.IsVisible = true;
        }
        else if (first != "" && second != "" && third != "" && code != "158")
        {
            PlayError();
            Opened.IsVisible = false;
            Closed.IsVisible = true;
            await Task.Delay(2000);
            ChestCode.IsVisible = false;
            Closed.IsVisible = false;
            CodeEntry1.Focus();
        }

        /// @brief Odtwarza dŸwiêk sukcesu.
        async void PlayOK()
        {
            var audioManager = AudioManager.Current;

            var stream = await FileSystem.OpenAppPackageFileAsync("chest_succes.mp3");

            player = audioManager.CreatePlayer(stream);

            player.Loop = false;
            player.Play();
        }

        /// @brief Odtwarza dŸwiêk b³êdu.
        async void PlayError()
        {
            var audioManager = AudioManager.Current;

            var stream = await FileSystem.OpenAppPackageFileAsync("chest_error.mp3");

            player = audioManager.CreatePlayer(stream);

            player.Loop = false;
            player.Play();
        }

    }

    /// @brief Obs³uga klikniêcia chleba (loot).
    /// @async
    /// @details Dodaje przedmiot do ekwipunku i wraca do mapy.
    private async void OnBreadClicked(object sender, EventArgs e)
    {
        Chleb.IsVisible = false;
        Chleb.IsEnabled = false;
        Levels.Level = 2;
        Inventory.Source = "inventory_bread.png";
        await Task.Delay(2000);
        await Shell.Current.GoToAsync(nameof(Page1));

    }


}

