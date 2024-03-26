namespace Blazor.Virtualization.HyBirdSamples
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Resources.Add("services", Startup.Services);
            InitializeComponent();
        }
    }
}
