using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WrapGrid.Selectors
{
    public abstract class DataTemplateSelector: DependencyObject
    {
        public abstract DataTemplate SelectTemplate(object item);
    }
}
