using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Abituria.pages
{
    public class BasePage : UserControl///Bazowa strona z podstawowymi funkcjonalnościami
    {
        private object mViewModel;/// The View Model associated with this page
        public object ViewModelObject /// The View Model associated with this page
        {
            get => mViewModel;
            set
            {
                if (mViewModel == value)///Jeśli nic się nie zmieniło, zwróć
                    return;
                mViewModel = value;///Zaktualizuj wartość
                OnViewModelChanged();///Odpal metode zmiany modelu
                DataContext = mViewModel;///Ustaw kontekst danych na tę strone
            }
        }
        public BasePage() ///Standardowy konstruktor
        {
        }
        protected virtual void OnViewModelChanged()///Odpalane, gdy zmienia sie model widoku
        {
        }
    }
}