using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace TeachersToolbox.App.Views;

public sealed partial class TimerPage : Page
{
    private System.Threading.Timer? _timer;
    private TimeSpan _elapsed = TimeSpan.Zero;
    private TimeSpan _targetTime = TimeSpan.Zero;
    private bool _isRunning = false;

    public TimerPage()
    {
        this.InitializeComponent();
    }

    private void Timer_Tick(object? state)
    {
        if (!_isRunning) return;

        DispatcherQueue.TryEnqueue(() =>
        {
            if (CountdownToggle.IsOn)
            {
                _targetTime = _targetTime.Add(TimeSpan.FromMilliseconds(-100));
                if (_targetTime <= TimeSpan.Zero)
                {
                    _targetTime = TimeSpan.Zero;
                    StopTimer();
                }
                TimerText.Text = _targetTime.ToString(@"mm\:ss\:ff");
            }
            else
            {
                _elapsed = _elapsed.Add(TimeSpan.FromMilliseconds(100));
                TimerText.Text = _elapsed.ToString(@"hh\:mm\:ss");
            }
        });
    }

    private void SetTime_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && int.TryParse(button.Tag?.ToString(), out int seconds))
        {
            _targetTime = TimeSpan.FromSeconds(seconds);
            TimerText.Text = _targetTime.ToString(@"mm\:ss\:ff");
        }
    }

    private void SetCustomTime_Click(object sender, RoutedEventArgs e)
    {
        int.TryParse(MinutesBox.Text, out int minutes);
        int.TryParse(SecondsBox.Text, out int seconds);
        _targetTime = TimeSpan.FromMinutes(minutes).Add(TimeSpan.FromSeconds(seconds));
        TimerText.Text = _targetTime.ToString(@"mm\:ss\:ff");
    }

    private void StartButton_Click(object sender, RoutedEventArgs e)
    {
        StartTimer();
    }

    private void PauseButton_Click(object sender, RoutedEventArgs e)
    {
        StopTimer();
    }

    private void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        StopTimer();
        _elapsed = TimeSpan.Zero;
        _targetTime = TimeSpan.Zero;
        TimerText.Text = "00:00:00";
    }

    private void StartTimer()
    {
        _isRunning = true;
        _timer = new System.Threading.Timer(Timer_Tick, null, 0, 100);
        StartButton.IsEnabled = false;
        PauseButton.IsEnabled = true;
    }

    private void StopTimer()
    {
        _isRunning = false;
        _timer?.Dispose();
        _timer = null;
        StartButton.IsEnabled = true;
        PauseButton.IsEnabled = false;
    }
}
