using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizApp.ViewModel.HelpModels
{
    public class ListModelAndInfo<T> where T : class
    {
        public List<T> TransferModel {get; set; }
        public string Guid { get; set; }
        public string HelpGuid { get; set; }

    }
}