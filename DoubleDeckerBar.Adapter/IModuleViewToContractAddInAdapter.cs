using DoubleDeckerBar.View;
using Start9.Api.Contracts;
using System.AddIn.Pipeline;

namespace DoubleDeckerBar.Adapter
{
    [AddInAdapterAttribute]
    public class IModuleViewToContractAddInAdapter : ContractBase, IModuleContract
    {
        private IModule _view;

        public IModuleViewToContractAddInAdapter(IModule view)
        {
            _view = view;
        }

        public virtual IMessageContract SendMessage(IMessageContract message)
        {
            return IMessageAddInAdapter.ViewToContractAdapter(_view.SendMessage(IMessageAddInAdapter.ContractToViewAdapter(message)));
        }

        internal IModule GetSourceView()
        {
            return _view;
        }
    }
}

