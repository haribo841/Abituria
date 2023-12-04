using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows;

namespace Abituria.viewmodel
{
    public class WindowResizer///Naprawia błędy z zakrywaniem paska zadań
    {
        private Window mWindow;///Okno do zmieniania rozmiaru
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);
        [DllImport("user32.dll")]
        static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr MonitorFromPoint(POINT pt, MonitorOptions dwFlags);
        public WindowResizer(Window window)///Standardowy konstruktor z parametrem okna do poprawnej maksymalizacji
        {
            mWindow = window;
            mWindow.SourceInitialized += Window_SourceInitialized; ///Nasłuchuje na inicjalizacje źródła
        }
        private void Window_SourceInitialized(object sender, System.EventArgs e)///Inicjalizuje i zaczepia okno
        {
            var handle = (new WindowInteropHelper(mWindow)).Handle;///Bierze uchwyt okna
            var handleSource = HwndSource.FromHwnd(handle);
            if (handleSource == null)///Jak nie znajdzie, to kończy
                return;
            handleSource.AddHook(WindowProc);///Zaczepie do wiadomości okna
        }
        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)///Nasłuchuje wszystkich wiadomości tego okna
        {
            switch (msg)///Uchwyt GetMinMaxInfo okna
            {
                case 0x0024:/* WM_GETMINMAXINFO */
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
            }
            return (IntPtr)0;
        }
        private void WmGetMinMaxInfo(System.IntPtr hwnd, System.IntPtr lParam)///Bierze min./maks. rozmiar okna i poprawnie podpina na psku zadań
        {
            POINT lMousePosition;
            GetCursorPos(out lMousePosition);
            IntPtr lPrimaryScreen = MonitorFromPoint(new POINT(0, 0), MonitorOptions.MONITOR_DEFAULTTOPRIMARY);
            MONITORINFO lPrimaryScreenInfo = new MONITORINFO();
            if (GetMonitorInfo(lPrimaryScreen, lPrimaryScreenInfo) == false)
            {
                return;
            }
            IntPtr lCurrentScreen = MonitorFromPoint(lMousePosition, MonitorOptions.MONITOR_DEFAULTTONEAREST);
            MINMAXINFO lMmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
            if (lPrimaryScreen.Equals(lCurrentScreen) == true)
            {
                lMmi.ptMaxPosition.X = lPrimaryScreenInfo.rcWork.Left;
                lMmi.ptMaxPosition.Y = lPrimaryScreenInfo.rcWork.Top;
                lMmi.ptMaxSize.X = lPrimaryScreenInfo.rcWork.Right - lPrimaryScreenInfo.rcWork.Left;
                lMmi.ptMaxSize.Y = lPrimaryScreenInfo.rcWork.Bottom - lPrimaryScreenInfo.rcWork.Top;
            }
            else
            {
                lMmi.ptMaxPosition.X = lPrimaryScreenInfo.rcMonitor.Left;
                lMmi.ptMaxPosition.Y = lPrimaryScreenInfo.rcMonitor.Top;
                lMmi.ptMaxSize.X = lPrimaryScreenInfo.rcMonitor.Right - lPrimaryScreenInfo.rcMonitor.Left;
                lMmi.ptMaxSize.Y = lPrimaryScreenInfo.rcMonitor.Bottom - lPrimaryScreenInfo.rcMonitor.Top;
            }
            Marshal.StructureToPtr(lMmi, lParam, true);///Mając maks. rozmiar hostmoże dostosowywać
        }
    }
    enum MonitorOptions : uint
    {
        MONITOR_DEFAULTTONULL = 0x00000000,
        MONITOR_DEFAULTTOPRIMARY = 0x00000001,
        MONITOR_DEFAULTTONEAREST = 0x00000002
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class MONITORINFO
    {
        public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
        public Rectangle rcMonitor = new Rectangle();
        public Rectangle rcWork = new Rectangle();
        public int dwFlags = 0;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct Rectangle
    {
        public int Left, Top, Right, Bottom;
        public Rectangle(int left, int top, int right, int bottom)
        {
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct MINMAXINFO
    {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;///Współrzędna X punktu
        public int Y;///Współrzędna Y punktu
        public POINT(int x, int y)///Tworzy punkt o współrzędnych (X,Y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}