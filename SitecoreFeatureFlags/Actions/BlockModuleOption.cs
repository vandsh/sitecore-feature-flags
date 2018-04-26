using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data;
using Sitecore.Rules.Actions;
using SitecoreFeatureFlags.RulesContext;

namespace SitecoreFeatureFlags.Actions
{
    public class BlockModuleOption<T> : RuleAction<T> where T : ModuleOptionsRuleContext
    {
        public ID OptionId { get; set; }
        public override void Apply(T ruleContext)
        {
            Sitecore.Diagnostics.Assert.ArgumentNotNull(ruleContext, "ruleContext");
            Sitecore.Diagnostics.Assert.ArgumentNotNull(OptionId, "OptionId");
            Sitecore.Diagnostics.Assert.ArgumentNotNull(ruleContext.OptionsToBlock, "BlockModuleOption");
            var optionToBlock = ruleContext.Item.Database.GetItem(OptionId);
            ruleContext.OptionsToBlock.Add(optionToBlock);
        }
    }
}