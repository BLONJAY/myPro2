using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YSRA
{
    /// <summary>
    /// 可以把其他窗体应用程序嵌入此容器
    /// </summary>
    [ToolboxBitmap(typeof(AppContainer), "AppControl.bmp")]
    public partial class AppContainer : System.Windows.Forms.Panel
    {
        Action<object, EventArgs> appIdleAction = null;
        EventHandler appIdleEvent = null;

        public AppContainer(bool showEmbedResult = false)
        {
            InitializeComponent();
            this.ShowEmbedResult = showEmbedResult;
            appIdleAction = new Action<object, EventArgs>(Application_Idle);
            appIdleEvent = new EventHandler(appIdleAction);
        }

        public AppContainer(IContainer container, bool showEmbedResult = false)
        {
            container.Add(this);
            InitializeComponent();
            this.ShowEmbedResult = showEmbedResult;
            appIdleAction = new Action<object, EventArgs>(Application_Idle);
            appIdleEvent = new EventHandler(appIdleAction);
        }
        /// <summary>
        /// 将属性<code>AppFilename</code>指向的应用程序打开并嵌入此容器
        /// </summary>
        public void Start()
        {
            if (AppProcess != null)
            {
                Stop();
            }

            try
            {
                ProcessStartInfo info = new ProcessStartInfo(this.m_AppFilename);
                info.UseShellExecute = true;
                //info.UseShellExecute = false;
                //info.RedirectStandardOutput = true;
                //info.WindowStyle = ProcessWindowStyle.Minimized; ZZQ
                //info.WindowStyle = ProcessWindowStyle.Hidden;
                info.Arguments = "-gGUIITEMS_96"; //-h1984
                info.WorkingDirectory = Path.GetDirectoryName(this.m_AppFilename);
                //info.Verb = "runas";
                AppProcess = System.Diagnostics.Process.Start(info);
                // Wait for process to be created and enter idle condition
                AppProcess.WaitForInputIdle();
                //todo:下面这两句会引发 NullReferenceException 异常                
                //AppProcess.Exited += new EventHandler(AppProcess_Exited);
                //AppProcess.EnableRaisingEvents = true;
                Application.Idle += appIdleEvent;
            }
            catch (Exception ex)
            {
                /*
                 * int i = 0;
                int len = 100;
                IntPtr hwnd;
                do
                {
                    Thread.Sleep(2000);
                    hwnd = Win32API.FindWindow("Qt5QWindowIcon", null);  //"V-REP PRO EDU - "
                    StringBuilder WindowTitle = new StringBuilder(len);
                    int rlt = Win32API.GetWindowText(hwnd, WindowTitle, len);
                    if (hwnd != IntPtr.Zero)
                    {
                        if (String.Compare("V-REP PRO EDU - ", 0, WindowTitle.ToString(), 0, 15) !=0)
                            hwnd = IntPtr.Zero;
                    }
                    i++;
                }while (hwnd == IntPtr.Zero && i < 30);

                if (hwnd == IntPtr.Zero)
                {
                    MessageBox.Show(this, string.Format("{1}{0}{2}{0}{3}"
                       , Environment.NewLine
                        , "*" + ex.ToString()
                        , "*StackTrace:" + ex.StackTrace
                        , "*Source:" + ex.Source
                        ), "Failed to load app.");

                }
                else
                {
                    if (EmbedProcess(AppProcess, this))
                    {
                        Application.Idle -= appIdleEvent;
                    }
                    else if (AppProcess != null)
                    {
                        if (!AppProcess.HasExited)
                            AppProcess.Kill();
                        AppProcess = null;
                    }
                }*/
            }

            try
            {
                int i = 0;
                int len = 100;
                IntPtr hwnd;
                do
                {
                    Thread.Sleep(3000);
                    hwnd = Win32API.FindWindow("Qt5QWindowIcon", null);  //"V-REP PRO EDU - "
                    StringBuilder WindowTitle = new StringBuilder(len);
                    int rlt = Win32API.GetWindowText(hwnd, WindowTitle, len);
                    if (hwnd != IntPtr.Zero)
                    {
                        if (String.Compare("V-REP PLAYER - ", 0, WindowTitle.ToString(), 0, 12) != 0) 
                            hwnd = IntPtr.Zero;
                        //if (String.Compare("V-REP PRO EDU - ", 0, WindowTitle.ToString(), 0, 13) != 0)  //15
                        //    hwnd = IntPtr.Zero;
                    }
                    i++;
                } while (hwnd == IntPtr.Zero && i < 5); //5

                if (hwnd == IntPtr.Zero)
                {
                    MessageBox.Show(this, string.Format("{1}{0}{2}{0}"
                       , Environment.NewLine
                        , "* Error:"
                        , "*Source file: " + m_AppFilename
                        ), "Failed to load app.");

                }
                else
                {
                    if (EmbedProcess(AppProcess, this))
                    {
                        Application.Idle -= appIdleEvent;
                    }
                    else if (AppProcess != null)
                    {
                        if (!AppProcess.HasExited)
                            AppProcess.Kill();
                        AppProcess = null;
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(this, string.Format("{1}{0}{2}{0}{3}"
                   , Environment.NewLine
                    , "*" + ex.ToString()
                    , "*StackTrace:" + ex.StackTrace
                    , "*Source:" + ex.Source
                    ), "Failed to load app.");
            }

        }
        /// <summary>
        /// 确保应用程序嵌入此容器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Application_Idle(object sender, EventArgs e)
        {
            if (this.AppProcess == null || this.AppProcess.HasExited)
            {
                this.AppProcess = null;
                Application.Idle -= appIdleEvent;
                return;
            }
            if (AppProcess.MainWindowHandle == IntPtr.Zero) return;
            //Application.Idle -= appIdleEvent;
            if (EmbedProcess(AppProcess, this))
            { Application.Idle -= appIdleEvent; }
            //ShowWindow(AppProcess.MainWindowHandle, SW_SHOWNORMAL);
            //var parent = GetParent(AppProcess.MainWindowHandle);//不管用，全是0
            //if (parent == this.Handle)
            //{
            //    Application.Idle -= appIdleEvent;
            //}
        }
        /// <summary>
        /// 应用程序结束运行时要清除这里的标识
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AppProcess_Exited(object sender, EventArgs e)
        {
            AppProcess = null;
        }

        /// <summary>
        /// Close <code>AppFilename</code> 
        /// <para>将属性<code>AppFilename</code>指向的应用程序关闭</para>
        /// </summary>
        public void Stop()
        {
            if (AppProcess != null)// && AppProcess.MainWindowHandle != IntPtr.Zero)
            {
                try
                {
                    if (!AppProcess.HasExited)
                        AppProcess.Kill();
                }
                catch (Exception)
                {
                }
                AppProcess = null;
                embedResult = 0;
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            Stop();
            base.OnHandleDestroyed(e);
        }

        protected override void OnResize(EventArgs eventargs)
        {
            if (AppProcess != null)
            {
                Win32API.MoveWindow(AppProcess.MainWindowHandle, 0, 0, this.Width, this.Height, true);
            }

            base.OnResize(eventargs);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            this.Invalidate();
            base.OnSizeChanged(e);
        }

        #region 属性
        /// <summary>
        /// Embedded application's process
        /// </summary>
        public Process AppProcess { get; set; }

        /// <summary>
        /// Target app's file name(*.exe)
        /// </summary>
        private string m_AppFilename = "C:/Program Files (x86)/V-REP3/V-REP_PLAYER/vrep.exe";
        /// <summary>
        /// Target app's file name(*.exe)
        /// </summary>
        [Category("Data")]
        [Description("Target app's file name(*.exe)")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        //[Editor(typeof(AppFilenameEditor), typeof(UITypeEditor))]
        public string AppFilename
        {
            get
            {
                return m_AppFilename;
            }
            set
            {
                if (value == null || value == m_AppFilename) return;
                var self = Application.ExecutablePath;
                int exePathLen = value.ToLower().IndexOf(".exe");
                String exePath= value.Substring(0,exePathLen+4);
                String exeArguments = value.Substring(exePathLen, value.Length - exePathLen - 4);
                if (value.ToLower() == self.ToLower())
                {
                    MessageBox.Show("Please don't embed yourself！", "YS01 3D viewer");
                    return;
                }
                if (!exePath.ToLower().EndsWith(".exe"))
                {
                    MessageBox.Show("Target is not an *.exe！", "YS01 3D viewer");
                }
                if (!File.Exists(exePath))
                {
                    MessageBox.Show("V-REP.exe does not exist！Please reinstall it.", "YS01 3D viewer");
                    return;
                }
                m_AppFilename = value;
            }
        }
        /// <summary>
        /// 标识内嵌程序是否已经启动
        /// </summary>
        public bool IsStarted { get { return (this.AppProcess != null); } }

        #endregion 属性


        public void EmbedAgain()
        {
            EmbedProcess(AppProcess, this);
        }

        /// <summary>
        /// 如果函数成功，返回值为子窗口的原父窗口句柄；如果函数失败，返回值为NULL。若想获得多错误信息，请调用GetLastError函数。
        /// </summary>
        public int embedResult = 0;
        /// <summary>
        /// 将指定的程序嵌入指定的控件
        /// </summary>
        private bool EmbedProcess(Process app, Control control)
        {
            // Get the main handle
            if (app == null || app.MainWindowHandle == IntPtr.Zero || control == null) return false;

            embedResult = 0;

            try
            {
                // Put it into this container
                embedResult = Win32API.SetParent(app.MainWindowHandle, control.Handle);
            }
            catch (Exception)
            { }
            try
            {
                // Remove border and whatnot               
                Win32API.SetWindowLong(new HandleRef(this, app.MainWindowHandle), Win32API.GWL_STYLE, Win32API.WS_VISIBLE);
            }
            catch (Exception)
            { }
            try
            {
                // Move the window to overlay it on this window
                Win32API.MoveWindow(app.MainWindowHandle, 0, 0, control.Width, control.Height, true);
            }
            catch (Exception)
            { }

            if (ShowEmbedResult)
            {
                var errorString = Win32API.GetLastError();
                MessageBox.Show(errorString);
            }

            return (embedResult != 0);
        }

        /// <summary>
        /// Show a MessageBox to tell whether the embedding is successfully done.
        /// </summary>
        public bool ShowEmbedResult { get; set; }
    }
}
