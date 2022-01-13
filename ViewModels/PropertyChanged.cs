using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Marya.ViewModels
{
    public abstract class PropertyChangedAbs : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
