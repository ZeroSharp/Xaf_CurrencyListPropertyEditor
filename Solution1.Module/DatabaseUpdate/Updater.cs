using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using Solution1.Module.BusinessObjects;

namespace Solution1.Module.DatabaseUpdate
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppUpdatingModuleUpdatertopic
    public class Updater : ModuleUpdater
    {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
            base(objectSpace, currentDBVersion)
        {
        }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();

            var codes = 
                new Dictionary<string, string>()
                {
                    { "AUD", "Australian Dollars" },
                    { "CAD", "Canadian Dollars" },
                    { "CHF", "Swiss Francs" },
                    { "CNY", "Chinese Yuan Renminbi" },
                    { "EUR", "Euro" },
                    { "GBP", "British Pounds" },
                    { "HKD", "Hong-Kong Dollars" },
                    { "INR", "Indian Rupees" },
                    { "MYR", "Malaysian Ringgits" },
                    { "PHP", "Philippine Pesos" },
                    { "PLN", "Polish Zloty" },
                    { "SEK", "Swedish Krona" },
                    { "SGD", "Singapore Dollars" },
                    { "TWD", "Taiwan New Dollars" },
                    { "USD", "Australian Dollars" }
                };

            foreach (var keyValuePair in codes)
            {
                var currency = ObjectSpace.FindObject<Currency>(CriteriaOperator.Parse("Code=?", keyValuePair.Key));
                if (currency == null)
                {
                    currency = ObjectSpace.CreateObject<Currency>();
                    currency.Code = keyValuePair.Key;
                    currency.Name = keyValuePair.Value;
                }                
            }

            var container = ObjectSpace.FindObject<Container>(null);
            if (container == null)
            {
                container = ObjectSpace.CreateObject<Container>();
                container.Name = "Something";
                container.CurrencyList = "USD,GBP,CHF";
            }                
        }

        public override void UpdateDatabaseBeforeUpdateSchema()
        {
            base.UpdateDatabaseBeforeUpdateSchema();
        }
    }
}
