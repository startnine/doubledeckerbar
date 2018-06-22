using DoubleDeckerBar.View;
using Start9.Api.Contracts;
using System.AddIn.Pipeline;

namespace DoubleDeckerBar.Adapter
{    
    public class IMessageViewToContractAddInAdapter : ContractBase, IMessageContract
    {
        private IMessage _view;
        public IMessageViewToContractAddInAdapter(IMessage view)
        {
            _view = view;
        }

        public string Text
        {
            get
            {
                return _view.Text;
            }
        }

        public object Object
        {
            get
            {
                return _view.Object;
            }
        }

        internal IMessage GetSourceView()
        {
            return _view;
        }
    }
}

