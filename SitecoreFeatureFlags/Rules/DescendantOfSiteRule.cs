using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Rules;
using Sitecore.Rules.ConditionalRenderings;
using Sitecore.Rules.Conditions;
using Sitecore.Rules.InsertOptions;
using SitecoreFeatureFlags.RulesContext;

namespace SitecoreFeatureFlags.Rules
{
    public class DescendantOfSiteRule<T> : WhenCondition<T> where T : RuleContext
    {
        public ID SiteItemId { get; set; }
        protected override bool Execute(T ruleContext)
        {
            var ruleResponse = false;
            Assert.IsNotNull(ruleContext, "RuleContext is null");
            bool matchFound = false;
            try
            {
                var currentRootItem = ruleContext.Item;
                var siteItem = currentRootItem.Database.GetItem(SiteItemId);
                bool isDescendantOf = currentRootItem.Paths.FullPath.StartsWith(siteItem.Paths.FullPath) || currentRootItem.Paths.FullPath.Equals(siteItem.Paths.FullPath);
                ruleResponse = isDescendantOf;
                Log.Info(string.Format("BlockModulesBySiteSettingsRule:Comparison -- {0} :: {1}", siteItem, currentRootItem.Paths.FullPath), this);
            }
            catch (Exception ex)
            {
                Log.Debug(string.Format("BlockModulesBySiteSettingsRule -- {0}", ex.Message));
            }

            Log.Info(string.Format("BlockModulesBySiteSettingsRule -- {0}", ruleResponse), this);

            return ruleResponse;
        }
    }
}