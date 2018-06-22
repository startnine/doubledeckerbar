using System.AddIn.Pipeline;

namespace DoubleDeckerBar.View
{
    [AddInBase]
    public interface IModule
    {
        IMessage SendMessage(IMessage message);
    }
}

