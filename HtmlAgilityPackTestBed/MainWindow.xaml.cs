﻿using System.Windows;

namespace HtmlAgilityPackTestBed
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ((MainWindowViewModel)this.DataContext).ShowOpenUrlWindow = true;
        }
    }
}
