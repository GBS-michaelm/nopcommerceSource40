using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Stores;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Services.Catalog;
using Nop.Services.Discounts;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.DiscountRules.HasCategory.Controllers
{
    public class GetCategoryItemsController
    {
        private readonly ICategoryService _categoryService;

        public int getCatId(int proId)
        {
            var productCategories = _categoryService.GetProductCategoriesByProductId(proId, true);
            if (productCategories.Count > 0)
            {
                var category = productCategories[0].Category;
                if (category != null)
                {
                    return category.Id;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
    }

    /// <summary>
    /// Represents a discount requirement rule
    /// </summary>
    public partial interface IDiscountRequirementRuleCat : IPlugin
    {
        /// <summary>
        /// Check discount requirement
        /// </summary>
        /// <param name="request">Object that contains all information required to check the requirement (Current customer, discount, etc)</param>
        /// <returns>Result</returns>
        DiscountRequirementValidationResultCat CheckRequirement(DiscountRequirementValidationRequestCat request);

        /// <summary>
        /// Get URL for rule configuration
        /// </summary>
        /// <param name="discountId">Discount identifier</param>
        /// <param name="discountRequirementId">Discount requirement identifier (if editing)</param>
        /// <returns>URL</returns>
        string GetConfigurationUrl(int discountId, int? discountRequirementId);
    }

    /// <summary>
    /// Represents a result of discount requirement validation
    /// </summary>
    public partial class DiscountRequirementValidationResultCat
    {
        /// <summary>
        /// Gets or sets a alue indicating whether discount is valid
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Gets or sets an error that a customer should see when enterting a coupon code (in case if "IsValid" is set to "false")
        /// </summary>
        public string UserError { get; set; }
    }

    /// <summary>
    /// Represents a request of discount requirement validation
    /// </summary>
    public partial class DiscountRequirementValidationRequestCat
    {
        /// <summary>
        /// Gets or sets the appropriate discount requirement ID (identifier)
        /// </summary>
        public int DiscountRequirementId { get; set; }

        /// <summary>
        /// Gets or sets the customer
        /// </summary>
        public Customer Customer { get; set; }

        /// <summary>
        /// Gets or sets the store
        /// </summary>
        public Store Store { get; set; }
    }

    /// <summary>
    /// Represents a customer
    /// </summary>
    public partial class Customer : BaseEntity
    {
        private ICollection<ExternalAuthenticationRecord> _externalAuthenticationRecords;
        private ICollection<CustomerRole> _customerRoles;
        private ICollection<ShoppingCartItemCat> _shoppingCartItemsCat;
        private ICollection<ReturnRequest> _returnRequests;
        private ICollection<Address> _addresses;

        /// <summary>
        /// Ctor
        /// </summary>
        public Customer()
        {
            this.CustomerGuid = Guid.NewGuid();
        }

        /// <summary>
        /// Gets or sets the customer Guid
        /// </summary>
        public Guid CustomerGuid { get; set; }

        /// <summary>
        /// Gets or sets the username
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Gets or sets the email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Gets or sets the email that should be re-validated. Used in scenarios when a customer is already registered and wants to change an email address.
        /// </summary>
        public string EmailToRevalidate { get; set; }

        /// <summary>
        /// Gets or sets the admin comment
        /// </summary>
        public string AdminComment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer is tax exempt
        /// </summary>
        public bool IsTaxExempt { get; set; }

        /// <summary>
        /// Gets or sets the affiliate identifier
        /// </summary>
        public int AffiliateId { get; set; }

        /// <summary>
        /// Gets or sets the vendor identifier with which this customer is associated (maganer)
        /// </summary>
        public int VendorId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this customer has some products in the shopping cart
        /// <remarks>The same as if we run this.ShoppingCartItems.Count > 0
        /// We use this property for performance optimization:
        /// if this property is set to false, then we do not need to load "ShoppingCartItems" navigation property for each page load
        /// It's used only in a couple of places in the presenation layer
        /// </remarks>
        /// </summary>
        public bool HasShoppingCartItems { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer is required to re-login
        /// </summary>
        public bool RequireReLogin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating number of failed login attempts (wrong password)
        /// </summary>
        public int FailedLoginAttempts { get; set; }
        /// <summary>
        /// Gets or sets the date and time until which a customer cannot login (locked out)
        /// </summary>
        public DateTime? CannotLoginUntilDateUtc { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer is active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer account is system
        /// </summary>
        public bool IsSystemAccount { get; set; }

        /// <summary>
        /// Gets or sets the customer system name
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// Gets or sets the last IP address
        /// </summary>
        public string LastIpAddress { get; set; }

        /// <summary>
        /// Gets or sets the date and time of entity creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of last login
        /// </summary>
        public DateTime? LastLoginDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of last activity
        /// </summary>
        public DateTime LastActivityDateUtc { get; set; }

        /// <summary>
        ///  Gets or sets the store identifier in which customer registered
        /// </summary>
        public int RegisteredInStoreId { get; set; }

        #region Navigation properties

        /// <summary>
        /// Gets or sets customer generated content
        /// </summary>
        public virtual ICollection<ExternalAuthenticationRecord> ExternalAuthenticationRecords
        {
            get { return _externalAuthenticationRecords ?? (_externalAuthenticationRecords = new List<ExternalAuthenticationRecord>()); }
            protected set { _externalAuthenticationRecords = value; }
        }

        /// <summary>
        /// Gets or sets the customer roles
        /// </summary>
        public virtual ICollection<CustomerRole> CustomerRoles
        {
            get { return _customerRoles ?? (_customerRoles = new List<CustomerRole>()); }
            protected set { _customerRoles = value; }
        }

        /// <summary>
        /// Gets or sets shopping cart items
        /// </summary>
        public virtual ICollection<ShoppingCartItemCat> ShoppingCartItemsCat
        {
            get { return _shoppingCartItemsCat ?? (_shoppingCartItemsCat = new List<ShoppingCartItemCat>()); }
            protected set { _shoppingCartItemsCat = value; }
        }

        /// <summary>
        /// Gets or sets return request of this customer
        /// </summary>
        public virtual ICollection<ReturnRequest> ReturnRequests
        {
            get { return _returnRequests ?? (_returnRequests = new List<ReturnRequest>()); }
            protected set { _returnRequests = value; }
        }

        /// <summary>
        /// Default billing address
        /// </summary>
        public virtual Address BillingAddress { get; set; }

        /// <summary>
        /// Default shipping address
        /// </summary>
        public virtual Address ShippingAddress { get; set; }

        /// <summary>
        /// Gets or sets customer addresses
        /// </summary>
        public virtual ICollection<Address> Addresses
        {
            get { return _addresses ?? (_addresses = new List<Address>()); }
            protected set { _addresses = value; }
        }

        #endregion
    }

    /// <summary>
    /// Represents a shopping cart
    /// </summary>
    public static class ShoppingCartCatExtensions
    {
        public static IEnumerable<ShoppingCartItemCat> LimitPerStoreCat(this IEnumerable<ShoppingCartItemCat> cart, int storeId)
        {
            var shoppingCartSettings = EngineContext.Current.Resolve<ShoppingCartSettings>();
            if (shoppingCartSettings.CartsSharedBetweenStores)
                return cart;

            return cart.Where(x => x.StoreId == storeId);
        }
    }

    /// <summary>
    /// Represents a shopping cart item
    /// </summary>
    public partial class ShoppingCartItemCat : BaseEntity
    {
        /// <summary>
        /// Gets or sets the store identifier
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// Gets or sets the shopping cart type identifier
        /// </summary>
        public int ShoppingCartTypeId { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the category identifier
        /// </summary> 
        public int CategoryId { get; set; }
        //public int CategoryId2
        //{
        //    get
        //    {
        //        var productCategories = GetCategoryController.getCatId(ProductId)

        //        return CategoryId2;
        //    }
        //}

        /// <summary>
        /// Gets or sets the product attributes in XML format
        /// </summary>
        public string AttributesXml { get; set; }

        /// <summary>
        /// Gets or sets the price enter by a customer
        /// </summary>
        public decimal CustomerEnteredPrice { get; set; }

        /// <summary>
        /// Gets or sets the quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the rental product start date (null if it's not a rental product)
        /// </summary>
        public DateTime? RentalStartDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the rental product end date (null if it's not a rental product)
        /// </summary>
        public DateTime? RentalEndDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance update
        /// </summary>
        public DateTime UpdatedOnUtc { get; set; }

        /// <summary>
        /// Gets the log type
        /// </summary>
        public ShoppingCartType ShoppingCartType
        {
            get
            {
                return (ShoppingCartType)this.ShoppingCartTypeId;
            }
            set
            {
                this.ShoppingCartTypeId = (int)value;
            }
        }

        /// <summary>
        /// Gets or sets the product
        /// </summary>
        public virtual Product Product { get; set; }

        /// <summary>
        /// Gets or sets the customer
        /// </summary>
        public virtual Customer Customer { get; set; }

        /// <summary>
        /// Gets a value indicating whether the shopping cart item is free shipping
        /// </summary>
        public bool IsFreeShipping
        {
            get
            {
                var product = this.Product;
                if (product != null)
                    return product.IsFreeShipping;
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the shopping cart item is ship enabled
        /// </summary>
        public bool IsShipEnabled
        {
            get
            {
                var product = this.Product;
                if (product != null)
                    return product.IsShipEnabled;
                return false;
            }
        }

        /// <summary>
        /// Gets the additional shipping charge
        /// </summary> 
        public decimal AdditionalShippingCharge
        {
            get
            {
                decimal additionalShippingCharge = decimal.Zero;
                var product = this.Product;
                if (product != null)
                    additionalShippingCharge = product.AdditionalShippingCharge * Quantity;
                return additionalShippingCharge;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the shopping cart item is tax exempt
        /// </summary>
        public bool IsTaxExempt
        {
            get
            {
                var product = this.Product;
                if (product != null)
                    return product.IsTaxExempt;
                return false;
            }
        }
    }

}
