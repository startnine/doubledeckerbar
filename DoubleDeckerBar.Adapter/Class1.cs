using DoubleDeckerBar.View;
using Start9.Api.Contracts;
using System;
using System.AddIn.Pipeline;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoubleDeckerBar.Adapter
{
    [AddInAdapter]
    public class CalculatorViewToContractAddInSideAdapter : ContractBase, ICalc1Contract
    {
        private ICalculator _view;

        public CalculatorViewToContractAddInSideAdapter(ICalculator view)
        {
            _view = view;
        }

        public virtual Double Add(Double a, Double b)
        {
            return _view.Add(a, b);
        }

        public virtual Double Subtract(Double a, Double b)
        {
            return _view.Subtract(a, b);
        }

        public virtual Double Multiply(Double a, Double b)
        {
            return _view.Multiply(a, b);
        }

        public virtual Double Divide(Double a, Double b)
        {
            return _view.Divide(a, b);
        }
    }
}
