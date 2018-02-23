using System;
using System.AddIn;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DoubleDeckerBar.AddInView;

namespace DoubleDeckerBar
{
	[AddIn("Calculator AddIn", Version = "1.0.0.0")]
	public class AddInCalcV1 : ICalculator
	{
		public double Add(double a, double b)
		{
			return a + b;
		}

		public double Subtract(double a, double b)
		{
			return a - b;
		}

		public double Multiply(double a, double b)
		{
			return a * b;
		}

		public double Divide(double a, double b)
		{
			return a / b;
		}
	}

}
