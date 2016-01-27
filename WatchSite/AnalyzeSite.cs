using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;
using System.Windows.Threading;
using mshtml;

namespace WatchSite
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [ComVisible(true)]
    public class AnalyzeSite
    {

        public int countReNew = 0;
        public AnalyzeSite()
        {
            
        }
        WebBrowser _webBrowser;
        private dynamic doc;
        private dynamic doc2;
        private string pageSource2;

        
        public void GoToPage(string url, WebBrowser webBrowser)
        {
            
            //System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();

            //timer.Tick += new EventHandler(timerTick);
            //timer.Interval = new TimeSpan(0, 0, 4);
            //timer.Start();
            _webBrowser = webBrowser;
            _webBrowser.Navigate(url);
            //_webBrowser.TargetUpdated += WebBrowserOnTargetUpdated;
            //_webBrowser.Navigating += new NavigatingCancelEventHandler(browser_Navigating);
            //_webBrowser.Navigated += new NavigatedEventHandler(browser_Navigated);
            webBrowser.LoadCompleted += WebBrowser1OnLoadCompleted;

        }

        private void WebBrowserOnTargetUpdated(object sender, DataTransferEventArgs dataTransferEventArgs)
        {
            if (countReNew < 2)
            {
                doc = (mshtml.HTMLDocument) _webBrowser.Document;
                pageSource2 = doc.documentElement.innerHTML;
            }
        }

        private void timerTick(object sender, EventArgs e)
        {
            

        }

        public void WebBrowser1OnLoadCompleted(object sender, NavigationEventArgs e)
        {

            mshtml.HTMLDocument doc;
            doc = (mshtml.HTMLDocument)_webBrowser.Document;
            mshtml.HTMLDocumentEvents2_Event iEvent;
            iEvent = (mshtml.HTMLDocumentEvents2_Event)doc;
            iEvent.onclick += new mshtml.HTMLDocumentEvents2_onclickEventHandler(ClickEventHandler);
            iEvent.onafterupdate += IEventOnOnafterupdate;
            iEvent.oncellchange += IEventOnOncellchange;
            int countIntHtml = 0;
            //doc = _webBrowser.Document;
            string pageSource = doc.documentElement.innerHTML;
            //<span class="market_summary_text">140 items</span>
            var tmp = new Regex(@"<span class=""market_summary_text"">([0-9]+?)[A-Za-zА-Яа-я\s]+?</span>", RegexOptions.IgnoreCase).Match(pageSource);
            if (tmp.Success && countReNew == 0)
            {
                var countHtml = tmp.Groups[0].Value;
                countHtml = Regex.Replace(countHtml, @"<.*?>", "");
                countHtml = Regex.Replace(countHtml, @"[A-Za-zА-Яа-я]+?", "");
                countIntHtml = int.Parse(countHtml);
                countReNew = countIntHtml; //%21 + 2;
            }

            
            //else countReNew = 22;
            //doc = _webBrowser.Document;
            //var elc = doc.GetElementById("show_more");
            //elc.InvokeMember("Click");

            //_webBrowser.ObjectForScripting = new HtmlBridge();
            //doc2 = (mshtml.HTMLDocument)_webBrowser.Document;
            //IHTMLElement el = doc2.body;
            //mshtml.HTMLElementEvents2_Event iEvent2;
            //if (el != null)
            //{
            //    iEvent2 = (mshtml.HTMLElementEvents2_Event)el;
            //    iEvent2.onafterupdate += IEvent2OnOnafterupdate;
            //}



            //for (int i = countReNew; i > -2; i--)
            // {
            //    var elc = doc.all.item("show_more");
            //    elc.Click();
            //    Thread.Sleep(500);
            //     countReNew--;
            // }

            //string pageSource3 = doc.documentElement.innerHTML;

        }

        private void IEventOnOncellchange(IHTMLEventObj pEvtObj)
        {
            pageSource2 = doc.documentElement.innerHTML;
        }

        private void IEventOnOnafterupdate(IHTMLEventObj pEvtObj)
        {
            string pageSource4 = doc.documentElement.innerHTML;
        }

      
        private bool ClickEventHandler(mshtml.IHTMLEventObj e)
        {
            if (countReNew < 2)
            {
                doc = (mshtml.HTMLDocument)_webBrowser.Document;
                pageSource2 = doc.documentElement.innerHTML;
                return true;
            }
                
            return false;
        }
    }
}
