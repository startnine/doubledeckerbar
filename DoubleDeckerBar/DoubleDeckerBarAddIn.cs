using DoubleDeckerBar.View;
using System;
using System.AddIn;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DoubleDeckerBar
{
    [AddIn("Double Decker Bar", Version = "1.0.0.0")]
    public class DoubleDeckerBarAddIn : IModule
    {
        public IMessage SendMessage(IMessage message)
        {
            MessageBox.Show(message.Text + Environment.NewLine + message.Object.ToString());
            return Message.Empty;
        }

        public DoubleDeckerBarAddIn()
        {
            void Start()
            {
                Application.ResourceAssembly = Assembly.GetExecutingAssembly();
                App.Main();
            }

            var t = new Thread(Start);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }
    }
}
