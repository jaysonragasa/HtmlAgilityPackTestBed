using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace HtmlAgilityPackTestBed
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region events

        #endregion

        #region vars
        string _htmlstring = string.Empty;
        HtmlDocument _htmlDoc = new HtmlDocument();
        HtmlNode _htmlNode;
        HtmlNodeCollection _htmlNodeCol;
        #endregion

        #region properties
        public string _url = string.Empty;
        public string Url
        {
            get { return _url; }
            set
            {
                _url = value;
                NotifyUI();
            }
        }

        private bool _showOpenUrlWindow = false;
        public bool ShowOpenUrlWindow
        {
            get { return _showOpenUrlWindow; }
            set
            {
                _showOpenUrlWindow = value;
                NotifyUI();
            }
        }

        private string _FooterText = "Ready";
        public string FooterText
        {
            get { return _FooterText; }
            set
            {
                _FooterText = value;
                NotifyUI();
            }
        }

        private bool _UseSingleNode = true;
        public bool UseSingleNode
        {
            get { return _UseSingleNode; }
            set
            {
                _UseSingleNode = value;
                NotifyUI();

                GetXPathResult();
            }
        }

        private bool _UseNodeCollection = false;
        public bool UseNodeCollection
        {
            get { return _UseNodeCollection; }
            set
            {
                _UseNodeCollection = value;
                NotifyUI();

                GetXPathResult();
            }
        }

        private bool _ResultText = true;
        public bool ResultText
        {
            get { return _ResultText; }
            set
            {
                _ResultText = value;
                NotifyUI();
                GetXPathResult();
            }
        }

        private bool _ResultHTML = false;
        public bool ResultHTML
        {
            get { return _ResultHTML; }
            set
            {
                _ResultHTML = value;
                NotifyUI();
                GetXPathResult();
            }
        }

        private string _XPath = string.Empty;
        public string XPath
        {
            get { return _XPath; }
            set
            {
                _XPath = value;
                NotifyUI(nameof(XPath));

                GetXPathResult();
            }
        }

        private string _XPathResult = string.Empty;
        public string XPathResult
        {
            get { return _XPathResult; }
            set
            {
                _XPathResult = value;
                NotifyUI();
            }
        }
        #endregion

        #region commands
        public ICommand Command_OpenUrl { get; set; }
        public ICommand Command_TextChanged { get; set; }
        public ICommand Command_New { get; set; }
        #endregion

        #region ctors
        public MainWindowViewModel()
        {
            InitCommands();
        }
        #endregion

        #region command methods
        void Command_OpenUrl_Click(string url)
        {
            if (!string.IsNullOrEmpty(this.Url) && !string.IsNullOrWhiteSpace(this.Url))
            {
                if (this.Url.ToLowerInvariant().StartsWith("http"))
                {
                    LoadHtmlFromUrl();
                }
                else // something else
                {
                    LoadHtmlString();
                }

                this.FooterText = $"HtmlDocument lodead with {this._htmlstring.Length}bytes of data";

                this.ShowOpenUrlWindow = false;
                this.Url = string.Empty;
            }
            else
            {
                MessageBox.Show("The text field cannot be empty");
            }
        }

        void Command_TextChanged_Click()
        {
            GetXPathResult();
        }

        void Command_New_Click()
        {
            var res = MessageBox.Show("Would you like to open a new instance of this window?", "New", MessageBoxButton.YesNo);
            if(res == MessageBoxResult.Yes)
            {
                using (Process p = new Process())
                {
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.UseShellExecute = true;
                    psi.FileName = Assembly.GetExecutingAssembly().Location;
                    p.StartInfo = psi;
                    p.Start();
                }
            }
            else
            {
                New();
            }
        }

        void New()
        {
            this.XPath = string.Empty;
            this.XPathResult = string.Empty;
            this.Url = string.Empty;
            this.ShowOpenUrlWindow = true;
        }
        #endregion

        #region methods
        void InitCommands()
        {
            if (Command_OpenUrl == null) Command_OpenUrl = new RelayCommand<string>(Command_OpenUrl_Click);
            if (Command_TextChanged == null) Command_TextChanged = new RelayCommand(Command_TextChanged_Click);
            if (Command_New == null) Command_New = new RelayCommand(Command_New_Click);
        }

        void LoadHtmlString()
        {
            HtmlNode.ElementsFlags.Remove("form");
            this._htmlstring = this.Url;
            this._htmlDoc.LoadHtml(this._htmlstring);
        }

        async void LoadHtmlFromUrl()
        {
            HtmlNode.ElementsFlags.Remove("form");
            this._htmlstring = await MonkeyWeb.Monkey.GetStringAsync(this.Url);
            this._htmlDoc.LoadHtml(this._htmlstring);
        }

        void GetXPathResult()
        {
            if (string.IsNullOrEmpty(this.XPath)) return;

            var xp = this.XPath;

            try
            {
                if (this.UseSingleNode)
                {
                    this._htmlNode = this._htmlDoc.DocumentNode.SelectSingleNode(this.XPath);
                    if (this._htmlNode != null)
                    {
                        this.XPathResult = this.ResultText ? this._htmlNode.InnerText : this._htmlNode.InnerHtml;
                        this.FooterText = $"Found {this._htmlNode.InnerText.Length} characters.";
                    }
                    else
                    {
                        this.FooterText = "Your XPath did not return any result.";
                    }
                }
                else
                {
                    this._htmlNodeCol = this._htmlDoc.DocumentNode.SelectNodes(this.XPath);
                    if(this._htmlNodeCol != null)
                    {
                        this.FooterText = $"Found {this._htmlNodeCol.Count} nodes.";
                        string result = string.Empty;

                        for (int i = 0; i < this._htmlNodeCol.Count; i++)
                        {
                            var cn = this._htmlNodeCol[i];

                            result += $"{(this.ResultText ? cn.InnerText : cn.InnerHtml)}\r\n------------------------------------------------\r\n";
                        }

                        this.XPathResult = result;
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                this.XPathResult = "Invalid Xpath";
                this.FooterText = "ERROR " + ex.Message;
            }
        }
        #endregion

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyUI([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}