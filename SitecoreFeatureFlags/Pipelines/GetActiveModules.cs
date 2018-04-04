using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.GetPlaceholderRenderings;
using Sitecore.Rules;
using SitecoreFeatureFlags.RulesContext;

namespace SitecoreFeatureFlags.Pipelines
{
    public class GetActiveModules : GetAllowedRenderings
    {
        protected Item _contextItem;
        private const string AllowedModulesRulesField = "Allowed Modules Rule";

        public void Process(GetPlaceholderRenderingsArgs args)
        {
            Assert.IsNotNull(args, "args");
            try
            {
                _contextItem = _getContextItem();

                Item placeholderItem = null;
                if (args.DeviceId.IsNull)
                {
                    placeholderItem = Client.Page.GetPlaceholderItem(args.PlaceholderKey, args.ContentDatabase, args.LayoutDefinition);
                }
                else
                {
                    using (new DeviceSwitcher(args.DeviceId, args.ContentDatabase))
                    {
                         placeholderItem = Client.Page.GetPlaceholderItem(args.PlaceholderKey, args.ContentDatabase, args.LayoutDefinition);
                    }
                }
                if (placeholderItem != null)
                {
                    args.HasPlaceholderSettings = true;
                    this.ProcessPlaceholder(placeholderItem, args);
                }

            }
            catch (Exception ex)
            {
                Log.Error("Error in GetActiveModules", ex, this);
            }
        }
        
        private void ProcessPlaceholder(Item placeholderItem, GetPlaceholderRenderingsArgs args)
        {
            PlaceholderSettingsRuleContext context = this.EvaluateRules(placeholderItem);

            var controlsToAllow = context.ControlsToAllow;
            var controlsToBlock = context.ControlsToBlock;
            bool allowedControlsFieldSpecified = false;
            if (allowedControlsFieldSpecified)
            {
                args.Options.ShowTree = false;
            }
            foreach (var controlToAllow in controlsToAllow)
            {
                args.PlaceholderRenderings.Add(controlToAllow);
            }

            foreach (var controlToBlock in controlsToBlock)
            {
                args.PlaceholderRenderings.RemoveAll(c => c.ID == controlToBlock.ID);
            }
        }

        private PlaceholderSettingsRuleContext EvaluateRules(Item placeholder)
        {
            PlaceholderSettingsRuleContext context = new PlaceholderSettingsRuleContext();
            context.Item = _contextItem;
            foreach (Rule<PlaceholderSettingsRuleContext> rule in RuleFactory.GetRules<PlaceholderSettingsRuleContext>(new[] { placeholder }, AllowedModulesRulesField).Rules)
            {
                if (rule.Condition != null)
                {
                    rule.Execute(context);
                }
            }

            return context;
        }

        private Item _getContextItem()
        {
            Item contextItem = Context.Item;

            if (contextItem == null)
            {
                string itemId = _getContextItemId();
                var master = Factory.GetDatabase("master");
                contextItem = master.GetItem(new ID(itemId));
            }

            return contextItem;
        }

        private string _getContextItemId()
        {
            string result = string.Empty;
            if (Context.Request.QueryString["sc_itemid"] != null)
            {
                result = Context.Request.GetQueryString("sc_itemid");
            }
            else
            {
                var valueList = HttpUtility.ParseQueryString(Context.Request.GetQueryString("url"));
                result = valueList["sc_itemid"] ?? valueList["/?sc_itemid"];
            }

            return result;
        }
    }
}