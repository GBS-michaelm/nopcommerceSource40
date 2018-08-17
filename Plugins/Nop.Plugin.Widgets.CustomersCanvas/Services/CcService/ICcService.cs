using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Vendors;
using Nop.Core.Plugins;
using Nop.Plugin.Widgets.CustomersCanvas.Data;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Widgets.CustomersCanvas.Domain;
using Nop.Plugin.Widgets.CustomersCanvas.Model;

namespace Nop.Plugin.Widgets.CustomersCanvas.Services
{
    public interface ICcService
    {
        int AddDesign(string data, string imageUrl, string downloadUrlsJson);
        void UpdateDesign(int designId, string dataJson, string imageUrl, string downloadUrlsJson);
        CcDesign GetDesign(int id);

        #region attributes
        Dictionary<CcProductAttributes, string> CCAttributesSettingsNames();
        IEnumerable<int> GetCcAttrIds(IEnumerable<CcProductAttributes> attrs = null);
        int GetCcAttrId(CcProductAttributes attribute);
        string ServerHostUrl();
        int EditorDefinitionSpecificationAttributeOptionId();
        int EditorDefinitionSpecificationAttributeId();
        int EditorConfigurationSpecificationAttributeId();
        int EditorConfigurationSpecificationAttributeOptionId();
        bool IsItCcHiddenAttributeInProductPage(int attributeId);
        bool IsProductForCc(Product product);
        bool IsProductForCc(int productId);
        #endregion

        #region cc-result
        bool IsPluginEnabled();
        CcResult GetCcResult(string attributesXml);
        #endregion
        #region scripts
        string JsWritePromisePolyfill();
        string JsWriteScriptTags(IEnumerable<string> scripts);
        #endregion
        #region cc-widget
        string GetEditorDefinition(int productId);
        string GetEditorConfiguration(int productId);

        string GetEditorDefinition(Product product);
        string GetEditorConfiguration(Product product);

        List<string> GetCcOptionsFromConfig(int productId);
        #endregion

        #region editors
        List<EditorData> GetAllEditors();
        EditorData GetEditor(string editorName);
        ConfigData GetConfig(string editorName, string configName);
        #endregion

        string FormatOrderTokenString(Core.Domain.Orders.OrderItem item, string str);
        string RemoveAttrMappings(string attributesXml, IEnumerable<int> productAttributesIds);

        CcCartImagesReplacementModel GetCcCartItems(out int count);
    }
}
