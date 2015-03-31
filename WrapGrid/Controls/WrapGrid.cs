using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WrapGrid.Helpers;
using WrapGrid.Internals;
using WrapGrid.Selectors;

namespace WrapGrid.Controls
{
    [TemplatePart(Name=ScrollContainer, Type=typeof(ScrollViewer))]
    [TemplatePart(Name = ScrollGrid, Type = typeof(Grid))]
    public class WrapGrid: Control
    {
        private const string ScrollContainer = "PART_ScrollContainer";
        private const string ScrollGrid = "PART_ScrollGrid";

        private Grid scrollGrid;
        private ScrollViewer scrollContainer;
        private int indexCounter;
        private bool isProcessingData;

        private readonly ContainerGenerator containerGenerator;
        private readonly UIItemGenerator uiItemGenerator;
        private readonly ContainerPopulator itemPopulator;
        private readonly ScrollViewerMonitor scrollViewerMonitor;
        private readonly VirtualizationManager virtualizationManager;

        public event EventHandler<System.EventArgs> FetchMoreData;

        public WrapGrid()
        {
            DefaultStyleKey = typeof(WrapGrid);
            this.containerGenerator = new ContainerGenerator();
            this.uiItemGenerator = new UIItemGenerator();
            this.itemPopulator = new ContainerPopulator();
            this.scrollViewerMonitor = new ScrollViewerMonitor();
            this.virtualizationManager = new VirtualizationManager(scrollViewerMonitor);

            this.virtualizationManager.GetPanels += OnGetPanelsEventHandler;
            this.scrollViewerMonitor.ScrollChanged += OnScrollViewerScrollChanged;

        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(WrapGrid), new PropertyMetadata(null, ItemsSourcePropertyChangedCallback));
        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(WrapGrid), new PropertyMetadata(null));
        public static readonly DependencyProperty TemplateSelectorProperty = DependencyProperty.Register("TemplateSelector", typeof(DataTemplateSelector), typeof(WrapGrid), new PropertyMetadata(new DefaultGridSelector()));
        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register("Columns", typeof(int), typeof(WrapGrid), new PropertyMetadata(1));
        public static readonly DependencyProperty EnableIncrementalLoadingProperty = DependencyProperty.Register("EnableIncrementalLoading", typeof(bool), typeof(WrapGrid), new PropertyMetadata(true));
        public static readonly DependencyProperty EnableVirtualizationProperty = DependencyProperty.Register("EnableVirtualization", typeof(bool), typeof(WrapGrid), new PropertyMetadata(false));
        public static readonly DependencyProperty VirtualizedContentTemplateProperty = DependencyProperty.Register("VirtualizedContentTemplate", typeof(DataTemplate), typeof(WrapGrid), new PropertyMetadata(null));

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public DataTemplateSelector TemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(TemplateSelectorProperty); }
            set { SetValue(TemplateSelectorProperty, value); }
        }

        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        public bool EnableIncrementalLoading
        {
            get { return (bool)GetValue(EnableIncrementalLoadingProperty); }
            set { SetValue(EnableIncrementalLoadingProperty, value); }
        }

        public bool EnableVirtualization
        {
            get { return (bool)GetValue(EnableVirtualizationProperty); }
            set { SetValue(EnableVirtualizationProperty, value); }
        }

        public DataTemplate VirtualizedContentTemplate
        {
            get { return (DataTemplate)GetValue(VirtualizedContentTemplateProperty); }
            set { SetValue(VirtualizedContentTemplateProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            scrollGrid = GetTemplateChild(ScrollGrid) as Grid;
            scrollContainer = GetTemplateChild(ScrollContainer) as ScrollViewer;

            containerGenerator.SetContainer(scrollGrid);
            containerGenerator.GenerateContainer(Columns);

            uiItemGenerator.SetSelector(TemplateSelector);
            uiItemGenerator.SetTemplate(ItemTemplate);

            itemPopulator.SetRootContainer(scrollGrid);

            scrollViewerMonitor.Register(scrollContainer);
        }

        private static async void ItemsSourcePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as WrapGrid;
            if (control.ItemsSource is INotifyCollectionChanged)
            {
                var changingCollection = control.ItemsSource as INotifyCollectionChanged;

                changingCollection.CollectionChanged -= control.OnItemsSourceCollectionChanged;
                changingCollection.CollectionChanged += control.OnItemsSourceCollectionChanged;
            }

            //each time we generate clear table for elements
            control.PopulateItems();
        }

        private void OnScrollViewerScrollChanged(object sender, EventArgs.ScrollChangedEventArgs e)
        {
            if (scrollContainer.VerticalOffset >= scrollContainer.ScrollableHeight)
            {
                OnFetchMoreData();
            }

            if(EnableVirtualization)
            {
                itemPopulator.VirtualizeContainer(e.CurrentPosition);
            }
        }

        private async void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var control = sender as WrapGrid;
            await PopulateItems(e.NewItems, e.OldItems);
        }

        private IEnumerable<Panel> OnGetPanelsEventHandler()
        {
            return this.itemPopulator.Containers;
        }

        private async Task PopulateItems()
        {
            if (isProcessingData)
            {
                return;
            }

            isProcessingData = true;
            int localCounter = 0;

            if (ItemsSource == null)
            {
                DebugInfo.Log("ItemsSource is null");
                return;
            }

            var enumerableItems = ItemsSource.Cast<object>();
            DebugInfo.Log(string.Format("Processing {0} elements.", enumerableItems.Count()));

            for (localCounter = indexCounter; localCounter < enumerableItems.Count(); localCounter++)
            {
                var item = enumerableItems.ElementAt(localCounter);
                await AddElementToList(item);
            }

            indexCounter += localCounter;
            isProcessingData = false;
        }

        private async Task PopulateItems(IEnumerable itemsToAdd, IEnumerable itemsToRemove)
        {
            isProcessingData = true;
            if(ItemsSource != null)
            {
                if (itemsToAdd != null)
                {
                    foreach (var item in itemsToAdd)
                    {
                        await AddElementToList(item);
                    }
                }
                
                if(itemsToRemove != null)
                {
                    foreach (var item in itemsToRemove)
                    {
                        itemPopulator.RemoveElementFromContainer(item);
                    }
                }
                
            }

            isProcessingData = false;
        }

        private async Task AddElementToList(object model)
        {
            string controlGuid = Guid.NewGuid().ToString();

            var virtualizedControl = VirtualizedContentTemplate.LoadContent() as FrameworkElement;
            virtualizedControl.Visibility = System.Windows.Visibility.Collapsed;

            var element = uiItemGenerator.CreateVirtualizedElement(virtualizedControl, model, true);

            itemPopulator.AddElementToContainer(element);

            if (EnableIncrementalLoading)
            {
                await Task.Delay(10);
            }
        }

        protected void OnFetchMoreData()
        {
            var handler = FetchMoreData;
            if(handler != null)
            {
                handler(this, null);
            }
        }
    }
}
