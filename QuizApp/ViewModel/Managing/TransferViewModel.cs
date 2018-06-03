using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizApp.ViewModel.Managing
{
    public class TransferViewModel<T>  where T : class
    {
        public readonly List<T> TransferModels;
        public readonly T TransferModel;

        public TransferViewModel(List<T> Model)
        {
            TransferModels = Model;
            TransferModel = null;
        }
        public TransferViewModel(T Model)
        {
            TransferModels = null;
            TransferModel = Model;
        }
    }
}