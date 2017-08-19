using DandDAdventures.XAML.Listeners;
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

namespace DandDAdventures.XAML.Controls
{
    /// <summary>
    /// Logique d'interaction pour TabItem.xaml
    /// </summary>
    public partial class PJTabItemCtrl : UserControl
    {
        private IActionListener m_addListener = null;

        public PJTabItemCtrl() : base()
        {
            InitializeComponent();
        }

        public void SetAddListener(IActionListener l)
        {
            m_addListener = l;
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            m_addListener?.OnFire();
        }
    }
}
