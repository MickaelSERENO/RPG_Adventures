using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;


namespace DandDAdventures
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected WindowData m_windowData;
        protected DBHandler  m_dbHandler;

        public MainWindow()
        {
            InitializeComponent();
            m_windowData = new WindowData();
            this.DataContext = m_windowData;
        }

        //Simple menu commands
        private void NewFile(object sender, RoutedEventArgs e)
        {
            if (m_dbHandler != null)
            {
                //TODO maybe something
            }

            m_dbHandler = new DBHandler("Data Source=:memory:");
            m_windowData.CanSave = true;
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            //TODO
        }

        private void SaveFile(object sender, RoutedEventArgs e)
        {

        }
    }

    public class WindowData : INotifyPropertyChanged
    {
        public bool m_canSave = false;
        public event PropertyChangedEventHandler PropertyChanged;

        public WindowData()
        {}

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public bool CanSave
        {
            get { return m_canSave; }
            set
            {
                m_canSave = value;
                OnPropertyChanged("CanSave");
            }
        }
    }
}
