using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ClientWPF
{
    public class ViewModel: INotifyPropertyChanged
    {
        Person md;
        public ViewModel() 
        {
            md = new Person();
        }

        private ObservableCollection<Person> persons;
        public ObservableCollection<Person> Persons { get => persons; set { persons = value; OnPropertyChanged(nameof(Persons)); } }
        private Person selectedPerson;
        public Person SelectedPerson
        {
            get { return selectedPerson; }
            set
            {
                selectedPerson = value;
                OnPropertyChanged(nameof(SelectedPerson));
            }
        }
        public Command GetAllData
        {
             get => new Command(obj =>
             {
                 Persons = md.ShowAll();
             });
        }
        public Command SaveData
        {
            get => new Command(obj =>
            {
                md.Save(Persons);
            });
        }
        public Command DeleteData
        {
            get => new Command(obj =>
            {
                md.DeleteSelected(SelectedPerson);
            });
        }
        public Command ClearDB
        {
            get => new Command(obj =>
            {
                md.DeleteAll(Persons);
            });
        }
        public Command AddData
        {
            get => new Command(obj =>
            {
                md.AddData(Persons);
            });
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}

