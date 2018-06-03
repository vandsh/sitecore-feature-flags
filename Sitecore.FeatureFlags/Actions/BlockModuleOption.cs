using Sitecore.Data;
using Sitecore.FeatureFlags.RulesContext;
using Sitecore.Rules.Actions;

namespace Sitecore.FeatureFlags.Actions
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