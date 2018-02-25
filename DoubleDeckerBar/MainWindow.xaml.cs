using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Start9.Api.Controls;
using Start9.Api.Objects;
using Start9.Api.Programs;
using Start9.Api.Tools;

namespace DoubleDeckerBar
{
	/// <summary>
	///     Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
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

		[Flags]
		public enum ThumbnailFlags
		{
			RectDetination = 1,
			RectSource = 2,
			Opacity = 4,
			Visible = 8,
			SourceClientAreaOnly = 16
		}

		private const int GclHiconsm = -34;
		private const int GclHicon = -14;
		private const int IconSmall = 0;
		private const int IconBig = 1;
		private const int IconSmall2 = 2;
		private const int WmGeticon = 0x7F;
		private const int GwlStyle = -16;
		private const int GwlExstyle = -20;
		private const int Taskstyle = 0x10000000 | 0x00800000;
		private const int WsExToolwindow = 0x00000080;

		private const int SpifSendwininichange = 2;
		private const int SpifUpdateinifile = 1;
		private const int SpifChange = SpifUpdateinifile | SpifSendwininichange;
		private const int SpiSetworkarea = 47;
		private const int SpiGetworkarea = 48;

		public static readonly DependencyProperty ScrollAnimatorProperty = DependencyProperty.Register("ScrollAnimator",
			typeof(double), typeof(MainWindow),
			new FrameworkPropertyMetadata((double) 0, FrameworkPropertyMetadataOptions.AffectsRender));

		private readonly Timer _activeWindowTimer = new Timer
		{
			Interval = 1
		};

		private readonly Timer _clockTimer = new Timer
		{
			Interval = 1
		};

		public IntPtr Current = IntPtr.Zero;
		public IntPtr Handle = IntPtr.Zero;

		public List<IntPtr> Thumbs = new List<IntPtr>();


		public MainWindow()
		{
			InitializeComponent();
			Left = 0;
			Top = SystemParameters.PrimaryScreenHeight - Height;
			Width = SystemParameters.PrimaryScreenWidth;
			SetWorkspace(new Rect
			{
				Left = (int) SystemParameters.WorkArea.Left,
				Top = (int) SystemParameters.WorkArea.Top,
				Right = (int) SystemParameters.WorkArea.Right,
				Bottom = (int) Top
			});
			ProgramWindow.WindowOpened += WindowOpened;
			Loaded += MainWindow_Loaded;
		}


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
			get => (double) GetValue(ScrollAnimatorProperty);
			set => SetValue(ScrollAnimatorProperty, value);
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

		[DllImport("user32.dll")]
		public static extern IntPtr GetWindow(IntPtr hWnd, GetWindowCmd uCmd);

		[DllImport("dwmapi.dll")]
		private static extern void DwmEnableBlurBehindWindow(IntPtr hwnd, ref DwmBlurbehind blurBehind);


		public static IntPtr GetWindowLong(HandleRef hWnd, int nIndex)
		{
			if (IntPtr.Size == 4) return GetWindowLong32(hWnd, nIndex);
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
			ref Rect pvParam,
			int fWinIni);

		//https://stackoverflow.com/questions/6267206/how-can-i-resize-the-desktop-work-area-using-the-spi-setworkarea-flag
		private static bool SetWorkspace(Rect rect)
		{
			// Since you've declared the P/Invoke function correctly, you don't need to
			// do the marshaling yourself manually. The .NET FW will take care of it.

			bool result = SystemParametersInfo(SpiSetworkarea,
				0,
				ref rect,
				SpifChange);
			if (!result)
				MessageBox.Show("The last error was: " +
				                Marshal.GetLastWin32Error());

			return result;
		}

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool IsWindow(IntPtr hWnd);

		private void WindowOpened(object sender, WindowEventArgs e)
		{
			Debug.WriteLine("WINDOW OPENED");
			Dispatcher.Invoke(new Action(() =>
			{
				IconButton b = GetIconButton(e.Window.Hwnd);
				if (b != null) TaskBand.Children.Add(b);
			}));
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);
			IntPtr hwnd = new WindowInteropHelper(this).Handle;
			IntPtr extendedStyle = WinApi.GetWindowLong(hwnd, WinApi.GwlExstyle);
			WinApi.SetWindowLong(hwnd, WinApi.GwlExstyle, extendedStyle.ToInt32() | WinApi.WsExToolwindow);
			var blur = new DwmBlurbehind
			{
				dwFlags = DwmBb.Enable,
				fEnable = true,
				hRgnBlur = IntPtr.Zero
			};
			DwmEnableBlurBehindWindow(new WindowInteropHelper(this).EnsureHandle(), ref blur);
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			/*Binding scrollBinding = new Binding()
			{
			    Source = this,
			    Path = new PropertyPath("ScrollAnimator"),
			    Mode = BindingMode.OneWay,
			    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			BindingOperations.SetBinding(TaskBandScrollViewer, MainWindow.ScrollAnimatorProperty, scrollBinding);*/

			Handle = ((HwndSource) PresentationSource.FromVisual(this)).Handle;

