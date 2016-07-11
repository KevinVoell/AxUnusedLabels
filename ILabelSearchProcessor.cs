using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace FindUnusedLabels
{
    internal interface ILabelSearchProcessor
    {
        void Run(string searchPath, string filter);
    }
}
