using Nop.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using System.Xml;

namespace Nop.Plugin.Widgets.CustomersCanvas.Services
{

    public interface ICcIProductAttributeParser : IProductAttributeParser
    {
        string RemoveAttrMappings(string attributesXml, IEnumerable<int> productAttributesIds);
    }
    public class CcProductAttributeParser : ProductAttributeParser, ICcIProductAttributeParser
    {
        private readonly IProductAttributeService _productAttributeService;
        public CcProductAttributeParser(IProductAttributeService productAttributeService) : base(productAttributeService)
        {
            this._productAttributeService = productAttributeService;
        }

        public string RemoveAttrMappings(string attributesXml, IEnumerable<int> productAttributesIds)
        {
            var xmlDoc = new XmlDocument();
            if (string.IsNullOrEmpty(attributesXml))
                return attributesXml;

            xmlDoc.LoadXml(attributesXml);

            var rootElement = (XmlElement)xmlDoc.SelectSingleNode(@"//Attributes");
            if (rootElement == null)
                return attributesXml;

            var nodeList = xmlDoc.SelectNodes(@"//Attributes/ProductAttribute");
            if (nodeList == null)
                return attributesXml;

            Func<XmlNode, ProductAttributeMapping> getProductAttributeMapping = n =>
            {
                if (n.Attributes == null)
                    return null;

                var idStr = n.Attributes["ID"].InnerText.Trim();

                int mappingId;
                if (!int.TryParse(idStr, out mappingId))
                    return null;

                return _productAttributeService.GetProductAttributeMappingById(mappingId);
            };

            var nodeToRemove = nodeList.Cast<XmlNode>().Where(n =>
            {
                var mapping = getProductAttributeMapping(n);

                return mapping != null && productAttributesIds.Contains(mapping.ProductAttributeId);
            }).ToList();

            foreach (var node in nodeToRemove)
                rootElement.RemoveChild(node);

            return xmlDoc.OuterXml;
        }

    }
}
