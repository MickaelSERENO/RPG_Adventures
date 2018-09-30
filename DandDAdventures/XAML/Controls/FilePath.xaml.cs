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
    /// Logic of FilePath.xaml
    /// </summary>
    public partial class FilePath : UserControl
    {
        //////////////////////////////////////////
        ////ADD NEW PROPERTIES TO THIS CONTROL////
        //////////////////////////////////////////

        /// <summary>
        /// Add a Property "Text" representing the path selected
        /// Type : String
        /// </summary>
        public static DependencyProperty TextProperty        = DependencyProperty.Register("Text", typeof(string), typeof(FilePath),
                                                                                            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Add a Property "Description" representing the Description displayed in the browser dialog
        /// Type : String
        /// </summary>
        public static DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(FilePath),
                                                                                            new PropertyMetadata(null));

        /// <summary>
        /// The Text value (Path)
        /// </summary>
        public string Text { get { return GetValue(TextProperty) as string; } set { SetValue(TextProperty, value); } }

        /// <summary>
        /// The Description value
        /// </summary>
        public string Description { get { return GetValue(DescriptionProperty) as string; } set { SetValue(DescriptionProperty, value); } }


        public FilePath()
        {
            InitializeComponent();
        }

        private void BrowseFolder(object sender, RoutedEventArgs e)
        {
            String path = null;
            if(Utils.OpenFileDialog(Description, out path))
            {
                Text = path;
            }
        }
    }
}