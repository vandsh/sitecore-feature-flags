using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.GetLookupSourceItems;
using Sitecore.Rules;
using Sitecore.StringExtensions;
using SitecoreFeatureFlags.RulesContext;

namespace SitecoreFeatureFlags.Pipelines
{
    public class GetAllowedOptions
    {
        protected Item _contextItem;
        protected Item _datasourceItem;
        public const string ModuleTypeRulesField = "Module Type Rules";
        public void Process(GetLookupSourceItemsArgs args)
        {
            var source = args.Source;
            if (!source.IsNullOrEmpty())
            {
                _datasourceItem = args.Item.Database.GetItem(source);
                _contextItem = args.Item;
            }
            try
            {
                var rules = RuleFactory.GetRules<ModuleOptionsRuleContext>(new[] {_datasourceItem}, ModuleTypeRulesField).Rules;
                if (rules.Any())
                {
                    var moduleOptionsRuleContext = EvaluateRules(rules);
                    var optionsToAllow = moduleOptionsRuleContext.OptionsToAllow;
                    var optionsToBlock = moduleOptionsRuleContext.OptionsToBlock;

                    var optionIdsToBlock = optionsToBlock.Select(x => x.ID);
                    var optionsToRemove = args.Result.Cast<Item>().Where(oi => optionIdsToBlock.Contains(oi.ID)).ToList();
                    foreach (var optionToRemove in optionsToRemove)
                    {
                        args.Result.Remove(optionToRemove);
                        Log.Info(string.Format("GetAllowedOptions: {0} removed for {1}", optionToRemove.Name, _contextItem.Name), this);
                    }

                    foreach (var optionToAllow in optionsToAllow)
                    {
                        if (!args.Result.ContainsID(optionToAllow.ID))
                        {
                            args.Result.Add(optionToAllow);
                            Log.Info(string.Format("GetAllowedOptions: {0} allowed for {1}", optionToAllow.Name, _contextItem.Name), this);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in GetAllowedOptions", ex, this);
            }
        }

        private ModuleOptionsRuleContext EvaluateRules(IEnumerable<Rule<ModuleOptionsRuleContext>> rules)
        {
            var ruleContext = new ModuleOptionsRuleContext();
            ruleContext.Item = _contextItem;
            foreach (Rule<ModuleOptionsRuleContext> rule in rules)
            {
                if (rule.Condition != null)
                {
                    var passed = rule.Evaluate(ruleContext);
                    if (passed)
                        rule.Execute(ruleContext);
                }
            }

            return ruleContext;
        }
    }
}