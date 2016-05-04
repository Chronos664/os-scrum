using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenSourceScrumTool.Models
{
    public interface IModelContent
    {
        object ToDTO();
        object GetDetails();
        void UpdateItem(object item);
    }
}
