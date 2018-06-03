using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
                Assert.IsNotNull(_contextItem, "_contextItem");

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
            if (placeholderItem == null)
            {
                Log.Info("ProcessPlaceholder: placeholderItem empty, stopping execution of Rule", this);
                return;
            }
            if (args == null)
            {
                Log.Info("ProcessPlaceholder: args empty, stopping execution of Rule", this);
                return;
            }

            PlaceholderSettingsRuleContext context = null;
            List<Item> controlsToAllow = null;
            List<Item> controlsToBlock = null;
            try
            {
                context = this.EvaluateRules(placeholderItem);
            }
            catch (Exception e)
            {
                Log.Info(string.Format("ProcessPlaceholder: unable to EvaluateRules (placeholderItem {0}, args {1})",  placeholderItem.ID, string.Join(",", args.PlaceholderRenderings.Select(r => r.ID))), this);
            }

            try
            {
                if (context != null)
                {
                    controlsToAllow = context.ControlsToAllow;
                    controlsToBlock = context.ControlsToBlock;
                    bool allowedControlsFieldSpecified = false;
                    if (allowedControlsFieldSpecified)
                    {
                        args.Options.ShowTree = false;
                    }

                    foreach (var controlToBlock in controlsToBlock)
                    {
                        if(args.PlaceholderRenderings != null && args.PlaceholderRenderings.Any()) { 
                            args.PlaceholderRenderings.RemoveAll(c => c.ID == controlToBlock.ID);
                            Log.Info( string.Format("GetActiveModules: {0} blocked for {1}", controlToBlock.Name, _contextItem.Name), this);
                        }
                    }

                    foreach (var controlToAllow in controlsToAllow)
                    {
                        if (args.PlaceholderRenderings == null)
                        {
                            args.PlaceholderRenderings = new List<Item>();
                        }
                        args.PlaceholderRenderings.Add(controlToAllow);
                        Log.Info( string.Format("GetActiveModules: {0} allowed for {1}", controlToAllow.Name, _contextItem.Name), this);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Info(string.Format("ProcessPlaceholder: unable to process BlockedModules (placeholderItem {0}, controlsToAllow {1}, controlsToBlock {2})", placeholderItem.ID, string.Join(",", controlsToAllow.Select(r => r.ID)), string.Join(",", controlsToBlock.Select(r => r.ID))), this);
            }

        }

        private PlaceholderSettingsRuleContext EvaluateRules(Item placeholder)
        {
            PlaceholderSettingsRuleContext ruleContext = new PlaceholderSettingsRuleContext();
            ruleContext.Item = _contextItem;
            foreach (Rule<PlaceholderSettingsRuleContext> rule in RuleFactory.GetRules<PlaceholderSettingsRuleContext>(new[] { placeholder }, AllowedModulesRulesField).Rules)
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

        private Item _getContextItem()
        {
            Item contextItem = Context.Item;

            if (contextItem == null)
            {
                try
                {
                    string itemId = _getContextItemId();
                    var master = Factory.GetDatabase("master");
                    contextItem = master.GetItem(new ID(itemId));
                }
                catch (Exception ex)
                {
                    Log.Warn(string.Format("GetActiveModules: could not extract context item from query string - {0}", ex.Message), this);
                }
            }

            return contextItem;
        }

        private string _getContextItemId()
        {
            string empty = string.Empty;
            if (Context.Request.QueryString["id"] == null)
            {
                int? indexOf = Context.Request.GetQueryString("url")?.IndexOf("?");
                if (indexOf.HasValue)
                {
                    NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(Context.Request.GetQueryString("url").Substring(indexOf.Value));
                    empty = nameValueCollection["sc_itemid"];
                }
            }
            else
            {
                empty = Context.Request.GetQueryString("id");
            }
            return empty;
        }
    }
}