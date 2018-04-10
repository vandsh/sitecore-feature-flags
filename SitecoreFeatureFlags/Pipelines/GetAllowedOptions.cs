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
        public const string ModuleTypeRulesField = "Module Type Rules";
        public void Process(GetLookupSourceItemsArgs args)
        {
            var source = args.Source;
            if (!source.IsNullOrEmpty())
            {
               _contextItem = args.Item.Database.GetItem(source);
            }
            try
            {
                var moduleOptionsRuleContext = EvaluateRules(_contextItem);
                var optionsToAllow = moduleOptionsRuleContext.OptionsToAllow;
                var optionsToBlock = moduleOptionsRuleContext.OptionsToBlock;

                foreach (var optionToAllow in optionsToAllow)
                {
                    args.Result.Add(optionToAllow);
                }
                foreach (var optionToBlock in optionsToBlock)
                {
                    var optionToBlockItem = args.Result.Cast<Item>().FirstOrDefault(rs => rs.ID == optionToBlock.ID);
                    args.Result.Remove(optionToBlockItem);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in GetAllowedOptions", ex, this);
            }
        }

        private ModuleOptionsRuleContext EvaluateRules(Item moduleItem)
        {
            var context = new ModuleOptionsRuleContext();
            context.Item = _contextItem;
            foreach (Rule<ModuleOptionsRuleContext> rule in RuleFactory.GetRules<ModuleOptionsRuleContext>(new[] { moduleItem }, ModuleTypeRulesField).Rules)
            {
                if (rule.Condition != null)
                {
                    rule.Execute(context);
                }
            }

            return context;
        }

        //private Item _getContextItem()
        //{
        //    Item contextItem = Context.Item;

        //    if (contextItem == null)
        //    {
        //        string itemId = _getContextItemId();
        //        var master = Factory.GetDatabase("master");
        //        contextItem = master.GetItem(new ID(itemId));
        //    }

        //    return contextItem;
        //}

        //private string _getContextItemId()
        //{
        //    string result = string.Empty;
        //    if (Context.Request.QueryString["sc_itemid"] != null)
        //    {
        //        result = Context.Request.GetQueryString("sc_itemid");
        //    }
        //    else
        //    {
        //        var valueList = HttpUtility.ParseQueryString(Context.Request.GetQueryString("url"));
        //        result = valueList["sc_itemid"] ?? valueList["/?sc_itemid"];
        //    }

        //    return result;
        //}
    }
}