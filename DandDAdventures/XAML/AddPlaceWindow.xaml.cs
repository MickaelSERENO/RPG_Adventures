using System;
using System.Collections.Generic;
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
        protected WindowData m_wd;

        public AddPlaceWindow(WindowData wd)
        {
            m_wd = wd;
            this.DataContext = new AddPlaceDatas(wd);
            InitializeComponent();
        }

        private void AddPlaceClick(object sender, RoutedEventArgs e)
        {
            AddPlaceDatas datas = DataContext as AddPlaceDatas;

            m_wd.SQLDatabase.AddPlace(new Place { Name = datas.Name, Story = datas.PlaceStory });
            datas.PlaceAdded = true;
            Close();
        }
    }

    public class AddPlaceDatas : INotifyPropertyChanged
    {
        protected WindowData m_wd;
        public event PropertyChangedEventHandler PropertyChanged;

        public AddPlaceDatas(WindowData wd)
        {
            m_wd = wd;
        }

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public bool   PlaceAdded { get; set; } = false;
        public String Name { get; set; }
        public String PlaceStory { get; set; }
    }
}