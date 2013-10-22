using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web.ASPxEditors;
using Solution1.Module.BusinessObjects;

namespace Solution1.Module.Web
{
    public class CurrencyListBoxTemplate : ASPxListBox, ITemplate
    {
        public CurrencyListBoxTemplate()
        {
            SelectionMode = ListEditSelectionMode.CheckColumn;
            EnableClientSideAPI = true;
            Width = Unit.Percentage(100.0);
            Height = 300;
        }

        private string _DropDownId;

        public void InstantiateIn(Control container)
        {
            InitClientSideEvents();
            container.Controls.Add(this);
        }

        private void InitClientSideEvents()
        {
            ClientSideEvents.Init =
                @"function (s, args) {
                    var listBox = ASPxClientControl.Cast(s);
                    listBox.autoResizeWithContainer = true;
                }";

            ClientSideEvents.SelectedIndexChanged =
                @"function (s, args) {
                    var listBox = ASPxClientControl.Cast(s);
                    var checkComboBox = ASPxClientControl.Cast(" + _DropDownId + @");

                    var selectedItems = listBox.GetSelectedItems();

                    var values = [];
                    for(var i = 0; i < selectedItems.length; i++)
                        values.push(selectedItems[i].value);

                    checkComboBox.SetText(values.join(','));
                }";
        }

        public void SetDropDownId(string id)
        {
            _DropDownId = id;
        }

        public void SetValue(string value)
        {
            foreach (ListEditItem item in Items)
            {
                item.Selected = value.Contains(item.Value.ToString());
            }
        }
    }

    [PropertyEditor(typeof(String), false)]
    public class CurrencyListPropertyEditor : ASPxPropertyEditor, IComplexViewItem
    {
        public IObjectSpace ObjectSpace { get; private set; }

        public CurrencyListPropertyEditor(Type objectType, IModelMemberViewItem info)
            : base(objectType, info) { }

        ASPxDropDownEdit _DropDownControl;
        
        private CurrencyListBoxTemplate _CurrencyListBoxTemplate;
        public CurrencyListBoxTemplate CurrencyListBoxTemplate
        {
            get
            {
                if (_CurrencyListBoxTemplate == null)
                    _CurrencyListBoxTemplate = new CurrencyListBoxTemplate();
                return _CurrencyListBoxTemplate;
            }
        }

        protected override WebControl CreateEditModeControlCore()
        {
            _DropDownControl = new ASPxDropDownEdit();
            _DropDownControl.ValueChanged += new EventHandler(ExtendedEditValueChangedHandler);
            _DropDownControl.EnableClientSideAPI = true;
            _DropDownControl.DropDownWindowTemplate = CurrencyListBoxTemplate;
            _DropDownControl.ClientInstanceName = "CurrencyListPropertyEditor_" + PropertyName;
            _DropDownControl.ReadOnly = true;

            CurrencyListBoxTemplate.SetDropDownId(_DropDownControl.ClientInstanceName);

            CurrencyListBoxTemplate.Items.Clear();

            var Currencies = ObjectSpace
                .GetObjects<Currency>()
                .OrderBy(x => x.Code);

            foreach (Currency currency in Currencies)
            {
                CurrencyListBoxTemplate.Items.Add(currency.Code + "\t" + currency.Name, currency.Code);
            }

            if (PropertyValue != null)
                CurrencyListBoxTemplate.SetValue(PropertyValue.ToString());

            return _DropDownControl;
        }

        protected override void ApplyReadOnly()
        {
            if (CurrencyListBoxTemplate != null)
            {
                CurrencyListBoxTemplate.ReadOnly = !base.AllowEdit;
            }
        }

        public override void BreakLinksToControl(bool unwireEventsOnly)
        {
            if (_DropDownControl != null)
            {
                _DropDownControl.ValueChanged -= new EventHandler(ExtendedEditValueChangedHandler);
            }

            if (_CurrencyListBoxTemplate != null)
            {
                _CurrencyListBoxTemplate.Dispose();
                _CurrencyListBoxTemplate = null;
            }

            base.BreakLinksToControl(unwireEventsOnly);
        }
       
        public void Setup(IObjectSpace objectSpace, XafApplication application)
        {
            ObjectSpace = objectSpace;
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (_CurrencyListBoxTemplate != null)
                    {
                        _CurrencyListBoxTemplate.Dispose();
                        _CurrencyListBoxTemplate = null;
                    }
                    if (_DropDownControl != null)
                    {
                        _DropDownControl.Dispose();
                        _DropDownControl = null;
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}