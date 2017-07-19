using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using System.Web.Mvc;
using Nop.Web.Framework.Controllers;
using Nop.Plugin.Widgets.CustomersCanvas.Model;
using Nop.Plugin.Widgets.CustomersCanvas.Services;
using Nop.Core.Domain.Catalog;
using System.IO;
using System.Web;
using System.Web.Routing;
using Nop.Admin.Models.Catalog;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Security;
using System.Configuration;
using System.Net;
using System.Collections.Specialized;
using System.Threading;

namespace Nop.Plugin.Widgets.CustomersCanvas.Controllers
{

    public class CcProductController : BasePluginController
    {
        #region fields
        private readonly ISettingService _settingService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly ICcService _ccService;
        private readonly IPictureService _pictureService;
        private readonly IProductService _productService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly ILogger _logger;
        #endregion

        #region ctor

        public CcProductController(ISettingService settingService,
            IProductAttributeService productAttributeService,
            ISpecificationAttributeService specificationAttributeService,
            ICcService ccService,
            IPictureService pictureService,
            IProductService productService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            ILogger logger)
        {
            _settingService = settingService;
            _productAttributeService = productAttributeService;
            _specificationAttributeService = specificationAttributeService;
            _ccService = ccService;
            _pictureService = pictureService;
            _productService = productService;
            _localizationService = localizationService;
            _workContext = workContext;
            _logger = logger;
        }
        #endregion

        public string ProductAdminTabString(int id)
        {
            var settings = _settingService.LoadSetting<CcSettings>();
            var specAttrs = _specificationAttributeService.GetProductSpecificationAttributes(id);

            var editorName = "";
            var editorSpecAttr = specAttrs.FirstOrDefault(x => x.SpecificationAttributeOption.SpecificationAttributeId == settings.EditorDefinitionSpecificationAttributeId);
            if (editorSpecAttr != null)
            {
                editorName = editorSpecAttr.CustomValue;
            }
            var configName = "";
            var configSpecattr = specAttrs.FirstOrDefault(x => x.SpecificationAttributeOption.SpecificationAttributeId == settings.EditorConfigurationSpecificationAttributeId);
            if (configSpecattr != null)
            {
                configName = configSpecattr.CustomValue;
            }

            var model = new CcProductSetting
            {
                ProductId = id,
                EditorSpecAttrId = settings.EditorDefinitionSpecificationAttributeId,
                EditorSpecAttrOptionId = settings.EditorDefinitionSpecificationOptionId,
                EditorConfigSpecAttrId = settings.EditorConfigurationSpecificationAttributeId,
                EditorConfigSpecAttrOptionId = settings.EditorConfigurationSpecificationOptionId,
                EditorList = _ccService.GetAllEditors(),
                EditorName = editorName,
                ConfigName = configName,
                CcUrl = settings.ServerHostUrl
            };

            var contents = Utils.GetRazorViewAsString(model, "~/Plugins/Widgets.CustomersCanvas/Views/Product/ProductAdminTab.cshtml");
            return contents;
        }

        #region envelope color attributes
        public string ProductAdminTabColorSetting(int id)
        {
            var attributeMappings = _productAttributeService.GetProductAttributeMappingsByProductId(id);
            var colorMapping = attributeMappings.FirstOrDefault(x => x.ProductAttribute.Name.ToLower().IndexOf("envelope color") >= 0);
            if (colorMapping != null)
            {
                var productAttributeMapping = _productAttributeService.GetProductAttributeMappingById(colorMapping.Id);
                if (productAttributeMapping == null)
                    throw new ArgumentException("No product attribute mapping found with the specified id");

                var product = _productService.GetProductById(productAttributeMapping.ProductId);
                if (product == null)
                    throw new ArgumentException("No product found with the specified id");

                var model = new ProductModel.ProductAttributeValueListModel
                {
                    ProductName = product.Name,
                    ProductId = productAttributeMapping.ProductId,
                    ProductAttributeName = productAttributeMapping.ProductAttribute.Name,
                    ProductAttributeMappingId = productAttributeMapping.Id,
                };


                var contents = Utils.GetRazorViewAsString(model, "~/Plugins/Widgets.CustomersCanvas/Views/Product/ProductAttributeList.cshtml");
                return contents;
            }
            else
            {
                return "";
            }
        }

