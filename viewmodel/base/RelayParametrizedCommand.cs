using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PropertyChanged;
namespace Abituria.viewmodel
{
    class RelayParametrizedCommand : ICommand///Podstawowa komenda odpalacjąca Akcje
    {
        private readonly Action<object> mAction;///Akcja do odpalenia
        private Func<object, bool> mCanExecute;
        public event EventHandler CanExecuteChanged = (sender, e) => { };///Wydarzenie odpalane, kiedy wartość obiektu jest zmieniona
        public RelayParametrizedCommand(Action<object> action)///Standardowy konstruktor
        {
            mAction = action;
        }
        public bool CanExecute(object parameter)/// Polecenie przekaźnika zawsze może zostać wykonane
        {
            return true;
        }
        public void Execute(object parameter)/// Wykonuje komende Akcja
        {
            mAction(parameter);
        }
    }
}