using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DotNetBrowser;
using DotNetBrowser.WinForms;

namespace RevitReactLoader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            BrowserView browserView = new WinFormsBrowserView();
            Control browserWindow = (Control)browserView;
            browserWindow.Dock = DockStyle.Fill;
            Controls.Add(browserWindow);
            browserView.Browser.FullScreenHandler = new SampleFullScreenHandler(this, browserView);
            BrowserForm browserForm = new BrowserForm(Path.Combine(addInFolder, "ui-build", @"index.html#/edit"), 800, 1100,{});
            browserView.Browser.LoadURL("http://www.google.com");


        }
    }

    internal class SampleFullScreenHandler : FullScreenHandler
    {
        private Form parent;
        private Form fullScreenForm;
        private BrowserView view;

        public SampleFullScreenHandler(Form form, BrowserView view)
        {
            this.parent = form;
            this.view = view;
        }

        public void OnFullScreenEnter()
        {
            parent.BeginInvoke((Action)(() =>
            {
                fullScreenForm = new Form();
                fullScreenForm.TopMost = true;
                fullScreenForm.ShowInTaskbar = false;
                fullScreenForm.FormBorderStyle = FormBorderStyle.None;
                fullScreenForm.WindowState = FormWindowState.Maximized;
                fullScreenForm.Owner = parent;

                fullScreenForm.Activated += delegate
                {
                    UpdateSize();
                };
                parent.Controls.Remove((Control)view);
                fullScreenForm.Controls.Add((Control)view);
                fullScreenForm.Show();
            }));
        }

        void UpdateSize()
        {
            if (fullScreenForm.Width == 0 && fullScreenForm.Height == 0)
            {
                return;
            }
            view.UpdateSize(fullScreenForm.Width, fullScreenForm.Height);
        }

        public void OnFullScreenExit()
        {
            parent.BeginInvoke((Action)(() =>
            {
                fullScreenForm.Controls.Clear();
                parent.Controls.Add((Control)view);
                fullScreenForm.Hide();
                fullScreenForm.Close();

            }));
        }
    }
}
