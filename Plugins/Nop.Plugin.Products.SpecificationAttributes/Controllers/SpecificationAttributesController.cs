using System.Web.Mvc;
using Nop.Web.Framework.Controllers;
using Nop.Services.Customers;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Plugin.Products.SpecificationAttributes.Models;
using Nop.Services.Configuration;
using Nop.Services.Stores;
using Nop.Core;
using Nop.Services.Vendors;
using Nop.Services.Catalog;
using System;
using System.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Security;
using Nop.Web.Models.Catalog;
using Nop.Core.Domain.Customers;
using System.Collections.Generic;
using Nop.Core.Caching;
using Nop.Core.Domain.Media;
using Nop.Web.Infrastructure.Cache;
using Nop.Services.Security;
using Nop.Services.Tax;
using Nop.Services.Media;
using Nop.Services.Directory;
using Nop.Web.Controllers;
using Nop.Web.Extensions;
using Nop.Services.Seo;
using Nop.Web.Models.Media;
using Nop.Web.Factories;
using System.Collections;
using Nop.Services.Orders;
using Nop.Core.Domain.Orders;

namespace Nop.Plugin.Products.SpecificationAttributes.Controllers
{
    public class SpecificationAttributesController : BasePluginController
    {
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

        public SpecificationAttributesController(
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
            IProductAttributeParser productAttributeParser)
        {
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
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            ConfigureModel model = new ConfigureModel();
            return View("~/Plugins/Products.SpecificationAttributes/Views/SpecificationAttributes/Configure.cshtml", model);
        }

        [AdminAuthorize]
        public ActionResult ProductTab(int id)
        {
            ConfigureModel model = new ConfigureModel();
            model.Id = id;

            var product = _productService.GetProductById(id);
            if (product != null)
            {
                if (product.ProductSpecificationAttributes.Any())
                {
                    var prodSpecAttriOptions = product.ProductSpecificationAttributes.Select(x => x.SpecificationAttributeOption);
                    if (prodSpecAttriOptions.Any())
                    {
                        var specAttr = prodSpecAttriOptions.Where(x => x.SpecificationAttribute.Name == "Artist").FirstOrDefault();
                        if (specAttr != null)
                        {
                            model.ArtistName = specAttr.Name;
                        }
                    }
                }
            }

            return View("~/Plugins/Products.SpecificationAttributes/Views/SpecificationAttributes/ProductTab.cshtml", model);
        }

        [HttpPost]
        public ActionResult ProductTabPost(int id, string ArtistName)
        {
            var product = _productService.GetProductById(id);
            if (product != null)
            {
                // _genericAttributeService.SaveAttribute<string>(product, ProductArtistAttributeNames.ArtistName, ArtistName, _storeContext.CurrentStore.Id);

                var artistAttr = _specificationAttributeService.GetSpecificationAttributes().Where(x => x.Name == "Artist").FirstOrDefault();
                if (artistAttr != null)
                {
                    var value = artistAttr.SpecificationAttributeOptions.Where(x => x.Name == ArtistName).FirstOrDefault();
                    if (value != null)
                    {
                        ProductSpecificationAttribute psA = new ProductSpecificationAttribute();
                        psA.SpecificationAttributeOption = value;
                        psA.ProductId = product.Id;
                        psA.AllowFiltering = true;
                        psA.AttributeType = SpecificationAttributeType.Option;
                        psA.ShowOnProductPage = true;
                        _specificationAttributeService.InsertProductSpecificationAttribute(psA);
                    }
                    else
                    {
                        value = new SpecificationAttributeOption
                        {
                            Name = ArtistName,
                            SpecificationAttributeId = artistAttr.Id
                        };
                        _specificationAttributeService.InsertSpecificationAttributeOption(value);

                        ProductSpecificationAttribute psA = new ProductSpecificationAttribute();
                        psA.SpecificationAttributeOption = value;
                        psA.ProductId = product.Id;
                        psA.AllowFiltering = true;
                        psA.AttributeType = SpecificationAttributeType.Option;
                        psA.ShowOnProductPage = true;
                        _specificationAttributeService.InsertProductSpecificationAttribute(psA);
                    }
                }
            }

            return Content(ArtistName);
        }

