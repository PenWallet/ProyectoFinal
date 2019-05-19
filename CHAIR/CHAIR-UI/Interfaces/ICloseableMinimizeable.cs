using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHAIR_UI.Interfaces
{
    /// <summary>
    /// Interface that inherits both ICloseable and IMinimizeable
    /// </summary>
    public interface ICloseableMinimizeable : ICloseable, IMinimizeable
    {

    }
}
