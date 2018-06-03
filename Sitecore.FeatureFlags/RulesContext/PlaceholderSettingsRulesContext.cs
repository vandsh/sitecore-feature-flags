using System.Collections.Generic;
using Sitecore.Data.Items;
using Sitecore.Rules;

namespace Sitecore.FeatureFlags.RulesContext
{
    public class PlaceholderSettingsRuleContext : RuleContext
    {
        public List<Item> ControlsToAllow { get; set; }
        public List<Item> ControlsToBlock { get; set; }

        public PlaceholderSettingsRuleContext()
            : base()
        {
            ControlsToAllow = new List<Item>();
            ControlsToBlock = new List<Item>();
        }
    }
}