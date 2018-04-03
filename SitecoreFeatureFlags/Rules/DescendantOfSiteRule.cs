using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Rules;
using Sitecore.Rules.ConditionalRenderings;
using Sitecore.Rules.Conditions;
using SitecoreFeatureFlags.RulesContext;

namespace SitecoreFeatureFlags.Rules
{
    public class DescendantOfSiteRule<T> : StringOperatorCondition<T> where T : RuleContext
    {
        public ID SiteItemId { get; set; }
        protected override bool Execute(T ruleContext)
        {
            var ruleResponse = false;
            Assert.IsNotNull(ruleContext, "RuleContext is null");
            bool matchFound = false;
            try
            {
                var placeholderSettingsRuleContext = ruleContext as PlaceholderSettingsRuleContext;
                var siteItem = Sitecore.Context.Database.GetItem(SiteItemId);
                var currentRootItem = Sitecore.Context.Database.GetItem(Sitecore.Context.Site.RootPath);
                if (SiteItemId == currentRootItem.ID)
                {
                    ruleResponse = true;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(string.Format("BlockModulesBySiteSettingsRule -- {0}", ex.Message));
            }

            return ruleResponse;
        }
    }
}