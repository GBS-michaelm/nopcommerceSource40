using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Plugin.BusinessDataAccess.GBS;
using Nop.Services.Catalog;
using Nop.Services.Logging;

namespace Nop.Plugin.BusinessLogic.GBS.Domain
{
        
    public class Image
    {
        public string image { get; set; }
        public bool locked { get; set; }
    }

    public class InfoSettings
    {
        public PostScript font { get; set; }
        public string text { get; set; }
        public string color { get; set; }
    }

    public class PostScript
    {
        public string postScriptName { get; set; }
    }

    public class InfoSettingsName
    {
        public PostScriptName font { get; set; }
        public string text { get; set; }
        public string color { get; set; }
    }

    public class PostScriptName
    {
        public string postScriptName { get; set; }
        public bool fauxBold { get; set; }
    }
    
    public class FillColor
    {
        public string fillColor { get; set; }
    }
    
    public class Field
    {
        [JsonProperty(PropertyName = "FieldId")]
        public int fieldId { get; set; }
        [JsonProperty(PropertyName = "IsRequired")]
        public bool isRequired { get; set; }
        [JsonProperty(PropertyName = "FieldName")]
        public string fieldName { get; set; }
        [JsonProperty(PropertyName = "FieldLabel")]
        public string fieldLabel { get; set; }
        [JsonProperty(PropertyName = "AllowBold")]
        public bool allowBold { get; set; }
        [JsonProperty(PropertyName = "AllowItalic")]
        public bool allowItalic { get; set; }
        [JsonProperty(PropertyName = "SortOrder")]
        public int sortOrder { get; set; }
    }

    //public class JSONEditor
    //{

    //}

    public class CompanyProduct
    {
        DBManager manager = new DBManager();
        ISpecificationAttributeService specService = EngineContext.Current.Resolve<ISpecificationAttributeService>();
        IProductService productService = EngineContext.Current.Resolve<IProductService>();
        ILogger logger = EngineContext.Current.Resolve<ILogger>();

        //check bobby json to get class attributes
        [JsonProperty(PropertyName = "BGColor-Primary")]
        public FillColor BGColor_Primary { get; set; }
        [JsonProperty(PropertyName = "BGColor-Secondary")]
        public FillColor BGColor_Secondary { get; set; }
        [JsonProperty(PropertyName = "LogoOnLight-H")]
        public Image LogoOnLight_H { get; set; }
        [JsonProperty(PropertyName = "LogoOnDark-H")]
        public Image LogoOnDark_H { get; set; }
        [JsonProperty(PropertyName = "Symbol-1")]
        public Image Symbol_1 { get; set; }
        [JsonProperty(PropertyName = "Symbol-2")]
        public Image Symbol_2 { get; set; }
        [JsonProperty(PropertyName = "Symbol-3")]
        public Image Symbol_3 { get; set; }
        [JsonProperty(PropertyName = "Name")]
        public InfoSettingsName Name { get; set; }
        [JsonProperty(PropertyName = "Title")]
        public InfoSettings Title { get; set; }
        [JsonProperty(PropertyName = "Phone-1")]
        public InfoSettings Phone_1 { get; set; }
        [JsonProperty(PropertyName = "Phone-2")]
        public InfoSettings Phone_2 { get; set; }
        [JsonProperty(PropertyName = "Website")]
        public InfoSettings Website { get; set; }
        [JsonProperty(PropertyName = "Email")]
        public InfoSettings Email { get; set; }
        [JsonProperty(PropertyName = "Adress-1")]
        public InfoSettings Address_1 { get; set; }
        [JsonProperty(PropertyName = "Adress-2")]
        public InfoSettings Address_2 { get; set; }
        [JsonProperty(PropertyName = "License")]
        public InfoSettings License { get; set; }
        [JsonProperty(PropertyName = "Fields")]
        public List<Field> Fields { get; set; } = new List<Field>();
        public string Sku { get; set; } = "0";
        public string Template { get; set; } = "NBEX0006";
        public string TemplateShape { get; set; }
        public string BorderDefault { get; set; }
        public List<string> FrameOptions { get ; set; }

        public List<CompanyProduct> GetProductsAndFields(int companyCode)
        {
            List<CompanyProduct> productList = new List<CompanyProduct>();
            List<Field> fieldsList = null;
            dynamic companyProductsJson;
            dynamic fieldsJson;
            string productSku;
            string productTemplate;
            string productTemplateShape;
            string productBorderDefault;

            try
            {
                //TEST ID 26764
                //companyCode = 26764;
                companyCode = 6758;

                //use bobby stored proc here and create 
                //company product list along with a list of fields needed
                Dictionary<string, Object> productListDic = new Dictionary<string, Object>();
                productListDic.Add("@CategoryId", companyCode);

                string select = "EXEC usp_SelectMarketCenterWtpPageJson @CategoryId";

                DataView companyProductDataView = manager.GetParameterizedDataView(select, productListDic);

                if (companyProductDataView.Count > 0)
                {
                    fieldsJson = companyProductDataView[0]["NameBadgeFields"];
                    for (int i = 0; i < companyProductDataView.Count; i++)
                    {
                        if (i == 0)
                        {
                            fieldsList = JsonConvert.DeserializeObject<List<Field>>(fieldsJson);
                        }

                        companyProductsJson = companyProductDataView[i]["EditorJson"];
                        productSku = companyProductDataView[i]["ProductCode"].ToString();
                        productTemplateShape = productSku.Substring(4, 1) == "O" ? "oval" : "rect";
                        productBorderDefault = productTemplateShape == "oval" ? "/images/badgeframes/OVAL-BLACK.png" : "/images/badgeframes/RECT-BLACK.png";
                        //productTemplate = companyProductDataView[i]["Template"].ToString(); //not currently in stored proc

                        CompanyProduct companyProduct = JsonConvert.DeserializeObject<CompanyProduct>(companyProductsJson);

                        companyProduct.Sku = productSku;
                        companyProduct.TemplateShape = productTemplateShape;
                        companyProduct.BorderDefault = productBorderDefault;

                        //may need to be apart of returned JSON for speed
                        //PROBABLY NEEDS TO BE DB


                        Product prod = productService.GetProductBySku(productSku);
                        IList<ProductSpecificationAttribute> specAttrList = new List<ProductSpecificationAttribute>();
                        specAttrList = specService.GetProductSpecificationAttributes(prod.Id);
                        foreach (var attr in specAttrList)
                        {
                            string typeOptionValue = "";
                            if (attr.SpecificationAttributeOption.SpecificationAttribute.Name == "Frame Style")
                            {
                                int specAttributeId = attr.SpecificationAttributeOption.SpecificationAttribute.Id;




                                //typeOptionValue = attr.SpecificationAttributeOption.Name;
                                //if (typeOptionValue == customType)
                                //{
                                //    specAttributeValueOption = attr.SpecificationAttributeOption.Id;
                                //    break;
                                //}


                            }
                        }


                        //add is last
                        productList.Add(companyProduct);

                    }


                    List<Field> sortedFields = fieldsList.OrderBy(x => x.sortOrder).ToList();

                    foreach (Field field in sortedFields)
                    {
                        productList[0].Fields.Add(field);
                    }

                }

            }catch(Exception ex)
            {
                logger.Error("CompanyProduct.cs Exception: ->", ex);
            }                              

            return productList;

        }
        
    }

    


}
    



