using DevExpress.Data.Filtering;
using DevExpress.Emf;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Web.Internal.XmlProcessor;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Newtonsoft.Json;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace XafGetCaptions.Module.Web.Controllers
{
    public class ModelObjectData
    {
        public string TypeName { get; set; }
        public string Caption { get; set; }
        public string TableName { get; set; }
        public List<ModelPropertyData> Properties { get; set; } = new List<ModelPropertyData>();

    }
    public class ModelPropertyData
    {
        public ModelObjectData ModelObjectData { get; set; }
        public string ColumnName { get; set; }
        public string Caption { get; set; }
        public ModelPropertyData()
        {
            
        }
    }
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class CaptionController : ViewController
    {
        SimpleAction GetCaption;
        // Use CodeRush to create Controllers and Actions with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/403133/
        public CaptionController()
        {
            InitializeComponent();
            GetCaption = new SimpleAction(this, "GetCaptions", "View");
            GetCaption.Execute += GetCaption_Execute;

            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        private void GetCaption_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            List<ModelObjectData> modelObjects = new List<ModelObjectData>();

            List<IModelView> modelViews = this.View.Model.Application.Views.ToList();
            foreach (IModelView modelView in modelViews)
            {
                var Mdv = modelView as IModelDetailView;
                if (Mdv == null)
                    continue;

               

                ITypeInfo typeInfo = this.ObjectSpace.TypesInfo.FindTypeInfo(Mdv.ModelClass.TypeInfo.FullName);
                
                if (!typeInfo.IsPersistent)
                    continue;

                var Xos = this.ObjectSpace as XPObjectSpace;

                Debug.WriteLine(typeInfo.FullName);

              

                var XpoClassInfo = Xos.Session.GetClassInfo(typeInfo.Type);


                string tableName = XpoClassInfo.TableName;
             
                ModelObjectData modelObject = new ModelObjectData { TableName = tableName, Caption = Mdv.Caption, TypeName= typeInfo.Type.Name };
                modelObjects.Add(modelObject);
                Debug.WriteLine($"{Mdv.Caption},{tableName}");
                foreach (IModelViewItem modelViewItem in Mdv.Items)
                { 
                    var Property= modelViewItem as IModelPropertyEditor;
                    if(tableName == null)
                        continue;


                    modelObject.Properties.Add(new ModelPropertyData { ColumnName = Property.PropertyName, Caption = modelViewItem.Caption, ModelObjectData= modelObject });


                    Debug.WriteLine($"{tableName}.{Property.PropertyName},{modelViewItem.Caption }");
                   
                }
            }
           var Json=JsonConvert.SerializeObject(modelObjects, Formatting.Indented, new JsonSerializerSettings
           {
               PreserveReferencesHandling = PreserveReferencesHandling.Objects
           });    
            // Execute your business logic (https://docs.devexpress.com/eXpressAppFramework/112737/).
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
}
