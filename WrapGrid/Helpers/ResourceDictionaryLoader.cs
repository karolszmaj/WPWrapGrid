using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WrapGrid.Helpers
{
    internal class ResourceDictionaryLoader
    {
        private Lazy<ResourceDictionary> resource = new Lazy<ResourceDictionary>(() =>
            {
                var resourceDict = new ResourceDictionary();
                resourceDict.Source = new Uri("/WrapGrid;component/Themes/Generic.xaml", UriKind.Relative);

                return resourceDict;
            });

        public ResourceDictionary Dictionary
        {
            get
            {
                return resource.Value;
            }
        }
    }
}
    