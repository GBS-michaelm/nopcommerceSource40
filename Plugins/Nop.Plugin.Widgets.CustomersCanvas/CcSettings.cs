﻿using Nop.Core.Configuration;
using System.Collections.Generic;

namespace Nop.Plugin.Widgets.CustomersCanvas
{
    public class CcSettings : ISettings
    {
        public string ServerHostUrl { get; set; }
        public int CcIdAttributeId { get; set; }

        public string CustomersCanvasApiKey { get; set; }

        public string DesignFileName { get; set; }

        public bool IsOrderExportButton { get; set; }
        public string OrderExportPath { get; set; }

        public int EditorDefinitionSpecificationAttributeId { get; set; }
        public int EditorDefinitionSpecificationOptionId { get; set; }

        public int EditorConfigurationSpecificationAttributeId { get; set; }
        public int EditorConfigurationSpecificationOptionId { get; set; }

        public List<int> SampleProducts { get; set; }
    }
}
