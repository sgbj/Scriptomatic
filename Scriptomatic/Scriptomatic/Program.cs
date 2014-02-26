using Castle.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestStack.White;
using TestStack.White.Configuration;
using TestStack.White.UIItems.WindowItems;
using V8.Net;
using System.Windows.Automation;

namespace Scriptomatic
{
    class Program
    {
        static void Main(string[] args)
        {
            CoreAppXmlConfiguration.Instance.LoggerFactory = new NullLogFactory();
            Script.RunFile("coffee/app.coffee");
        }
    }
}
