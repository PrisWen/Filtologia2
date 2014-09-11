namespace Fitologia.Common
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class Notification : INotifyPropertyChanged
    {
        public Notification()
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            var Handler = PropertyChanged;
            if (Handler != null)
                Handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
