﻿using System;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;

namespace Sitecore.FeatureFlags.Rules
{
    public class DescendantOfSiteRule<T> : WhenCondition<T> where T : RuleContext
    {
        public ID SiteItemId { get; set; }
        protected override bool Execute(T ruleContext)
        {
            var ruleResponse = false;
            Assert.IsNotNull(ruleContext, "RuleContext is null");
            try
            {
                var currentRootItem = ruleContext.Item;
                var siteItem = currentRootItem.Database.GetItem(SiteItemId);
                bool isDescendantOf = currentRootItem.Paths.FullPath.StartsWith(siteItem.Paths.FullPath) || currentRootItem.Paths.FullPath.Equals(siteItem.Paths.FullPath);
                ruleResponse = isDescendantOf;
                Log.Info(string.Format("BlockModulesBySiteSettingsRule:Comparison -- {0} :: {1}", siteItem.ID, currentRootItem.Paths.FullPath), this);
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