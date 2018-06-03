using Sitecore.Data;
using Sitecore.FeatureFlags.RulesContext;
using Sitecore.Rules.Actions;

namespace Sitecore.FeatureFlags.Actions
{
    public class AllowModuleOption<T> : RuleAction<T> where T : ModuleOptionsRuleContext
    {
        public ID OptionId { get; set; }
        public override void Apply(T ruleContext)
        {
            Sitecore.Diagnostics.Assert.ArgumentNotNull(ruleContext, "ruleContext");
            Sitecore.Diagnostics.Assert.ArgumentNotNull(OptionId, "OptionId");
            Sitecore.Diagnostics.Assert.ArgumentNotNull(ruleContext.OptionsToAllow, "AllowModuleOption");
            var optionToAllow = ruleContext.Item.Database.GetItem(OptionId);
            ruleContext.OptionsToAllow.Add(optionToAllow);
        }
    }
}