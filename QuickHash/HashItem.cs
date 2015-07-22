using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace QuickHash
{
    public class HashItem : INotifyPropertyChanged
    {
        private int progressValue;
        private string hash;
        private string file;

        public string File
        {
            get
            {
                return file;
            }
            set
            {
                if (value == file) return;
                file = value;
                OnPropertyChanged();
            }
        }

        public string Hash
        {
            get
            {
                return hash;
            }
            set
            {
                if (value == hash) return;
                hash = value;
                OnPropertyChanged();
            }
        }

        public int ProgressValue
        {
            get
            {
                return progressValue;
            }
            set
            {
                if (value == progressValue) return;
                progressValue = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}