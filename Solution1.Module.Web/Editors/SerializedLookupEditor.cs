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
using System.Collections.Generic;
using DevExpress.Xpo;

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
            //EnableTheming = true;
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
    public class CurrencyListPropertyEditor : ASPxStringPropertyEditor, IComplexViewItem
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
            base.CreateEditModeControlCore();

            _DropDownControl = new ASPxDropDownEdit();
            _DropDownControl.ValueChanged += new EventHandler(ExtendedEditValueChangedHandler);
            _DropDownControl.EnableClientSideAPI = true;
            _DropDownControl.DropDownWindowTemplate = CurrencyListBoxTemplate;
            _DropDownControl.ClientInstanceName = "CurrencyListPropertyEditor_" + PropertyName;
            //_DropDownControl.EnableTheming = true;

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
                CurrencyListBoxTemplate.ReadOnly = (bool)!base.AllowEdit;
            }
            if (_DropDownControl != null)
            {
                _DropDownControl.ReadOnly = (bool)!base.AllowEdit;
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


    //[PropertyEditor(typeof(String), false)]
    //public class SerializedLookupPropertyEditor : ASPxObjectPropertyEditorBase, IDependentPropertyEditor, ITestable, ISupportViewShowing, IFrameContainer
    //{
    //    // Fields
    //    private List<IObjectSpace> createdObjectSpaces;
    //    private ASPxLookupDropDownEdit dropDownEdit;
    //    private string editorId;
    //    private ASPxLookupFindEdit findEdit;
    //    internal NestedFrame frame;
    //    private WebLookupEditorHelper helper;
    //    private ListView listView;
    //    private object newObject;
    //    private IObjectSpace newObjectSpace;
    //    private NewObjectViewController newObjectViewController;
    //    private PopupWindowShowAction newObjectWindowAction;
    //    private PopupWindowShowAction showFindSelectWindowAction;
    //    private static int windowHeight = 480;
    //    private static int windowWidth = 800;

    //    // Events
    //    event EventHandler<EventArgs> ISupportViewShowing.ViewShowingNotification
    //    {
    //        add
    //        {
    //            this.viewShowingNotification += value;
    //        }
    //        remove
    //        {
    //            this.viewShowingNotification -= value;
    //        }
    //    }

    //    private event EventHandler<EventArgs> viewShowingNotification;

    //    // Methods
    //    public SerializedLookupPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model)
    //    {
    //        this.createdObjectSpaces = new List<IObjectSpace>();
    //        base.skipEditModeDataBind = true;
    //    }

    //    private void action_FindWindowParamsCustomizing(object sender, CustomizePopupWindowParamsEventArgs args)
    //    {
    //        this.OnViewShowingNotification();
    //        args.View = this.helper.CreateListView(base.CurrentObject);
    //        FindLookupDialogController controller = this.helper.Application.CreateController<FindLookupDialogController>();
    //        controller.Initialize(this.helper);
    //        args.DialogController = controller;
    //    }

    //    private void actionFind_OnExecute(object sender, PopupWindowShowActionExecuteEventArgs args)
    //    {
    //        string objectKey = this.helper.GetObjectKey(((ListView) args.PopupWindow.View).CurrentObject);
    //        ((PopupWindow) args.PopupWindow).ClosureScript = "if(window.dialogOpener != null) window.dialogOpener.findLookupResult = '" + this.EscapeObjectKey(objectKey) + "';";
    //    }

    //    protected override void ApplyReadOnly()
    //    {
    //        if (this.findEdit != null)
    //        {
    //            this.findEdit.ReadOnly = (bool) !base.AllowEdit;
    //        }
    //        if (this.dropDownEdit != null)
    //        {
    //            this.dropDownEdit.ReadOnly = (bool) !base.AllowEdit;
    //        }
    //    }

    //    public override void BreakLinksToControl(bool unwireEventsOnly)
    //    {
    //        if (this.findEdit != null)
    //        {
    //            this.findEdit.ValueChanged -= new EventHandler(this.findLookup_ValueChanged);
    //            this.findEdit.Callback -= new EventHandler<CallbackEventArgs>(this.findLookup_Callback);
    //        }
    //        if (this.dropDownEdit != null)
    //        {
    //            this.dropDownEdit.DropDown.SelectedIndexChanged -= new EventHandler(this.dropDownLookup_SelectedIndexChanged);
    //            this.dropDownEdit.Callback -= new EventHandler<CallbackEventArgs>(this.dropDownLookup_Callback);
    //            this.dropDownEdit.Init -= new EventHandler(this.dropDownLookup_Init);
    //            this.dropDownEdit.PreRender -= new EventHandler(this.dropDownLookup_PreRender);
    //        }
    //        if (!unwireEventsOnly)
    //        {
    //            this.findEdit = null;
    //            this.dropDownEdit = null;
    //        }
    //        base.BreakLinksToControl(unwireEventsOnly);
    //    }

    //    private ASPxLookupDropDownEdit CreateDropDownLookupControl()
    //    {
    //        ASPxLookupDropDownEdit editor = new ASPxLookupDropDownEdit {
    //            Width = Unit.Percentage(100.0)
    //        };
    //        editor.DropDown.ClientSideEvents.SelectedIndexChanged = "function(sender, args) {{ args.processOnServer = false; }}";
    //        editor.DropDown.SelectedIndexChanged += new EventHandler(this.dropDownLookup_SelectedIndexChanged);
    //        this.UpdateDropDownLookup(editor);
    //        editor.ReadOnly = (bool) !base.AllowEdit;
    //        editor.Init += new EventHandler(this.dropDownLookup_Init);
    //        editor.PreRender += new EventHandler(this.dropDownLookup_PreRender);
    //        editor.Callback += new EventHandler<CallbackEventArgs>(this.dropDownLookup_Callback);
    //        return editor;
    //    }

    //    protected override WebControl CreateEditModeControlCore()
    //    {
    //        if (this.newObjectWindowAction == null)
    //        {
    //            this.newObjectWindowAction = new PopupWindowShowAction(null, "New", PredefinedCategory.Unspecified.ToString());
    //            this.newObjectWindowAction.Execute += new PopupWindowShowActionExecuteEventHandler(this.newObjectWindowAction_OnExecute);
    //            this.newObjectWindowAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(this.newObjectWindowAction_OnCustomizePopupWindowParams);
    //            this.newObjectWindowAction.Application = this.helper.Application;
    //        }
    //        Panel panel = new Panel();
    //        this.dropDownEdit = this.CreateDropDownLookupControl();
    //        panel.Controls.Add(this.dropDownEdit);
    //        this.findEdit = this.CreateFindLookupControl();
    //        panel.Controls.Add(this.findEdit);
    //        this.UpdateControlsVisibility();
    //        return panel;
    //    }

    //    private ASPxLookupFindEdit CreateFindLookupControl()
    //    {
    //        if (this.showFindSelectWindowAction == null)
    //        {
    //            this.showFindSelectWindowAction = new PopupWindowShowAction(null, base.MemberInfo.Name + "_ASPxLookupEditor_ShowFindWindow", PredefinedCategory.Unspecified);
    //            this.showFindSelectWindowAction.Execute += new PopupWindowShowActionExecuteEventHandler(this.actionFind_OnExecute);
    //            this.showFindSelectWindowAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(this.action_FindWindowParamsCustomizing);
    //            this.showFindSelectWindowAction.Application = base.application;
    //            this.showFindSelectWindowAction.AcceptButtonCaption = "";
    //        }
    //        ASPxLookupFindEdit edit = new ASPxLookupFindEdit {
    //            Width = Unit.Percentage(100.0),
    //            ReadOnly = (bool) !base.AllowEdit
    //        };
    //        edit.ValueChanged += new EventHandler(this.findLookup_ValueChanged);
    //        edit.Init += new EventHandler(this.findLookup_Init);
    //        edit.Callback += new EventHandler<CallbackEventArgs>(this.findLookup_Callback);
    //        return edit;
    //    }

    //    protected override void Dispose(bool disposing)
    //    {
    //        try
    //        {
    //            if (disposing)
    //            {
    //                if (this.showFindSelectWindowAction != null)
    //                {
    //                    this.showFindSelectWindowAction.Execute -= new PopupWindowShowActionExecuteEventHandler(this.actionFind_OnExecute);
    //                    this.showFindSelectWindowAction.CustomizePopupWindowParams -= new CustomizePopupWindowParamsEventHandler(this.action_FindWindowParamsCustomizing);
    //                    base.DisposeAction(this.showFindSelectWindowAction);
    //                    this.showFindSelectWindowAction = null;
    //                }
    //                if (this.newObjectWindowAction != null)
    //                {
    //                    this.newObjectWindowAction.Execute -= new PopupWindowShowActionExecuteEventHandler(this.newObjectWindowAction_OnExecute);
    //                    this.newObjectWindowAction.CustomizePopupWindowParams -= new CustomizePopupWindowParamsEventHandler(this.newObjectWindowAction_OnCustomizePopupWindowParams);
    //                    base.DisposeAction(this.newObjectWindowAction);
    //                    this.newObjectWindowAction = null;
    //                }
    //                if (this.newObjectViewController != null)
    //                {
    //                    this.newObjectViewController.ObjectCreating -= new EventHandler<ObjectCreatingEventArgs>(this.newObjectViewController_ObjectCreating);
    //                    this.newObjectViewController.ObjectCreated -= new EventHandler<ObjectCreatedEventArgs>(this.newObjectViewController_ObjectCreated);
    //                    this.newObjectViewController = null;
    //                }
    //                if (this.frame != null)
    //                {
    //                    this.frame.SetView(null);
    //                    this.frame.Dispose();
    //                    this.frame = null;
    //                }
    //                if (this.listView != null)
    //                {
    //                    this.listView.Dispose();
    //                    this.listView = null;
    //                }
    //                foreach (IObjectSpace space in this.createdObjectSpaces)
    //                {
    //                    if (!space.IsDisposed)
    //                    {
    //                        space.Dispose();
    //                    }
    //                }
    //                this.createdObjectSpaces.Clear();
    //                this.newObject = null;
    //                this.newObjectSpace = null;
    //            }
    //        }
    //        finally
    //        {
    //            base.Dispose(disposing);
    //        }
    //    }

    //    private void dropDownLookup_Callback(object sender, CallbackEventArgs e)
    //    {
    //        if (!string.IsNullOrEmpty(e.Argument))
    //        {
    //            this.FillEditorValues(this.GetObjectByKey(e.Argument));
    //        }
    //    }

    //    private void dropDownLookup_Init(object sender, EventArgs e)
    //    {
    //        this.UpdateDropDownLookup((ASPxLookupDropDownEdit) sender);
    //    }

    //    private void dropDownLookup_PreRender(object sender, EventArgs e)
    //    {
    //        this.UpdateDropDownLookup((ASPxLookupDropDownEdit) sender);
    //    }

    //    private void dropDownLookup_SelectedIndexChanged(object source, EventArgs e)
    //    {
    //        if (!((ASPxComboBox) source).IsCallback)
    //        {
    //            base.EditValueChangedHandler(source, EventArgs.Empty);
    //        }
    //    }

    //    private string EscapeObjectKey(string key)
    //    {
    //        return key.Replace("'", @"\'");
    //    }

    //    private void FillEditorValues(object currentSelectedObject)
    //    {
    //        ASPxComboBox dropDown = this.dropDownEdit.DropDown;
    //        dropDown.Items.Clear();
    //        dropDown.Items.Add(WebPropertyEditor.EmptyValue, null);
    //        dropDown.SelectedIndex = 0;
    //        if (base.CurrentObject != null)
    //        {
    //            if (this.DataSource == null)
    //            {
    //                this.RecreateListView(true);
    //            }
    //            else
    //            {
    //                this.helper.ReloadCollectionSource(this.DataSource, base.CurrentObject);
    //                this.RecreateListView(true);
    //            }
    //            if (this.DataSource == null)
    //            {
    //                throw new ArgumentNullException("DataSource");
    //            }
    //            ArrayList list = new ArrayList();
    //            if (this.DataSource.List != null)
    //            {
    //                for (int i = 0; i < this.DataSource.List.Count; i++)
    //                {
    //                    list.Add(this.DataSource.List[i]);
    //                }
    //            }
    //            else
    //            {
    //                if (currentSelectedObject != null)
    //                {
    //                    dropDown.Items.Add(this.helper.GetEscapedDisplayText(currentSelectedObject, WebPropertyEditor.EmptyValue, base.DisplayFormat), -1);
    //                    dropDown.SelectedIndex = 1;
    //                }
    //                return;
    //            }
    //            if ((currentSelectedObject != null) && (list.IndexOf(currentSelectedObject) == -1))
    //            {
    //                list.Add(currentSelectedObject);
    //            }
    //            if (this.helper.LookupListViewModel.Sorting.Count == 0)
    //            {
    //                //list.Sort(new DisplayValueComparer(this.helper, WebPropertyEditor.EmptyValue));
    //            }
    //            foreach (object obj2 in list)
    //            {
    //                dropDown.Items.Add(this.helper.GetEscapedDisplayText(obj2, WebPropertyEditor.EmptyValue, base.DisplayFormat), this.helper.GetObjectKey(obj2));
    //                if (currentSelectedObject == obj2)
    //                {
    //                    dropDown.SelectedIndex = list.IndexOf(obj2) + 1;
    //                }
    //            }
    //        }
    //    }

    //    private void findLookup_Callback(object sender, CallbackEventArgs e)
    //    {
    //        if (e.Argument == "clear")
    //        {
    //            this.findEdit.Text = this.helper.GetDisplayText(null, WebPropertyEditor.EmptyValue, base.DisplayFormat);
    //        }
    //        else if (e.Argument.StartsWith("found"))
    //        {
    //            this.findEdit.Text = this.helper.GetDisplayText(this.GetObjectByKey(e.Argument.Substring(5)), WebPropertyEditor.EmptyValue, base.DisplayFormat);
    //        }
    //    }

    //    private void findLookup_Init(object sender, EventArgs e)
    //    {
    //        ASPxLookupFindEdit findEdit = (ASPxLookupFindEdit) sender;
    //        findEdit.Init -= new EventHandler(this.findLookup_Init);
    //        this.UpdateFindButtonScript(findEdit);
    //    }

    //    private void findLookup_ValueChanged(object sender, EventArgs e)
    //    {
    //        base.EditValueChangedHandler(sender, EventArgs.Empty);
    //    }

    //    protected override WebControl GetActiveControl()
    //    {
    //        if (this.ActiveControl is ASPxLookupFindEdit)
    //        {
    //            return ((ASPxLookupFindEdit) this.ActiveControl).TextBox;
    //        }
    //        if (this.ActiveControl is ASPxLookupDropDownEdit)
    //        {
    //            return ((ASPxLookupDropDownEdit) this.ActiveControl).DropDown;
    //        }
    //        return base.GetActiveControl();
    //    }

    //    protected override object GetControlValueCore()
    //    {
    //        if ((base.ViewEditMode != ViewEditMode.Edit) || (base.Editor == null))
    //        {
    //            return base.PropertyValue;
    //        }
    //        if (this.UseFindEdit())
    //        {
    //            return this.GetObjectByKey(this.findEdit.Value);
    //        }
    //        ASPxComboBox dropDown = this.dropDownEdit.DropDown;
    //        if (((dropDown.SelectedIndex != -1) && (dropDown.Value != null)) && (this.DataSource != null))
    //        {
    //            CollectionSourceBase dataSource = this.DataSource;
    //            return this.GetObjectByKey(dropDown.Value.ToString());
    //        }
    //        return null;
    //    }

    //    protected override string GetEditorClientId()
    //    {
    //        if (!this.UseFindEdit())
    //        {
    //            return this.dropDownEdit.ClientID;
    //        }
    //        return this.findEdit.ClientID;
    //    }

    //    //protected internal override IJScriptTestControl GetEditorTestControlImpl()
    //    //{
    //    //    if ((this.findEdit != null) && this.UseFindEdit())
    //    //    {
    //    //        return new JSASPxLookupProperytEditorTestControl();
    //    //    }
    //    //    return new JSASPxSimpleLookupTestControl();
    //    //}

    //    //protected internal override IJScriptTestControl GetInplaceViewModeEditorTestControlImpl()
    //    //{
    //    //    return new JSButtonTestControl();
    //    //}

    //    private object GetObjectByKey(string key)
    //    {
    //        return this.helper.GetObjectByKey(base.CurrentObject, key);
    //    }

    //    protected override string GetPropertyDisplayValue()
    //    {
    //        return this.GetPropertyDisplayValueForObject(base.PropertyValue);
    //    }

    //    private string GetPropertyDisplayValueForObject(object obj)
    //    {
    //        return this.helper.GetDisplayText(obj, WebPropertyEditor.EmptyValue, base.DisplayFormat);
    //    }

    //    internal string GetSearchActionName()
    //    {
    //        return this.Frame.GetController<FilterController>().FullTextFilterAction.Caption;
    //    }

    //    public void InitializeFrame()
    //    {
    //        if (this.frame == null)
    //        {
    //            this.frame = this.helper.Application.CreateNestedFrame(this, TemplateContext.LookupControl);
    //            this.newObjectViewController = this.frame.GetController<NewObjectViewController>();
    //            if (this.newObjectViewController != null)
    //            {
    //                this.newObjectViewController.ObjectCreating += new EventHandler<ObjectCreatingEventArgs>(this.newObjectViewController_ObjectCreating);
    //                this.newObjectViewController.ObjectCreated += new EventHandler<ObjectCreatedEventArgs>(this.newObjectViewController_ObjectCreated);
    //            }
    //        }
    //    }

    //    private void newObjectViewController_ObjectCreated(object sender, ObjectCreatedEventArgs e)
    //    {
    //        this.newObject = e.CreatedObject;
    //        this.newObjectSpace = e.ObjectSpace;
    //        this.createdObjectSpaces.Add(this.newObjectSpace);
    //    }

    //    private void newObjectViewController_ObjectCreating(object sender, ObjectCreatingEventArgs e)
    //    {
    //        e.ShowDetailView = false;
    //        if (e.ObjectSpace is INestedObjectSpace)
    //        {
    //            e.ObjectSpace = base.application.CreateObjectSpace(e.ObjectType);
    //        }
    //    }

    //    private void newObjectWindowAction_OnCustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs args)
    //    {
    //        if (!this.DataSource.AllowAdd)
    //        {
    //            throw new InvalidOperationException();
    //        }
    //        if (this.newObjectViewController != null)
    //        {
    //            this.OnViewShowingNotification();
    //            this.newObjectViewController.NewObjectAction.DoExecute(this.newObjectViewController.NewObjectAction.Items[0]);
    //            args.View = base.application.CreateDetailView(this.newObjectSpace, this.newObject, this.listView);
    //        }
    //    }

    //    private void newObjectWindowAction_OnExecute(object sender, PopupWindowShowActionExecuteEventArgs args)
    //    {
    //        if (!this.DataSource.AllowAdd)
    //        {
    //            throw new InvalidOperationException();
    //        }
    //        if (base.objectSpace != args.PopupWindow.View.ObjectSpace)
    //        {
    //            args.PopupWindow.View.ObjectSpace.CommitChanges();
    //        }
    //        this.DataSource.Add(this.helper.ObjectSpace.GetObject(((DetailView) args.PopupWindow.View).CurrentObject));
    //        ((PopupWindow) args.PopupWindow).ClosureScript = "if(window.dialogOpener != null) window.dialogOpener.ddLookupResult = '" + this.helper.GetObjectKey(((DetailView) args.PopupWindow.View).CurrentObject) + "';";
    //    }

    //    protected override void OnCurrentObjectChanged()
    //    {
    //        if (base.Editor != null)
    //        {
    //            this.RecreateListView(false);
    //            this.UpdateControlsVisibility();
    //        }
    //        base.OnCurrentObjectChanged();
    //        if (this.dropDownEdit != null)
    //        {
    //            this.UpdateDropDownLookup(this.dropDownEdit);
    //        }
    //    }

    //    private void OnViewShowingNotification()
    //    {
    //        if (this.viewShowingNotification != null)
    //        {
    //            this.viewShowingNotification(this, EventArgs.Empty);
    //        }
    //    }

    //    protected override void ReadEditModeValueCore()
    //    {
    //        this.UpdateControlsVisibility();
    //        object propertyValue = base.PropertyValue;
    //        if ((this.dropDownEdit != null) && this.dropDownEdit.Visible)
    //        {
    //            this.FillEditorValues(propertyValue);
    //        }
    //        if (this.findEdit != null)
    //        {
    //            this.findEdit.Value = this.helper.GetObjectKey(propertyValue);
    //            this.findEdit.Text = this.GetPropertyDisplayValueForObject(propertyValue);
    //        }
    //    }

    //    private void RecreateListView(bool ifNotCreatedOnly)
    //    {
    //        if ((base.ViewEditMode == ViewEditMode.Edit) && (!ifNotCreatedOnly || (this.listView == null)))
    //        {
    //            this.listView = null;
    //            if (base.CurrentObject != null)
    //            {
    //                this.listView = this.helper.CreateListView(base.CurrentObject);
    //            }
    //            this.Frame.SetView(this.listView);
    //        }
    //    }

    //    public override void Refresh()
    //    {
    //        this.UpdateControlsVisibility();
    //        base.Refresh();
    //    }

    //    public void SetControlValue(object val)
    //    {
    //        object controlValueCore = this.GetControlValueCore();
    //        if ((((controlValueCore == null) && (val == null)) || (controlValueCore != val)) && (base.CurrentObject != null))
    //        {
    //            this.OnValueStoring(this.helper.GetDisplayText(val, WebPropertyEditor.EmptyValue, base.DisplayFormat));
    //            base.PropertyValue = this.helper.ObjectSpace.GetObject(val);
    //            this.OnValueStored();
    //            base.ReadValue();
    //        }
    //    }

    //    protected override void SetEditorId(string controlId)
    //    {
    //        this.editorId = controlId;
    //        this.UpdateControlId();
    //    }

    //    protected override void SetImmediatePostDataCompanionScript(string script)
    //    {
    //        this.dropDownEdit.DropDown.SetClientSideEventHandler("GotFocus", script);
    //    }

    //    protected override void SetImmediatePostDataScript(string script)
    //    {
    //        this.dropDownEdit.DropDown.ClientSideEvents.SelectedIndexChanged = script;
    //        this.findEdit.TextBox.ClientSideEvents.TextChanged = script;
    //    }

    //    public override void Setup(IObjectSpace objectSpace, XafApplication application)
    //    {
    //        base.Setup(objectSpace, application);
    //        this.helper = new WebLookupEditorHelper(application, objectSpace, base.MemberInfo.MemberTypeInfo, base.Model);
    //    }

    //    public void SetValueToControl(object obj)
    //    {
    //        if (this.dropDownEdit != null)
    //        {
    //            ASPxComboBox dropDown = this.dropDownEdit.DropDown;
    //            foreach (ListEditItem item in dropDown.Items)
    //            {
    //                string str = item.Value as string;
    //                if (str == this.helper.GetObjectKey(obj))
    //                {
    //                    dropDown.SelectedIndex = item.Index;
    //                    break;
    //                }
    //            }
    //        }
    //    }

    //    private void UpdateControlId()
    //    {
    //        if (this.ActiveControl != null)
    //        {
    //            this.ActiveControl.ID = this.editorId;
    //        }
    //        if (this.InactiveControl != null)
    //        {
    //            this.InactiveControl.ID = this.editorId + "_inactive";
    //        }
    //    }

    //    private void UpdateControlsVisibility()
    //    {
    //        if ((this.dropDownEdit != null) || (this.findEdit != null))
    //        {
    //            bool flag = this.UseFindEdit();
    //            if (this.dropDownEdit != null)
    //            {
    //                this.dropDownEdit.Visible = !flag;
    //            }
    //            if (this.findEdit != null)
    //            {
    //                this.findEdit.Visible = flag;
    //            }
    //        }
    //        this.UpdateControlId();
    //        this.UpdateFindButtonScript(this.findEdit);
    //    }

    //    private void UpdateDropDownLookup(ASPxLookupDropDownEdit editor)
    //    {
    //        if (!this.UseFindEdit())
    //        {
    //            if (this.newObjectViewController != null)
    //            {
    //                editor.NewActionCaption = this.newObjectViewController.NewObjectAction.Caption;
    //            }
    //            this.UpdateDropDownLookupControlAddButton(editor);
    //            if (base.application != null)
    //            {
    //                editor.SetClientNewButtonScript(base.application.PopupWindowManager.GenerateModalOpeningScript(editor, this.newObjectWindowAction, WindowWidth, WindowHeight, false, editor.GetProcessNewObjFunction()));
    //            }
    //        }
    //    }

    //    private void UpdateDropDownLookupControlAddButton(ASPxLookupDropDownEdit control)
    //    {
    //        control.AddingEnabled = false;
    //        if (base.CurrentObject != null)
    //        {
    //            string diagnosticInfo = "";
    //            this.RecreateListView(true);
    //            control.AddingEnabled = (base.AllowEdit != null) && DataManipulationRight.CanCreate(this.listView, this.helper.LookupObjectType, this.listView.CollectionSource, out diagnosticInfo);
    //            if (control.AddingEnabled && (this.newObjectViewController != null))
    //            {
    //                control.AddingEnabled = (this.newObjectViewController.NewObjectAction.Active != null) && ((bool) this.newObjectViewController.NewObjectAction.Enabled);
    //            }
    //        }
    //    }

    //    private void UpdateFindButtonScript(ASPxLookupFindEdit findEdit)
    //    {
    //        if ((findEdit != null) && (base.application != null))
    //        {
    //            ICallbackManagerHolder currentRequestPage = WebWindow.CurrentRequestPage as ICallbackManagerHolder;
    //            string str = (base.Model.ImmediatePostData && (currentRequestPage != null)) ? currentRequestPage.CallbackManager.GetScript().Replace("'", "\\\\\"") : "";
    //            string callBackFuncName = "xafFindLookupProcessFindObject('" + findEdit.UniqueID + "', '" + findEdit.Hidden.ClientID + "', window.findLookupResult, '" + str + "');";
    //            findEdit.SetFindButtonClientScript(base.application.PopupWindowManager.GenerateModalOpeningScript(findEdit, this.showFindSelectWindowAction, WindowWidth, windowHeight, false, callBackFuncName));
    //        }
    //    }

    //    public virtual bool UseFindEdit()
    //    {
    //        bool flag = this.helper.IsSearchEditorMode();
    //        if (!flag)
    //        {
    //            flag = this.helper.CanFilterDataSource(this.DataSource, base.CurrentObject);
    //        }
    //        return flag;
    //    }

    //    // Properties
    //    private WebControl ActiveControl
    //    {
    //        get
    //        {
    //            if ((this.dropDownEdit != null) && this.dropDownEdit.Visible)
    //            {
    //                return this.dropDownEdit;
    //            }
    //            return this.findEdit;
    //        }
    //    }

    //    protected CollectionSourceBase DataSource
    //    {
    //        get
    //        {
    //            if (this.listView != null)
    //            {
    //                return this.listView.CollectionSource;
    //            }
    //            return null;
    //        }
    //    }

    //    IList<string> IDependentPropertyEditor.MasterProperties
    //    {
    //        get
    //        {
    //            return this.helper.MasterProperties;
    //        }
    //    }

    //    public ASPxLookupDropDownEdit DropDownEdit
    //    {
    //        get
    //        {
    //            return this.dropDownEdit;
    //        }
    //    }

    //    public ASPxLookupFindEdit FindEdit
    //    {
    //        get
    //        {
    //            return this.findEdit;
    //        }
    //    }

    //    public Frame Frame
    //    {
    //        get
    //        {
    //            this.InitializeFrame();
    //            return this.frame;
    //        }
    //    }

    //    internal LookupEditorHelper Helper
    //    {
    //        get
    //        {
    //            return this.helper;
    //        }
    //    }

    //    private WebControl InactiveControl
    //    {
    //        get
    //        {
    //            if ((this.dropDownEdit != null) && !this.dropDownEdit.Visible)
    //            {
    //                return this.dropDownEdit;
    //            }
    //            return this.findEdit;
    //        }
    //    }

    //    public WebLookupEditorHelper WebLookupEditorHelper
    //    {
    //        get
    //        {
    //            return this.helper;
    //        }
    //    }

    //    public static int WindowHeight
    //    {
    //        get
    //        {
    //            return windowHeight;
    //        }
    //        set
    //        {
    //            windowHeight = value;
    //        }
    //    }

    //    public static int WindowWidth
    //    {
    //        get
    //        {
    //            return windowWidth;
    //        }
    //        set
    //        {
    //            windowWidth = value;
    //        }
    //    }
    //}
}