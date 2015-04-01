using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WrapGrid.Interfaces;
using System.Reactive;
using System.Reactive.Linq;
using WrapGrid.EventArgs;
using System.Reactive.Concurrency;
using System.Threading;

namespace WrapGrid.Internals
{
    public class VirtualizationManager
    {
        private Grid rootControl;
        private ScrollViewer scrollViewer;
        private readonly ScrollViewerMonitor scrollMonitor;
        private bool isVirtualizing;

        public event Func<IEnumerable<Panel>> GetPanels;
        public event Func<bool> CanVirtualize;

        public VirtualizationManager(ScrollViewerMonitor scrollMonitor)
        {
            this.scrollMonitor = scrollMonitor;

            var subjectScrollChanged = Observable.FromEventPattern<EventHandler<ScrollChangedEventArgs>, ScrollChangedEventArgs>(handler => scrollMonitor.ScrollChanged += handler, handler => scrollMonitor.ScrollChanged -= handler)
                .Select(x => x.EventArgs)
                .Throttle(TimeSpan.FromMilliseconds(200));


            subjectScrollChanged
                .ObserveOnDispatcher()
                .Subscribe(args =>
                {
                    OnScrollChangedEventHandler(args);
                });
        }

        private void OnScrollChangedEventHandler(EventArgs.ScrollChangedEventArgs e)
        {
            if (OnCanVirtualize())
            {
                Debug.WriteLine(e.CurrentPosition);

                VirtualizeElements(e.Notifier);
            }

        }

        protected IEnumerable<Panel> OnGetPanels()
        {
            IEnumerable<Panel> result = Enumerable.Empty<Panel>();
            var handler = GetPanels;

            if (GetPanels != null)
            {
                result = GetPanels();
            }

            return result;
        }

        protected bool OnCanVirtualize()
        {
            bool result = false;
            var handler = CanVirtualize;

            if (handler != null)
            {
                result = handler();
            }

            return result;
        }

        private void VirtualizeElements(ScrollViewer notifier)
        {
            Stopwatch watcher = new Stopwatch();
            watcher.Start();
            isVirtualizing = true;
            var panels = OnGetPanels();

            if (panels != null && panels.Any())
            {
                Rect screenBounds = new Rect(0, 0, notifier.ActualWidth, notifier.ActualHeight);
                foreach (var panel in panels)
                {
                    foreach (var child in panel.Children)
                    {
                        if (child is IVirtualized)
                        {
                            var vChild = child as IVirtualized;

                            bool isChildVisibile = IsItemVisible(child as FrameworkElement);// VisualTreeHelper.FindElementsInHostCoordinates(screenBounds, notifier).Contains(child);

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

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }

            watcher.Stop();
            Debug.WriteLine("Virtualized in {0} ms", watcher.ElapsedMilliseconds);
        }

        private bool IsItemVisible(FrameworkElement element)
        {
            bool isItemVisible = false;

            GeneralTransform childTransform = scrollViewer.TransformToVisual(element);
            //Rect rectangle = childTransform.TransformBounds(new Rect(new Point(0, 0), scrollViewer.RenderSize));
            var childPosition = childTransform.Transform(new Point());

            if (childPosition.Y - element.ActualHeight > 0)
            {
                isItemVisible = false;
            }
            else if (Math.Abs(childPosition.Y) > scrollViewer.ActualHeight)
            {
                isItemVisible = false;
            }
            else
            {
                //items is below so we need to calculate if the control is visible in scrollviewer height
                isItemVisible = true;
            }

            //Check if the elements Rect intersects with that of the scrollviewer's
            /*new Rect(new Point(0, 0), element.RenderSize).Intersect(rectangle);
            {
                itemIsIntersected = true;
            };*/


            return isItemVisible;
        }

        public void SetRootControl(Grid rootControl)
        {
            this.rootControl = rootControl;
        }

        public void SetScrollViewer(ScrollViewer scrollViewer)
        {
            this.scrollViewer = scrollViewer;
        }
    }
}
