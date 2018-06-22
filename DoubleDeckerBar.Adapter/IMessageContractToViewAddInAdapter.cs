using DoubleDeckerBar.View;
using Start9.Api.Contracts;
using System.AddIn.Pipeline;

namespace DoubleDeckerBar.Adapter
{
    public class IMessageContractToViewAddInAdapter : IMessage
    {
        private IMessageContract _contract;
        private ContractHandle _handle;

        static IMessageContractToViewAddInAdapter()
        {
        }

        public IMessageContractToViewAddInAdapter(IMessageContract contract)
        {
            _contract = contract;
            _handle = new ContractHandle(contract);
        }

        public string Text
        {
            get
            {
                return _contract.Text;
            }
        }

        public object Object
        {
            get
            {
                return _contract.Object;
            }
        }

        internal IMessageContract GetSourceContract()
        {
            return _contract;
        }
    }
}

