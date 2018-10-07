using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;

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
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = Description;

            if(openFile.ShowDialog() == true)
                Text = openFile.FileName;
        }
    }
}