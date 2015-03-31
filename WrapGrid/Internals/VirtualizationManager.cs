using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WrapGrid.Interfaces;

namespace WrapGrid.Internals
{
    public class VirtualizationManager
    {
        private readonly ScrollViewerMonitor scrollMonitor;

        public event Func<IEnumerable<Panel>> GetPanels;
        public VirtualizationManager(ScrollViewerMonitor scrollMonitor)
        {
            this.scrollMonitor = scrollMonitor;
            this.scrollMonitor.ScrollChanged += OnScrollChangedEventHandler;
        }

        private void OnScrollChangedEventHandler(object sender, EventArgs.ScrollChangedEventArgs e)
        {
            VirtualizeElements(e.Notifier);
        }

        protected IEnumerable<Panel> OnGetPanels()
        {
            IEnumerable<Panel> result = Enumerable.Empty<Panel>();
            var handler = GetPanels;

            if(GetPanels != null)
            {
                result = GetPanels();
            }

            return result;
        }

        private void VirtualizeElements(ScrollViewer notifier)
        {
            var panels = OnGetPanels();
            var rootVisual = Application.Current.RootVisual as FrameworkElement;

            if(panels != null && panels.Any())
            {
                foreach (var panel in panels)
                {
                    Rect screenBounds = new Rect(0, 0, notifier.ActualWidth, notifier.ActualHeight);

                    foreach (var child in panel.Children)
                    {
                        if(child is IVirtualized)
                        {
                            var vChild = child as IVirtualized;
                            bool isChildVisibile = VisualTreeHelper.FindElementsInHostCoordinates(screenBounds, notifier).Contains(child);

                            if (isChildVisibile == false)
                            {
                                vChild.Susped();
                            }
                            else
                            {
                                vChild.Activate();
                            }
                        }
                    }
                }
            }
           
        }
    }
}
