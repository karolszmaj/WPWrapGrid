using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrapGrid.Interfaces
{
    interface IScrollAware
    {
        void UpdateCurrentScrollPosition(double verticalPosition);
    }
}
