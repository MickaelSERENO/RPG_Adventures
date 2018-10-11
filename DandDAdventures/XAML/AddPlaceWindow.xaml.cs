using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DandDAdventures.XAML
{
    /// <summary>
    /// Logique d'interaction pour AddPlaceWindow.xaml
    /// </summary>
    public partial class AddPlaceWindow : Window
    {
        /// <summary>
        /// The Application Data
        /// </summary>
        protected WindowData m_wd;

        /// <summary>
        /// The Place Image
        /// </summary>
        protected Image      m_placeImage;

        public AddPlaceWindow(WindowData wd)
        {
            m_wd             = wd;
            this.DataContext = new AddPlaceDatas(wd);

            InitializeComponent();
            m_placeImage = (Image)FindName("PlaceImageIcon");
        }

        /// <summary>
        /// Function called when the OK button is pressed. Validate the place
        /// </summary>
        /// <param name="sender">The sender (the button OK)</param>
        /// <param name="e">The parameter associated</param>
        private void AddPlaceClick(object sender, RoutedEventArgs e)
        {
            AddPlaceDatas datas = DataContext as AddPlaceDatas;
            String iconResKey = "";
            if(datas.IconDefined)
            {
                iconResKey = datas.PlaceIconKey;
                m_wd.SQLDatabase.AddResource(iconResKey);
            }

            m_wd.SQLDatabase.AddPlace(new Place { Name = datas.Name, Icon = iconResKey, Story = datas.PlaceStory });
            datas.PlaceAdded = true;
            Close();
        }

        /// <summary>
        /// Function called when the user wants to change the place icon (by clicking on the Image Button)
        /// </summary>
        /// <param name="sender">The sender (Button)</param>
        /// <param name="e">The event associated</param>
        private void ChangePlaceIcon(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = "Sélectionner la carte du lieu courant";
            if (openFile.ShowDialog() == true)
                (DataContext as AddPlaceDatas).IconPath = openFile.FileName;

            m_placeImage.Stretch = Stretch.Uniform;
        }
    }

    /// <summary>
    /// DataContext for the AddPlaceWindow window
    /// </summary>
    public class AddPlaceDatas : INotifyPropertyChanged
    {
        protected    WindowData                  m_wd;
        public event PropertyChangedEventHandler PropertyChanged;
        public       String                      m_iconPath; 
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="wd">The Main application data</param>
        public AddPlaceDatas(WindowData wd)
        {
            m_wd     = wd;
            IconPath = System.AppDomain.CurrentDomain.BaseDirectory + "/Resources/DefaultCaracterIcon.png";
        }

        /// <summary>
        /// Function to use to specify that a property has changed
        /// </summary>
        /// <param name="name">The name of the property</param>
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// Is the place added ?
        /// </summary>
        public bool   PlaceAdded { get; set; } = false;

        /// <summary>
        /// The Place Name associated in the XAML
        /// </summary>
        public String Name { get; set; } = "";

        /// <summary>
        /// The Story associated in the XAML
        /// </summary>
        public String PlaceStory { get; set; } = "";

        /// <summary>
        /// The IconPath for this place
        /// </summary>
        public String IconPath { get => m_iconPath; set { m_iconPath = value; OnPropertyChanged("IconPath"); IconDefined = true; } }

        /// <summary>
        /// Is the icon defined ?
        /// </summary>
        public bool   IconDefined { get; set; } = false;

        /// <summary>
        /// The Parent place Image
        /// </summary>
        public BitmapImage ParentImage { get; set; } = null;

        /// <summary>
        /// The place icon generated
        /// </summary>
        public String PlaceIconKey { get => Name + "_Icon"; }

        /// <summary>
        /// The Places known by the application. Quick access to another List in the application
        /// </summary>
        public ObservableCollection<Place> Places { get => m_wd.PlaceDatas.PlaceList; }
    }
}