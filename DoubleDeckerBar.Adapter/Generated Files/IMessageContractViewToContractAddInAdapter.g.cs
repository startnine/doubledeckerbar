//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoubleDeckerBar.Adapters
{
    
    public class IMessageContractViewToContractAddInAdapter : System.AddIn.Pipeline.ContractBase, Start9.Api.Contracts.IMessageContractContract
    {
        private DoubleDeckerBar.Views.IMessageContract _view;
        public IMessageContractViewToContractAddInAdapter(DoubleDeckerBar.Views.IMessageContract view)
        {
            _view = view;
        }
        public System.AddIn.Contract.IListContract<Start9.Api.Contracts.IMessageEntryContract> Entries
        {
            get
            {
                return System.AddIn.Pipeline.CollectionAdapters.ToIListContract<DoubleDeckerBar.Views.IMessageEntry, Start9.Api.Contracts.IMessageEntryContract>(_view.Entries, DoubleDeckerBar.Adapters.IMessageEntryAddInAdapter.ViewToContractAdapter, DoubleDeckerBar.Adapters.IMessageEntryAddInAdapter.ContractToViewAdapter);
            }
        }
        internal DoubleDeckerBar.Views.IMessageContract GetSourceView()
        {
            return _view;
        }
    }
}

