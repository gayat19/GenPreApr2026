using BankingBLLibrary.Interfaces;
using BankingBLLibrary.Services;
using BankingModelLibrary;
using BankingModelLibrary.Exceptions;

namespace BankingFEApplication
{
    internal class Program
    {
        // public delegate void MyDelegate(int n1, int n2);//Declare the type

        public delegate void MyDelegate<T,K>(T n1, K n2);
        
        Action<int,int> delegateRef;//refference for the type
        //MyDelegate<int, int> del;

        public void Add(int num1, int num2)//Method that could be delegated
        {
            var result = num1 + num2;
            Console.WriteLine($"The sum of {num1} and {num2} is {result}");
        }

        public void Product(int num1, int num2)//Method that could be delegated
        {
            var result = num1 * num2;
            Console.WriteLine($"The product of {num1} and {num2} is {result}");
        }

        public Program()//Constructore for instan
        {
            delegateRef = new Action<int,int>(Product);
            //delegateRef += delegate (int num1, int num2) //anon method
            //{
            //    var result = num1 + num2;
            //    Console.WriteLine($"The sum of {num1} and {num2} is {result}");
            //};

            delegateRef += (num1, num2)=> Console.WriteLine($"The sum of {num1} and {num2} is {(num1+num2)}");

            delegateRef -= Product;
        }

        void Calculate(Action<int,int> del) //takes functionality as parameter
        {
            del(100, 200);
        }
        static void Main(string[] args)
        {
            //Program program = new Program();
            //program.Calculate(program.delegateRef);

            List<Account> accounts = new List<Account>()
            {
                new SavingAccount{AccountNumber="12345",NameOnAccount="Ramu",Balance=12342.2f,DateOfBirth=new DateTime(2000,12,12),Email="Ramu@gmail.com",Phone="9876543210"},
                new CurrentAccount{AccountNumber="12346",NameOnAccount="Bamu",Balance=54321.5f,DateOfBirth=new DateTime(1999,12,12),Email="Bamu@gmail.com",Phone="4321098765"}
            };

            var givenAccountNumber = "12346";
            //Predicate<Account> checkAccountWithAccNum = (account) =>account.AccountNumber == givenAccountNumber;
            //var MyAccount = accounts.Find(checkAccountWithAccNum);
            var MyAccount = accounts.Find(a=>a.AccountNumber == givenAccountNumber);
            Console.WriteLine(MyAccount);
            Console.WriteLine("----------------------------------------------------");
            foreach (var item in accounts)
                Console.WriteLine(item);
            Console.WriteLine("----------------------------------------------------");
           // var sortedAccounts = from a in accounts  orderby a.NameOnAccount select a;
           

            var sortedAccounts = accounts.OrderBy(a => a.NameOnAccount);
            foreach(var item in  sortedAccounts)
                Console.WriteLine(item);
        }
    }
}
