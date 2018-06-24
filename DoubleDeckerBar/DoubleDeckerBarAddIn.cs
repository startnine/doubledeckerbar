using DoubleDeckerBar.Views;
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
    [AddIn("Double Decker Bar", Description = "Double-decker taskbar, created from concepts", Version = "1.0.0.0", Publisher = "Start9")]
    public class DoubleDeckerBarAddIn : IModule
    {
        public static DoubleDeckerBarAddIn Instance { get; private set; }

        public IConfiguration Configuration { get; } = new DoubleDeckerBarConfiguration();

        public IMessageContract MessageContract { get; } = new DoubleDeckerBarMessageContract();

        public IReceiverContract ReceiverContract => null;

        public void Initialize(IHost host)
        {
            void Start()
            {
                Application.ResourceAssembly = Assembly.GetExecutingAssembly();
                App.Main();
                Instance = this;
            }

            var t = new Thread(Start);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }
    }

    public class DoubleDeckerBarMessageContract : IMessageContract
    {
        public IList<IMessageEntry> Entries => new[] { ButtonClickedEntry };

        public IMessageEntry ButtonClickedEntry { get; } = new MessageEntry(typeof(DBNull), "Start button clicked");
    }

    public class DoubleDeckerBarConfiguration : IConfiguration
    {
        public IList<IConfigurationEntry> Entries => new[] { new ConfigurationEntry(GroupItems, "Group Items") };

        public Boolean GroupItems { get; set; } = true;
    }
}
