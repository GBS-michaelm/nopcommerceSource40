using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Nop.Plugin.Widgets.MarketCenters.GBS.Models.Clients
{
    public partial class StyleGuideModel : BaseNopEntityModel
    {
        public StyleGuideModel()
        {
            AvailablePrimaryFonts = new List<SelectListItem>();
            AvailableSecondaryFonts = new List<SelectListItem>();
            AvailableBodyCopyFonts = new List<SelectListItem>();
            AvailableColorEnvelopes = new List<SelectListItem>();
            AvailablePens = new List<SelectListItem>();
        }
        // Logos           
        //[DisplayName("Primary horizontal on light")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.PrimaryHorizontalLight")]
        [UIHint("Picture")]
        public int PrimaryHorizontalLight { get; set; }
        //[DisplayName("Primary horizontal on dark")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.PrimaryHorizontalDark")]
        [UIHint("Picture")]
        public int PrimaryHorizontalDark { get; set; }
        //[DisplayName("Primary square on light")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.PrimarySquareLight")]
        [UIHint("Picture")]
        public int PrimarySquareLight { get; set; }
        [DisplayName("Primary square on dark")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.PrimarySquareDark")]
        [UIHint("Picture")]
        public int PrimarySquareDark { get; set; }
        //[DisplayName("One color black horizontal")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.OneColorBlackHorizontal")]
        [UIHint("Picture")]
        public int OneColorBlackHorizontal { get; set; }
        //[DisplayName("One color black vertical")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.OneColorBlackVertical")]
        [UIHint("Picture")]
        public int OneColorBlackVertical { get; set; }
        //[DisplayName("One color white horizontal")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.OneColorWhiteHorizontal")]
        [UIHint("Picture")]
        public int OneColorWhiteHorizontal { get; set; }
        //[DisplayName("One color white vertical")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.OneColorWhiteVertical")]
        [UIHint("Picture")]
        public int OneColorWhiteVertical { get; set; }
        //[DisplayName("All horizontal on light")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.AltHorizontalLight")]
        [UIHint("Picture")]
        public int AltHorizontalLight { get; set; }
        //[DisplayName("All horizontal on dark")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.AltHorizontalDark")]
        [UIHint("Picture")]
        public int AltHorizontalDark { get; set; }
        //[DisplayName("All square on light")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.AltSquareLight")]
        [UIHint("Picture")]
        public int AltSquareLight { get; set; }
        //[DisplayName("All square on dark")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.AltSquareDark")]
        [UIHint("Picture")]
        public int AltSquareDark { get; set; }


        //Color and Background Textures
        //[DisplayName("Primary Color")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.PrimaryColor")]
        public string PrimaryColor { get; set; }
        //[DisplayName("Secondary Color")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.SecondaryColor")]
        public string SecondaryColor { get; set; }
        //[DisplayName("Accent Color")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.AccentColor")]
        public string AccentColor { get; set; }
        //[DisplayName("Light body color")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.LightBodyColor")]
        public string LightBodyColor { get; set; }
        //[DisplayName("Dark body color")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.DarkBodyColor")]
        public string DarkBodyColor { get; set; }
        //[DisplayName("Text on light background")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.LightBackground")]
        public string LightBackground { get; set; }
        //[DisplayName("Text on dark background")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.DarkBackground")]
        public string DarkBackground { get; set; }
        [UIHint("Picture")]
        //[DisplayName("Background 1")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.Background1")]
        public int Background1 { get; set; }
        [UIHint("Picture")]
        //[DisplayName("Background 2")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.Background2")]
        public int Background2 { get; set; }
        [UIHint("Picture")]
        //[DisplayName("Background 3")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.Background3")]
        public int Background3 { get; set; }
        //[DisplayName("Restrict Color Palette")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.ColorPalette")]
        public bool ColorPalette { get; set; }
        //[DisplayName("Assign Color by Product")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.AssignColor")]
        public string AssignColor { get; set; }


        // Fonts
        //[DisplayName("Primary Font")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.PrimaryFont")]
        public int PrimaryFont { get; set; }
        public List<SelectListItem> AvailablePrimaryFonts { get; set; }
        //[DisplayName("Primary Letter-Spacing")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.PrimaryLetterSpecing")]
        public string PrimaryLetterSpecing { get; set; }
        //[DisplayName("Secondary Font")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.SecondaryFont")]
        public int SecondaryFont { get; set; }
        public List<SelectListItem> AvailableSecondaryFonts { get; set; }
        //[DisplayName("Secondary Letter-Spacing")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.SecondaryLetterSpecing")]
        public string SecondaryLetterSpecing { get; set; }
        //[DisplayName("Body Copy Font")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.BodyCopyFont")]
        public int BodyCopyFont { get; set; }
        public List<SelectListItem> AvailableBodyCopyFonts { get; set; }
        //[DisplayName("Body Letter-Spacing")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.BodyLetterSpecing")]
        public string BodyLetterSpecing { get; set; }

        // Symbols
        //[DisplayName("Default Symbol 1")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.Symbol1")]
        [UIHint("Picture")]
        public int Symbol1 { get; set; }
        //[DisplayName("Default Symbol 2")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.Symbol2")]
        [UIHint("Picture")]
        public int Symbol2 { get; set; }
        //[DisplayName("Default Symbol 3")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.Symbol3")]
        [UIHint("Picture")]
        public int Symbol3 { get; set; }

        // Advanced Editor
        //[DisplayName("Enable Advanced Editor")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.EnableAdvacedEditor")]
        public bool EnableAdvacedEditor { get; set; }
        //[DisplayName("Force Advanced Editor")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.ForceAdvacedEditor")]
        public bool ForceAdvacedEditor { get; set; }

        // Product Color Family
        //[DisplayName("Pens")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.Pen")]
        public int Pen { get; set; }
        public List<SelectListItem> AvailablePens { get; set; }
        //[DisplayName("Color Envelopes")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.ColorEnvelope")]
        public int ColorEnvelope { get; set; }
        public List<SelectListItem> AvailableColorEnvelopes { get; set; }

        // Placeholder Text
        //[DisplayName("Agent Name")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.AgentName")]
        public string AgentName { get; set; }
        //[DisplayName("Title")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.Title")]
        public string Title { get; set; }
        //[DisplayName("Mobile Phone")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.MobilePhone")]
        public string MobilePhone { get; set; }
        //[DisplayName("Office Phone")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.OfficePhone")]
        public string OfficePhone { get; set; }
        //[DisplayName("Fax")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.Fax")]
        public string Fax { get; set; }
        //[DisplayName("Email")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.Email")]
        public string Email { get; set; }
        //[DisplayName("Website")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.Website")]
        public string Website { get; set; }
        //[DisplayName("Address")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.Address")]
        public string Address { get; set; }
        //[DisplayName("Liecense")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.Liecense")]
        public string Liecense { get; set; }
        //[DisplayName("Disclaimer Text")]
        [NopResourceDisplayName("MarketCenters.Client.StyleGuide.DisclaimerText")]
        public string DisclaimerText { get; set; }
    }
}