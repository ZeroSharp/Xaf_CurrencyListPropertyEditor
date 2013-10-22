using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace Solution1.Module.BusinessObjects
{
    //[DefaultClassOptions]
    //public class Currency : BaseObject
    //{
    //    public Currency(Session session) : base(session) { }

    //    [Size(3)]
    //    public string Code;

    //    public string Name;

    //    [Association("List1Currencies")]
    //    public XPCollection<Container> List1Container
    //    {
    //        get { return GetCollection<Container>("List1Container"); }
    //    }
    //}

    [DefaultClassOptions]
    [DefaultProperty("Code")]
    public class Currency : BaseObject
    { 
        public Currency(Session session)
            : base(session)
        {
        }

        private string _Code;
        [Size(3)]
        public string Code
        {
            get
            {
                return _Code;
            }
            set
            {
                SetPropertyValue("Code", ref _Code, value);
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
