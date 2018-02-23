using System;
using System.AddIn.Pipeline;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoubleDeckerBar.AddInView
{
	[AddInBase]
	public interface ICalculator
	{
		double Add(double a, double b);
		double Subtract(double a, double b);
		double Multiply(double a, double b);
		double Divide(double a, double b);
	}
}
