using System;
using System.Windows.Forms;

namespace MultiTabsBrowserExample
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();

            Load += Main_Load;
            btnGo.Click += btnGo_Click;
            btnNext.Click += btnNext_Click;
            btnPrev.Click += btnPrev_Click;
            Tabs.SelectedIndexChanged += Tabs_SelectedIndexChanged;
            tsCloseCurrentTab.Click += tsCloseCurrentTab_Click;
            tsNewTab.Click += tsNewTab_Click;
            tsCloseOthers.Click += tsCloseOthers_Click;
            tsExit.Click += tsExit_Click;
            tsAbout.Click += tsAbout_Click;
        }

        void tsAbout_Click(object sender, EventArgs e)
        {
            new About().ShowDialog();
        }

        void tsExit_Click(object sender, EventArgs e)
        {
            var dialog = MessageBox.Show("Are you sure?","Confirm",MessageBoxButtons.OKCancel,MessageBoxIcon.Question);

            if (dialog == DialogResult.OK)
                Close();
        }

        void tsCloseOthers_Click(object sender, EventArgs e)
        {
            var selectedTab = Tabs.SelectedTab;

            foreach (TabPage tab in Tabs.TabPages)
            {
                if (tab.Tag != selectedTab.Tag)
                    Tabs.TabPages.Remove(tab);
            }
        }

        void tsNewTab_Click(object sender, EventArgs e)
        {
            NewTab();
        }

        void tsCloseCurrentTab_Click(object sender, EventArgs e)
        {
            CloseCurrentTab();
        }

        void Tabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            var browser = GetCurrentBrowser();
            var text = string.Empty;

            if (browser!=null)
            {
                if (browser.Url != null)
                {
                    text = browser.Url.ToString();
                    status.Text = browser.Url.Host;
                }                    
            }
            
            textUrl.Text = text;            
        }

        void btnNext_Click(object sender, EventArgs e)
        {
            var browser = GetCurrentBrowser();
            if (browser.CanGoForward)
                browser.GoForward();
        }

        void btnPrev_Click(object sender, EventArgs e)
        {
            var browser = GetCurrentBrowser();
            if (browser.CanGoBack)
                browser.GoBack();
        }

        void btnGo_Click(object sender, EventArgs e)
        {
            var url = FormatUrl(textUrl.Text.Trim());

            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("invalid url","Url Error",MessageBoxButtons.OK,MessageBoxIcon.Information);
                return;
            }

            BrowserNavigate(url);
        }

        void Main_Load(object sender, EventArgs e)
        {
            NewTab();
            status.Text = string.Empty;
        }

        void BrowserNavigate(string url)
        {
            var browser = GetCurrentBrowser();
            
            if (browser==null)
                return;

            try
            {
                browser.Navigate(url);
                status.Text = "Go to " + url;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Navigate Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        WebBrowser GetCurrentBrowser()
        {
            var selectedTab = Tabs.SelectedTab;
            if (selectedTab == null)
                return null;

            var browserName = "browser_" + selectedTab.Tag;

            if (selectedTab.Controls.ContainsKey(browserName))
            {
                return (WebBrowser)selectedTab.Controls[browserName];
            }

            return null;
        }

        void NewTab()
        {
            var tab = new TabPage();
            tab.Text = "about: blank";
            
            // add tab to tabcontrol
            Tabs.TabPages.Add(tab);
            Tabs.SelectedTab = tab;
            tab.Tag = Tabs.SelectedIndex;

            // add new web browser to tab
            var browser = new WebBrowser();
            browser.Parent = tab;
            browser.Dock = DockStyle.Fill;
            browser.Name = "browser_" + tab.Tag;
            browser.Navigating += browser_Navigating;
            browser.Navigated += browser_Navigated;
        }

        void CloseCurrentTab()
        {
            if (Tabs.TabPages.Count==1)
            {
                Tabs.TabPages.Clear();
                NewTab();                
            }
            else
            {
                Tabs.TabPages.RemoveAt(Tabs.SelectedIndex);
            }
            
        }

        string TruncateUrl(Uri url, int length=20)
        {
            if (url==null)
            {
                return string.Empty;
            }

            var surl = url.ToString();

            if (surl.Length <= length)
                return surl;

            return surl.Substring(0, length) + "...";
        }

        void browser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            var selectedTab = Tabs.SelectedTab;

            if (e.Url!=null)
            {
                selectedTab.Text = e.Url.Host;
            }
            //else
            //{
            //    selectedTab.Text = textUrl.Text.Trim();
            //}

            status.Text = "Done";
        }

        void browser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            var selectedTab = Tabs.SelectedTab;
            var url ="Loading "+ TruncateUrl(e.Url);

            // selectedTab.Text = url;
            status.Text = url;
        }

        string FormatUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return string.Empty;

            if (!url.Contains("http"))
                url = string.Format("http://{0}", url);

            return url;
        }
    }
}
