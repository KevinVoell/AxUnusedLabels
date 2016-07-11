using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FindUnusedLabels
{
    internal class AxLabelData : ILabelData
    {
        #region ILabelData Members

        public string LabelId { get; set; }

        public bool IsUsed { get; set; }

        public string Text { get; set; }

        public string Description { get; set; }

        #endregion
    }
}
