using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WrapGrid.Enums;

namespace WrapGrid.Interfaces
{
    internal interface IVirtualized
    {
        void Virtualize(ScrollViewer parnetScroll, bool useLazyVirtualization);
    }
}
