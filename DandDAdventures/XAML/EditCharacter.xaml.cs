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
    /// Logique d'interaction pour EditCharacter.xaml
    /// </summary>
    public partial class EditCharacter : Window
    {
        WindowData m_wd;
        public EditCharacter(WindowData wd, Character chara)
        {
            m_wd = wd;
            DataContext = new EditCharacterDatas(wd, chara);
            InitializeComponent();
        }

        private void OKClick(object sender, RoutedEventArgs e)
        {
            EditCharacterDatas datas = DataContext as EditCharacterDatas;
            m_wd.SQLDatabase.SetChara(datas.Chara);
            Close();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public class EditCharacterDatas : INotifyPropertyChanged
    {
        protected WindowData m_wd;
        protected Character  m_chara;

        public event PropertyChangedEventHandler PropertyChanged;

        public EditCharacterDatas(WindowData wd, Character chara)
        {
            m_chara = chara;
            m_wd    = wd;
        }

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public Character Chara { get => m_chara; set => m_chara = value; }
    }
}
