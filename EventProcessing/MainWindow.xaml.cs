using System.Collections.ObjectModel;
using System.Reflection.Emit;
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
using System.Windows.Threading;

namespace EventSearching;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, IObserverGUI
{
    EventManager eventManager;

    private ObservableCollection<CustomEvent> tableEventData = new ObservableCollection<CustomEvent>();
    private ObservableCollection<EventAlert> tableAlertData = new ObservableCollection<EventAlert>();
    private DispatcherTimer timer;
    private TimeSpan elapsedTime;
    public MainWindow()
    {
        InitializeComponent();
        eventManager = new EventManager(this);
        AlertGridView.ItemsSource = tableAlertData;
        EventGridView.ItemsSource = tableEventData;
        
        timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromMilliseconds(1000);
        timer.Tick += Timer_Tick;

        elapsedTime = TimeSpan.Zero;
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        
        elapsedTime = elapsedTime.Add(TimeSpan.FromMilliseconds(1000) * TimeFactor.Factor);
        lblTimerVal.Content = elapsedTime.ToString(@"hh\:mm\:ss");
    }

    public void EventAdded(CustomEvent customEvent)
    {
        Dispatcher.Invoke(() => tableEventData.Add(customEvent));
        Dispatcher.Invoke(() => progressBarAdded.Value++);
    }

    public void EventProcessed(CustomEvent customEvent)
    {
        Dispatcher.Invoke(() => progressBarProcessed.Value++);
    }

    public void EventProcessingEnded()
    {
        timer.Stop();
        Dispatcher.Invoke(() => buttonStart.IsEnabled = true);
        
    }

    private void ButtonStart_Click(object sender, RoutedEventArgs e)
    {
        tableEventData.Clear();
        tableAlertData.Clear();
        progressBarAdded.Value = 0;
        progressBarProcessed.Value = 0;
        elapsedTime = TimeSpan.Zero;
        lblTimerVal.Content = elapsedTime.ToString(@"hh\:mm\:ss");
        cmbBoxTimeScale.IsEnabled = false;

        if (!timer.IsEnabled)
            timer.Start();

        eventManager.Start();
        buttonStart.IsEnabled = false;
    }

    public void EventAlert(EventAlert eventAlert)
    {
        Dispatcher.Invoke(() => tableAlertData.Add(eventAlert));
    }

    private void cmbBoxTimeScale_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        int selectedIndex = cmbBoxTimeScale.SelectedIndex;

        switch (selectedIndex)
        {
            case 0:
                TimeFactor.Factor = 1;
                break;
            case 1:
                TimeFactor.Factor = 10;
                break;
            case 2:
                TimeFactor.Factor = 100;
                break;
            default:
                break;
        }
    }
}
