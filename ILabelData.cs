using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FindUnusedLabels
{
    internal interface ILabelData
    {
        string LabelId { get; set; }

        bool IsUsed { get; set; }

        string Text { get; set; }

        string Description { get; set; }
    }
}
