//using DoubleDeckerBar.View;
using DoubleDeckerBar.View;
using Start9.Api.Contracts;
using System;
using System.AddIn;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace DoubleDeckerBar
{
    [AddIn("Calculator AddIn", Version = "1.0.0.0")]
    public class AddInCalcV1 : ICalculator
    {
        public Double Add(Double a, Double b)
        {
            return a + b;
        }

        public Double Subtract(Double a, Double b)
        {
            return a - b;
        }

        public Double Multiply(Double a, Double b)
        {
            return a * b;
        }

        public Double Divide(Double a, Double b)
        {
            return a / b;
        }
    }
}
