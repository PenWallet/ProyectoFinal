using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHAIR_UI.Interfaces
{
    public interface IBasicActions : ICloseable, IMinimizeable, IMaximizable
    {
        void ShowPopUp(string message);
        void OpenWindow(string window);
    }
}
