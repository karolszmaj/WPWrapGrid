using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WrapGrid.EventArgs
{
    public class ScrollChangedEventArgs: System.EventArgs
    {

        private double currentPosition;
        private double totalHeight;
        ScrollViewer notifier;

        public ScrollViewer Notifier
        {
            get { return notifier; }
        }

        public double TotalHeight
        {
            get { return totalHeight; }
        }

        public double CurrentPosition
        {
            get { return currentPosition; }
        }

        public ScrollChangedEventArgs(double currentPosition, double totalHeight, ScrollViewer notifier)
        {
            this.currentPosition = currentPosition;
            this.totalHeight = totalHeight;
            this.notifier = notifier;
        }


    }
}
