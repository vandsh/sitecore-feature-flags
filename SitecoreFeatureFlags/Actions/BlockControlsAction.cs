using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data;
using Sitecore.Rules.Actions;
using SitecoreFeatureFlags.RulesContext;

namespace SitecoreFeatureFlags.Actions
{
    public class BlockControlsAction<T> : RuleAction<T> where T : PlaceholderSettingsRuleContext
    {
        public ID ControlId { get; set; }
        public override void Apply(T ruleContext)
        {
            Sitecore.Diagnostics.Assert.ArgumentNotNull(ruleContext, "ruleContext");
            Sitecore.Diagnostics.Assert.ArgumentNotNull(ControlId, "RenderingId");
            Sitecore.Diagnostics.Assert.ArgumentNotNull(ruleContext.ControlsToBlock, "ControlsToAllow");
            var renderingToAllow = ruleContext.Item.Database.GetItem(ControlId);
            ruleContext.ControlsToBlock.Add(renderingToAllow);
        }
    }
}