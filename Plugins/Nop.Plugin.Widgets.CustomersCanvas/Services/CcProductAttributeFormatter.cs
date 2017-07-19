using Nop.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Tax;
using Nop.Services.Media;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services;
using System.Web;
using Nop.Core.Html;
using Nop.Plugin.Widgets.CustomersCanvas.Services;

namespace Nop.Plugin.Widgets.CustomersCanvas.Services
{
    public class CcProductAttributeFormatter : ProductAttributeFormatter
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
        private readonly ICcService _ccService;
        #endregion

        #region ctor
        public CcProductAttributeFormatter(IWorkContext workContext, 
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
            ICcService ccService)
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
            this._ccService = ccService;
        }
        #endregion

        public override string FormatAttributes(Product product, string attributesXml,
            Customer customer, string serapator = "<br />", bool htmlEncode = true, bool renderPrices = true,
            bool renderProductAttributes = true, bool renderGiftCardAttributes = true,
            bool allowHyperlinks = true)
        { 

            var isProdcutForCc = _ccService.IsProductForCc(product);

            if (isProdcutForCc)
            { 
                attributesXml = _ccService.RemoveAttrMappings(attributesXml, _ccService.GetCcAttrIds());
            }

            var result = base.FormatAttributes(product, attributesXml, customer, 
                serapator, htmlEncode, renderPrices, renderProductAttributes, 
                renderGiftCardAttributes, allowHyperlinks);

            return result.ToString();
        }
    }
}