        [ChildActionOnly]
        public ActionResult PublicInfo(string widgetZone, object additionalData = null)
        {
            var id = 0;
            object o = additionalData;
            IEnumerable list = o as IEnumerable;
            if (list != null)
            {
                IEnumerable<Nop.Web.Models.Order.CustomerOrderListModel.OrderDetailsModel> c = list.Cast<Nop.Web.Models.Order.CustomerOrderListModel.OrderDetailsModel>();//.OfType<CustomerOrderListModel.OrderDetailsModel>();

                if (widgetZone == "order_listing_widget")
                {
                    return OrderListing(c.ToList());
                }
            }
            else
            {
                id = Convert.ToInt32(additionalData);
                if (id <= 0)
                    return Content("");

                var product = _productService.GetProductById(id);
                if (product == null)
                    return Content("");
                //if (widgetZone == "orderdetails_product_line")
                //{
                //    return OrderProductImage(product.Id);
                //}
                if (widgetZone == "product_by_artist")
                {
                    return ArtistProducts(product.Id);
                }
             
                if (widgetZone == "product_listing_widget" || widgetZone == "product_details_widget")
                {
                    var products = new List<Product>();
                    products.Add(product);

                    ViewBag.ProductId = product.Id;
                    ViewBag.SeName = product.GetSeName();

                    var specAttr = _specificationAttributeService.GetProductSpecificationAttributes(product.Id);
                    var imageSpecAttrOption = specAttr.Select(x => x.SpecificationAttributeOption);

                    if (imageSpecAttrOption.Any())
                    {
                        var defaultColorOption = imageSpecAttrOption.Where(x => x.SpecificationAttribute.Name == "DefaultEnvelopeColor");
                        var defaultEnvelopType = imageSpecAttrOption.Where(x => x.SpecificationAttribute.Name == "Orientation");

                        // Background Image / Pic
                        if (defaultEnvelopType.Any())
                        {
                            string className = defaultEnvelopType.FirstOrDefault().Name;
                            ViewBag.ClassName = className;
                            if (defaultColorOption.Any())
                            {
                                var optionValue = defaultColorOption.FirstOrDefault().ColorSquaresRgb;
                                ViewBag.DefaultColor = optionValue;
                                if (optionValue.Contains("#") && optionValue.Length == 7)
                                {
                                    ViewBag.DefaultColor = "background-color:" + optionValue;
                                }
                                else
                                {
                                    ViewBag.DefaultColor = "background-image:url('" + optionValue + "')";
                                }
                            }
                            else
                            {
                                ViewBag.DefaultColor = "";
                            }

                            //Prodcut Detail
                            if (widgetZone == "product_details_widget")
                            {
                                var productDetailsModel = _productModelFactory.PrepareProductDetailsModel(product, null, false);
                                return View("~/Plugins/Products.SpecificationAttributes/Views/SpecificationAttributes/ImageBackgroundDetail.cshtml", productDetailsModel);
                            }
                            var productOverviewModel = _productModelFactory.PrepareProductOverviewModels(products, false, true, null, false, false).FirstOrDefault();
                            return View("~/Plugins/Products.SpecificationAttributes/Views/SpecificationAttributes/ImageBackground.cshtml", productOverviewModel);
                        }

                    }
                    else
                    {
                       var productOverviewModel = _productModelFactory.PrepareProductOverviewModels(products, false, true, null, false, false).FirstOrDefault();
                        return View("~/Plugins/Products.SpecificationAttributes/Views/SpecificationAttributes/ImageBackground.cshtml", productOverviewModel);
                    }

                    //Product Detail
                    if (widgetZone == "product_details_widget")
                    {
                        var productDetailsModel = _productModelFactory.PrepareProductDetailsModel(product, null, false);
                        return View("~/Plugins/Products.SpecificationAttributes/Views/SpecificationAttributes/ImageBackgroundDetail.cshtml", productDetailsModel);
                    }
                    var productOverviewModels = _productModelFactory.PrepareProductOverviewModels(products, false, true, null, false, false).FirstOrDefault();
                    return View("~/Plugins/Products.SpecificationAttributes/Views/SpecificationAttributes/ImageBackground.cshtml", productOverviewModels);
                }

                //Product Category
                if (widgetZone == "product_category_titles")
                {
                    IEnumerable<ProductCategoryTitleModel> productCategoriesModel = new List<ProductCategoryTitleModel>();
                    var categories = product.ProductCategories.Select(x => x.Category);
                    productCategoriesModel = categories.Select(x =>
                    {
                        return new ProductCategoryTitleModel
                        {
                            CategoryName = x.Name,
                            SeName = x.GetSeName()
                        };
                    });
                    return View("~/Plugins/Products.SpecificationAttributes/Views/SpecificationAttributes/PoductCategoriesTitles.cshtml", productCategoriesModel);
                }

                // Artist 
                if (product.ProductSpecificationAttributes.Any())
                {
                    var prodSpecAttriOptions = product.ProductSpecificationAttributes.Select(x => x.SpecificationAttributeOption);
                  
                    var categories = product.ProductCategories.Select(x => x.Category);
                    var storeId = _storeContext.CurrentStore.Id;
                    var artistcategory = _categoryService.GetAllCategories("Artist", storeId).Where(x => x.Name == "Artist").FirstOrDefault();
                    if (artistcategory != null)
                    {
                        if (prodSpecAttriOptions.Any())
                        {
                            var seName = categories.Where(x => x.ParentCategoryId == artistcategory.Id);
                            var specAttr = prodSpecAttriOptions.Where(x => x.SpecificationAttribute.Name == "Artist").FirstOrDefault();
                            if (specAttr != null)
                            {                               
                                if (seName.Any())
                                {
                                    ViewBag.seName = seName.Select(a => a.GetSeName()).First();
                                    ViewBag.ArtistName = specAttr.Name;
                                    return View("~/Plugins/Products.SpecificationAttributes/Views/SpecificationAttributes/PublicInfo.cshtml");
                                }
                                else
                                {
                                    return Content("");
                                }
                            }
                            else
                            {
                                if (seName.Any())
                                {
                                    ViewBag.seName = seName.Select(a => a.GetSeName()).First();
                                    ViewBag.ArtistName = seName.Select(x => x.Name ).First();
                                    return View("~/Plugins/Products.SpecificationAttributes/Views/SpecificationAttributes/PublicInfo.cshtml");
                                }
                                else
                                {
                                    return Content("");
                                }
                            }
                        }
                    }
                    else
                    {
                        return Content("");
                    }
                }
            }
            return Content("");
        }

