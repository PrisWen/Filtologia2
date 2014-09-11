using Fitologia.Common;
using Fitologia.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Fitologia.ViewModel
{
    public class PageVM : Notification
    {
        private DataTemas _tema;
        public DataTemas Tema
        {
            get
            {
                if (_tema == null)
                {
                    _tema = new DataTemas("", "", "", "");
                }
                return _tema;
            }
            set
            {
                _tema = value;
                RaisePropertyChanged();
            }
        }

        private List<DataImage> _images;
        public List<DataImage> Images
        {
            get
            {
                if (_images == null)
                {
                    _images = new List<DataImage>();
                }
                return _images;
            }
            set
            {
                _images = value;
                RaisePropertyChanged();
            }
        }

        DataSource service;

        public PageVM()
        {
            service = new DataSource();
            service.GetDataAsync();
            service.GetTemaCompleted += (s, e) =>
            {
                Tema = service.GetTemasAsync("Tema 01");
                Images = service.GetImageAsync();
            };
        }
    }
}
