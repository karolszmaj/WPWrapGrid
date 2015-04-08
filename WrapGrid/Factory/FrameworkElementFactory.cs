using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WrapGrid.Factory
{
    internal static class FrameworkElementFactory
    {
        public static FrameworkElement CreateVirtualizedControl(FrameworkElement content, FrameworkElement virtualizedContent)
        {
            FrameworkElement result = null;
            result = CreateGrid(content, virtualizedContent);

            return result;
        }

        public static FrameworkElement CreateVirtualizedControl(object model, DataTemplate contentTemplate, DataTemplate virtualizedContentTemplate)
        {
            Grid container = new Grid();
            ContentPresenter content = new ContentPresenter()
            {
                Content = model,
                ContentTemplate = contentTemplate
            };

            ContentPresenter virtualizedContent = new ContentPresenter()
            {
                ContentTemplate = virtualizedContentTemplate
            };

            container.Children.Add(virtualizedContent);
            container.Children.Add(content);

            return container;
        }

        private static FrameworkElement CreateGrid(FrameworkElement content, FrameworkElement virtualizedContent)
        {
            Grid container = new Grid();
            SetUniqueNames(content, virtualizedContent);
            container.Children.Add(virtualizedContent);
            container.Children.Add(content);

            return container;
        }

        private static void SetUniqueNames(FrameworkElement content, FrameworkElement virtualizedContent)
        {
            var guid = Guid.NewGuid().ToString();
            content.Name = "_content_" + guid;
            virtualizedContent.Name = "_virtualized_" + guid;
        }
    }
}
