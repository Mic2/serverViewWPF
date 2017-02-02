using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ServerViewWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

#if DEBUG
            Debug_button.IsEnabled = true;
#else
            Debug_button.IsEnabled = false;
#endif

        }

        //bad way of doing it, but we can use it as a test :D
        private void Debug_button_Click(object sender, RoutedEventArgs e)
        {
            DebugConsole.Create();
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Warning! If you close the console you close the program!");
            Console.ResetColor();
        }
    }
}
