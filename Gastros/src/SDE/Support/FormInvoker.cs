using System;
using System.Windows.Forms;
using GastrOs.Sde.Directives;
using GastrOs.Sde.ViewControls;
using GastrOs.Sde.Views;
using OceanEhr.OpenEhrV1.AM.Archetype.ConstraintModel;

namespace GastrOs.Sde.Support
{
    /// <summary>
    /// Utility class for invoking an auto-scrollable containing form for showing and
    /// editing a generated SDE view object.
    /// </summary>
    public static class FormInvoker
    {
        public static LockEditForm InvokeLockEditForm(ViewControl control, bool editing, ButtonsConfig buttons)
        {
            IView view = control.View;
            if (!(view is Control))
            {
                throw new ArgumentException("Specified view must be a Windows Forms component");
            }
            LockEditForm form = new LockEditForm(view as Control, view.RequiresExternalScrolling, buttons);
            form.Text = string.IsNullOrEmpty(view.Title) ? control.Constraint.ExtractOntologyText() : view.Title;
            form.Editing = editing;
            
            //Apply the formAspects directive
            FormAspectsDirective formAspects = view.Directives.GetDirectiveOfType<FormAspectsDirective>();
            if (formAspects == null)
            {
                //take 2: try and salvage any formAspects directive up the archetype hierarchy
                for (CObject parent = control.Constraint.GetParent();
                    parent != null && formAspects == null;
                    parent = parent.GetParent())
                {
                    formAspects = GastrOsService.OperationalTemplate.FindDirectiveOfType<FormAspectsDirective>(parent);
                }
            }
            if (formAspects != null)
            {
                if (formAspects.Width > 0)
                    form.Width = formAspects.Width;
                if (formAspects.Height > 0)
                    form.Height = formAspects.Height;
            }

            //form.Show();
            return form;
        }

        public static LockEditForm InvokeLockEditForm(Control widget, bool editing, string title, bool autoScroll, ButtonsConfig buttons)
        {
            LockEditForm form = new LockEditForm(widget, autoScroll, buttons);
            form.Text = title;
            form.Editing = editing;
            //form.Show();
            return form;
        }
    }
}