        [ChildActionOnly]
        public ActionResult OrderListing(IEnumerable<Nop.Web.Models.Order.CustomerOrderListModel.OrderDetailsModel> list)
        {
            var model1 = new List<CustomerOrderListModel.OrderDetailsModel>();
            if (list != null)
            {

                foreach (var l in list)
                {
                    var order = _orderService.GetOrderById(l.Id);
                    var proitems = order.OrderItems;
                    var firstName = order.ShippingAddress != null ? order.ShippingAddress.FirstName : "";
                    var lastName = order.ShippingAddress != null ? order.ShippingAddress.LastName : "";

                    var model = new CustomerOrderListModel.OrderDetailsModel()
                    {
                        Id = l.Id,
                        OrderTotal = l.OrderTotal,
                        OrderStatus = l.OrderStatus,
                        PaymentStatus = l.PaymentStatus,
                        ShippingStatus = l.ShippingStatus,
                        CreatedOn = l.CreatedOn,
                        IsReturnRequestAllowed = _orderProcessingService.IsReturnRequestAllowed(order),
                        IsReOrderAllowed = _orderSettings.IsReOrderAllowed,
                        ShipTo = firstName + " " + lastName
                    };

                    foreach (var orderItem in proitems)
                    {
                        var products = new List<Product>();
                        products.Add(orderItem.Product);
                        var Products = _productModelFactory.PrepareProductOverviewModels(products, false, true, null, false, false).FirstOrDefault();
                        var product = _productService.GetProductById(orderItem.ProductId);
                        var productDetailsModel = _productModelFactory.PrepareProductDetailsModel(product, null, false);
                        var orderItemModel = new CustomerOrderListModel.OrderItemModel
                        {
                            Id = orderItem.Id,
                            OrderItemGuid = orderItem.OrderItemGuid,
                            Sku = orderItem.Product.FormatSku(orderItem.AttributesXml,  _productAttributeParser),
                            ProductId = orderItem.Product.Id,
                            ProductName = orderItem.Product.GetLocalized(x => x.Name),
                            ProductSeName = orderItem.Product.GetSeName(),
                            Quantity = orderItem.Quantity,
                            AttributeInfo = orderItem.AttributeDescription,
                            UnitPrice = _priceFormatter.FormatPrice(orderItem.UnitPriceInclTax),
                            ImageUrl = Products.DefaultPictureModel.ImageUrl

                        };
                        var specAttr = _specificationAttributeService.GetProductSpecificationAttributes(orderItem.Product.Id);
                        var imageSpecAttrOption = specAttr.Select(x => x.SpecificationAttributeOption);
                        if (imageSpecAttrOption.Any())
                        {
                            var defaultColorOption = imageSpecAttrOption.Where(x => x.SpecificationAttribute.Name == "DefaultEnvelopeColor");
                            var defaultEnvelopType = imageSpecAttrOption.Where(x => x.SpecificationAttribute.Name == "Orientation");
                            if (defaultEnvelopType.Any())
                            {
                                string className = defaultEnvelopType.FirstOrDefault().Name;
                                orderItemModel.ClassName = className;

                                if (defaultColorOption.Any())
                                {
                                    var optionValue = defaultColorOption.FirstOrDefault().ColorSquaresRgb;
                                    orderItemModel.DefaultColor = optionValue;
                                    if (optionValue.Contains("#") && optionValue.Length == 7)
                                    {
                                        orderItemModel.DefaultColor = "background-color:" + optionValue;
                                    }
                                    else
                                    {
                                        orderItemModel.DefaultColor = "background-image:url('" + optionValue + "')";
                                    }
                                }
                                else
                                {
                                    orderItemModel.DefaultColor = "";
                                }
                            }
                        }
                        //rental info
                        if (orderItem.Product.IsRental)
                        {
                            var rentalStartDate = orderItem.RentalStartDateUtc.HasValue ? orderItem.Product.FormatRentalDate(orderItem.RentalStartDateUtc.Value) : "";
                            var rentalEndDate = orderItem.RentalEndDateUtc.HasValue ? orderItem.Product.FormatRentalDate(orderItem.RentalEndDateUtc.Value) : "";
                            orderItemModel.RentalInfo = string.Format(_localizationService.GetResource("Order.Rental.FormattedDate"),
                                rentalStartDate, rentalEndDate);
                        }
                        model.Items.Add(orderItemModel);
                    }
                    model1.Add(model);
                }
            }
            return View("~/Plugins/Products.SpecificationAttributes/Views/SpecificationAttributes/OrderListing.cshtml", model1.ToList());
        }

