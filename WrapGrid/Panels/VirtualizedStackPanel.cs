using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using WrapGrid.Interfaces;

namespace WrapGrid.Panels
{
    class VirtualizedStackPanel : Panel, IScrollAware
    {
        protected double verticalParentScrollPosition;

        private Size CalculateContainerSize(Size availableSize)
        {
            Size result = new Size();
            foreach (var item in Children)
            {
                item.Measure(availableSize);
                result.Height += item.DesiredSize.Height;
                result.Width += item.DesiredSize.Width;
            }

            return result;
        }

        private Size ArrangeElements(Size finalSize)
        {
            double previousChildHeight = 0;
            foreach (var child in Children)
            {
                Rect childSize = new Rect(0, previousChildHeight, child.DesiredSize.Width, child.DesiredSize.Height);
                child.Arrange(childSize);
                previousChildHeight += child.DesiredSize.Height;
            }

            return finalSize;
        }


        private void VirtualizeElements()
        {
            foreach (var child in Children)
            {
                if (child is IVirtualized)
                {
                    var vChildren = child as IVirtualized;
                    Rect screenBounds = new Rect(0, 0, this.ActualWidth, this.ActualHeight);
                    bool isChildVisibile = VisualTreeHelper.FindElementsInHostCoordinates(screenBounds, this).Contains(child);

                    if (isChildVisibile == false)
                    {
                        //vChildren.Susped();
                    }
                }
            }
        }

        protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize)
        {
            base.ArrangeOverride(finalSize);
            return ArrangeElements(finalSize);
        }

        protected override System.Windows.Size MeasureOverride(System.Windows.Size availableSize)
        {
            base.MeasureOverride(availableSize);
            return CalculateContainerSize(availableSize);
        }

        public void UpdateCurrentScrollPosition(double verticalPosition)
        {
            this.verticalParentScrollPosition = verticalPosition;

            VirtualizeElements();
        }

    }
}
