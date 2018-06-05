using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuizApp.Annotations
{
    public class MyTimeAttribute: ValidationAttribute
    {
       public override bool IsValid(object value)
        {
            if (value != null)
            {
                string[] FieldTime =  value.ToString().Split(':',' ','.');
                if (Convert.ToInt32(FieldTime[2]) > 59) return false;
                if (Convert.ToInt32(FieldTime[1]) > 59) return false;
                if (Convert.ToInt32(FieldTime[0]) > 24) return false;
            }
            return true;
        }
    }
}