        [ChildActionOnly]
        public ActionResult OrderProductImage(int additionalData)
        {
            var id = Convert.ToInt32(additionalData);

            if (id <= 0)
                return Content("");

            var orderitem = _orderService.GetOrderItemById(id);
            var product = _productService.GetProductById(orderitem.ProductId);

            if (product == null)
                return Content("");

            var specAttr = _specificationAttributeService.GetProductSpecificationAttributes(product.Id);
            var imageSpecAttrOption = specAttr.Select(x => x.SpecificationAttributeOption);
            if (imageSpecAttrOption.Any())
            {
                var defaultColorOption = imageSpecAttrOption.Where(x => x.SpecificationAttribute.Name == "DefaultEnvelopeColor");
                var defaultEnvelopType = imageSpecAttrOption.Where(x => x.SpecificationAttribute.Name == "Orientation");
                if (defaultEnvelopType.Any())
                {
                    string className = defaultEnvelopType.FirstOrDefault().Name;
                    ViewBag.ClassName = className;

                    if (defaultColorOption.Any())
                    {
                        var optionValue = defaultColorOption.FirstOrDefault().ColorSquaresRgb;
                        ViewBag.DefaultColor = optionValue;
                        if (optionValue.Contains("#") && optionValue.Length == 7)
                        {
                            ViewBag.DefaultColor = "background-color:" + optionValue;
                        }
                        else
                        {
                            ViewBag.DefaultColor = "background-image:url('" + optionValue + "')";
                        }
                    }
                    else
                    {
                        ViewBag.DefaultColor = "";
                    }
                    ViewBag.ProductId = product.Id;
                }
            }
            var products = new List<Product>();
            products.Add(product);
            var productPicModel = _productModelFactory.PrepareProductOverviewModels(products, false, true, null, false, false).FirstOrDefault();
            ViewBag.ProductId = product.Id;
            return View("~/Plugins/Products.SpecificationAttributes/Views/SpecificationAttributes/OrderProductImage.cshtml", productPicModel);
        }

