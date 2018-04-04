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
                if (ruleContext is PlaceholderSettingsRuleContext)
                {
                    var currentRootItem = Sitecore.Context.Database.GetItem(Sitecore.Context.Site.RootPath);
                    if (SiteItemId == currentRootItem.ID)
                    {
                        ruleResponse = true;
                    }
                    //shouldn't need to test for descendants, site should be at the rootPath level
                }
                else if (ruleContext is InsertOptionsRuleContext)
                {
                    var insertOptionsRuleContext = ruleContext as InsertOptionsRuleContext;
                    var currentRootItem = insertOptionsRuleContext.Item;
                    if (SiteItemId == currentRootItem.ID)
                    {
                        ruleResponse = true;
                    }
                    else
                    {
                        var siteItem = currentRootItem.Database.GetItem(SiteItemId);
                        var isDescendantOf = currentRootItem.Paths.FullPath.StartsWith(siteItem.Paths.FullPath);
                        ruleResponse = isDescendantOf;
                    }
                }
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