        [HttpPost]
        [AdminAntiForgery(true)]
        public ActionResult AsyncUpload()
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.UploadPictures))
            //    return Json(new { success = false, error = "You do not have required permissions" }, "text/plain");

            //we process it distinct ways based on a browser
            //find more info here http://stackoverflow.com/questions/4884920/mvc3-valums-ajax-file-upload
            Stream stream = null;
            var fileName = "";
            var contentType = "";
            if (String.IsNullOrEmpty(Request["qqfile"]))
            {
                // IE
                HttpPostedFileBase httpPostedFile = Request.Files[0];
                if (httpPostedFile == null)
                    throw new ArgumentException("No file uploaded");
                stream = httpPostedFile.InputStream;
                fileName = Path.GetFileName(httpPostedFile.FileName);
                contentType = httpPostedFile.ContentType;
            }
            else
            {
                //Webkit, Mozilla
                stream = Request.InputStream;
                fileName = Request["qqfile"];
            }

            var fileBinary = new byte[stream.Length];
            stream.Read(fileBinary, 0, fileBinary.Length);

            var fileExtension = Path.GetExtension(fileName);
            if (!String.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();
            //contentType is not always available 
            //that's why we manually update it here
            //http://www.sfsu.edu/training/mimetype.htm
            if (String.IsNullOrEmpty(contentType))
            {
                switch (fileExtension)
                {
                    case ".bmp":
                        contentType = MimeTypes.ImageBmp;
                        break;
                    case ".gif":
                        contentType = MimeTypes.ImageGif;
                        break;
                    case ".jpeg":
                    case ".jpg":
                    case ".jpe":
                    case ".jfif":
                    case ".pjpeg":
                    case ".pjp":
                        contentType = MimeTypes.ImageJpeg;
                        break;
                    case ".png":
                        contentType = MimeTypes.ImagePng;
                        break;
                    case ".tiff":
                    case ".tif":
                        contentType = MimeTypes.ImageTiff;
                        break;
                    default:
                        break;
                }
            }

            var picture = _pictureService.InsertPicture(fileBinary, contentType, null);

            var tempUrl = _pictureService.GetPictureUrl(picture);

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                /* run your code here */
                SendToCustomersCanvas(tempUrl);
            }).Start();

            //SendToCustomersCanvas(tempUrl);

            //when returning JSON the mime-type must be set to text/plain
            //otherwise some browsers will pop-up a "Save As" dialog.
            return Json(new
                {
                    success = true,
                    pictureId = picture.Id,
                    imageUrl = _pictureService.GetPictureUrl(picture, 100)
                },
                MimeTypes.TextPlain);
        }

        private void SendToCustomersCanvas(string imageUrl)
        {
            try
            {
                var settings = _settingService.LoadSetting<CcSettings>();
                var ccUrl = settings.ServerHostUrl;

                var link = ccUrl + "/ImagesUploader/ImageUpload/Upload";
                using (var webClient = new WebClient())
                {
                    // Создаём коллекцию параметров
                    var pars = new NameValueCollection();

                    // Добавляем необходимые параметры в виде пар ключ, значение
                    pars.Add("format", "json");
                    pars.Add("imageUrl", imageUrl);

                    // Посылаем параметры на сервер
                    // Может быть ответ в виде массива байт
                    var response = webClient.UploadValues(link, pars);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Cann't send product image to CC.", ex);
            }
        }

        public ActionResult ProductAttributeValueCreatePopup(int productAttributeMappingId)
        {
            var productAttributeMapping = _productAttributeService.GetProductAttributeMappingById(productAttributeMappingId);
            if (productAttributeMapping == null)
                throw new ArgumentException("No product attribute mapping found with the specified id");

            var product = _productService.GetProductById(productAttributeMapping.ProductId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List", "Product");

            var model = new ProductModel.ProductAttributeValueModel();
            model.ProductAttributeMappingId = productAttributeMappingId;

            //color squares
            model.DisplayColorSquaresRgb = productAttributeMapping.AttributeControlType == AttributeControlType.ColorSquares;
            model.ColorSquaresRgb = "#000000";
            //image squares
            model.DisplayImageSquaresPicture = productAttributeMapping.AttributeControlType == AttributeControlType.ImageSquares;

            //default qantity for associated product
            model.Quantity = 1;

            //pictures
            model.ProductPictureModels = _productService.GetProductPicturesByProductId(product.Id)
                .Select(x => new ProductModel.ProductPictureModel
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    PictureId = x.PictureId,
                    PictureUrl = _pictureService.GetPictureUrl(x.PictureId),
                    DisplayOrder = x.DisplayOrder
                })
                .ToList();

            return View("~/Plugins/Widgets.CustomersCanvas/Views/Product/ProductAttributeValueCreatePopup.cshtml", model);
        }

        [HttpPost]
        public ActionResult ProductAttributeValueCreatePopup(ProductModel.ProductAttributeValueModel model, string btnId = "", string formId = "")
        {
            btnId = "btnRefresh";
            formId = "productattribute-form";

            var productAttributeMapping = _productAttributeService.GetProductAttributeMappingById(model.ProductAttributeMappingId);
            if (productAttributeMapping == null)
                //No product attribute found with the specified id
                return RedirectToAction("List", "Product");

            var product = _productService.GetProductById(productAttributeMapping.ProductId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List", "Product");

            if (productAttributeMapping.AttributeControlType == AttributeControlType.ColorSquares)
            {
                //ensure valid color is chosen/entered
                if (String.IsNullOrEmpty(model.ColorSquaresRgb))
                    ModelState.AddModelError("", "Color is required");
                try
                {
                    //ensure color is valid (can be instanciated)
                    System.Drawing.ColorTranslator.FromHtml(model.ColorSquaresRgb);
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError("", exc.Message);
                }
            }

            //ensure a picture is uploaded
            if (productAttributeMapping.AttributeControlType == AttributeControlType.ImageSquares && model.ImageSquaresPictureId == 0)
            {
                ModelState.AddModelError("", "Image is required");
            }

            if (ModelState.IsValid)
            {
                var pav = new ProductAttributeValue
                {
                    ProductAttributeMappingId = model.ProductAttributeMappingId,
                    AttributeValueTypeId = model.AttributeValueTypeId,
                    AssociatedProductId = model.AssociatedProductId,
                    Name = model.Name,
                    ColorSquaresRgb = model.ColorSquaresRgb,
                    ImageSquaresPictureId = model.ImageSquaresPictureId,
                    PriceAdjustment = model.PriceAdjustment,
                    WeightAdjustment = model.WeightAdjustment,
                    Cost = model.Cost,
                    Quantity = model.Quantity,
                    IsPreSelected = model.IsPreSelected,
                    DisplayOrder = model.DisplayOrder,
                    PictureId = model.PictureId,
                };

                _productAttributeService.InsertProductAttributeValue(pav);

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                ViewBag.formId = formId;
                return View("~/Plugins/Widgets.CustomersCanvas/Views/Product/ProductAttributeValueCreatePopup.cshtml", model);
            }

            //If we got this far, something failed, redisplay form


            //pictures
            model.ProductPictureModels = _productService.GetProductPicturesByProductId(product.Id)
                .Select(x => new ProductModel.ProductPictureModel
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    PictureId = x.PictureId,
                    PictureUrl = _pictureService.GetPictureUrl(x.PictureId),
                    DisplayOrder = x.DisplayOrder
                })
                .ToList();

            var associatedProduct = _productService.GetProductById(model.AssociatedProductId);
            model.AssociatedProductName = associatedProduct != null ? associatedProduct.Name : "";

            return View("~/Plugins/Widgets.CustomersCanvas/Views/Product/ProductAttributeValueCreatePopup.cshtml", model);
        }

        public ActionResult ProductAttributeValueEditPopup(int id)
        {
            var pav = _productAttributeService.GetProductAttributeValueById(id);
            if (pav == null)
                //No attribute value found with the specified id
                return RedirectToAction("List", "Product");

            var product = _productService.GetProductById(pav.ProductAttributeMapping.ProductId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List", "Product");

            var associatedProduct = _productService.GetProductById(pav.AssociatedProductId);

            var model = new ProductModel.ProductAttributeValueModel
            {
                ProductAttributeMappingId = pav.ProductAttributeMappingId,
                AttributeValueTypeId = pav.AttributeValueTypeId,
                AttributeValueTypeName = pav.AttributeValueType.GetLocalizedEnum(_localizationService, _workContext),
                AssociatedProductId = pav.AssociatedProductId,
                AssociatedProductName = associatedProduct != null ? associatedProduct.Name : "",
                Name = pav.Name,
                ColorSquaresRgb = pav.ColorSquaresRgb,
                DisplayColorSquaresRgb = pav.ProductAttributeMapping.AttributeControlType == AttributeControlType.ColorSquares,
                ImageSquaresPictureId = pav.ImageSquaresPictureId,
                DisplayImageSquaresPicture = pav.ProductAttributeMapping.AttributeControlType == AttributeControlType.ImageSquares,
                PriceAdjustment = pav.PriceAdjustment,
                WeightAdjustment = pav.WeightAdjustment,
                Cost = pav.Cost,
                Quantity = pav.Quantity,
                IsPreSelected = pav.IsPreSelected,
                DisplayOrder = pav.DisplayOrder,
                PictureId = pav.PictureId
            };
            if (model.DisplayColorSquaresRgb && String.IsNullOrEmpty(model.ColorSquaresRgb))
            {
                model.ColorSquaresRgb = "#000000";
            }
            //pictures
            model.ProductPictureModels = _productService.GetProductPicturesByProductId(product.Id)
                .Select(x => new ProductModel.ProductPictureModel
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    PictureId = x.PictureId,
                    PictureUrl = _pictureService.GetPictureUrl(x.PictureId),
                    DisplayOrder = x.DisplayOrder
                })
                .ToList();

            return View("~/Plugins/Widgets.CustomersCanvas/Views/Product/ProductAttributeValueEditPopup.cshtml", model);
        }

        [HttpPost]
        public ActionResult ProductAttributeValueEditPopup(ProductModel.ProductAttributeValueModel model, string btnId = "", string formId = "")
        {
            btnId = "btnRefresh";
            formId = "productattribute-form";

            var pav = _productAttributeService.GetProductAttributeValueById(model.Id);
            if (pav == null)
                //No attribute value found with the specified id
                return RedirectToAction("List", "Product");

            var product = _productService.GetProductById(pav.ProductAttributeMapping.ProductId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List", "Product");

            if (pav.ProductAttributeMapping.AttributeControlType == AttributeControlType.ColorSquares)
            {
                //ensure valid color is chosen/entered
                if (String.IsNullOrEmpty(model.ColorSquaresRgb))
                    ModelState.AddModelError("", "Color is required");
                try
                {
                    //ensure color is valid (can be instanciated)
                    System.Drawing.ColorTranslator.FromHtml(model.ColorSquaresRgb);
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError("", exc.Message);
                }
            }

            //ensure a picture is uploaded
            if (pav.ProductAttributeMapping.AttributeControlType == AttributeControlType.ImageSquares && model.ImageSquaresPictureId == 0)
            {
                ModelState.AddModelError("", "Image is required");
            }

            if (ModelState.IsValid)
            {
                pav.AttributeValueTypeId = model.AttributeValueTypeId;
                pav.AssociatedProductId = model.AssociatedProductId;
                pav.Name = model.Name;
                pav.ColorSquaresRgb = model.ColorSquaresRgb;
                pav.ImageSquaresPictureId = model.ImageSquaresPictureId;
                pav.PriceAdjustment = model.PriceAdjustment;
                pav.WeightAdjustment = model.WeightAdjustment;
                pav.Cost = model.Cost;
                pav.Quantity = model.Quantity;
                pav.IsPreSelected = model.IsPreSelected;
                pav.DisplayOrder = model.DisplayOrder;
                pav.PictureId = model.PictureId;
                _productAttributeService.UpdateProductAttributeValue(pav);

                ViewBag.RefreshPage = true;
                ViewBag.btnId = btnId;
                ViewBag.formId = formId;
                return View("~/Plugins/Widgets.CustomersCanvas/Views/Product/ProductAttributeValueEditPopup.cshtml", model);
            }

            //If we got this far, something failed, redisplay form

            //pictures
            model.ProductPictureModels = _productService.GetProductPicturesByProductId(product.Id)
                .Select(x => new ProductModel.ProductPictureModel
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    PictureId = x.PictureId,
                    PictureUrl = _pictureService.GetPictureUrl(x.PictureId),
                    DisplayOrder = x.DisplayOrder
                })
                .ToList();

            var associatedProduct = _productService.GetProductById(model.AssociatedProductId);
            model.AssociatedProductName = associatedProduct != null ? associatedProduct.Name : "";

            return View("~/Plugins/Widgets.CustomersCanvas/Views/Product/ProductAttributeValueEditPopup.cshtml", model);
        }

        [HttpPost]
        public ActionResult ProductAttributeValueList(int productAttributeMappingId, DataSourceRequest command)
        {
            var productAttributeMapping = _productAttributeService.GetProductAttributeMappingById(productAttributeMappingId);
            if (productAttributeMapping == null)
                throw new ArgumentException("No product attribute mapping found with the specified id");

            var product = _productService.GetProductById(productAttributeMapping.ProductId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your product");

            var values = _productAttributeService.GetProductAttributeValues(productAttributeMappingId);
            var gridModel = new DataSourceResult
            {
                Data = values.Select(x =>
                {
                    Nop.Core.Domain.Catalog.Product associatedProduct = null;
                    if (x.AttributeValueType == AttributeValueType.AssociatedToProduct)
                    {
                        associatedProduct = _productService.GetProductById(x.AssociatedProductId);
                    }
                    var pictureThumbnailUrl = _pictureService.GetPictureUrl(x.ImageSquaresPictureId, 75, false);
                    //little hack here. Grid is rendered wrong way with <img> without "src" attribute
                    if (String.IsNullOrEmpty(pictureThumbnailUrl))
                        pictureThumbnailUrl = _pictureService.GetPictureUrl(null, 1, true);
                    return new ProductModel.ProductAttributeValueModel
                    {
                        Id = x.Id,
                        ProductAttributeMappingId = x.ProductAttributeMappingId,
                        AttributeValueTypeId = x.AttributeValueTypeId,
                        AttributeValueTypeName = x.AttributeValueType.GetLocalizedEnum(_localizationService, _workContext),
                        AssociatedProductId = x.AssociatedProductId,
                        AssociatedProductName = associatedProduct != null ? associatedProduct.Name : "",
                        Name = x.ProductAttributeMapping.AttributeControlType != AttributeControlType.ColorSquares ? x.Name : string.Format("{0} - {1}", x.Name, x.ColorSquaresRgb),
                        ColorSquaresRgb = x.ColorSquaresRgb,
                        ImageSquaresPictureId = x.ImageSquaresPictureId,
                        PriceAdjustment = x.PriceAdjustment,
                        PriceAdjustmentStr = x.AttributeValueType == AttributeValueType.Simple ? x.PriceAdjustment.ToString("G29") : "",
                        WeightAdjustment = x.WeightAdjustment,
                        WeightAdjustmentStr = x.AttributeValueType == AttributeValueType.Simple ? x.WeightAdjustment.ToString("G29") : "",
                        Cost = x.Cost,
                        Quantity = x.Quantity,
                        IsPreSelected = x.IsPreSelected,
                        DisplayOrder = x.DisplayOrder,
                        PictureId = x.PictureId,
                        PictureThumbnailUrl = pictureThumbnailUrl
                    };
                }),
                Total = values.Count()
            };

            return Json(gridModel);
        }


        #endregion

        #region save/delete editor/config attributes
        [HttpPost]
        [AdminAuthorize]
        public JsonResult SaveCcAttributes(int productId, int attributeId, int attributeOptionId, string attributeValue)
        {
            try
            {
                var specificationAttributes = _specificationAttributeService.GetProductSpecificationAttributes(productId);
                var prodSpecAttr = specificationAttributes.FirstOrDefault(
                    x => x.SpecificationAttributeOption.SpecificationAttributeId == attributeId);
                if (prodSpecAttr == null)
                {
                    prodSpecAttr = new ProductSpecificationAttribute
                    {
                        AttributeType = SpecificationAttributeType.CustomText,
                        ProductId = productId,
                        SpecificationAttributeOptionId = attributeOptionId,
                        DisplayOrder = 0,
                        ShowOnProductPage = false,
                        AllowFiltering = false,
                        CustomValue = attributeValue,
                    };
                    _specificationAttributeService.InsertProductSpecificationAttribute(prodSpecAttr);
                }
                else
                {
                    prodSpecAttr.CustomValue = attributeValue;
                    prodSpecAttr.ShowOnProductPage = false;
                    _specificationAttributeService.UpdateProductSpecificationAttribute(prodSpecAttr);
                }

                var productAttributeMappings = _productAttributeService.GetProductAttributeMappingsByProductId(productId);
                foreach (var attrId in _ccService.GetCcAttrIds())
                {
                    if (productAttributeMappings.All(map => map.ProductAttributeId != attrId))
                        _productAttributeService.InsertProductAttributeMapping(new ProductAttributeMapping
                        {
                            AttributeControlType = AttributeControlType.TextBox,
                            DefaultValue = "",
                            DisplayOrder = 100,
                            ProductId = productId,
                            ProductAttributeId = attrId
                        });
                }

                return Json(new { status = "success" });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", message = ex.Message });
            }
        }


        [HttpPost]
        [AdminAuthorize]
        public JsonResult DeleteCcAttributes(int productId)
        {
            try
            {
                var specificationAttributes = _specificationAttributeService.GetProductSpecificationAttributes(productId);
                if (
                    specificationAttributes.Any(
                        attribute =>
                            attribute.SpecificationAttributeOptionId ==
                            _ccService.EditorDefinitionSpecificationAttributeOptionId()))
                {
                    _specificationAttributeService.DeleteProductSpecificationAttribute(
                        specificationAttributes.First(attribute =>
                            attribute.SpecificationAttributeOptionId ==
                            _ccService.EditorDefinitionSpecificationAttributeOptionId()));
                }

                if (
                    specificationAttributes.Any(
                        attribute =>
                            attribute.SpecificationAttributeOptionId ==
                            _ccService.EditorConfigurationSpecificationAttributeOptionId()))
                {
                    _specificationAttributeService.DeleteProductSpecificationAttribute(
                        specificationAttributes.First(attribute =>
                            attribute.SpecificationAttributeOptionId ==
                            _ccService.EditorConfigurationSpecificationAttributeOptionId()));
                }

                var productAttributes = _productAttributeService.GetProductAttributeMappingsByProductId(productId);
                foreach (var attrId in _ccService.GetCcAttrIds())
                {
                    if (productAttributes.Any(attr => attr.ProductAttributeId == attrId))
                        _productAttributeService.DeleteProductAttributeMapping(
                            productAttributes.First(attr => attr.ProductAttributeId == attrId));
                }

                return Json(new { status = "success" });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", message = ex.Message });
            }
        }


        [HttpPost]
        [AdminAuthorize]
        public JsonResult SaveCcAttribute(int productId, string attributeName, string attributeValue)
        {
            try
            {
                var specificationAttributes = _specificationAttributeService.GetProductSpecificationAttributes(productId);
                var prodSpecAttr = specificationAttributes.FirstOrDefault(
                    x => x.SpecificationAttributeOption.SpecificationAttribute.Name == attributeName);
                if (prodSpecAttr == null)
                {
                    var allSpecAttrs = _specificationAttributeService.GetSpecificationAttributes();
                    var currSpecAttr = allSpecAttrs.FirstOrDefault(x => x.Name == attributeName);
                    if (currSpecAttr == null)
                    {
                        currSpecAttr = new SpecificationAttribute();
                        currSpecAttr.Name = attributeName;
                        currSpecAttr.DisplayOrder = 0;
                        _specificationAttributeService.InsertSpecificationAttribute(currSpecAttr);

                        var newAttrOption = new SpecificationAttributeOption();
                        newAttrOption.SpecificationAttribute = currSpecAttr;
                        newAttrOption.Name = attributeName;
                        newAttrOption.DisplayOrder = 0;
                        _specificationAttributeService.InsertSpecificationAttributeOption(newAttrOption);
                    }

                    prodSpecAttr = new ProductSpecificationAttribute
                    {
                        AttributeType = SpecificationAttributeType.CustomText,
                        ProductId = productId,
                        SpecificationAttributeOptionId = currSpecAttr.SpecificationAttributeOptions.First().Id,
                        DisplayOrder = 0,
                        ShowOnProductPage = false,
                        AllowFiltering = false,
                        CustomValue = attributeValue,
                    };
                    _specificationAttributeService.InsertProductSpecificationAttribute(prodSpecAttr);
                }
                else
                {
                    prodSpecAttr.CustomValue = attributeValue;
                    prodSpecAttr.ShowOnProductPage = false;
                    _specificationAttributeService.UpdateProductSpecificationAttribute(prodSpecAttr);
                }

                var productAttributeMappings = _productAttributeService.GetProductAttributeMappingsByProductId(productId);
                foreach (var attrId in _ccService.GetCcAttrIds())
                {
                    if (productAttributeMappings.All(map => map.ProductAttributeId != attrId))
                        _productAttributeService.InsertProductAttributeMapping(new ProductAttributeMapping
                        {
                            AttributeControlType = AttributeControlType.TextBox,
                            DefaultValue = "",
                            DisplayOrder = 100,
                            ProductId = productId,
                            ProductAttributeId = attrId
                        });
                }

                return Json(new { status = "success" });
            }
            catch (Exception ex)
            {
                var message = ex.InnerException != null ? ex.InnerException.Message : "";
                return Json(new { status = "error", message = ex.Message + " " + message });
            }
        }


        [HttpPost]
        [AdminAuthorize]
        public JsonResult DeleteSpecAttribute(int productId, string attributeName)
        {
            try
            {
                var specificationAttributes = _specificationAttributeService.GetProductSpecificationAttributes(productId);

                var prodSpecAttr = specificationAttributes.FirstOrDefault(
                    x => x.SpecificationAttributeOption.SpecificationAttribute.Name == attributeName);
                if (prodSpecAttr != null)
                {
                    _specificationAttributeService.DeleteProductSpecificationAttribute(prodSpecAttr);
                }

                return Json(new { status = "success" });
            }
            catch (Exception ex)
            {
                var message = ex.InnerException != null ? ex.InnerException.Message : "";
                return Json(new { status = "error", message = ex.Message + " " + message });
            }
        }


        [HttpPost]
        [AdminAuthorize]
        public JsonResult GetSpecAttributes(int productId)
        {
            try
            {
                var specificationAttributes = _specificationAttributeService.GetProductSpecificationAttributes(productId);
                var list = new List<dynamic>();
                foreach (var spec in specificationAttributes)
                {
                    dynamic attr = new
                    {
                        name = spec.SpecificationAttributeOption.SpecificationAttribute.Name,
                        value = spec.CustomValue
                    };
                    list.Add(attr);
                }

                return Json(new { status = "success", data = list });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", message = ex.Message });
            }
        }
        #endregion
    }
}