        public ActionResult ArtistProducts(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null)
                return Content("");

            ProductByArtistModel model = new ProductByArtistModel();

            var prodSpecAttriOptions = product.ProductSpecificationAttributes.Select(x => x.SpecificationAttributeOption);
            if (prodSpecAttriOptions.Any())
            {
                var specAttr = prodSpecAttriOptions.Where(x => x.SpecificationAttribute.Name == "Artist").FirstOrDefault();
                if (specAttr != null)
                {
                    var artistId = specAttr.Id;
                    var artistName = specAttr.Name;

                    model.ArtistName = artistName;
                    model.Id = artistId;

                    IList<int> alreadyFilteredSpecOptionIds = new List<int>() { artistId };
                    IList<int> filterableSpecificationAttributeOptionIds;
                    var products = _productService.SearchProducts(out filterableSpecificationAttributeOptionIds,
                                   true,
                                   categoryIds: null,
                                   storeId: _storeContext.CurrentStore.Id,
                                   visibleIndividuallyOnly: true,
                                   featuredProducts: _catalogSettings.IncludeFeaturedProductsInNormalLists ? null : (bool?)false,
                                   filteredSpecs: alreadyFilteredSpecOptionIds,
                                   orderBy: ProductSortingEnum.Position,
                                   pageIndex: 0,
                                   pageSize: int.MaxValue);
                    model.Products = _productModelFactory.PrepareProductOverviewModels(products).ToList();

                    return View("~/Plugins/Products.SpecificationAttributes/Views/SpecificationAttributes/ArtistProducts.cshtml", model);
                }

                return Content("");
            }

            return Content("");
        }

        [NopHttpsRequirement(SslRequirement.No)]
        public ActionResult ArtistCategory(int specs, CatalogPagingFilteringModel command)
        {
            var artistAttr = _specificationAttributeService.GetSpecificationAttributes().Where(x => x.Name == "Artist").FirstOrDefault();
            if (artistAttr != null)
            {
                var value = artistAttr.SpecificationAttributeOptions.Where(x => x.Id == specs).FirstOrDefault();
                if (value != null)
                    ViewBag.Name = value.Name;
                else
                    ViewBag.Name = "";
            }

            //'Continue shopping' URL
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
            SystemCustomerAttributeNames.LastContinueShoppingPage,
            _webHelper.GetThisPageUrl(false),
            _storeContext.CurrentStore.Id);

            var model = new SearchModel();

            //sorting
            PrepareSortingOptions(model.PagingFilteringContext, command);
            //view mode
            PrepareViewModes(model.PagingFilteringContext, command);
            //page size
            PreparePageSizeOptions(model.PagingFilteringContext, command,
                 _catalogSettings.SearchPageAllowCustomersToSelectPageSize,
                 _catalogSettings.SearchPagePageSizeOptions,
                 _catalogSettings.SearchPageProductsPerPage);

            var pictureSize = _mediaSettings.CategoryThumbPictureSize;

