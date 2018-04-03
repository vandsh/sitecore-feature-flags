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
                _contextItem = Context.Item;
                if (_contextItem == null)
                {
                    string itemId = Context.Request.GetQueryString("sc_itemid");
                    var master = Factory.GetDatabase("master");
                    _contextItem = master.GetItem(new ID(itemId));
                }

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
            //string allowedMessage = string.Format("GetActiveModules (allowed): Item-{0} PlaceholderItem-{1} PlaceholderKey-{2} Renderings-{3}", _contextItem.Paths.Path, placeholderItem.Paths.Path, args.PlaceholderKey, controlsToAllow.Select(i => i.Paths.Path).Aggregate((i, j) => i + "; " + j));
            //Log.Info(allowedMessage, this);
        }

        private PlaceholderSettingsRuleContext EvaluateRules(Item placeholder)
        {
            PlaceholderSettingsRuleContext context = new PlaceholderSettingsRuleContext();
            context.Item = _contextItem;
            foreach (Rule<PlaceholderSettingsRuleContext> rule in RuleFactory.GetRules<PlaceholderSettingsRuleContext>(new[] { placeholder }, AllowedModulesRulesField).Rules)
            {
                if (rule.Condition != null)
                {
                    var passed = rule.Evaluate(context);
                    if (passed)
                    {
                        rule.Execute(context);
                    }
                }
            }

            return context;
        }
    }
}