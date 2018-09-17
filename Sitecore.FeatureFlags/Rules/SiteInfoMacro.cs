using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Rules.RuleMacros;
using Sitecore.Shell.Applications.Dialogs.ItemLister;
using Sitecore.Text;
using Sitecore.Web;
using Sitecore.Web.UI.Sheer;

namespace Sitecore.FeatureFlags.Rules
{
    public class SiteInfoMacro : IRuleMacro
    {
        protected Dictionary<string, SiteInfo> _sites;

        public void Execute(XElement element, string name, UrlString parameters, string value)
        {
            Assert.ArgumentNotNull(element, "element");
            Assert.ArgumentNotNull(name, "name");
            Assert.ArgumentNotNull(parameters, "parameters");
            Assert.ArgumentNotNull(value, "value");
            var siteInfoList = Sitecore.Configuration.Factory.GetSiteInfoList();
            var siteItems = new List<Item>();
            foreach (var siteInfo in siteInfoList)
            {
                var site = Client.ContentDatabase.GetItem(siteInfo.RootPath);
                if (site != null)
                {
                    siteItems.Add(site);
                }
            }

            var itemListerOptions = new ItemListerOptions();
            itemListerOptions.Items = siteItems.GroupBy(si => si.ID).Select(g => g.First()).ToList(); ;
            itemListerOptions.Title = "Select Site";
            itemListerOptions.Text = string.Empty;
            itemListerOptions.Icon = "applications/32x32/media_stop.png";
            SheerResponse.ShowModalDialog(itemListerOptions.ToUrlString().ToString(), "700px", "700px", "", true);
        }
    }
}