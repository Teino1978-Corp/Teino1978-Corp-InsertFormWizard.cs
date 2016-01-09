using System;
using System.Text;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;

namespace Sitecore.Custom.WFFM.UI
{
    public class InsertFormWizard : Sitecore.Forms.Shell.UI.InsertFormWizard
    {
        protected Radiobutton ChooseExistingForm;

        protected override void Localize()
        {
            base.Localize();
            this.ChooseExistingForm.Header = "Select an existing form";
        }

        protected override bool ActivePageChanging(string page, ref string newpage)
        {
            if (newpage == "SelectPlaceholder" && page == "SelectForm" && this.ChooseExistingForm.Checked)
                newpage = string.IsNullOrEmpty(base.Placeholder) ? "SelectPlaceholder" : "ConfirmationPage";

            if (this.ChooseExistingForm.Checked && newpage == "AnalyticsPage")
                newpage = "ConfirmationPage";

            bool flag = base.ActivePageChanging(page, ref newpage);
            if (newpage == "ConfirmationPage" && this.ChooseExistingForm.Checked)
            {
                base.ChoicesLiteral.Text = RenderFormSelection();
            }
            return flag;
        }

        private string RenderFormSelection()
        {
            Item source = ExistingFormSelection();

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("<fieldset class='scfGroupSection' >");
            stringBuilder.Append("<legend>FORM</legend>");
            stringBuilder.Append("<p>Using existing form: " + source.Paths.FullPath + "</p>");
            return stringBuilder.ToString();
        }

        protected override void ActivePageChanged(string page, string oldPage)
        {
            base.ActivePageChanged(page, oldPage);
            if (page == "ConfirmationPage" && this.ChooseExistingForm.Checked)
            {
                this.NextButton.Header = "Confirm";
            }
        }

        protected override void OnNext(object sender, EventArgs formEventArgs)
        {
            if (this.NextButton.Header == "Confirm" && this.ChooseExistingForm.Checked)
            {
                this.SaveExistingFormSelection();
                SheerResponse.SetModified(false);
                this.Next();
                return;
            }
            base.OnNext(sender, formEventArgs);
        }

        private void SaveExistingFormSelection()
        {
            Item source = ExistingFormSelection();
            this.ServerProperties[this.newFormUri] = source.Uri.ToString();
            Registry.SetString("/Current_User/Dialogs//sitecore/shell/default.aspx?xmlcontrol=Forms.FormDesigner", "1250,500");
            SheerResponse.SetDialogValue(source.ID.ToString());
        }

        private Item ExistingFormSelection()
        {
            string queryString = Sitecore.Web.WebUtil.GetQueryString("la");
            Language result = Context.ContentLanguage;
            if (!string.IsNullOrEmpty(queryString))
                Language.TryParse(Sitecore.Web.WebUtil.GetQueryString("la"), out result);

            Item source = this.FormsRoot.Database.GetItem(this.multiTree.Selected, result);
            return source;
        }
    }
}