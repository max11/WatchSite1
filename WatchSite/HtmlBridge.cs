using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WatchSite
{
    [ComVisible(true)]
    public class HtmlBridge
    {
        public void WebClick(string source)
        {
            MessageBox.Show("Received: " + source);
        }
    }
}
