using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class CalculationRequest
    {
        public int Number1 { get; set; }
        public int Number2 { get; set; }
        public OperationType Operation { get; set; }
        
        public CalculationRequest()
        {
                
        }

        public CalculationRequest(int number1, int number2, OperationType operation)
        {
            Number1 = number1;
            Number2 = number2;
            Operation = operation;
        }

        public override string ToString()
        {
            return Number1 + " " + (Operation == OperationType.Add ? "+" : "-") + " " + Number2;
        }
    }
}
