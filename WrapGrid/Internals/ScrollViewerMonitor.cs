using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WrapGrid.EventArgs;

namespace WrapGrid.Internals
{
    public class ScrollViewerMonitor
    {

        public event EventHandler<ScrollChangedEventArgs> ScrollChanged;
        private ScrollViewer _scrollViewer;

        public void Register(ScrollViewer scrollViewer)
        {
            _scrollViewer = scrollViewer;
            _scrollViewer.Tag = this;

            RegisterVerticalOffsetWatcher(_scrollViewer);
        }

        protected void OnScrollChanged(ScrollChangedEventArgs args)
        {
            var handler = ScrollChanged;
            if (handler != null)
            {
                ScrollChanged(this, args);
            }
        }

        private static void OnListenerVerticalOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            var scrollViewer = d as ScrollViewer;
            var monitor = scrollViewer.Tag as ScrollViewerMonitor;

            if(monitor != null)
            {
                monitor.OnScrollChanged(new ScrollChangedEventArgs(scrollViewer.VerticalOffset, scrollViewer.ScrollableHeight, scrollViewer));
            }
        }

        private static void RegisterVerticalOffsetWatcher(ScrollViewer scrollViewer)
        {
            Binding binding = new Binding("VerticalOffset") { Source = scrollViewer };
            var prop = DependencyProperty.RegisterAttached(
            "ListenVerticalOffset", typeof(object),
            typeof(ScrollViewerMonitor),
            new PropertyMetadata(OnListenerVerticalOffsetPropertyChanged));
            scrollViewer.SetBinding(prop, binding);
        }

    }
}
