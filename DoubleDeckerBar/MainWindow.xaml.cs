using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Start9.Api.Controls;
using Start9.Api.Tools;
using Start9.Api.Programs;
using System.Timers;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Drawing;
using System.Windows.Media.Animation;
using System.IO;
using System.Windows.Controls.Primitives;

namespace DoubleDeckerBar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, GetWindowCmd uCmd);

        [DllImport("dwmapi.dll")]
        static extern void DwmEnableBlurBehindWindow(IntPtr hwnd, ref DWM_BLURBEHIND blurBehind);

        [StructLayout(LayoutKind.Sequential)]
        struct DWM_BLURBEHIND
        {
            public DWM_BB dwFlags;
            public bool fEnable;
            public IntPtr hRgnBlur;
            public bool fTransitionOnMaximized;

            public DWM_BLURBEHIND(bool enabled)
            {
                fEnable = enabled ? true : false;
                hRgnBlur = IntPtr.Zero;
                fTransitionOnMaximized = false;
                dwFlags = DWM_BB.Enable;
            }

            public System.Drawing.Region Region
            {
                get { return System.Drawing.Region.FromHrgn(hRgnBlur); }
            }

            public bool TransitionOnMaximized
            {
                get { return fTransitionOnMaximized != false; }
                set
                {
                    fTransitionOnMaximized = value ? true : false;
                    dwFlags |= DWM_BB.TransitionMaximized;
                }
            }

            public void SetRegion(System.Drawing.Graphics graphics, System.Drawing.Region region)
            {
                hRgnBlur = region.GetHrgn(graphics);
                dwFlags |= DWM_BB.BlurRegion;
            }
        }

        [Flags]
        enum DWM_BB
        {
            Enable = 1,
            BlurRegion = 2,
            TransitionMaximized = 4
        }

        [Flags]
        public enum ThumbnailFlags : int
        {
            RectDetination = 1,
            RectSource = 2,
            Opacity = 4,
            Visible = 8,
            SourceClientAreaOnly = 16
        }

        public enum GetWindowCmd : uint
        {
            First = 0,
            Last = 1,
            Next = 2,
            Prev = 3,
            Owner = 4,
            Child = 5,
            EnabledPopup = 6
        }

        const int GclHiconsm = -34;
        const int GclHicon = -14;
        const int IconSmall = 0;
        const int IconBig = 1;
        const int IconSmall2 = 2;
        const int WmGeticon = 0x7F;
        const int GWL_STYLE = -16;
        const int GWL_EXSTYLE = -20;
        const int TASKSTYLE = 0x10000000 | 0x00800000;
        const int WS_EX_TOOLWINDOW = 0x00000080;

        public IntPtr current = IntPtr.Zero;
        public IntPtr handle = IntPtr.Zero;

        public List<IntPtr> thumbs = new List<IntPtr>();


        public static IntPtr GetWindowLong(HandleRef hWnd, int nIndex)
        {
            if (IntPtr.Size == 4)
            {
                return GetWindowLong32(hWnd, nIndex);
            }
            return GetWindowLongPtr64(hWnd, nIndex);
        }


        [DllImport("user32.dll", EntryPoint = "GetWindowLong", CharSet = CharSet.Auto)]
        private static extern IntPtr GetWindowLong32(HandleRef hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", CharSet = CharSet.Auto)]
        private static extern IntPtr GetWindowLongPtr64(HandleRef hWnd, int nIndex);

        // 1. Change the function to call the Unicode variant, where applicable.
        // 2. Ask the marshaller to alert you to any errors that occur.
        // 3. Change the parameter types to make marshaling easier. 
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SystemParametersInfo(
                                                        int uiAction,
                                                        int uiParam,
                                                        ref RECT pvParam,
                                                        int fWinIni);

        private const Int32 SPIF_SENDWININICHANGE = 2;
        private const Int32 SPIF_UPDATEINIFILE = 1;
        private const Int32 SPIF_change = SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE;
        private const Int32 SPI_SETWORKAREA = 47;
        private const Int32 SPI_GETWORKAREA = 48;

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public Int32 Left;
            public Int32 Top;   // top is before right in the native struct
            public Int32 Right;
            public Int32 Bottom;
        }

        //https://stackoverflow.com/questions/6267206/how-can-i-resize-the-desktop-work-area-using-the-spi-setworkarea-flag
        private static bool SetWorkspace(RECT rect)
        {
            // Since you've declared the P/Invoke function correctly, you don't need to
            // do the marshaling yourself manually. The .NET FW will take care of it.

            bool result = SystemParametersInfo(SPI_SETWORKAREA,
                                               0,
                                               ref rect,
                                               SPIF_change);
            if (!result)
            {
                // Find out the error code
                MessageBox.Show("The last error was: " +
                                Marshal.GetLastWin32Error().ToString());
            }

            return result;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindow(IntPtr hWnd);

        readonly Timer _activeWindowTimer = new Timer
        {
            Interval = 1
        };

        readonly Timer _clockTimer = new Timer
        {
            Interval = 1
        };







        /*DoubleAnimation anim = new DoubleAnimation()
        {
            From = TaskBandScrollViewer.HorizontalOffset,
            To = finalValue,
            Duration = TimeSpan.FromMilliseconds(250),
            EasingFunction = new QuinticEase()
            {
                EasingMode = EasingMode.EaseOut
            }
        };
        Timer animTimer = new Timer
        {
            Interval = 1
        };
        animTimer.Elapsed += delegate
            {
                Dispatcher.Invoke(new Action(() =>
                {
            TaskBandScrollViewer.ScrollToHorizontalOffset(ScrollAnimator);
        }));
            };*/

        // public PatrickStart.MainWindow StartWindow = new PatrickStart.MainWindow();

        public double ScrollAnimator
        {
            get => (double)GetValue(ScrollAnimatorProperty);
            set => SetValue(ScrollAnimatorProperty, value);
        }

        public static readonly DependencyProperty ScrollAnimatorProperty = DependencyProperty.Register("ScrollAnimator",
            typeof(double), typeof(MainWindow),
            new FrameworkPropertyMetadata((double)0, FrameworkPropertyMetadataOptions.AffectsRender));



        public MainWindow()
        {
            InitializeComponent();
            Left = 0;
            Top = SystemParameters.PrimaryScreenHeight - Height;
            Width = SystemParameters.PrimaryScreenWidth;
            SetWorkspace(new RECT()
            {
                Left = (int)SystemParameters.WorkArea.Left,
                Top = (int)SystemParameters.WorkArea.Top,
                Right = (int)SystemParameters.WorkArea.Right,
                Bottom = (int)Top
            });
            ProgramWindow.WindowOpened += WindowOpened;
            Loaded += MainWindow_Loaded;
        }

        private void WindowOpened(object sender, Start9.Api.Objects.WindowEventArgs e)
        {
            Debug.WriteLine("WINDOW OPENED");
            Dispatcher.Invoke(new Action(() =>
            {
                IconButton b = GetIconButton(e.Window);
                if (b != null)
                {
                    TaskBand.Children.Add(b);
                }
            }));
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            var extendedStyle = WinApi.GetWindowLong(hwnd, WinApi.GwlExstyle);
            WinApi.SetWindowLong(hwnd, WinApi.GwlExstyle, extendedStyle.ToInt32() | WinApi.WsExToolwindow);
            DWM_BLURBEHIND blur = new DWM_BLURBEHIND()
            {
                dwFlags = DWM_BB.Enable,
                fEnable = true,
                hRgnBlur = IntPtr.Zero
            };
            DwmEnableBlurBehindWindow(new WindowInteropHelper(this).EnsureHandle(), ref blur);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            /*Start.ContextMenu = new TouchableContextMenu()
            {
                ItemsSource = new List<MenuItem>()
                {
                    new MenuItem()
                    {
                        Header = "Test once"
                    },
                    new MenuItem()
                    {
                        Header = "Test again"
                    },
                    new MenuItem()
                    {
                        Header = "sample text"
                    },
                    new MenuItem()
                    {
                        Header = "¿ C L I C C"
                    },
                    new MenuItem()
                    {
                        Header = "or"
                    },
                    new MenuItem()
                    {
                        Header = "T O C C H ?"
                    }
                }
            };*/

            foreach (ProgramWindow p in ProgramWindow.UserPerceivedProgramWindows)
            {
                IconButton b = GetIconButton(p);
                if (b != null)
                {
                    TaskBand.Children.Add(b);
                }
            }

            foreach (string f in Directory.EnumerateFiles(Environment.ExpandEnvironmentVariables(@"%appdata%\microsoft\Internet Explorer\Quick Launch")))
            {
                string path = f;
                if (Path.GetExtension(path).Contains("lnk"))
                {
                    //Get Executable instead of shortcut here ASAP
                    path = ShortcutTools.GetTargetPath(path);
                }
                Button quickButton = new Button()
                {
                    Background = new ImageBrush(MiscTools.GetIconFromFilePath(path, 16)),
                    BorderThickness = new Thickness(0),
                    Width = 16,
                    Height = 16,
                    Margin = new Thickness(0, 0, 7, 7)
                };
                quickButton.Click += delegate
                {
                    try
                    {
                        Process.Start(path);
                    }
                    catch
                    {

                    }
                };
                QuickLaunch.Children.Add(quickButton);
            }

                _activeWindowTimer.Elapsed += delegate
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    var active = WinApi.GetForegroundWindow();
                    for (int i = 0; i < TaskBand.Children.Count; i++)
                    {
                        IconButton t = TaskBand.Children[i] as IconButton;
                        var prwnd = (t.Tag as ProgramWindow).Hwnd;
                        try
                        {
                            if (prwnd == active)
                            {
                                if (t.IsEnabled)
                                {
                                    t.BringIntoView();
                                }
                                t.IsEnabled = false;
                            }
                            else if (IsWindow(prwnd))
                            {
                                t.IsEnabled = true;
                            }
                            else
                            {
                                TaskBand.Children.Remove(t);
                                i = i - 1;
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                }
                }));
            };
            _activeWindowTimer.Start();

            _clockTimer.Elapsed += delegate
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    MinutesHandTransform.Angle = (((double)(DateTime.Now.Minute)) / 60) * 360;
                    HoursHandTransform.Angle = (((double)(DateTime.Now.Hour)) / 12) * 360;

                    if (DateTime.Now.Hour <= 12)
                    {
                        ClockHours.Text = DateTime.Now.Hour.ToString();
                    }
                    else
                    {
                        ClockHours.Text = (DateTime.Now.Hour - 12).ToString();
                    }
                    if (DateTime.Now.Minute < 10)
                    {
                        ClockMinutes.Text = "0" + DateTime.Now.Minute.ToString();
                    }
                    else
                    {
                        ClockMinutes.Text = DateTime.Now.Minute.ToString();
                    }
                    ClockAmOrPm.Text = DateTime.Now.ToString("tt", System.Globalization.CultureInfo.InvariantCulture).ToLower();
                }));
            };
            _clockTimer.Start();
        }

        
        public IconButton GetIconButton(ProgramWindow p)
        {
            IconButton taskItemButton;
            if (((!(string.IsNullOrWhiteSpace(p.Name)))) & (p.Hwnd != new WindowInteropHelper(this).Handle))
            {
                taskItemButton = new IconButton()
                {
                    Content = p.Name,
                    Style = (Style)Resources["TaskItemButtonStyle"],
                    Tag = p
                };
                try
                {
                    taskItemButton.Icon = new Canvas()
                    {
                        Width = 16,
                        Height = 16,
                        //Background = new ImageBrush(GetIconFromProgramWindowWithoutGoingThroughCoreDllBecauseWeSupportWindowsNotTen(p).ToBitmapSource())
                        Background = new ImageBrush(p.Icon.ToBitmapSource())
                    };
                }
                catch { Debug.WriteLine("ICON FAILED"); };
                taskItemButton.Click += TaskItemButton_Click;
                taskItemButton.MouseRightButtonUp += delegate { p.ShowSystemMenu(); };
                return taskItemButton;
            }
            else
            {
                return null;
            }
        }

        private Icon GetIconFromProgramWindowWithoutGoingThroughCoreDllBecauseWeSupportWindowsNotTen(ProgramWindow p)
        {
                var iconHandle = SendMessage(p.Hwnd, WmGeticon, IconSmall2, 0);

                if (iconHandle == IntPtr.Zero)
                    iconHandle = SendMessage(p.Hwnd, WmGeticon, IconSmall, 0);
                if (iconHandle == IntPtr.Zero)
                    iconHandle = SendMessage(p.Hwnd, WmGeticon, IconBig, 0);
                if (iconHandle == IntPtr.Zero)
                    iconHandle = WinApi.GetClassLongPtr(p.Hwnd, GclHicon);
                if (iconHandle == IntPtr.Zero)
                    iconHandle = WinApi.GetClassLongPtr(p.Hwnd, GclHiconsm);

                if (iconHandle == IntPtr.Zero)
                    return null;

                try
                {
                    return System.Drawing.Icon.FromHandle(iconHandle);
                }
                finally
                {
                    WinApi.DestroyIcon(iconHandle);
                }
        }

        private void TaskItemButton_Click(object sender, RoutedEventArgs e)
        {
            var programWindow = ((sender as IconButton).Tag as ProgramWindow);
            programWindow.Show();
        }

        private void ScrollViewer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double finalValue = TaskBandScrollViewer.HorizontalOffset - (e.Delta * 2);
            /*animTimer.Start();

            anim.Completed += delegate
            {
                animTimer.Stop();
                BeginAnimation(MainWindow.ScrollAnimatorProperty, null);
                ScrollAnimator = finalValue;
            };

            BeginAnimation(MainWindow.ScrollAnimatorProperty, anim);
            //ScrollAnimator -= e.Delta;
            Debug.WriteLine("Scrolling... " + ScrollAnimator);*/
            //TaskBandScrollViewer.BeginAnimation(ScrollViewer.HorizontalOffsetProperty, anim);
            //TaskBandScrollViewer.scrollby.HorizontalOffset += e.Delta;
            /*var sv = (ScrollViewer)Template.FindName("PART_MyScrollViewer", this); // If you do not already have a reference to it somewhere.
            var ip = (ItemsPresenter)sv.Content;
            var point = item.TranslatePoint(new System.Windows.Point() - (Vector)e.GetPosition(sv), ip);*/
            /*double distance = TaskBandScrollViewer.HorizontalOffset;*/
            if (e.Delta > 0)
            {
                TaskBandScrollViewer.ScrollToHorizontalOffset(TaskBandScrollViewer.HorizontalOffset - 134);
            }
            else
            {
                TaskBandScrollViewer.ScrollToHorizontalOffset(TaskBandScrollViewer.HorizontalOffset + 134);
            }
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            //if (StartWindow.IsVisible)
            //{
            //    StartWindow.Hide();
            //}
            //else
            //{
            //    StartWindow.Top = (Top - StartWindow.Height) + 24;
            //    StartWindow.Show();
            //}
        }
    }
}
