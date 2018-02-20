using System.Collections.Generic;
using WebServices.Models;

namespace Nop.Plugin.Widgets.Marketing.EmailPref.Models
{

    public class EmailListModel
    {
        public int Id { get; set; }
        public string ListHeader { get; set; }
        public string ListTag { get; set; }
        public string ListDetails { get; set; }
        public string ListImage { get; set; }
        public string SubListName { get; set; }
        public string UnSubListName { get; set; }
        public bool SendWeekly { get; set; }
        public bool SendMonthly { get; set; }
        public bool Unsubscribe { get; set; }
        public string ListWebsite { get; set; }
        public bool Master { get; set; }
        public bool listSubscribeStatus
        {
            get { return _listSubscribeStatus; }
            set { _listSubscribeStatus = value; }
        }
        private bool _listSubscribeStatus = false;

        public bool listFound
        {
            get { return _listFound; }
            set { _listFound = value; }
        }
        private bool _listFound = false;

    }

    public class EmailPreferencesModel
    {
        public IList<EmailListModel> ShowLists { get; set; }
        public IList<EmailListModel> ShowPartnerLists { get; set; }
        public MarketingContactModel ContactPref { get; set; }
        public string subscribeStatusMaster { get; set; }
        public string subscribeStatusPartner { get; set; }
        public bool masterFound
        {
            get { return _masterFound; }
            set { _masterFound = value; }
        }
        private bool _masterFound = false;

        public bool partnerFound
        {
            get { return _partnerFound; }
            set { _partnerFound = value; }
        }
        private bool _partnerFound = false;

        public EmailPreferencesModel()
        {
            ShowLists = new List<EmailListModel>();
            ShowPartnerLists = new List<EmailListModel>();
            ContactPref = new MarketingContactModel();
        }

    }

    public class EmailPreferencesErrorModel
    {
        public string ErrorMessage { get; set; }
    }

    public class MarketingFormModel
    {
        public string EmailList { get; set; }
        //public string Accounts { get; set; }
    }
}
