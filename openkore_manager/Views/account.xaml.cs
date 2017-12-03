using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace openkore_manager
{
    /// <summary>
    /// Interaction logic for account.xaml
    /// </summary>
    public partial class account : BasePage<AccountViewModel>
    {
        public account(string name)
        {
            InitializeComponent();
            ViewModel.BotName = name;
            ViewModel.InitConfig();
        }

        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("USER32.DLL")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("USER32.DLL")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        //assorted constants needed
        public static int GWL_STYLE = -16;
        public static int WS_BORDER = 0x00800000; //window with border
        public static int WS_DLGFRAME = 0x00400000; //window with double border but no title
        public static int WS_CAPTION = WS_BORDER | WS_DLGFRAME; //window with a title bar
        private const int WS_SIZEBOX = 0x00040000;
        private const int WS_SYSMENU = 0x00080000;      //window with no borders etc.
        private const int WS_MAXIMIZEBOX = 0x00010000;
        private const int WS_MINIMIZEBOX = 0x00020000;  //window with minimizebox
        private Process p = new Process();
        private ProcessStartInfo info = new ProcessStartInfo();
        private bool isRunning = false;

        private void Button_Start(object sender, RoutedEventArgs e)
        {
            if (!isRunning)
            {
                info.FileName = @"start.exe";
                info.Arguments = @"--control=../" + ViewModel.Control;
                info.WorkingDirectory = @"openkore";
                p.StartInfo = info;
                
                p.Start();

                System.Threading.Thread.Sleep(100);
                SetParent(p.MainWindowHandle, panel.Handle);
                int style = GetWindowLong(p.MainWindowHandle, GWL_STYLE);
                SetWindowLong(p.MainWindowHandle, GWL_STYLE, (style & ~(WS_CAPTION | WS_SIZEBOX) ));
                MoveWindow(p.MainWindowHandle, 0, 0, (int)panel.Width, (int)panel.Height, true);
            }
            else
            {
                p.CloseMainWindow();
                p.Close();
            }
            StartButton.ToolTip = isRunning ? "Start":"Stop" ;
            StartButtonIcon.Kind = isRunning ? MaterialDesignThemes.Wpf.PackIconKind.Play : MaterialDesignThemes.Wpf.PackIconKind.Stop;
            isRunning = !isRunning;
        }
        
    }
}
