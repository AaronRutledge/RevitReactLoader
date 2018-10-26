using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DotNetBrowser;
using DotNetBrowser.Events;
using DotNetBrowser.WinForms;

namespace RevitReactLoader
    public class UiBrowserState
    {
        public string State { get; set; }
        public string ProjectGuid { get; set; }
        private BrowserForm.Del CloseWindowCallBack;

        public UiBrowserState(BrowserForm.Del del, string initialState)
        {
            CloseWindowCallBack =del;
            ProjectGuid = "Something unique";
            State = initialState;

        }
        //public void UpdateState(string state)
        //{
        //    State = state;
        //}
        public bool CloseWindow()
        {
            CloseWindowCallBack();
            return true;
        }

    }

    public class BrowserForm : System.Windows.Forms.Form
    {
        public WinFormsBrowserView BrowserView;
        public UiBrowserState BrowserState;
        private Browser browser;
        public string StateObj;
        public JSValue jsStateValue;
        public delegate bool Del();

        //public string GetUiState()
        //{
        //    JSObject document = (JSObject)browser.ExecuteJavaScriptAndReturnValue("document.stairDesigner");
        //    return document.ToJSONString();
        //}

        public bool CloseWindow()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() =>
                {
                    this.Close();
                }));
            }
            else
            {
                this.Close();
                BrowserView.Dispose();
            }

            return true;
        }

        public BrowserForm(string url, int height = 800, int width = 1100, string initialState = "{}")
        {
            // InitializeComponent();

            this.Height = height;
            this.Width = width;
            browser = BrowserFactory.Create(BrowserType.LIGHTWEIGHT);
            
            BrowserView = new WinFormsBrowserView(browser);
            ManualResetEvent waitEvent = new ManualResetEvent(false);
            BrowserView.DocumentLoadedInMainFrameEvent += delegate (object sender, LoadEventArgs e)
            {
                waitEvent.Set();
            };

            BrowserView.Dock = DockStyle.Fill;
            Controls.Add(BrowserView);
            BrowserView.Browser.LoadURL(url);
            waitEvent.WaitOne();
            Del handler = CloseWindow;

            BrowserView.ScriptContextCreated += delegate (object sender, ScriptContextEventArgs e)
            {
                jsStateValue = browser.ExecuteJavaScriptAndReturnValue("window");
                jsStateValue.AsObject().SetProperty("UiBrowserState", new UiBrowserState(handler, initialState));

            };
            // Dispose Browser instance.
            //BrowserView.Dispose();

        }
    }
}
