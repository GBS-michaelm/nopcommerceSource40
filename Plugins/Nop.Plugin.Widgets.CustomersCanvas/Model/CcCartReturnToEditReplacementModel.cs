using System.Collections.Generic;

namespace Nop.Plugin.Widgets.CustomersCanvas.Model
{
    public class CcCartReturnToEditReplacementModel
    {
        public CcCartReturnToEditReplacementModel()
        {
            Items = new List<Item>();
        }

        public List<Item> Items { get; private set; }

        public class Item : ICcCartScriptItem
        {
            public Item()
            {
                NopVersion = Nop.Core.NopVersion.CurrentVersion;
            }
            public int CartItemId { get; set; }
            public string OldUrl { get; set; }
            public string Url { get; set; }
            public string NopVersion { get; set; }
            public string Script
            {
                get
                {
                    return string.Format("ccWidget.replaceReturnToEditUrl({0}, '{1}', '{2}', '{3}');", CartItemId, OldUrl, Url, NopVersion);
                }
            }
        }

    }
}
