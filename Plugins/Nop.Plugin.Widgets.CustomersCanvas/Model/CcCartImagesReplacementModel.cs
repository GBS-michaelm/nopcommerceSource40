using System.Collections.Generic;

namespace Nop.Plugin.Widgets.CustomersCanvas.Model
{
    public class CcCartImagesReplacementModel
    {
        public CcCartImagesReplacementModel()
        {
            Items = new List<Item>();
        }

        public List<Item> Items { get; private set; }

        public class Item: ICcCartScriptItem
        {
            public Item()
            {
                NopVersion = Nop.Core.NopVersion.CurrentVersion;
                Index = -1;
            }

            public int CartItemId { get; set; }
            public string NopVersion { get; set; }
            public int Index { get; set; }
            public string ImageSource { get; set; }

            public string Script
            {
                get
                {
                    return string.Format("ccWidget.replaceCartItemImage({0}, {1}, '{2}', '{3}');", CartItemId, Index,
                        ImageSource, NopVersion);
                }
            }
        }
    }

}