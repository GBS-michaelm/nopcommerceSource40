using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Plugin.DataAccess.GBS;
using Nop.Plugin.Order.GBS.Orders;
using Nop.Plugin.Products.SpecificationAttributes.Models;
using Nop.Plugin.Widgets.CustomersCanvas.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Custom.Orders;
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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Nop.Plugin.Order.GBS.Orders.OrderExtensions;

namespace Nop.Plugin.Products.SpecificationAttributes.Components
{
	[ViewComponent(Name = "SpecificationAttributesPublicInfo")]
	public class SpecificationAttributesPublicInfoViewComponent : NopViewComponent
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
		private readonly IGBSOrderService _gbsOrderService;
		private readonly IProductAttributeParser parser = EngineContext.Current.Resolve<IProductAttributeParser>();
		private readonly ICcService ccService = EngineContext.Current.Resolve<ICcService>();
		private readonly ShoppingCartSettings _shoppingCartSettings;
		private readonly IShoppingCartModelFactory _shoppingCartModelFactory;

		public SpecificationAttributesPublicInfoViewComponent(
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
			IShoppingCartModelFactory shoppingCartModelFactory,
            IGBSOrderService gbsOrderService)
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
            this._gbsOrderService = gbsOrderService; //EngineContext.Current.Resolve<GBSOrderService>(); // (GBSOrderService)System.Web.Mvc.DependencyResolver.Current.GetServices(typeof(GBSOrderService)).Where(x => x is GBSOrderService).FirstOrDefault();
			this._shoppingCartSettings = shoppingCartSettings;
			this._shoppingCartModelFactory = shoppingCartModelFactory;
		}

		public IViewComponentResult Invoke(string widgetZone, object additionalData)
		{
			var id = 0;
			object o = additionalData;
			IEnumerable list = o as IEnumerable;
			if (list != null)
			{
				IEnumerable<Nop.Web.Models.Order.CustomerOrderListModel.OrderDetailsModel> c = list.Cast<Nop.Web.Models.Order.CustomerOrderListModel.OrderDetailsModel>();
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

				if (widgetZone == "orderdetails_product_line_product")
				{
					var orderitem = _gbsOrderService.GetOrderItemById(id);
					if (orderitem != null)
					{
						if (orderitem is LegacyOrderItem && ((LegacyOrderItem)orderitem).webToPrintType == "canvas")
						{
							//var gbsOrdercontroller = System.Web.Mvc.DependencyResolver.Current.GetService(typeof(GBSOrderController)) as GBSOrderController;
							//gbsOrdercontroller.ControllerContext = new ControllerContext(this.Request.HttpContext, gbsOrdercontroller);
							//return gbsOrdercontroller.DisplayLegacyOrderItemImage("",orderitem.Id);
							return Content("");
						}
						else
						{
							return OrderProductImage(orderitem);
						}
					}
					return null;
				}

				Product product = null;
				ShoppingCartItem shoppingCartItem = null;
				if (widgetZone == "shoppingcart_custom_image")
				{
					shoppingCartItem = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.Id == id).FirstOrDefault();
					product = shoppingCartItem.Product;
				}
				else
				{
					product = _productService.GetProductById(id);

				}

				if (product == null)
					return Content("");

				if (widgetZone == "product_by_artist")
				{
					return ArtistProducts(product.Id);
				}

				if (widgetZone == "product_listing_widget" || widgetZone == "product_details_widget" || widgetZone == "shoppingcart_custom_image")
				{
					var products = new List<Product>();
					products.Add(product);

					ViewBag.ProductId = product.Id;
					ViewBag.SeName = product.GetSeName();

					var specAttr = _specificationAttributeService.GetProductSpecificationAttributes(product.Id);
					var imageSpecAttrOption = specAttr.Select(x => x.SpecificationAttributeOption);

					if (imageSpecAttrOption.Any())
					{
						var thumbnailBackground = imageSpecAttrOption.Where(x => x.SpecificationAttribute.Name == "Treatment");
						if (thumbnailBackground.Any())
						{	
							foreach (var thbackground in thumbnailBackground)
							{
								var gbsBackGroundName = thbackground.Name;
								ViewBag.fill = "";
								ViewBag.ClassName = "";
								if (gbsBackGroundName.Any())
								{
									switch (gbsBackGroundName)
									{
										case "TreatmentImage":
											var backGroundShapeName = imageSpecAttrOption.Where(x => x.SpecificationAttribute.Name == gbsBackGroundName);
											if (backGroundShapeName.Any())
											{
												ViewBag.ClassName = backGroundShapeName.FirstOrDefault().Name;
											}
											break;
										case "TreatmentFill":
											var backGroundFillOption = imageSpecAttrOption.Where(x => x.SpecificationAttribute.Name == gbsBackGroundName);
											if (backGroundFillOption.Any())
											{
												var fillOptionValue = backGroundFillOption.FirstOrDefault().Name;
												switch (fillOptionValue)
												{
													case "TreatmentFillPattern":
														var img = imageSpecAttrOption.Where(x => x.SpecificationAttribute.Name == backGroundFillOption.FirstOrDefault().Name);
														if (img.Any())
														{
															ViewBag.fill = "background-image:url('" + img.FirstOrDefault().ColorSquaresRgb + "')";

														}
														break;
													case "TreatmentFillColor":
														var color = imageSpecAttrOption.Where(x => x.SpecificationAttribute.Name == backGroundFillOption.FirstOrDefault().Name);
														if (color.Any())
														{
															ViewBag.fill = "background-color:" + color.FirstOrDefault().ColorSquaresRgb;

														}
														break;
												}
											}
											break;
									}
								}
							}
						}
						else
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
									ViewBag.fill = optionValue;
									if (optionValue.Contains("#") && optionValue.Length == 7)
									{
										ViewBag.fill = "background-color:" + optionValue;
									}
									else
									{
										ViewBag.fill = "background-image:url('" + optionValue + "')";
									}
								}
								else
								{
									ViewBag.fill = "";
								}
								if (widgetZone == "shoppingcart_custom_image")
								{
									//replace envelope color with user selected color
									var productAttributeMappings = parser.ParseProductAttributeMappings(shoppingCartItem.AttributesXml);
									if (productAttributeMappings != null)
									{
										foreach (var productAttributeMapping in productAttributeMappings)
										{
											if (productAttributeMapping.ProductAttribute.Name == "Envelope Color")
											{
												var attrValues = parser.ParseValues(shoppingCartItem.AttributesXml, productAttributeMapping.Id);
												var optionValue = productAttributeMapping.ProductAttributeValues.Where(x => x.Id == Int32.Parse(attrValues[0])).FirstOrDefault().ColorSquaresRgb;
												if (optionValue.Contains("#") && optionValue.Length == 7)
												{
													ViewBag.fill = "background-color:" + optionValue;
												}
												else
												{
													ViewBag.fill = "background-image:url('" + optionValue + "')";
												}
											}

										}
									}
								}

								//Prodcut Detail
								if (widgetZone == "product_details_widget")
								{
									var productDetailsModel = _productModelFactory.PrepareProductDetailsModel(product, null, false);
									return View("~/Plugins/Products.SpecificationAttributes/Views/SpecificationAttributes/ImageBackgroundDetail.cshtml", productDetailsModel);
								}
								if (widgetZone == "shoppingcart_custom_image")
								{
									var productDetailsModel = _productModelFactory.PrepareProductDetailsModel(product, null, false);


									var productAttributeMappings = parser.ParseProductAttributeMappings(shoppingCartItem.AttributesXml);
									if (productAttributeMappings != null)
									{
										foreach (var productAttributeMapping in productAttributeMappings)
										{
											if (productAttributeMapping.ProductAttribute.Name == "CustomImgUrl")
											{
												var attrValues = parser.ParseValues(shoppingCartItem.AttributesXml, productAttributeMapping.Id);
												productDetailsModel.DefaultPictureModel.ImageUrl = attrValues[0];
											}
											else if (_shoppingCartSettings.ShowProductImagesOnShoppingCart)
											{
												productDetailsModel.DefaultPictureModel.ImageUrl = _shoppingCartModelFactory.PrepareCartItemPictureModel(shoppingCartItem,
													_mediaSettings.CartThumbPictureSize, true, shoppingCartItem.Product.Name).ImageUrl;
											}

										}
									}
									return View("~/Plugins/Products.SpecificationAttributes/Views/SpecificationAttributes/ImageBackgroundDetail.cshtml", productDetailsModel);

								}
								var productOverviewModel = _productModelFactory.PrepareProductOverviewModels(products, false, true, null, false, false).FirstOrDefault();
								return View("~/Plugins/Products.SpecificationAttributes/Views/SpecificationAttributes/ImageBackground.cshtml", productOverviewModel);
							}

						}
					}
					else
					{
						var productOverviewModel = _productModelFactory.PrepareProductOverviewModels(products, false, true, null, false, false).FirstOrDefault();
						if (widgetZone == "shoppingcart_custom_image" && _shoppingCartSettings.ShowProductImagesOnShoppingCart)
						{
							productOverviewModel.DefaultPictureModel.ImageUrl = _shoppingCartModelFactory.PrepareCartItemPictureModel(shoppingCartItem,
								_mediaSettings.CartThumbPictureSize, true, shoppingCartItem.Product.Name).ImageUrl;
						}
						return View("~/Plugins/Products.SpecificationAttributes/Views/SpecificationAttributes/ImageBackground.cshtml", productOverviewModel);
					}

					//Product Detail
					if (widgetZone == "product_details_widget")
					{
						var productDetailsModel = _productModelFactory.PrepareProductDetailsModel(product, null, false);
						return View("~/Plugins/Products.SpecificationAttributes/Views/SpecificationAttributes/ImageBackgroundDetail.cshtml", productDetailsModel);
					}
					//Shopping Cart
					if (widgetZone == "shoppingcart_custom_image")
					{
						var productDetailsModel = _productModelFactory.PrepareProductDetailsModel(product, null, false);


						var productAttributeMappings = parser.ParseProductAttributeMappings(shoppingCartItem.AttributesXml);
						if (productAttributeMappings != null)
						{
							foreach (var productAttributeMapping in productAttributeMappings)
							{
								if (productAttributeMapping.ProductAttribute.Name == "CustomImgUrl")
								{
									var attrValues = parser.ParseValues(shoppingCartItem.AttributesXml, productAttributeMapping.Id);
									productDetailsModel.DefaultPictureModel.ImageUrl = attrValues[0];
								}
								else if (_shoppingCartSettings.ShowProductImagesOnShoppingCart)
								{
									productDetailsModel.DefaultPictureModel.ImageUrl = _shoppingCartModelFactory.PrepareCartItemPictureModel(shoppingCartItem,
										_mediaSettings.CartThumbPictureSize, true, shoppingCartItem.Product.Name).ImageUrl;
								}

							}
						}

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
					//var artistcategory = _categoryService.GetAllCategories("Artist", storeId).Where(x => x.Name == "Artist").FirstOrDefault();
					var artistcategory = "";  //changed this to input ID from settings instead of lookup by name for performance reasons.
					if (artistcategory != null)
					{
						if (prodSpecAttriOptions.Any())
						{
							var seName = categories.Where(x => x.ParentCategoryId == _specificationAttributesSettings.ArtistCategoryID);
							// var seName = categories.Where(x => x.ParentCategoryId == artistcategory.Id);
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
									ViewBag.ArtistName = seName.Select(x => x.Name).First();
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

			return View("~/Plugins/Products.SpecificationAttributes/Views/SpecificationAttributes/PublicInfo.cshtml");
		}

		public IViewComponentResult OrderListing(IEnumerable<Nop.Web.Models.Order.CustomerOrderListModel.OrderDetailsModel> list)
		{
			var model1 = new List<CustomerOrderListModel.OrderDetailsModel>();

			if (list != null)
			{

				foreach (var l in list)
				{
					joecounter++;
					Nop.Core.Domain.Orders.Order order = null;

					if (l.CustomProperties.ContainsKey("isLegacy") && Convert.ToBoolean(l.CustomProperties["isLegacy"]) == true)
					{
						order = new Nop.Plugin.Order.GBS.Orders.OrderExtensions().GetOrderById(l.Id, true);
					}
					else
					{
						//order = Nop.Plugin.Order.GBS.Orders.OrderExtensions.GetOrderById(l.Id, false);
						order = _orderService.GetOrderById(l.Id);
						l.CustomProperties["orderNo"] = DBManager.getGBSOrderID(l.Id);
					}
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
						ShipTo = firstName + " " + lastName,
						Monthvalue = (DateTime.Now.Year - l.CreatedOn.Year) * 12 + DateTime.Now.Month - l.CreatedOn.Month,
						CustomProperties = l.CustomProperties
					};

					foreach (var orderItem in proitems)
					{
						//var orderExtensions = new Nop.Plugin.Order.GBS.Orders.OrderExtensions();
						//orderExtensions.prepareLegacyOrderItem(orderItem);
						if (orderItem.Product.Id != 0)
						{
							var products = new List<Product>();
							products.Add(orderItem.Product);
							var Products = _productModelFactory.PrepareProductOverviewModels(products, false, true, null, false, false).FirstOrDefault();
							var product = _productService.GetProductById(orderItem.ProductId);
							if (product == null)
							{
								product = orderItem.Product;
							}
							var productDetailsModel = _productModelFactory.PrepareProductDetailsModel(product, null, false);
							var orderItemModel = new CustomerOrderListModel.OrderItemModel
							{
								Id = orderItem.Id,
								OrderItemGuid = orderItem.OrderItemGuid,
								Sku = orderItem.Product.FormatSku(orderItem.AttributesXml, _productAttributeParser),
								ProductId = orderItem.Product.Id,
								ProductName = orderItem.Product.GetLocalized(x => x.Name),
								ProductSeName = orderItem.Product.GetSeName(),
								Quantity = orderItem.Quantity,
								AttributeInfo = orderItem.AttributeDescription,
								UnitPrice = _priceFormatter.FormatPrice(orderItem.UnitPriceInclTax),
								ImageUrl = Products.DefaultPictureModel.ImageUrl

							};
							var orderItemPicture = product.GetProductPicture(orderItem.AttributesXml, _pictureService, _productAttributeParser);
							orderItemModel.ImageUrl = _pictureService.GetPictureUrl(orderItemPicture, 0, true);

							if (l.CustomProperties.ContainsKey("isLegacy") && Convert.ToBoolean(l.CustomProperties["isLegacy"]) == true)
							{
								Nop.Plugin.Order.GBS.Orders.OrderExtensions.LegacyOrderItem legacyOrderItem = (Nop.Plugin.Order.GBS.Orders.OrderExtensions.LegacyOrderItem)new Nop.Plugin.Order.GBS.Orders.OrderExtensions().GetOrderItemById(orderItem.Id, true);

								orderItemModel.ImageUrl = legacyOrderItem.legacyPicturePath;
							}
							//add custom image
							var productAttributeMappings = parser.ParseProductAttributeMappings(orderItem.AttributesXml);
							if (productAttributeMappings != null)
							{
								foreach (var productAttributeMapping in productAttributeMappings)
								{
									if (productAttributeMapping.ProductAttribute.Name == "CustomImgUrl")
									{
										var attrValues = parser.ParseValues(orderItem.AttributesXml, productAttributeMapping.Id);
										orderItemModel.ImageUrl = attrValues[0];
									}

								}
							}

							var specAttr = _specificationAttributeService.GetProductSpecificationAttributes(orderItem.Product.Id);
							var imageSpecAttrOption = specAttr.Select(x => x.SpecificationAttributeOption);
							if (imageSpecAttrOption.Any())
							{
								var thumbnailBackground = imageSpecAttrOption.Where(x => x.SpecificationAttribute.Name == "Treatment");
								if (thumbnailBackground.Any())
								{	
									foreach (var thbackground in thumbnailBackground)
									{
										var gbsBackGroundName = thbackground.Name;
										ViewBag.fill = "";
										ViewBag.ClassName = "";
										if (gbsBackGroundName.Any())
										{
											switch (gbsBackGroundName)
											{
												case "TreatmentImage":
													var backGroundShapeName = imageSpecAttrOption.Where(x => x.SpecificationAttribute.Name == gbsBackGroundName);
													if (backGroundShapeName.Any())
													{
														ViewBag.ClassName = backGroundShapeName.FirstOrDefault().Name;
														orderItemModel.ClassName = backGroundShapeName.FirstOrDefault().Name;
													}
													break;
												case "TreatmentFill":
													var backGroundFillOption = imageSpecAttrOption.Where(x => x.SpecificationAttribute.Name == gbsBackGroundName);
													if (backGroundFillOption.Any())
													{
														var fillOptionValue = backGroundFillOption.FirstOrDefault().Name;
														switch (fillOptionValue)
														{
															case "TreatmentFillPattern":
																var img = imageSpecAttrOption.Where(x => x.SpecificationAttribute.Name == backGroundFillOption.FirstOrDefault().Name);
																if (img.Any())
																{
																	ViewBag.fill = "background-image:url('" + img.FirstOrDefault().ColorSquaresRgb + "')";
																	orderItemModel.DefaultColor = "background-image:url('" + img.FirstOrDefault().ColorSquaresRgb + "')";

																}
																break;
															case "TreatmentFillColor":
																var color = imageSpecAttrOption.Where(x => x.SpecificationAttribute.Name == backGroundFillOption.FirstOrDefault().Name);
																if (color.Any())
																{
																	ViewBag.fill = "background-color:" + color.FirstOrDefault().ColorSquaresRgb;
																	orderItemModel.DefaultColor = "background-color:" + color.FirstOrDefault().ColorSquaresRgb;

																}
																break;
														}
													}
													break;
											}
										}

									}
								}
								else
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

										//replace envelope color with user selected color and Custom Canvas image.
										productAttributeMappings = parser.ParseProductAttributeMappings(orderItem.AttributesXml);
										if (productAttributeMappings != null)
										{
											foreach (var productAttributeMapping in productAttributeMappings)
											{
												if (productAttributeMapping.ProductAttribute.Name == "Envelope Color")
												{
													var attrValues = parser.ParseValues(orderItem.AttributesXml, productAttributeMapping.Id);
													var optionValue = productAttributeMapping.ProductAttributeValues.Where(x => x.Id == Int32.Parse(attrValues[0])).FirstOrDefault().ColorSquaresRgb;
													if (optionValue.Contains("#") && optionValue.Length == 7)
													{
														orderItemModel.DefaultColor = "background-color:" + optionValue;
													}
													else
													{
														orderItemModel.DefaultColor = "background-image:url('" + optionValue + "')";
													}
												}
												if (productAttributeMapping.ProductAttribute.Name == "CcId")
												{
													var ccResult = ccService.GetCcResult(orderItem.AttributesXml);
													if (ccResult != null && ccResult.ProofUrls.Count() > 0)
													{
														orderItemModel.ImageUrl = ccResult.ProofUrls[0];
													}
												}
											}
										}
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
						else
						{
							var orderItemModel = new CustomerOrderListModel.OrderItemModel
							{
								Id = orderItem.Id,
								OrderItemGuid = orderItem.OrderItemGuid,
								Sku = orderItem.Product.Sku,
								ProductName = orderItem.Product.Name,
								Quantity = orderItem.Quantity,
								AttributeInfo = orderItem.AttributeDescription,
								UnitPrice = _priceFormatter.FormatPrice(orderItem.UnitPriceExclTax),

							};
							model.Items.Add(orderItemModel);
						}
					}
					model1.Add(model);
				}
			}

			return View("~/Plugins/Products.SpecificationAttributes/Views/SpecificationAttributes/OrderListing.cshtml", model1.ToList());
		}

		public IViewComponentResult OrderProductImage(OrderItem orderItem)
		{
			var id = orderItem.Product.Id;    //Convert.ToInt32(productId);

			if (id <= 0)
				return Content("");

			//var orderitem = _orderService.GetOrderItemById(id);
			//var product = _productService.GetProductById(orderitem.ProductId);
			var product = orderItem.Product;  //_productService.GetProductById(id);

			if (product == null)
				return Content("");

			var specAttr = _specificationAttributeService.GetProductSpecificationAttributes(product.Id);
			var imageSpecAttrOption = specAttr.Select(x => x.SpecificationAttributeOption);
			if (imageSpecAttrOption.Any())
			{
				var thumbnailBackground = imageSpecAttrOption.Where(x => x.SpecificationAttribute.Name == "Treatment");
				if (thumbnailBackground.Any())
				{
					foreach (var thbackground in thumbnailBackground)
					{
						var gbsBackGroundName = thbackground.Name;
						ViewBag.fill = "";
						ViewBag.ClassName = "";
						if (gbsBackGroundName.Any())
						{
							switch (gbsBackGroundName)
							{
								case "TreatmentImage":
									var backGroundShapeName = imageSpecAttrOption.Where(x => x.SpecificationAttribute.Name == gbsBackGroundName);
									if (backGroundShapeName.Any())
									{
										ViewBag.ClassName = backGroundShapeName.FirstOrDefault().Name;
									}
									break;
								case "TreatmentFill":
									var backGroundFillOption = imageSpecAttrOption.Where(x => x.SpecificationAttribute.Name == gbsBackGroundName);
									if (backGroundFillOption.Any())
									{
										var fillOptionValue = backGroundFillOption.FirstOrDefault().Name;
										switch (fillOptionValue)
										{
											case "TreatmentFillPattern":
												var img = imageSpecAttrOption.Where(x => x.SpecificationAttribute.Name == backGroundFillOption.FirstOrDefault().Name);
												if (img.Any())
												{
													ViewBag.fill = "background-image:url('" + img.FirstOrDefault().ColorSquaresRgb + "')";

												}
												break;
											case "TreatmentFillColor":
												var color = imageSpecAttrOption.Where(x => x.SpecificationAttribute.Name == backGroundFillOption.FirstOrDefault().Name);
												if (color.Any())
												{
													ViewBag.fill = "background-color:" + color.FirstOrDefault().ColorSquaresRgb;

												}
												break;
										}
									}
									break;
							}
						}
					}
				}
				else
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
							ViewBag.fill = optionValue;
							if (optionValue.Contains("#") && optionValue.Length == 7)
							{
								ViewBag.fill = "background-color:" + optionValue;
							}
							else
							{
								ViewBag.fill = "background-image:url('" + optionValue + "')";
							}
						}
						else
						{
							ViewBag.fill = "";
						}
						//replace envelope color with user selected color
						var productAttributeMappings2 = parser.ParseProductAttributeMappings(orderItem.AttributesXml);
						if (productAttributeMappings2 != null)
						{
							foreach (var productAttributeMapping in productAttributeMappings2)
							{
								if (productAttributeMapping.ProductAttribute.Name == "Envelope Color")
								{
									var attrValues = parser.ParseValues(orderItem.AttributesXml, productAttributeMapping.Id);
									var optionValue = productAttributeMapping.ProductAttributeValues.Where(x => x.Id == Int32.Parse(attrValues[0])).FirstOrDefault().ColorSquaresRgb;
									if (optionValue.Contains("#") && optionValue.Length == 7)
									{
										ViewBag.fill = "background-color:" + optionValue;
									}
									else
									{
										ViewBag.fill = "background-image:url('" + optionValue + "')";
									}
								}


							}
						}
						ViewBag.ProductId = product.Id;
					}
				}

			}
			var products = new List<Product>();
			products.Add(product);
			var productPicModel = _productModelFactory.PrepareProductOverviewModels(products, false, true, null, false, false).FirstOrDefault();
			ViewBag.ProductId = product.Id;
			//custom image
			var orderItemPicture = product.GetProductPicture(orderItem.AttributesXml, _pictureService, _productAttributeParser);
			productPicModel.DefaultPictureModel.ImageUrl = _pictureService.GetPictureUrl(orderItemPicture, 0, true);
			var productAttributeMappings = parser.ParseProductAttributeMappings(orderItem.AttributesXml);
			if (productAttributeMappings != null)
			{
				foreach (var productAttributeMapping in productAttributeMappings)
				{
					if (productAttributeMapping.ProductAttribute.Name == "CustomImgUrl")
					{
						var attrValues = parser.ParseValues(orderItem.AttributesXml, productAttributeMapping.Id);
						productPicModel.DefaultPictureModel.ImageUrl = attrValues[0];
					}

				}
			}
			return View("~/Plugins/Products.SpecificationAttributes/Views/SpecificationAttributes/OrderProductImage.cshtml", productPicModel);
		}

		public IViewComponentResult ArtistProducts(int productId)
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
	}
}