			Current = GetWindow(Handle, GetWindowCmd.First);

			do
			{
				IntPtr style = GetWindowLongPtr64(new HandleRef(null, Current), WinApi.GwlStyle);
				IntPtr exStyle = GetWindowLongPtr64(new HandleRef(null, Current), WinApi.GwlExstyle);
				if (Convert.ToBoolean(0x10000000 & style.ToInt64()) & Convert.ToBoolean(exStyle.ToInt64() | WinApi.WsExToolwindow))
					Thumbs.Add(Current);

				Current = GetWindow(Current, GetWindowCmd.Next);
			} while (Current != IntPtr.Zero);


			foreach (IntPtr thumb in Thumbs)
			{
				IconButton b = GetIconButton(thumb);
				if (b != null) TaskBand.Children.Add(b);
			}

			_activeWindowTimer.Elapsed += delegate
			{
				Dispatcher.Invoke(new Action(() =>
				{
					IntPtr active = WinApi.GetForegroundWindow();
					for (var i = 0; i < TaskBand.Children.Count; i++)
					{
						var t = TaskBand.Children[i] as IconButton;
						IntPtr prwnd = (t.Tag as ProgramWindow).Hwnd;
						try
						{
							if (prwnd == active)
							{
								if (t.IsEnabled) t.BringIntoView();
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
					MinutesHandTransform.Angle = (double) DateTime.Now.Minute / 60 * 360;
					HoursHandTransform.Angle = (double) DateTime.Now.Hour / 12 * 360;

					if (DateTime.Now.Hour <= 12)
						ClockHours.Text = DateTime.Now.Hour.ToString();
					else
						ClockHours.Text = (DateTime.Now.Hour - 12).ToString();
					if (DateTime.Now.Minute < 10)
						ClockMinutes.Text = "0" + DateTime.Now.Minute;
					else
						ClockMinutes.Text = DateTime.Now.Minute.ToString();
					ClockAmOrPm.Text = DateTime.Now.ToString("tt", CultureInfo.InvariantCulture).ToLower();
				}));
			};
			_clockTimer.Start();
		}


		public IconButton GetIconButton(IntPtr hWnd)
		{
			//TaskBand.Children.Add(taskItemButton);
			var p = new ProgramWindow(hWnd);
			IconButton taskItemButton;
			if (!string.IsNullOrWhiteSpace(p.Name) & (hWnd != new WindowInteropHelper(this).Handle))
			{
				taskItemButton = new IconButton
				{
					Content = p.Name,
					Style = (Style) Resources["TaskItemButtonStyle"],
					Tag = p
				};
				try
				{
					taskItemButton.Icon = new Canvas
					{
						Width = 16,
						Height = 16,
						Background = new ImageBrush(GetIconFromProgramWindowWithoutGoingThroughCoreDllBecauseWeSupportWindowsNotTen(p)
							.ToBitmapSource())
					};
				}
				catch
				{
					Debug.WriteLine("ICON FAILED");
				}

				;
				taskItemButton.Click += TaskItemButton_Click;
				return taskItemButton;
			}

			return null;
		}

		private Icon GetIconFromProgramWindowWithoutGoingThroughCoreDllBecauseWeSupportWindowsNotTen(ProgramWindow p)
		{
			IntPtr iconHandle = SendMessage(p.Hwnd, WmGeticon, IconSmall2, 0);

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
			var programWindow = (sender as IconButton).Tag as ProgramWindow;
			programWindow.Show();
		}

		private void ScrollViewer_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			double finalValue = TaskBandScrollViewer.HorizontalOffset - e.Delta * 2;
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
				TaskBandScrollViewer.ScrollToHorizontalOffset(TaskBandScrollViewer.HorizontalOffset - 134);
			else
				TaskBandScrollViewer.ScrollToHorizontalOffset(TaskBandScrollViewer.HorizontalOffset + 134);
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

		[StructLayout(LayoutKind.Sequential)]
		private struct DwmBlurbehind
		{
			public DwmBb dwFlags;
			public bool fEnable;
			public IntPtr hRgnBlur;
			public bool fTransitionOnMaximized;

			public DwmBlurbehind(bool enabled)
			{
				fEnable = enabled ? true : false;
				hRgnBlur = IntPtr.Zero;
				fTransitionOnMaximized = false;
				dwFlags = DwmBb.Enable;
			}

			public Region Region => Region.FromHrgn(hRgnBlur);

			public bool TransitionOnMaximized
			{
				get => fTransitionOnMaximized;
				set
				{
					fTransitionOnMaximized = value ? true : false;
					dwFlags |= DwmBb.TransitionMaximized;
				}
			}

			public void SetRegion(Graphics graphics, Region region)
			{
				hRgnBlur = region.GetHrgn(graphics);
				dwFlags |= DwmBb.BlurRegion;
			}
		}

		[Flags]
		private enum DwmBb
		{
			Enable = 1,
			BlurRegion = 2,
			TransitionMaximized = 4
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct Rect
		{
			public int Left;
			public int Top; // top is before right in the native struct
			public int Right;
			public int Bottom;
		}
	}
}