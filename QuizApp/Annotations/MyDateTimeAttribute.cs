using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data;


namespace QuizApp.Annotations
{
    public class MyDateTimeAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value != null)
            {
                string[] FieldTime = value.ToString().Split(':','.',' ');
                 List<int> inf = new List<int>(6);
                foreach (var info in FieldTime)
                {
                    inf.Add(Convert.ToInt32(info));
                }
                if (Convert.ToInt32(FieldTime[0]) > 31) return false;
                if (Convert.ToInt32(FieldTime[1]) > 12) return false;

                if (Convert.ToInt32(FieldTime[4]) > 59) return false;
                if (Convert.ToInt32(FieldTime[5]) > 59) return false;
                if (Convert.ToInt32(FieldTime[3]) > 24) return false;

                DateTime enterDateTime = new DateTime(inf[2],inf[1],inf[0],inf[3],inf[4],inf[5]);
                DateTime currentDateTime = DateTime.Now.AddHours(1);
                int result = DateTime.Compare(enterDateTime, currentDateTime);
                if (result >= 0) return true;
                return false;
                
            }
            return true;
        }
    }
}

