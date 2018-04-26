using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;

namespace SitecoreFeatureFlags.Rules
{
    public class IsExactTemplate<T> : WhenCondition<T> where T : RuleContext
    {
        private ID templateId;

        public ID TemplateId
        {
            get
            {
                return this.templateId;
            }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                this.templateId = value;
            }
        }

        public IsExactTemplate()
        {
            this.templateId = ID.Null;
        }

        public IsExactTemplate(ID templateId)
        {
            Assert.ArgumentNotNull(templateId, "templateId");
            this.templateId = templateId;
        }

        protected override bool Execute(T ruleContext)
        {
            var ruleResponse = false;
            Assert.ArgumentNotNull(ruleContext, "ruleContext");
            Item item = ruleContext.Item;
            if (item == null)
            {
                ruleResponse = false;
            }
            if (item.TemplateID == this.TemplateId)
            {
                ruleResponse = true;
            }
            return ruleResponse;
        }
    }
}