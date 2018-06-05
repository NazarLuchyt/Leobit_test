using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuizApp.Annotations
{
    public class MyNumberAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value != null)
            {
                int result = Convert.ToInt32(value);
                if(result<0) return false;
            }
            return true;
        }
    }
}