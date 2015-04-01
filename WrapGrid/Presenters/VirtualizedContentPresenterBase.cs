using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WrapGrid.Enums;
using WrapGrid.Interfaces;

namespace WrapGrid.Presenters
{
    internal abstract class VirtualizedContentPresenterBase : ContentControl, IVirtualized
    {
        private ItemState state;
        private ScrollViewer parentScroll;

        public DataTemplate ContentTemplateScheme { get; set; }

        void VirtualizedContentPresenterBase_Loaded(object sender, RoutedEventArgs e)
        {
            Virtualize(this.parentScroll, false);
        }

        public void Virtualize(ScrollViewer parentScrollContainer, bool useLazyVirtualization)
        {
            this.parentScroll = parentScrollContainer;

            if(!useLazyVirtualization)
            {
                if (IsItemVisible(this.parentScroll))
                {
                    Activate();
                }
                else
                {
                    Susped();
                }
            }
            
        }

        private void Susped()
        {
            if(state == ItemState.Virtualized)
            {
                return;
            }
            
            if (Content is Grid)
            {
                var gridContent = Content as Grid;

                var virtualizationControl = gridContent.Children.FirstOrDefault() as FrameworkElement;
                var contentControl = gridContent.Children.LastOrDefault() as FrameworkElement;

                if (virtualizationControl != null && contentControl != null)
                {
                    virtualizationControl.Visibility = System.Windows.Visibility.Visible;
                    virtualizationControl.Height = contentControl.ActualHeight;
                    virtualizationControl.Width = contentControl.ActualWidth;
                    contentControl.DataContext = null;
                    gridContent.Children.Remove(contentControl);
                    contentControl = null;
                    //contentControl.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
            
            this.UpdateLayout();
            state = ItemState.Virtualized;
        }

        private void Activate()
        {
            if(state == ItemState.Created)
            {
                return;
            }
            
            var content = ContentTemplateScheme.LoadContent() as FrameworkElement;

            var gridContent = Content as Grid;
            var virtualizationControl = gridContent.Children.FirstOrDefault() as FrameworkElement;

            if (virtualizationControl != null)
            {
                virtualizationControl.Visibility = System.Windows.Visibility.Collapsed;
                content.Height = virtualizationControl.ActualHeight;
                content.Width = virtualizationControl.ActualWidth;
                gridContent.Children.Add(content);
            }
            
            this.UpdateLayout();
            
            state = ItemState.Created;
        }

        private bool IsItemVisible(ScrollViewer scrollViewer)
        {
            bool isItemVisible = false;

            GeneralTransform childTransform = scrollViewer.TransformToVisual(this);
            var childPosition = childTransform.Transform(new Point());

            if (childPosition.Y - this.ActualHeight > 0)
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

            return isItemVisible;
        }

    }
}
