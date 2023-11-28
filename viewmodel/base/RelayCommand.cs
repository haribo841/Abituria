using System;
using System.Windows.Input;
using PropertyChanged;
namespace Abituria.viewmodel
{
    class RelayCommand : ICommand///Podstawowa komenda odpalacjąca Akcje
    {
        private readonly Action mAction;///Akcja do odpalenia
        public event EventHandler CanExecuteChanged = (sender, e) => { };///Wydarzenie odpalane, kiedy wartość <see cref="CanExecute(object)"/> jest zmieniona
        public RelayCommand(Action action)///Standardowy konstruktor
        {
            mAction = action;
        }
        public bool CanExecute(object parameter)/// Polecenie przekaźnika zawsze może zostać wykonane
        {
            return true;
        }
        public void Execute(object parameter)/// Wykonuje komende Akcja
        {
            mAction();
        }
    }
}