using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media;

namespace Abituria.viewmodel
{
    public enum WindowDockPosition
    {
        Undocked = 0,///Nie zadokowany
        Left = 1,///Zadokowany po lewej
        Right = 2,///Zadokowany po prawej
        TopBottom = 3,///Zadokowany na górze/dole
        TopLeft = 4,///Zadokowany na górze lewej
        TopRight = 5,///Zadokowany na górze po prawej
        BottomLeft = 6,///Zadokowany na dole po lewej
        BottomRight = 7,///Zadokowany na dole po prawej
    }
    public class WindowResizer///Naprawia błędy z zakrywaniem paska zadań
    {
        private Window mWindow;///Okno do zmieniania rozmiaru
        private Rect mScreenSize = new Rect();///Ostatni obliczony rozmiar ekranu
        private int mEdgeTolerance = 1;///Jak blisko krawędzi ma być wykryte okno, tak jak na krawędzi ekranu
        private DpiScale? mMonitorDpi;///Macierz transformacji używana do konwersji rozmiarów WPF na piksele
        private IntPtr mLastScreen;///Ostatni ekran na którym było okno
        private WindowDockPosition mLastDock = WindowDockPosition.Undocked;///Ostatnia znana pozycja doku
        private bool mBeingMoved = false;///Flaga wskazująca czy okno jest obecnie przeciągane
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);
        [DllImport("user32.dll")]
        static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr MonitorFromPoint(POINT pt, MonitorOptions dwFlags);
        [DllImport("user32.dll")]
        static extern IntPtr MonitorFromWindow(IntPtr hwnd, MonitorOptions dwFlags);
        public event Action<WindowDockPosition> WindowDockChanged = (dock) => { };///Wywoływane, kiedy pozycja doku okna się zmienia
        public event Action WindowStartedMove = () => { };///Wywoływane, kiedy zaczęto przeciągać okno
        public event Action WindowFinishedMove = () => { };///Wywoływane, kiedy skończono przeciągać okno
        public Rectangle CurrentMonitorSize { get; set; } = new Rectangle();///Rozmiar i pozycja obecnego monitora na którym jest okno
        public Thickness CurrentMonitorMargin { get; private set; } = new Thickness();///Margines wokół okna
        public Rect CurrentScreenSize => mScreenSize;///Rozmiar i pozycja obecnego ekranu, kiedy jest ich kilka
        public WindowResizer(Window window)///Standardowy konstruktor z parametrem okna do poprawnej maksymalizacji
        {
            mWindow = window;
            mWindow.SourceInitialized += Window_SourceInitialized; ///Nasłuchuje na inicjalizacje źródła
            mWindow.SizeChanged += Window_SizeChanged;///Do dokowanmia
            mWindow.LocationChanged += Window_LocationChanged;
        }
        private void Window_SourceInitialized(object sender, System.EventArgs e)///Inicjalizuje i zaczepia okno
        {
            var handle = (new WindowInteropHelper(mWindow)).Handle;///Bierze uchwyt okna
            var handleSource = HwndSource.FromHwnd(handle);
            if (handleSource == null)///Jak nie znajdzie, to kończy
                return;
            handleSource.AddHook(WindowProc);///Zaczepie do wiadomości okna
        }
        private void Window_LocationChanged(object sender, EventArgs e)///Do poruszania oknem
        {
            Window_SizeChanged(null, null);
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)///Do zmiany rozmiaru
        {
            WmGetMinMaxInfo(IntPtr.Zero, IntPtr.Zero);///Aktualna informacja o monitorze            
            mMonitorDpi = VisualTreeHelper.GetDpi(mWindow);///Transformacja z obecnej pozycji            
            if (mMonitorDpi == null)///Nie można obliczyć rozmiaru dopóki skaluje
                return;
            var top = mWindow.Top;///Prostokąt okna
            var left = mWindow.Left;
            var bottom = top + mWindow.Height;
            var right = left + mWindow.Width;
            var windowTopLeft = new Point(left * mMonitorDpi.Value.DpiScaleX, top * mMonitorDpi.Value.DpiScaleX); ///Pozycja i rozmiar okna w pikselach
            var windowBottomRight = new Point(right * mMonitorDpi.Value.DpiScaleX, bottom * mMonitorDpi.Value.DpiScaleX);
            var edgedTop = windowTopLeft.Y <= (mScreenSize.Top + mEdgeTolerance) && windowTopLeft.Y >= (mScreenSize.Top - mEdgeTolerance);///Sprawdzenie czy krawędzie są zadokowane
            var edgedLeft = windowTopLeft.X <= (mScreenSize.Left + mEdgeTolerance) && windowTopLeft.X >= (mScreenSize.Left - mEdgeTolerance);
            var edgedBottom = windowBottomRight.Y >= (mScreenSize.Bottom - mEdgeTolerance) && windowBottomRight.Y <= (mScreenSize.Bottom + mEdgeTolerance);
            var edgedRight = windowBottomRight.X >= (mScreenSize.Right - mEdgeTolerance) && windowBottomRight.X <= (mScreenSize.Right + mEdgeTolerance);
            var dock = WindowDockPosition.Undocked;///Zadokowanie pozycji
            if (edgedTop && edgedBottom && edgedLeft)///Zadokowanie po lewej
                dock = WindowDockPosition.Left;
            else if (edgedTop && edgedBottom && edgedRight)///Zadokowanie po prawej
                dock = WindowDockPosition.Right;
            else if (edgedTop && edgedBottom)///Zadokowanie na dole/górze
                dock = WindowDockPosition.TopBottom;
            else if (edgedTop && edgedLeft)///Zadokowanie na górze po lewej
                dock = WindowDockPosition.TopLeft;
            else if (edgedTop && edgedRight)///Zadokowanie na górze po prawej
                dock = WindowDockPosition.TopRight;
            else if (edgedBottom && edgedLeft)///Zadokowanie na dole po lewej
                dock = WindowDockPosition.BottomLeft;
            else if (edgedBottom && edgedRight)///Zadokowanie na dole po prawej
                dock = WindowDockPosition.BottomRight;
            else
                dock = WindowDockPosition.Undocked;///Nie dokuj
            if (dock != mLastDock)///Jeśli dokowanie sie zmieniło
                WindowDockChanged(dock);///Poinformowanie o dokowaniu
            mLastDock = dock;///Zapisanie ostatniej pozycji dokowania
        }
        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)///Nasłuchuje wszystkich wiadomości tego okna
        {
            switch (msg)///Uchwyt GetMinMaxInfo okna
            {
                case 0x0024:/* WM_GETMINMAXINFO */
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
                case 0x0231: // WM_ENTERSIZEMOVE
                    mBeingMoved = true;///Początek przesuwania
                    WindowStartedMove();
                    break;
                case 0x0232: // WM_EXITSIZEMOVE
                    mBeingMoved = false;///Koniec przesuwania
                    WindowFinishedMove();
                    break;
            }
            return (IntPtr)0;
        }
        private void WmGetMinMaxInfo(System.IntPtr hwnd, System.IntPtr lParam)///Bierze min./maks. rozmiar okna i poprawnie podpina na psku zadań
        {
            GetCursorPos(out var lMousePosition);
            var lCurrentScreen = mBeingMoved ?///Weź obecny ekran
                MonitorFromPoint(lMousePosition, MonitorOptions.MONITOR_DEFAULTTONULL) :///Jeżeli przeciągane weź z pozycji myszki
                MonitorFromWindow(hwnd, MonitorOptions.MONITOR_DEFAULTTONULL);///Inaczej weź z pozycji okna
            var lPrimaryScreen = MonitorFromPoint(new POINT(0, 0), MonitorOptions.MONITOR_DEFAULTTOPRIMARY);
            var lCurrentScreenInfo = new MONITORINFO();///Weź informacje o obecnym ekranie
            if (GetMonitorInfo(lCurrentScreen, lCurrentScreenInfo) == false)
                return;
            var lPrimaryScreenInfo = new MONITORINFO();///Weź informacje o głównym ekranie
            if (GetMonitorInfo(lPrimaryScreen, lPrimaryScreenInfo) == false)
                return;
            mMonitorDpi = VisualTreeHelper.GetDpi(mWindow);
            mLastScreen = lCurrentScreen;///Przechowuje ostatnią znaną pozycje ekranu
            var currentX = lCurrentScreenInfo.RCWork.Left - lCurrentScreenInfo.RCMonitor.Left;///Obszar roboczy
            var currentY = lCurrentScreenInfo.RCWork.Top - lCurrentScreenInfo.RCMonitor.Top;
            var currentWidth = (lCurrentScreenInfo.RCWork.Right - lCurrentScreenInfo.RCWork.Left);
            var currentHeight = (lCurrentScreenInfo.RCWork.Bottom - lCurrentScreenInfo.RCWork.Top);
            var currentRatio = (float)currentWidth / (float)currentHeight;
            var primaryX = lPrimaryScreenInfo.RCWork.Left - lPrimaryScreenInfo.RCMonitor.Left;
            var primaryY = lPrimaryScreenInfo.RCWork.Top - lPrimaryScreenInfo.RCMonitor.Top;
            var primaryWidth = (lPrimaryScreenInfo.RCWork.Right - lPrimaryScreenInfo.RCWork.Left);
            var primaryHeight = (lPrimaryScreenInfo.RCWork.Bottom - lPrimaryScreenInfo.RCWork.Top);
            var primaryRatio = (float)primaryWidth / (float)primaryHeight;
            if (lParam != IntPtr.Zero)
            {
                var lMmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));///Struktura do wypełnienia informacjami
                lMmi.PointMaxPosition.X = lPrimaryScreenInfo.RCMonitor.Left;///Ustaw do rozmiaru głównego monitora
                lMmi.PointMaxPosition.Y = lPrimaryScreenInfo.RCMonitor.Top;
                lMmi.PointMaxSize.X = lPrimaryScreenInfo.RCMonitor.Right;
                lMmi.PointMaxSize.Y = lPrimaryScreenInfo.RCMonitor.Bottom;
                var minSize = new Point(mWindow.MinWidth * mMonitorDpi.Value.DpiScaleX, mWindow.MinHeight * mMonitorDpi.Value.DpiScaleX);///Ustaw minimalny rozmiar
                lMmi.PointMinTrackSize.X = (int)minSize.X;
                lMmi.PointMinTrackSize.Y = (int)minSize.Y;
                Marshal.StructureToPtr(lMmi, lParam, true);///Mając maks. rozmiar host może dostosowywać
            }
            CurrentMonitorSize = new Rectangle(currentX, currentY, currentWidth + currentX, currentHeight + currentY);///Ustaw rozmiar monitora
            CurrentMonitorMargin = new Thickness(///Weź marines wokół okna
                (lCurrentScreenInfo.RCWork.Left - lCurrentScreenInfo.RCMonitor.Left) / mMonitorDpi.Value.DpiScaleX,
                (lCurrentScreenInfo.RCWork.Top - lCurrentScreenInfo.RCMonitor.Top) / mMonitorDpi.Value.DpiScaleY,
                (lCurrentScreenInfo.RCMonitor.Right - lCurrentScreenInfo.RCWork.Right) / mMonitorDpi.Value.DpiScaleX,
                (lCurrentScreenInfo.RCMonitor.Bottom - lCurrentScreenInfo.RCWork.Bottom) / mMonitorDpi.Value.DpiScaleY
                );
            mScreenSize = new Rect(lCurrentScreenInfo.RCWork.Left, lCurrentScreenInfo.RCWork.Top, currentWidth, currentHeight);///Przechowaj nowy rozmiar
        }
        public Point GetCursorPosition()///Weź obecną pozycje kursora w koordynatach
        {
            GetCursorPos(out var lMousePosition);///Weź pozycje myszki
            return new Point(lMousePosition.X / mMonitorDpi.Value.DpiScaleX, lMousePosition.Y / mMonitorDpi.Value.DpiScaleY);///Skalowanie DPI
        }
    }
    public enum MonitorOptions : uint
    {
        MONITOR_DEFAULTTONULL = 0x00000000,
        MONITOR_DEFAULTTOPRIMARY = 0x00000001,
        MONITOR_DEFAULTTONEAREST = 0x00000002
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class MONITORINFO
    {
        public int CBSize = Marshal.SizeOf(typeof(MONITORINFO));
        public Rectangle RCMonitor = new Rectangle();
        public Rectangle RCWork = new Rectangle();
        public int DWFlags = 0;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct Rectangle
    {
        public int Left, Top, Right, Bottom;
        public Rectangle(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct MINMAXINFO
    {
        public POINT PointReserved;
        public POINT PointMaxSize;
        public POINT PointMaxPosition;
        public POINT PointMinTrackSize;
        public POINT PointMaxTrackSize;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;///Współrzędna X punktu
        public int Y;///Współrzędna Y punktu
        public POINT(int x, int y)///Tworzy punkt o współrzędnych (X,Y)
        {
            X = x;
            Y = y;
        }
        public override string ToString()
        {
            return $"{X} {Y}";
        }
    }
}