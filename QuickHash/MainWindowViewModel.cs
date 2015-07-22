using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GongSolutions.Wpf.DragDrop;

namespace QuickHash
{
    public class MainWindowViewModel : IDropTarget, INotifyPropertyChanged
    {
        public DelegateCommand ClearCommand { get; private set; }

        private HashService hashService = new HashService();
        private Func<HashAlgorithm> _selectedAlgorithm;

        public ObservableCollection<HashItem> HashItems { get; private set; } = new ObservableCollection<HashItem>();

        private string title;

        public string Title
        {
            get
            {
                return title;
            }

            private set
            {
                title = value;
                OnPropertyChanged();
            }
        }

            public ObservableCollection<HashAlgorithmSelection> AvailableHashAlgorithms { get; private set;} = new ObservableCollection<HashAlgorithmSelection>
        {
            new HashAlgorithmSelection { Name = "MD5", ConstructorFunc = () => MD5.Create() },
            new HashAlgorithmSelection { Name = "SHA1", ConstructorFunc = () => SHA1.Create() },
            new HashAlgorithmSelection { Name = "SHA256", ConstructorFunc = () => SHA256.Create() },
            new HashAlgorithmSelection { Name = "SHA512", ConstructorFunc = () => SHA512.Create() }
        };

        public Func<HashAlgorithm> SelectedAlgorithm
        {
            get { return _selectedAlgorithm; }
            set
            {
                _selectedAlgorithm = value;
                OnPropertyChanged();
            }
        }

        public MainWindowViewModel()
        {
            ClearCommand = new DelegateCommand(ClearHashes, (o) => HashItems.Count > 0);

            HashItems.CollectionChanged += (a, b) => ClearCommand.RaiseCanExecuteChanged();

            var assembly = Assembly.GetEntryAssembly();
            var productVersion = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;
            Title = $"Quick Hash {productVersion}";
        }

        private void ClearHashes(object o)
        {
            HashItems.Clear();
        }

        public void DragOver(IDropInfo dropInfo)
        {
            CheckDrop(dropInfo);
        }

        public async void Drop(IDropInfo dropInfo)
        {
            CheckDrop(dropInfo);

            var dragFileList = ((DataObject)dropInfo.Data).GetFileDropList().Cast<string>();
            var files = dragFileList.Where(x => IsDirectory(x) && !HashItems.Any(hashItem => hashItem.File == x));

            foreach (var file in files)
            {
                var hashItem = new HashItem { File = file, Hash = string.Empty, ProgressValue = 0 };
                HashItems.Add(hashItem);
                hashItem.Hash = await hashService.CalculateHash(file, SelectedAlgorithm(), new Progress<int>((i) => hashItem.ProgressValue = i));
            }
        }

        private void CheckDrop(IDropInfo dropInfo)
        {
            var dragFileList = ((DataObject)dropInfo.Data).GetFileDropList().Cast<string>();
            dropInfo.Effects = dragFileList.Any(IsDirectory) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private bool IsDirectory(string path)
        {
            try
            {
                var attr = File.GetAttributes(path);

                return (attr & FileAttributes.Directory) != FileAttributes.Directory;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
