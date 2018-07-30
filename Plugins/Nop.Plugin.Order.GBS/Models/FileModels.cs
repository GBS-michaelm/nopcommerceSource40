//using Newtonsoft.Json;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Web;

//namespace Nop.Plugin.Order.GBS.Models
//{
//    public class Product
//    {
//        public List<string> categories { get; set; }
//        public string productCode { get; set; }
//        public string productType { get; set; }
//        public string orientation { get; set; }
//        public string surface { get; set; }
//        public string imprintColor { get; set; }
//        public List<string> tags { get; set; }
//        public string hiResPDFURL { get; set; }
//        public string productionFileName { get; set; }
//        public string sourceReference { get; set; }
//        public Product()
//        {
//            categories = new List<string>();
//            tags = new List<string>();
//        }
//        public string ToJSON()
//        {
//            string output = JsonConvert.SerializeObject(this, Formatting.Indented);
//            return output;
//        }
//        public static Product FromJSON(string input)
//        {
//            return JsonConvert.DeserializeObject<Product>(input);
//        }
//        public string ToJSON(bool sampleData)
//        {
//            if (sampleData)
//            {
//                categories.Add("Rider Signs");
//                productCode = "SNBKR1-001-100-00000";
//                productType = "sign";
//                tags.Add("Signs");
//                hiResPDFURL = "http://canvas.houseofmagnets.com/api/rendering/GetHiResOutput/anonymous_bqdm4ydjrfahtxtfydizorac/5361ab16-0693-4d2c-88d8-ea4869f32fe5/0_0.pdf";
//                productionFileName = "yu5yke-SNBKR1-r503205-s2.pdf";
//                sourceReference = "12345";
//            }

//            return this.ToJSON();
//        }
//    }
//    public class ProductFileModel
//    {
//        public Product product { get; set; }
//        public string requestSessionID { get; set; }
//        public ProductFileModel()
//        {
//            product = new Product();
//        }
//        public string ToJSON()
//        {
//            string output = JsonConvert.SerializeObject(this, Formatting.Indented);
//            return output;
//        }
//        public static ProductFileModel FromJSON(string input)
//        {
//            return JsonConvert.DeserializeObject<ProductFileModel>(input);
//        }
//        public string ToJSON(bool sampleData)
//        {
//            if (sampleData)
//            {
//                product = Product.FromJSON(new Product().ToJSON(true));
//                requestSessionID = "f2amh4wsa2wfop43qcbnvo2g";
//            }

//            return this.ToJSON();
//        }
//    }
//    public class FileCopyModel
//    {
//        public string sourcePath { get; set; }
//        public string targetPath { get; set; }
//        public string sourceProtocol { get; set; }
//        public bool overwrite { get; set; }
//        public bool impersonate { get; set; }

//        public FileCopyModel()
//        {
//            overwrite = true;
//            impersonate = true;
//        }

//        public string ToJSON()
//        {
//            string output = JsonConvert.SerializeObject(this, Formatting.Indented);
//            return output;
//        }

//        public string ToJSON(bool sampleData)
//        {
//            if (sampleData)
//            {
//                sourcePath = "http://www.houseofmagnets.com/images/banners/HOM-2016-calendar-countdownbanner.png";
//                targetPath = "c:\\temp\\output.png";
//                sourceProtocol = "http";
//                overwrite = true;
//            }

//            return this.ToJSON();
//        }

//        public static FileCopyModel FromJSON(string input)
//        {
//            return JsonConvert.DeserializeObject<FileCopyModel>(input);
//        }

//        public static List<string> getSupportedProtocols()
//        {
//            List<string> protocols = new List<string>();
//            protocols.Add("http");
//            protocols.Add("local");
//            protocols.Add("ftp");
//            return protocols;
//        }
//    }
//}