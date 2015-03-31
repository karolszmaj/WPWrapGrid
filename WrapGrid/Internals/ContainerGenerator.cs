using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WrapGrid.Panels;

namespace WrapGrid.Internals
{
    internal class ContainerGenerator
    {
        private Grid rootScope;

        public void GenerateContainer(int columns)
        {
            ClearContainer();
            GenerateColumns(columns);

            for (int i = 0; i < columns; i++)
            {
                var panel = new StackPanel()
                {
                    VerticalAlignment = VerticalAlignment.Top
                };
                Grid.SetColumn(panel, i);
                rootScope.Children.Add(panel);
            }
        }

        public void SetContainer(Grid container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container cannot be null");
            }

            this.rootScope = container;
        }

        private void ClearContainer()
        {
            this.rootScope.Children.Clear();
        }

        private void GenerateColumns(int count)
        {
            this.rootScope.ColumnDefinitions.Clear();
            for (int i = 0; i < count; i++)
			{
                this.rootScope.ColumnDefinitions.Add(new ColumnDefinition()
                    {
                        Width = new GridLength(0.5, GridUnitType.Star)
                    });
			}
            
        }
    }
}
