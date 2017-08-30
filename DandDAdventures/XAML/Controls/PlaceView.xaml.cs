using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace DandDAdventures.XAML.Controls
{
    /// <summary>
    /// Logique d'interaction pour PlaceView.xaml
    /// </summary>
    public partial class PlaceView : UserControl
    {
        public PlaceView(WindowData wd)
        {
            InitializeComponent();
        }

        public void LoadContent(WindowData wd)
        {
        }
    }

    public class PlaceDataContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private      WindowData m_wd;
        protected    ObservableCollection<Place> m_placeList;
        protected Place m_placeSelected = null;
        public PlaceDataContext(WindowData wd)
        {
            m_wd = wd;
            m_placeList = new ObservableCollection<Place>();
        }

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ObservableCollection<Place> PlaceList { get => m_placeList; set => m_placeList = value; }
        public Place                       PlaceSelected
        {
            get
            {
                return m_placeSelected;
            }
            set
            {
                m_placeSelected = value;
                OnPropertyChanged("PlaceSelected");
            }
        }
    }
}