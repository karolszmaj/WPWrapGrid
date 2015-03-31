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

namespace WrapGrid.Internals
{
    internal class ContainerPopulator
    {
        private Grid container;
        private List<Panel> containerColumns;
        private int columns;

        public List<Panel> Containers
        {
            get { return containerColumns; }
        }

        public void SetRootContainer(Grid container)
        {
            if(container == null)
            {
                throw new ArgumentNullException("container");
            }

            this.container = container;
            ExtractPanelsFromContainer();
        }

        public void AddElementToContainer(FrameworkElement element)
        {
            var panel = GetSmallestPanel();
            panel.Children.Add(element);
            panel.UpdateLayout();
        }

        public void VirtualizeContainer(double currentVerticalPosition)
        {/*
            containerColumns.ForEach((panel) =>
                {
                    foreach (var child in panel.Children)
                    {
                        if (child is IVirtualized)
                        {
                            var vChildren = child as IVirtualized;
                            Rect screenBounds = new Rect(0, 0, container.ActualWidth, container.ActualHeight);
                            bool isChildVisibile = VisualTreeHelper.FindElementsInHostCoordinates(screenBounds, container).Contains(child);

                            if (isChildVisibile == false)
                            {
                                vChildren.Susped();
                            }
                        }
                    }
                });*/
        }

        public void RemoveElementFromContainer(object dataContextModel)
        {
            for (int i = 0; i < containerColumns.Count; i++)
            {
                var modelContext = containerColumns[i].Children.OfType<FrameworkElement>().FirstOrDefault(x => x.DataContext == dataContextModel);
                if (modelContext != null)
                {
                    containerColumns[i].Children.Remove(modelContext);
                    return;
                }
            }
        }

        private void ExtractPanelsFromContainer()
        {
            containerColumns = new List<Panel>(container.Children.OfType<Panel>());
            columns = containerColumns.Count;
        }

        private Panel GetSmallestPanel()
        {
            var smallestPanelHeigth = containerColumns.Min(x => x.ActualHeight);
            var panel = containerColumns.FirstOrDefault(x => x.ActualHeight == smallestPanelHeigth);
            DisplayDebugSmallestContainerInfo(panel);
            return panel;
        }

        [Conditional("DEBUG")]
        private void DisplayDebugSmallestContainerInfo(Panel selectedPanel)
        {
            StringBuilder debugInfo = new StringBuilder();
            debugInfo.AppendFormat("Selected Panel for index: {0}", containerColumns.IndexOf(selectedPanel));
            debugInfo.AppendLine();

            for (int i = 0; i < containerColumns.Count; i++)
            {
                debugInfo.AppendFormat("Panel[{0}] h:{1} w:{2}", i, containerColumns[i].ActualHeight, containerColumns[i].ActualWidth);
            }

            Debug.WriteLine(debugInfo.ToString());
        }
    }
}
