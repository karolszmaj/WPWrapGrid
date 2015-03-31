using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WrapGrid.Helpers;
using WrapGrid.Selectors;

namespace WrapGrid.Internals
{
    internal class DefaultGridSelector: DataTemplateSelector
    {
        private const string DefaultTemplateKey = "DefaultTextBlockTemplate";
        private readonly ResourceDictionaryLoader dicionaryLoader;

        public DefaultGridSelector()
        {
            dicionaryLoader = new ResourceDictionaryLoader();
        }

        public override System.Windows.DataTemplate SelectTemplate(object item)
        {
            var template = dicionaryLoader.Dictionary[DefaultTemplateKey] as DataTemplate;
            return template;
        }
    }
}
