using System;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.InteropServices;
using PropertyChanged;
namespace Abituria.viewmodel
{
    [ImplementPropertyChanged]
    public class WindowViewModel : BaseViewModel///Model widoku dla niestandardowego okna
    {
        private readonly Window mWindow;///Okno, które kontroluje Model widoku
        private int mOuterMarginSize = 10;///Margines okna pozwalający na cień
        private int mWindowRadius = 10;///Promień od krawędzi okna
        public int ResizeBorder { get; set; } = 6;
        public Thickness ResizeBorderThickness { get { return new Thickness(ResizeBorder + OuterMarginSize); } }
        public int OuterMarginSize///Zmaksymalizowane okno nie będzie miało przeźroczystego marginesu
        {
            get
            {
                return mWindow.WindowState == WindowState.Maximized ? 0 : mOuterMarginSize;
            }
            set
            {
                mOuterMarginSize = value;
            }
        }
        public Thickness OuterMarginSizeThickness { get { return new Thickness(OuterMarginSize); } }
        public int WindowRadius
        {
            get
            {
                return mWindow.WindowState == WindowState.Maximized ? 0 : mWindowRadius;
            }
            set
            {
                mWindowRadius = value;
            }
        }
        public CornerRadius WindowCornerRadius { get { return new CornerRadius(WindowRadius); } }
        public int TitleHeight { get; set; } = 42;///wysokość paska tytułowego
        public GridLength TitleHeightGridLength { get { return new GridLength(ResizeBorder + TitleHeight); } }
        public ICommand MinimizeCommand { get; set; }///Komenda do minimalizacji okna
        public ICommand MaximizeCommand { get; set; }///Komenda do maksymalizacji okna
        public ICommand CloseCommand { get; set; }///Komenda do zamykania okna
        public ICommand MenuCommand { get; set; }///Komenda do pokazania menu okna
        public WindowViewModel(Window window)///Standardowy konstruktor
        {
            mWindow = window;
            mWindow.StateChanged += (sender, e) =>
            {
                OnPropertyChanged(nameof(ResizeBorderThickness));///Odpala wydarzenia dla wszystkich właściwości zmiany rozmiaru
                OnPropertyChanged(nameof(OuterMarginSize));
                OnPropertyChanged(nameof(OuterMarginSizeThickness));
                OnPropertyChanged(nameof(WindowRadius));
                OnPropertyChanged(nameof(WindowCornerRadius));
            };
            MinimizeCommand = new RelayCommand(() => mWindow.WindowState = WindowState.Minimized);///Tworzenie komend
            MaximizeCommand = new RelayCommand(() => mWindow.WindowState ^= WindowState.Maximized);
            CloseCommand = new RelayCommand(() => mWindow.Close());
            MenuCommand = new RelayCommand(() => SystemCommands.ShowSystemMenu(mWindow, GetMousePosition()));
        }
        private Point GetMousePosition()///Bierze aktualną pozycje kursora na ekranie
        {
            var position = Mouse.GetPosition(mWindow);///Pozycja myszki względem okna
            return new Point(position.X + mWindow.Left, position.Y + mWindow.Top);///Dodaje pozycje okna
        }
    }
}