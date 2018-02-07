using Nop.Services.Catalog;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Tax;
using Nop.Services.Media;
using Nop.Core;
using Nop.Core.Domain.Orders;
using System.Linq;
using Nop.Core.Plugins;

namespace Nop.Plugin.Catalog.GBS.Services
{
    class GBSProductAttributeFormatter : ProductAttributeFormatter
    {
        #region fields
        private readonly IWorkContext _workContext;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ICurrencyService _currencyService;
        private readonly ILocalizationService _localizationService;
        private readonly ITaxService _taxService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IDownloadService _downloadService;
        private readonly IWebHelper _webHelper;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly IPluginFinder _pluginFinder;
        private readonly IStoreContext _storeContext;

        #endregion

        #region ctor
        public GBSProductAttributeFormatter(IWorkContext workContext,
            IProductAttributeService productAttributeService,
            IProductAttributeParser productAttributeParser,
            ICurrencyService currencyService,
            ILocalizationService localizationService,
            ITaxService taxService,
            IPriceFormatter priceFormatter,
            IDownloadService downloadService,
            IWebHelper webHelper,
            IPriceCalculationService priceCalculationService,
            ShoppingCartSettings shoppingCartSettings,
            IPluginFinder pluginFinder,
            IStoreContext storeContext)
            : base(workContext, 
                  productAttributeService,
                  productAttributeParser, 
                  currencyService,
                  localizationService,
                  taxService, 
                  priceFormatter, 
                  downloadService,
                  webHelper, 
                  priceCalculationService, 
                  shoppingCartSettings)
        {
            this._workContext = workContext;
            this._productAttributeService = productAttributeService;
            this._productAttributeParser = productAttributeParser;
            this._currencyService = currencyService;
            this._localizationService = localizationService;
            this._taxService = taxService;
            this._priceFormatter = priceFormatter;
            this._downloadService = downloadService;
            this._webHelper = webHelper;
            this._priceCalculationService = priceCalculationService;
            this._shoppingCartSettings = shoppingCartSettings;
            this._pluginFinder = pluginFinder;
            this._storeContext = storeContext;
        }
        #endregion

        public override string FormatAttributes(Product product, string attributesXml,
            Customer customer, string serapator = "<br />", bool htmlEncode = true, bool renderPrices = true,
            bool renderProductAttributes = true, bool renderGiftCardAttributes = true,
            bool allowHyperlinks = true)
        {
            var miscPlugins = _pluginFinder.GetPlugins<CategoryNavigationProvider>(storeId: _storeContext.CurrentStore.Id).ToList();
            if (miscPlugins.Count > 0)
            {
                var mappings = _productAttributeParser.ParseProductAttributeMappings(attributesXml);
                //find mappings with Display Order of 77
                var mappingsToDelete = mappings.Where(attribute => attribute.DisplayOrder == 77).ToList();
                foreach (var mapping in mappingsToDelete)
                {
                    attributesXml = _productAttributeParser.RemoveProductAttribute(attributesXml, mapping);
                }
            }
            var result = base.FormatAttributes(product, attributesXml, customer,
                serapator, htmlEncode, renderPrices, renderProductAttributes,
                renderGiftCardAttributes, allowHyperlinks);

            return result.ToString();
        }
    }
}
