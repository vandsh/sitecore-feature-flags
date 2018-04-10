using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data;
using Sitecore.Rules.Actions;
using SitecoreFeatureFlags.RulesContext;

namespace SitecoreFeatureFlags.Actions
{
    public class AllowModuleOption<T> : RuleAction<T> where T : ModuleOptionsRuleContext
    {
        public ID OptionId { get; set; }
        public override void Apply(T ruleContext)
        {
            Sitecore.Diagnostics.Assert.ArgumentNotNull(ruleContext, "ruleContext");
            Sitecore.Diagnostics.Assert.ArgumentNotNull(OptionId, "OptionId");
            Sitecore.Diagnostics.Assert.ArgumentNotNull(ruleContext.OptionsToAllow, "AllowModuleOption");
            var renderingToAllow = ruleContext.Item.Database.GetItem(OptionId);
            ruleContext.OptionsToAllow.Add(renderingToAllow);
        }
    }
}