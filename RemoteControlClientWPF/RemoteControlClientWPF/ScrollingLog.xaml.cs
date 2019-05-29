using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace RemoteControlClientWPF
{
    /// <summary>
    /// Interaction logic for ScrollingLog.xaml
    /// </summary>
    public partial class ScrollingLog : UserControl
    {
        private bool AutoScroll { get; set; } = true;
        public ObservableCollection<string> LogLines { get; set; } = new ObservableCollection<string>();

        public ScrollingLog()
        {
            InitializeComponent();
            m_logList.DataContext = this;
        }

        public void Log(string text)
        {
            LogLines.Add(text);
        }

        private void ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // https://stackoverflow.com/questions/16743804/implementing-a-log-viewer-with-wpf

            // User scroll event : set or unset autoscroll mode
            if (e.ExtentHeightChange == 0)
            {   // Content unchanged : user scroll event
                if ((e.Source as ScrollViewer).VerticalOffset == (e.Source as ScrollViewer).ScrollableHeight)
                {   // Scroll bar is in bottom
                    // Set autoscroll mode
                    AutoScroll = true;
                }
                else
                {   // Scroll bar isn't in bottom
                    // Unset autoscroll mode
                    AutoScroll = false;
                }
            }

            // Content scroll event : autoscroll eventually
            if (AutoScroll && e.ExtentHeightChange != 0)
            {   // Content changed and autoscroll mode set
                // Autoscroll
                (e.Source as ScrollViewer).ScrollToVerticalOffset((e.Source as ScrollViewer).ExtentHeight);
            }
        }
    }
}
