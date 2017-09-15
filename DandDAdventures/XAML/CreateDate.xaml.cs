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
using System.Windows.Shapes;

namespace DandDAdventures.XAML
{
    /// <summary>
    /// Logique d'interaction pour CreateDate.xaml
    /// </summary>
    public partial class CreateDate : Window
    {
        protected WindowData  m_wd;
        protected TextBox     m_summary;
        protected Character[] m_characters;
        protected bool        m_results;

        public CreateDate(WindowData wd, Character[] characters)
        {
            InitializeComponent();
            m_wd = wd;
            m_characters = characters;
            
            m_summary = (TextBox)FindName("DescriptionBox");
        }

        private void CancelBtnClick(object sender, RoutedEventArgs e)
        {
            m_results = false;
            Close();
        }

        private void AddBtnClick(object sender, RoutedEventArgs e)
        {
            m_results = true;
            Close();
        }

        public String Description { get; set; } = "";
        public bool IsAdded { get => m_results; }
    }
}
