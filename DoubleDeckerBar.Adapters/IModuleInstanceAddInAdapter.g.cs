//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoubleDeckerBar.Adapters
{
    
    public class IModuleInstanceAddInAdapter
    {
        internal static DoubleDeckerBar.Views.IModuleInstance ContractToViewAdapter(Start9.Api.Contracts.IModuleInstanceContract contract)
        {
            if ((contract == null))
            {
                return null;
            }
            if (((System.Runtime.Remoting.RemotingServices.IsObjectOutOfAppDomain(contract) != true) 
                        && contract.GetType().Equals(typeof(IModuleInstanceViewToContractAddInAdapter))))
            {
                return ((IModuleInstanceViewToContractAddInAdapter)(contract)).GetSourceView();
            }
            else
            {
                return new IModuleInstanceContractToViewAddInAdapter(contract);
            }
        }
        internal static Start9.Api.Contracts.IModuleInstanceContract ViewToContractAdapter(DoubleDeckerBar.Views.IModuleInstance view)
        {
            if ((view == null))
            {
                return null;
            }
            if (view.GetType().Equals(typeof(IModuleInstanceContractToViewAddInAdapter)))
            {
                return ((IModuleInstanceContractToViewAddInAdapter)(view)).GetSourceContract();
            }
            else
            {
                return new IModuleInstanceViewToContractAddInAdapter(view);
            }
        }
    }
}

