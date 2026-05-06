using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingModelLibrary.Exceptions
{
    public class InvalidContactDetailException : Exception
    {
        private string _message;
        public InvalidContactDetailException()
        {
            _message = "Since the contact details was not valid not creating the account. Your account is not created!!!!";
        }
        public InvalidContactDetailException(string contactDetails)
        {
            _message = "Unable to create account due to incorrect details of " + contactDetails;
        }

        public override string Message => _message;
    }
}
