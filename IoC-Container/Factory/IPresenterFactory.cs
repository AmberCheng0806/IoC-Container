using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoC_Container.Factory
{
    public interface IPresenterFactory
    {
        TPresenter Create<TPresenter>(object view);
    }
}
