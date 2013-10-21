using System;
using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace Solution1.Module.BusinessObjects
{
    [DefaultClassOptions]
    public class Container : BaseObject
    {
        public Container(Session session)
            : base(session)
        { }

        private string _CurrencyList;
        [ModelDefault("PropertyEditorType", "Solution1.Module.Web.CurrencyListPropertyEditor")]
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

        private string _CurrencyList2;
        [ModelDefault("PropertyEditorType", "Solution1.Module.Web.CurrencyListPropertyEditor")]
        public string CurrencyList2
        {
            get
            {
                return _CurrencyList2;
            }
            set
            {
                SetPropertyValue("CurrencyList2", ref _CurrencyList2, value);
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
