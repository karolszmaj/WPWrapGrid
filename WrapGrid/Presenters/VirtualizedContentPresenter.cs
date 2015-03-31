using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrapGrid.Presenters
{
    internal class VirtualizedContentPresenter: VirtualizedContentPresenterBase
    {
        public VirtualizedContentPresenter()
        {
            DefaultStyleKey = typeof (VirtualizedContentPresenter);
        }
    }
}