            //products
            IList<int> alreadyFilteredSpecOptionIds = model.PagingFilteringContext.SpecificationFilter.GetAlreadyFilteredSpecOptionIds(_webHelper);
            IList<int> filterableSpecificationAttributeOptionIds;
            var products = _productService.SearchProducts(out filterableSpecificationAttributeOptionIds,
                true,
                categoryIds: null,
                storeId: _storeContext.CurrentStore.Id,
                visibleIndividuallyOnly: true,
                featuredProducts: _catalogSettings.IncludeFeaturedProductsInNormalLists ? null : (bool?)false,
                filteredSpecs: alreadyFilteredSpecOptionIds,
                orderBy: (ProductSortingEnum)command.OrderBy,
                pageIndex: command.PageNumber - 1,
                pageSize: command.PageSize);
                model.Products = _productModelFactory.PrepareProductOverviewModels(products).ToList();

                model.PagingFilteringContext.LoadPagedList(products);

            //specs
            model.PagingFilteringContext.SpecificationFilter.PrepareSpecsFilters(alreadyFilteredSpecOptionIds,
                filterableSpecificationAttributeOptionIds != null ? filterableSpecificationAttributeOptionIds.ToArray() : null,
                _specificationAttributeService,
                _webHelper,
                _workContext,
                _cacheManager);

