using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web.ASPxEditors;
using Solution1.Module.BusinessObjects;

namespace Solution1.Module.Web
{
    [PropertyEditor(typeof(String), false)]
    public class CurrencyListPropertyEditor : SerializedListPropertyEditor<Currency>
    {
        public CurrencyListPropertyEditor(Type objectType, IModelMemberViewItem info)
            : base(objectType, info) { }

        protected override string GetDisplayText(Currency currency)
        {
            return String.Format("{0}\t{1}", currency.Code, currency.Name);
        }

        protected override string GetValue(Currency currency)
        {
            return currency.Code;
        }
    }
}
