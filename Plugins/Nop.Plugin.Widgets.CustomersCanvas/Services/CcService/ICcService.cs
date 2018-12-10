using Nop.Core.Domain.Catalog;
using Nop.Plugin.Widgets.CustomersCanvas.Data;
using Nop.Plugin.Widgets.CustomersCanvas.Domain;
using Nop.Plugin.Widgets.CustomersCanvas.Model;
using System.Collections.Generic;

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
