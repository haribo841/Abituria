using System;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.InteropServices;
using PropertyChanged;
using Abituria.datamodels;
namespace Abituria.viewmodel
{
    public class MyPasswordBox
    {

    }
    [ImplementPropertyChanged]
    public class WindowViewModel : BaseViewModel///Model widoku dla niestandardowego okna
    {
        private readonly Window mWindow;///Okno, które kontroluje Model widoku
        private readonly WindowResizer mWindowResizer;///Utrzymuje odpowiedni rozmiar okna
        private int mOuterMarginSize = 10;///Margines okna pozwalający na cień
        private int mWindowRadius = 10;///Promień od krawędzi okna
        private WindowDockPosition mDockPosition = WindowDockPosition.Undocked;
        ///Ostatnia znana pozycja doku
        public double WindowMinimumWidth { get; set; } = 1115;///Najmniejsza szerokość jaką może mieć okno
        public double WindowMinimumHeight { get; set; } = 815;///Najmniejsza wysokość jaką może mieć okno
        public bool BeingMoved { get; set; }///Prawda jeśli okno jest obecnie przeciągane
        public bool Borderless { get { return (mWindow.WindowState == WindowState.Maximized || mDockPosition != WindowDockPosition.Undocked); } }///Prawda jeśli okno powinno być bez ramki bo jest zmaksymalizowane albo zadokowane
        public int ResizeBorder { get { return Borderless ? 0 : 6; } }///Rozmiar granicy zmiany rozmiaru wokół okna
        public Thickness ResizeBorderThickness { get { return new Thickness(ResizeBorder + OuterMarginSize); } }///Rozmiar obramówki okna do zewnętrznego marginesu
        public Thickness InnerContentPadding { get; set; } = new Thickness(0); ///Wypełnienie wewnętrznej zawartoścu okna
        public int OuterMarginSize///Margines wokół okna pozwalający na cień
        {
            get
            {
                return Borderless ? 0 : mOuterMarginSize;
            }
            set => mOuterMarginSize = value; ///Okno powinno być bez ramki bo jest zmaksymalizowane albo zadokowane
        }
        public Thickness OuterMarginSizeThickness { get { return new Thickness(OuterMarginSize); } }
        public int WindowRadius///Promień krawędzi okna
        {
            get => Borderless ? 0 : mWindowRadius;
            set => mWindowRadius = value;
        }
        public int FlatBorderThickness => Borderless && mWindow.WindowState != WindowState.Maximized ? 1 : 0;///Prostokątna granica wokół okna podczas zadokowania
        public CornerRadius WindowCornerRadius => new CornerRadius(WindowRadius);///Promień krawędzi okna
        public int TitleHeight { get; set; } = 42;///Wysokość paska tytułowego
        public GridLength TitleHeightGridLength => new GridLength(TitleHeight + ResizeBorder);///Wysokość paska tytułowego
        public ApplicationPage CurrentPage { get; set; } = ApplicationPage.Login;///Obecna strona aplikacji
        public ApplicationPage CurrentPage1 { get; set; } = ApplicationPage.Main;///Główna strona aplikacji
        public ApplicationPage CurrentPage2 { get; set; } = ApplicationPage.Menu;///Główna strona aplikacji
        public bool DimmableOverlayVisible { get; set; }
        public ICommand MinimizeCommand { get; set; }///Komenda do minimalizacji okna
        public ICommand MaximizeCommand { get; set; }///Komenda do maksymalizacji okna
        public ICommand CloseCommand { get; set; }///Komenda do zamykania okna
        public ICommand MenuCommand { get; set; }///Komenda do pokazania menu okna
        public WindowViewModel(Window window)///Standardowy konstruktor
        {
            mWindow = window;
            mWindow.StateChanged += (sender, e) =>
            {
                WindowResized();///Odpala wydarzenia dla wszystkich właściwości zmiany rozmiaru
            };
            MinimizeCommand = new RelayCommand(() => mWindow.WindowState = WindowState.Minimized);///Tworzenie komend
            MaximizeCommand = new RelayCommand(() => mWindow.WindowState ^= WindowState.Maximized);
            CloseCommand = new RelayCommand(() => mWindow.Close());
            MenuCommand = new RelayCommand(() => SystemCommands.ShowSystemMenu(mWindow, GetMousePosition()));
            mWindowResizer = new WindowResizer(mWindow);///Naprawia problem ze zmianą rozmiaru okna
            mWindowResizer.WindowDockChanged += (dock) =>///Nasłuchuje zmian w doku
            {
                mDockPosition = dock;///Przechowuje ostatnią pozycje
                WindowResized();///Odpala wydarzenia zmian rozmiaru
            };
            mWindowResizer.WindowStartedMove += () =>
            {
                BeingMoved = true;///Aktualizuje flage bycia przenoszonym
            };
            mWindowResizer.WindowFinishedMove += () =>///Naprawia upuszczanie niezadokowanego okna
            {
                BeingMoved = false;///Aktualizuje flage bycia przenoszonym
            };
        }
        private Point GetMousePosition()///Bierze aktualną pozycje kursora na ekranie
        {
            return mWindowResizer.GetCursorPosition();///Pozycja myszki względem okna
        }
        private void WindowResized()///Funkcja do konstruktora
        {
            OnPropertyChanged(nameof(Borderless));
            OnPropertyChanged(nameof(ResizeBorderThickness));
            OnPropertyChanged(nameof(OuterMarginSize));
            OnPropertyChanged(nameof(OuterMarginSizeThickness));
            OnPropertyChanged(nameof(WindowRadius));
            OnPropertyChanged(nameof(WindowCornerRadius));
        }
    }
}