using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Abituria.viewmodel
{
    [ImplementPropertyChanged]
    public class BaseViewModel : INotifyPropertyChanged///Bazowy model widoku odpala Property Changed kiedy trzeba
    {
        protected object mPropertyValueCheckLock = new object();///Globalna blokada wyrażeń instancji
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };///Wydarzenie odpala się, kiedy właściwość potomka się zmienia
        protected void OnPropertyChanged(string name)///Wywołuje wydarzenie PropertyChanged
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}