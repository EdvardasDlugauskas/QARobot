using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace QARobot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            var actorViewSource = (System.Windows.Data.CollectionViewSource)FindResource("ActorViewSource");
            // Load data by setting the CollectionViewSource.Source property:
            actorViewSource.Source = MainViewModel.SelectedActors;
            System.Windows.Data.CollectionViewSource filmViewSource = (System.Windows.Data.CollectionViewSource)FindResource("FilmViewSource");
            // Load data by setting the CollectionViewSource.Source property:
            filmViewSource.Source = MainViewModel.FilmsResult;
        }

        private void OnAddActorButton(object sender, RoutedEventArgs e)
        {
            MainViewModel.SelectedActors.Add(new Actor(NewActorTextBox.Text));
        }

        private void OnRemoveSelectedButton(object sender, RoutedEventArgs e)
        {
            var selection = ActorDataGrid.SelectedItems.Cast<Actor>().ToList();
            foreach (var actor in selection)
            {
                MainViewModel.SelectedActors.Remove(actor);
            }
        }

        private void OnSearchButton(object sender, RoutedEventArgs e)
        {
            MainViewModel.SearchActors();
        }
    }
}
