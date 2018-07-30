using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Media;
using System;
using System.Linq;

namespace Nop.Plugin.Products.SpecificationAttributes.Components
{
	[ViewComponent(Name = "CategoryImage")]
	public class CategoryImageViewComponent : NopViewComponent
	{
		public static int joecounter = 0;
		private readonly ICustomerService _customerService;
		private readonly ILanguageService _languageService;
		private readonly ISettingService _settingService;
		private readonly IStoreService _storeService;
		private readonly IWorkContext _workContext;
		private readonly ILocalizationService _localizationService;
		private readonly IGenericAttributeService _genericAttributeService;
		private readonly IVendorService _vendorService;
		private readonly IProductService _productService;
		private readonly IStoreContext _storeContext;
		private readonly ISpecificationAttributeService _specificationAttributeService;
		private readonly IWebHelper _webHelper;
		private readonly IPriceFormatter _priceFormatter;
		private readonly ICurrencyService _currencyService;
		private readonly CatalogSettings _catalogSettings;
		private readonly ICategoryService _categoryService;
		private readonly ICacheManager _cacheManager;
		private readonly MediaSettings _mediaSettings;
		private readonly IPriceCalculationService _priceCalculationService;
		private readonly IPermissionService _permissionService;
		private readonly ITaxService _taxService;
		private readonly IPictureService _pictureService;
		private readonly IMeasureService _measureService;
		private readonly IUrlRecordService _urlRecordService;
		private readonly IProductModelFactory _productModelFactory;
		private readonly IOrderProcessingService _orderProcessingService;
		private readonly OrderSettings _orderSettings;
		private readonly IOrderService _orderService;
		private readonly IProductAttributeParser _productAttributeParser;
		private readonly SpecificationAttributesSettings _specificationAttributesSettings;
		private readonly ILogger _logger;
		private readonly IProductAttributeParser parser = EngineContext.Current.Resolve<IProductAttributeParser>();
		private readonly ShoppingCartSettings _shoppingCartSettings;
		private readonly IShoppingCartModelFactory _shoppingCartModelFactory;

		public CategoryImageViewComponent(
			SpecificationAttributesSettings specificationAttributesSettings,
			ICustomerService customerService,
			ILanguageService languageService,
			ISettingService settingService,
			IStoreService storeService,
			IWorkContext workContext,
			ILocalizationService localizationService,
			IGenericAttributeService genericAttributeService,
			IVendorService vendorService,
			IProductService productService,
			IStoreContext storeContext,
			ISpecificationAttributeService specificationAttributeService,
			IWebHelper webHelper,
			IPriceFormatter priceFormatter,
			ICurrencyService currencyService,
			CatalogSettings catalogSettings,
			ICategoryService categoryService,
			ICacheManager cacheManager,
			MediaSettings mediaSettings,
			IPriceCalculationService priceCalculationService,
			IPermissionService permissionService,
			ITaxService taxService,
			IPictureService pictureService,
			IMeasureService measureService,
			IUrlRecordService urlRecordService,
			IProductModelFactory productModelFactory,
			IOrderService orderService,
			OrderSettings orderSettings,
			IOrderProcessingService orderProcessingService,
			IProductAttributeParser productAttributeParser,
			ILogger logger,
			ShoppingCartSettings shoppingCartSettings,
			IShoppingCartModelFactory shoppingCartModelFactory)
		{
			_specificationAttributesSettings = specificationAttributesSettings;
			_customerService = customerService;
			_languageService = languageService;
			_settingService = settingService;
			_storeService = storeService;
			_localizationService = localizationService;
			_workContext = workContext;
			_genericAttributeService = genericAttributeService;
			_vendorService = vendorService;
			_productService = productService;
			_storeContext = storeContext;
			_specificationAttributeService = specificationAttributeService;
			_webHelper = webHelper;
			_priceFormatter = priceFormatter;
			_currencyService = currencyService;
			_catalogSettings = catalogSettings;
			_categoryService = categoryService;
			_cacheManager = cacheManager;
			_mediaSettings = mediaSettings;
			_priceCalculationService = priceCalculationService;
			_permissionService = permissionService;
			_taxService = taxService;
			_pictureService = pictureService;
			_measureService = measureService;
			_urlRecordService = urlRecordService;
			_productModelFactory = productModelFactory;
			_orderSettings = orderSettings;
			_orderProcessingService = orderProcessingService;
			_orderService = orderService;
			_productAttributeParser = productAttributeParser;
			_logger = logger;
			this._shoppingCartSettings = shoppingCartSettings;
			this._shoppingCartModelFactory = shoppingCartModelFactory;
		}

		public IViewComponentResult Invoke(string widgetZone, object additionalData)
		{
			var id = Convert.ToInt32(additionalData);
			if (id <= 0)
				return Content("");

			var category = _categoryService.GetCategoryById(id);
			if (category == null)
				return Content("");

			if (category.PictureId <= 0)
				return Content("");

			var pictureSize = _mediaSettings.CategoryThumbPictureSize;
			var picture = _pictureService.GetPictureById(category.PictureId);
			var pictureModel = new PictureModel
			{
				FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
				ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
				Title = string.Format(_localizationService.GetResource("Media.Category.ImageLinkTitleFormat"), category.Name),
				AlternateText = string.Format(_localizationService.GetResource("Media.Category.ImageAlternateTextFormat"), category.Name)
			};

			return View("~/Plugins/Products.SpecificationAttributes/Views/SpecificationAttributes/CategoryImage.cshtml", pictureModel);
		}
	}
}