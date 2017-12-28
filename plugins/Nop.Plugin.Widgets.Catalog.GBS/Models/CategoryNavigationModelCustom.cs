﻿using System.Collections.Generic;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Catalog.GBS.Models
{
    public partial class CategoryNavigationModelCustom : BaseNopModel
    {
        public CategoryNavigationModelCustom()
        {
            Categories = new List<CategorySimpleModelCustom>();
        }

        public int CurrentCategoryId { get; set; }
        public bool AllCategory { get; set; }
        public int NoOfChildren { get; set; }        

        public List<CategorySimpleModelCustom> Categories { get; set; }
    }
}