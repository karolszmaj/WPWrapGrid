using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WrapGrid.Enums;

namespace WrapGrid.Interfaces
{
    internal interface IVirtualized
    {
        ItemState State { get; }

        void Susped();

        void Activate();
    }
}
