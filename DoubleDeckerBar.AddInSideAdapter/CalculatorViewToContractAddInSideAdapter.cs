using System;
using System.AddIn.Pipeline;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DoubleDeckerBar.AddInView;
using Start9.Api.Contracts;

namespace DoubleDeckerBar.AddInSideAdapter
{
	[AddInAdapter]
	public class CalculatorViewToContractAddInSideAdapter :
		ContractBase, ICalculatorContract
	{
		private ICalculator _view;

		public CalculatorViewToContractAddInSideAdapter(ICalculator view)
		{
			_view = view;
		}

		public virtual double Add(double a, double b)
		{
			return _view.Add(a, b);
		}

		public virtual double Subtract(double a, double b)
		{
			return _view.Subtract(a, b);
		}

		public virtual double Multiply(double a, double b)
		{
			return _view.Multiply(a, b);
		}

		public virtual double Divide(double a, double b)
		{
			return _view.Divide(a, b);
		}
	}
}