            return View("~/Plugins/Products.SpecificationAttributes/Views/SpecificationAttributes/ArtistCategory.cshtml", model);
        }

        public ActionResult CategoryImage(string widgetZone, object additionalData = null)
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

        #region Utilities
           
        [NonAction]
        protected virtual void PrepareSortingOptions(CatalogPagingFilteringModel pagingFilteringModel, CatalogPagingFilteringModel command)
        {
            if (pagingFilteringModel == null)
                throw new ArgumentNullException("pagingFilteringModel");

            if (command == null)
                throw new ArgumentNullException("command");

            var allDisabled = _catalogSettings.ProductSortingEnumDisabled.Count == Enum.GetValues(typeof(ProductSortingEnum)).Length;
            pagingFilteringModel.AllowProductSorting = _catalogSettings.AllowProductSorting && !allDisabled;

            var activeOptions = Enum.GetValues(typeof(ProductSortingEnum)).Cast<int>()
                .Except(_catalogSettings.ProductSortingEnumDisabled)
                .Select((idOption) =>
                {
                    int order;
                    return new KeyValuePair<int, int>(idOption, _catalogSettings.ProductSortingEnumDisplayOrder.TryGetValue(idOption, out order) ? order : idOption);
                })
                .OrderBy(x => x.Value);
            if (command.OrderBy == null)
                command.OrderBy = allDisabled ? 0 : activeOptions.First().Key;

            if (pagingFilteringModel.AllowProductSorting)
            {
                foreach (var option in activeOptions)
                {
                    var currentPageUrl = _webHelper.GetThisPageUrl(true);
                    var sortUrl = _webHelper.ModifyQueryString(currentPageUrl, "orderby=" + (option.Key).ToString(), null);

                    var sortValue = ((ProductSortingEnum)option.Key).GetLocalizedEnum(_localizationService, _workContext);
                    pagingFilteringModel.AvailableSortOptions.Add(new SelectListItem
                    {
                        Text = sortValue,
                        Value = sortUrl,
                        Selected = option.Key == command.OrderBy
                    });
                }
            }
        }

        [NonAction]
        protected virtual void PrepareViewModes(CatalogPagingFilteringModel pagingFilteringModel, CatalogPagingFilteringModel command)
        {
            if (pagingFilteringModel == null)
                throw new ArgumentNullException("pagingFilteringModel");

            if (command == null)
                throw new ArgumentNullException("command");

            pagingFilteringModel.AllowProductViewModeChanging = _catalogSettings.AllowProductViewModeChanging;

            var viewMode = !string.IsNullOrEmpty(command.ViewMode)
                ? command.ViewMode
                : _catalogSettings.DefaultViewMode;
            pagingFilteringModel.ViewMode = viewMode;
            if (pagingFilteringModel.AllowProductViewModeChanging)
            {
                var currentPageUrl = _webHelper.GetThisPageUrl(true);
                //grid
                pagingFilteringModel.AvailableViewModes.Add(new SelectListItem
                {
                    Text = _localizationService.GetResource("Catalog.ViewMode.Grid"),
                    Value = _webHelper.ModifyQueryString(currentPageUrl, "viewmode=grid", null),
                    Selected = viewMode == "grid"
                });
                //list
                pagingFilteringModel.AvailableViewModes.Add(new SelectListItem
                {
                    Text = _localizationService.GetResource("Catalog.ViewMode.List"),
                    Value = _webHelper.ModifyQueryString(currentPageUrl, "viewmode=list", null),
                    Selected = viewMode == "list"
                });
            }

        }

        [NonAction]
        protected virtual void PreparePageSizeOptions(CatalogPagingFilteringModel pagingFilteringModel, CatalogPagingFilteringModel command,
            bool allowCustomersToSelectPageSize, string pageSizeOptions, int fixedPageSize)
        {
            if (pagingFilteringModel == null)
                throw new ArgumentNullException("pagingFilteringModel");

            if (command == null)
                throw new ArgumentNullException("command");

            if (command.PageNumber <= 0)
            {
                command.PageNumber = 1;
            }
            pagingFilteringModel.AllowCustomersToSelectPageSize = false;
            if (allowCustomersToSelectPageSize && pageSizeOptions != null)
            {
                var pageSizes = pageSizeOptions.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (pageSizes.Any())
                {
                    // get the first page size entry to use as the default (category page load) or if customer enters invalid value via query string
                    if (command.PageSize <= 0 || !pageSizes.Contains(command.PageSize.ToString()))
                    {
                        int temp;
                        if (int.TryParse(pageSizes.FirstOrDefault(), out temp))
                        {
                            if (temp > 0)
                            {
                                command.PageSize = temp;
                            }
                        }
                    }

                    var currentPageUrl = _webHelper.GetThisPageUrl(true);
                    var sortUrl = _webHelper.ModifyQueryString(currentPageUrl, "pagesize={0}", null);
                    sortUrl = _webHelper.RemoveQueryString(sortUrl, "pagenumber");

                    foreach (var pageSize in pageSizes)
                    {
                        int temp;
                        if (!int.TryParse(pageSize, out temp))
                        {
                            continue;
                        }
                        if (temp <= 0)
                        {
                            continue;
                        }

                        pagingFilteringModel.PageSizeOptions.Add(new SelectListItem
                        {
                            Text = pageSize,
                            Value = String.Format(sortUrl, pageSize),
                            Selected = pageSize.Equals(command.PageSize.ToString(), StringComparison.InvariantCultureIgnoreCase)
                        });
                    }

                    if (pagingFilteringModel.PageSizeOptions.Any())
                    {
                        pagingFilteringModel.PageSizeOptions = pagingFilteringModel.PageSizeOptions.OrderBy(x => int.Parse(x.Text)).ToList();
                        pagingFilteringModel.AllowCustomersToSelectPageSize = true;

                        if (command.PageSize <= 0)
                        {
                            command.PageSize = int.Parse(pagingFilteringModel.PageSizeOptions.FirstOrDefault().Text);
                        }
                    }
                }
            }
            else
            {
                //customer is not allowed to select a page size
                command.PageSize = fixedPageSize;
            }

            //ensure pge size is specified
            if (command.PageSize <= 0)
            {
                command.PageSize = fixedPageSize;
            }
        }

        [NonAction]
        protected virtual List<int> GetChildCategoryIds(int parentCategoryId)
        {
            string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_CHILD_IDENTIFIERS_MODEL_KEY,
                parentCategoryId,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id);
            return _cacheManager.Get(cacheKey, () =>
            {
                var categoriesIds = new List<int>();
                var categories = _categoryService.GetAllCategoriesByParentCategoryId(parentCategoryId);
                foreach (var category in categories)
                {
                    categoriesIds.Add(category.Id);
                    categoriesIds.AddRange(GetChildCategoryIds(category.Id));
                }
                return categoriesIds;
            });
        }

        #endregion
    }
}