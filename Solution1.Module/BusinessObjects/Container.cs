using System;
using System.Linq;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Model;
using System.Collections.Generic;
using DevExpress.Persistent.Base.General;

namespace Solution1.Module.BusinessObjects
{
    [DefaultClassOptions]
    public class Container : BaseObject
    {
        public Container(Session session)
            : base(session)
        { }

        private string _CurrencyList;
        [ModelDefault("PropertyEditorType", "Solution1.Module.Web.SerializedLookupPropertyEditor")]
        public string CurrencyList
        {
            get
            {
                return _CurrencyList;
            }
            set
            {
                SetPropertyValue("CurrencyList", ref _CurrencyList, value);
            }
        }

        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                SetPropertyValue("Name", ref _Name, value);
            }
        }
    }
}
