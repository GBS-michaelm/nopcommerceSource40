using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.CustomersCanvas
{
    public class CcSettings : ISettings
    {
        public string ServerHostUrl { get; set; }
        public int CcIdAttributeId { get; set; }

        public int EditorDefinitionSpecificationAttributeId { get; set; }
        public int EditorDefinitionSpecificationOptionId { get; set; }

        public int EditorConfigurationSpecificationAttributeId { get; set; }
        public int EditorConfigurationSpecificationOptionId { get; set; }

        public List<int> SampleProducts { get; set; }

    }
}
