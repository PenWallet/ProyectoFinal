using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHAIR_UI.Interfaces
{
    public interface IBasicActions
    {
        void OpenWindow(string window);
        void Close();
        void Maximize();
        void Minimize();
    }
}
