using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework.UI.Paging;

namespace Nop.Plugin.Products.SpecificationAttributes.Models
{
    public partial class OrderHistoryPagingFilteringModel : BasePageableModel
    {
        #region Ctor

        /// <summary>
        /// Constructor
        /// </summary>
        public OrderHistoryPagingFilteringModel()
        {
            this.PageSizeOptions = new List<SelectListItem>();
        }

        #endregion

        #region Properties


        /// <summary>
        /// Available page size options
        /// </summary>
        public IList<SelectListItem> PageSizeOptions { get; set; }

    }
}