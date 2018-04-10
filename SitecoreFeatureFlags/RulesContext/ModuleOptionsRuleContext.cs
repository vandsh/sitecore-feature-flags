using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;
using Sitecore.Rules;

namespace SitecoreFeatureFlags.RulesContext
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