using Fitologia.DataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Fitologia.ViewModel
{
    public class MainVM : INotifyPropertyChanged
    {
        private ObservableCollection<DataTemas> _temas;

        public ObservableCollection<DataTemas> Temas
        {
            get { return _temas; }
            set
            {
                _temas = value;
                OnPropertyChanged();
            }
        }

        public MainVM()
        {
            GetTemas();
        }

        void GetTemas()
        {
            DataSource service = new DataSource();
            service.GetDataAsync();
            service.GetTemaCompleted += (s, e) =>
            {
                Temas = service.GetTemasAsync();
            };
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
