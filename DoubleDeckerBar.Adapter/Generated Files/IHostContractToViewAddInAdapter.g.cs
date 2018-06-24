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
    
    public class IHostContractToViewAddInAdapter : DoubleDeckerBar.Views.IHost
    {
        private Start9.Api.Contracts.IHostContract _contract;
        private System.AddIn.Pipeline.ContractHandle _handle;
        static IHostContractToViewAddInAdapter()
        {
        }
        public IHostContractToViewAddInAdapter(Start9.Api.Contracts.IHostContract contract)
        {
            _contract = contract;
            _handle = new System.AddIn.Pipeline.ContractHandle(contract);
        }
        public void SendMessage(DoubleDeckerBar.Views.IMessage message)
        {
            _contract.SendMessage(DoubleDeckerBar.Adapters.IMessageAddInAdapter.ViewToContractAdapter(message));
        }
        public void SaveConfiguration(DoubleDeckerBar.Views.IModule module)
        {
            _contract.SaveConfiguration(DoubleDeckerBar.Adapters.IModuleAddInAdapter.ViewToContractAdapter(module));
        }
        public System.Collections.Generic.IList<DoubleDeckerBar.Views.IModule> GetModules()
        {
            return System.AddIn.Pipeline.CollectionAdapters.ToIList<Start9.Api.Contracts.IModuleContract, DoubleDeckerBar.Views.IModule>(_contract.GetModules(), DoubleDeckerBar.Adapters.IModuleAddInAdapter.ContractToViewAdapter, DoubleDeckerBar.Adapters.IModuleAddInAdapter.ViewToContractAdapter);
        }
        public DoubleDeckerBar.Views.IConfiguration GetGlobalConfiguration()
        {
            return DoubleDeckerBar.Adapters.IConfigurationAddInAdapter.ContractToViewAdapter(_contract.GetGlobalConfiguration());
        }
        internal Start9.Api.Contracts.IHostContract GetSourceContract()
        {
            return _contract;
        }
    }
}

