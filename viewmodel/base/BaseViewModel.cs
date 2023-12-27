using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
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
        protected async Task RunCommand(Expression<Func<bool>> updatingFlag, Func<Task> action)///Odpala komende jeżeli flaga aktualizowania nie jest ustawiona
        {
            if (updatingFlag.GetPropertyValue())//.Compile().Invoke())
                return;
            updatingFlag.SetPropertyValue(true);
            try
            {
                await action();///Odpala akcje
            }
            finally
            {
                updatingFlag.SetPropertyValue(false);///Ustawia właściwość flagi na fałsz po skończeniu
            }
        }
    }
}