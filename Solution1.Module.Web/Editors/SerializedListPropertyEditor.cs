﻿using System;
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
    public class SerializedListBoxTemplate : ASPxListBox, ITemplate
    {
        public SerializedListBoxTemplate()
        {
            SelectionMode = ListEditSelectionMode.CheckColumn;
            EnableClientSideAPI = true;
            Width = Unit.Percentage(100.0);
            Height = 300;
        }

        private string _DropDownId;
        private char _SeparatorChar = ',';

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

                    checkComboBox.SetText(values.join('" + _SeparatorChar + @"'));
                }";
        }

        public void SetDropDownId(string id)
        {
            _DropDownId = id;
        }

        public void SetSeparatorChar(char separatorChar)
        {
            _SeparatorChar = separatorChar;
        }

        public void SetValue(string value)
        {
            foreach (ListEditItem item in Items)
            {
                item.Selected = value.Contains(item.Value.ToString());
            }
        }
    }

    public abstract class SerializedListPropertyEditor<T> : ASPxPropertyEditor, IComplexViewItem
    {
        public class ListBoxItem
        {
            public string DisplayText { get; set; }
            public string Value { get; set; }
        }

        public SerializedListPropertyEditor(Type objectType, IModelMemberViewItem info)
            : base(objectType, info) { }

        protected virtual IEnumerable<T> GetDataSource()
        { 
            if (Helper.IsPropertyDataSource)
            {
                var dataSource = Helper.CreateCollectionSource(this.CurrentObject);
                var enumerable = dataSource.Collection as IEnumerable;
                return enumerable.Cast<T>();
            }
            else
            {
                return ObjectSpace.GetObjects<T>(CriteriaOperator.Parse(this.Model.ModelMember.DataSourceCriteria));
            }
        }

        private IEnumerable<ListBoxItem> GetListBoxItems()
        {
            IEnumerable<T> dataSource = GetDataSource();
            return dataSource
                .Select(_ =>
                    new ListBoxItem()
                    {
                        DisplayText = GetDisplayText(_),
                        Value = GetValue(_)
                    })
                .OrderBy(_ => _.DisplayText);
        }

        protected abstract string GetDisplayText(T item);
        protected abstract string GetValue(T item);

        public IObjectSpace ObjectSpace { get; private set; }
        public ASPxDropDownEdit DropDownControl { get; private set; }
        public LookupEditorHelper Helper { get; private set; }

        private SerializedListBoxTemplate _ListBoxTemplate;
        public SerializedListBoxTemplate ListBoxTemplate
        {
            get
            {
                if (_ListBoxTemplate == null)
                    _ListBoxTemplate = new SerializedListBoxTemplate();
                return _ListBoxTemplate;
            }
        }

        private void PopulateListBoxItems()
        {
            ListBoxTemplate.Items.Clear();
            foreach (var item in GetListBoxItems())
            {
                ListBoxTemplate.Items.Add(item.DisplayText, item.Value);
            }
        }

        protected override WebControl CreateEditModeControlCore()
        {
            DropDownControl = new ASPxDropDownEdit();
            DropDownControl.ValueChanged += ExtendedEditValueChangedHandler;
            DropDownControl.EnableClientSideAPI = true;
            DropDownControl.DropDownWindowTemplate = ListBoxTemplate;
            DropDownControl.ClientInstanceName = "ListPropertyEditor_" + PropertyName;
            DropDownControl.ReadOnly = true;

            ListBoxTemplate.SetDropDownId(DropDownControl.ClientInstanceName);
            PopulateListBoxItems();

            if (PropertyValue != null)
                ListBoxTemplate.SetValue(PropertyValue.ToString());

            return DropDownControl;
        }

        protected override void ApplyReadOnly()
        {
            if (DropDownControl != null)
            {
                DropDownControl.Enabled = AllowEdit;
            }
        }

        public override void BreakLinksToControl(bool unwireEventsOnly)
        {
            if (DropDownControl != null)
            {
                DropDownControl.ValueChanged -= ExtendedEditValueChangedHandler;
            }

            if (_ListBoxTemplate != null)
            {
                _ListBoxTemplate.Dispose();
                _ListBoxTemplate = null;
            }

            base.BreakLinksToControl(unwireEventsOnly);
        }
       
        public void Setup(IObjectSpace objectSpace, XafApplication application)
        {
            Helper = new WebLookupEditorHelper(application, objectSpace, MemberInfo.MemberTypeInfo, Model);
            ObjectSpace = objectSpace;
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (_ListBoxTemplate != null)
                    {
                        _ListBoxTemplate.Dispose();
                        _ListBoxTemplate = null;
                    }
                    if (DropDownControl != null)
                    {
                        DropDownControl.Dispose();
                        DropDownControl = null;
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }

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