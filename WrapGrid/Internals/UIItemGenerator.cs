using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WrapGrid.Factory;
using WrapGrid.Presenters;
using WrapGrid.Selectors;

namespace WrapGrid.Internals
{
    internal class UIItemGenerator
    {
        private DataTemplateSelector selector;
        private DataTemplate itemTemplate;

        public void SetSelector(DataTemplateSelector selector)
        {
            this.selector = selector;
        }

        public void SetTemplate(DataTemplate itemTemplate)
        {
            this.itemTemplate = itemTemplate;
        }

        /// <summary>
        /// This returns VirtualizedContentPresenter
        /// </summary>
        /// <param name="virtualizedContent"></param>
        /// <param name="model"></param>
        /// <param name="bindModelToDataContext"></param>
        /// <returns></returns>
        public FrameworkElement CreateVirtualizedElement(FrameworkElement virtualizedContent, object model, bool bindModelToDataContext)
        {
            FrameworkElement result = null;
            DataTemplate template = null;

            if (itemTemplate != null)
            {
                template = itemTemplate;
            }
            else if (selector != null)
            {
                template = selector.SelectTemplate(model);
            }
            else
            {
                throw new InvalidOperationException("ItemTemplate or ItemTemplateSelector must be provided");
            }

            var generatedUIElement = template.LoadContent();
            var generatedFullControl = FrameworkElementFactory.CreateVirtualizedControl(generatedUIElement as FrameworkElement, virtualizedContent);

            result = new VirtualizedContentPresenter()
            {
                Content = generatedFullControl,
                ContentTemplateScheme = template
            };

            if (bindModelToDataContext)
            {
                result.DataContext = model;
            }

            return result;
        }
    }
}
