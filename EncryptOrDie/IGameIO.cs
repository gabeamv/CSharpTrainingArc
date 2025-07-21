using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptOrDie
{
    internal interface IGameIO
    {
        string promptInput(string message);
        void showMessage(string message);
    }
}
