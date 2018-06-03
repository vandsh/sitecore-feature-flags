using System.Collections.Generic;
using Sitecore.Data.Items;
using Sitecore.Rules;

namespace Sitecore.FeatureFlags.RulesContext
{
    public class ModuleOptionsRuleContext : RuleContext
    {
        public List<Item> OptionsToAllow { get; set; }
        public List<Item> OptionsToBlock { get; set; }

        public ModuleOptionsRuleContext()
            : base()
        {
            OptionsToAllow = new List<Item>();
            OptionsToBlock = new List<Item>();
        }
    }
}