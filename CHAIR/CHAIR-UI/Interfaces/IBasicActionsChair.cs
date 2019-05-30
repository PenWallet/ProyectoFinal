using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHAIR_UI.Interfaces
{
    public interface IBasicActionsChair : IBasicActions
    {
        void ChangePage(string page, object viewmodel);
        void OpenConversation();
        void MinimizeToTray();
        void CloseWithParameter(bool callShutdown);
    }
}
