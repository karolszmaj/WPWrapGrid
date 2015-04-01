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

        public ItemState State
        {
            get { return this.state; }
            private set { this.state = value; }
        }

        public DataTemplate ContentTemplateScheme { get; set; }

        public void Susped()
        {
            if(State == ItemState.Virtualized)
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
            State = ItemState.Virtualized;
        }

        public void Activate()
        {
            if(State == ItemState.Created)
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
            
            State = ItemState.Created;
        }
    }
}
