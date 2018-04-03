using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;
using Sitecore.Rules;

namespace SitecoreFeatureFlags.RulesContext
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