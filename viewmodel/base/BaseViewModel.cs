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
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) =>///Obserwowalny obiekt
        {
            if (sender is null)
            {
                throw new System.ArgumentNullException(nameof(sender));
            }
            if (e is null)
            {
                throw new System.ArgumentNullException(nameof(e));
            }
        };///Wydarzenie odpala się, kiedy właściwość potomka się zmienia
        protected void OnPropertyChanged([CallerMemberName] string name = null)///Wywołuje wydarzenie PropertyChanged
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}