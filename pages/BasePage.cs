using Abituria.viewmodel;
using System.Windows.Controls;
namespace Abituria
{
    public class BasePage<VM> : Page where VM : BaseViewModel, new()///Bazowa strona z podstawowymi funkcjonalnościami
    {
        private VM mViewModel;/// The View Model associated with this page
        public object ViewModelObject /// The View Model associated with this page
        {
            get => mViewModel;
            set
            {
                if (mViewModel == value)///Jeśli nic się nie zmieniło, zwróć
                    return;
                mViewModel = (VM)value;///Zaktualizuj wartość
                OnViewModelChanged();///Odpal metode zmiany modelu
                DataContext = mViewModel;///Ustaw kontekst danych na tę strone
            }
        }
        public VM ViewModel///Model widoku powiązany z tą stroną
        {
            get { return mViewModel; }
            set
            {
                if (mViewModel == value)///Jeśli nic sie nie zmieniło, zwróc
                    return;
                mViewModel = value;///Zaktualizuj wartość
                this.DataContext = mViewModel;///Ustaw kontekst danych dla tej strony
            }
        }
        public BasePage() ///Standardowy konstruktor
        {
            this.Loaded += BasePage_Loaded;///Nasłuchuj załadowania strony
            this.ViewModel = new VM();///Stwórz standardowy model widoku
        }
        private void BasePage_Loaded(object sender, System.Windows.RoutedEventArgs e)///Odpal po załadowaniu strony
        {
            return;
        }
        protected virtual void OnViewModelChanged()///Odpalane, gdy zmienia sie model widoku
        {
        }
    }
}