using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnderstandingTypesApp
{
    internal class TypeExamples
    {
        internal void ShowingConvertions()
        {
            int iNum1 = 100;
            float fNum2 = 123.6f;
            string strNum3 = null;
            fNum2 = iNum1; //implict type casting
            Console.WriteLine($"The value of fNum2 is {fNum2}");
            //iNum1 = (int)fNum2; //Explicit type casting
            iNum1 = Convert.ToInt32(Math.Round(fNum2)); //Explicit type casting
            Console.WriteLine("Please enter a number");
            iNum1 = Convert.ToInt32(Console.ReadLine());//Unboxing
            strNum3 = iNum1.ToString();//Boxing
            Console.WriteLine("The value of  num1 is "+iNum1);
        }
        internal void ShowingLimits()
        {
            checked
            {
                int num1 = int.MaxValue;
                Console.WriteLine($"The value of num1 before change is {num1}");
                num1++;
                Console.WriteLine($"The value of num1 after change is {num1}");
            }
        }
        internal void HandlingNulls()
        {
            int? num1 = null;
            int num2 = 100;
            //int sum = num1??0 + num2;//if num1 is null use 0 there
            //Console.WriteLine($"The sum of {num1} and {num2} is {sum}");
            //string strNum1 = "30";
            ////num2 = Convert.ToInt32(strNum1);//Handles null
            ////num2 = Int32.Parse(strNum1);

            ////It will try to convert strNum1 to integer. if successful then put value and return true
            ////if failed put base value and return false
            //if (Int32.TryParse(strNum1, out num2))
            //    Console.WriteLine("Conversion success!!!");
            //Console.WriteLine($"The value {strNum1} converted to int is {num2}");

            //Insist the user for proper input
            Console.WriteLine("Please enter a number");
            while(Int32.TryParse(Console.ReadLine(), out num2) == false)
                Console.WriteLine("Invalid entry. Please try again");
        }
    }
}
