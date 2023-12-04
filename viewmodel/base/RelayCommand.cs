using System;
using System.Windows.Input;
using PropertyChanged;
namespace Abituria.viewmodel
{
    class RelayCommand : ICommand///Podstawowa komenda odpalacjąca Akcje
    {
        private readonly Action mAction;///Akcja do odpalenia
        private Func<object, bool> mCanExecute;
        public event EventHandler CanExecuteChanged///Wydarzenie odpalane, kiedy wartość obiektu jest zmieniona
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public RelayCommand(Action action, Func<object, bool> canExecute = null)///Standardowy konstruktor
        {
            mAction = action;
            mCanExecute = canExecute;
        }
        public bool CanExecute(object parameter)/// Polecenie przekaźnika zawsze może zostać wykonane
        {
            return mCanExecute == null || mCanExecute(parameter);
        }
        public void Execute(object parameter)/// Wykonuje komende Akcja
        {
            mAction();
        }
    }
}