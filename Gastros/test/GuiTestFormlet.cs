using System;
using System.Drawing;
using System.Windows.Forms;
using GastrOs.Sde.Support;
using GastrOs.Sde.ViewControls;

namespace GastrOs.Sde.Test
{
    class GuiTestFormlet : Form
    {
        private string defaultKnowl = ".";
        private string defaultArch = "openEHR-EHR-COMPOSITION.testcomp1.v1";
        private string defaultOpt = "TestTemplate2.opt";
        private string defaultLanguage = "en-NZ";

        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private TextBox knowl;
        private TextBox opt;
        private TextBox arch;
        private TextBox lang;
        private Button generate;

        public GuiTestFormlet()
        {
            InitializeComponent();
            knowl.Text = defaultKnowl;
            arch.Text = defaultArch;
            opt.Text = defaultOpt;
            lang.Text = defaultLanguage;
            generate.Click += HandleGenerate;
        }

        private void HandleGenerate(object sender, EventArgs e)
        {
            try
            {
                EhrSerialiser.KnowledgePath = knowl.Text;
                GastrOsService.OperationalTemplateName = opt.Text;
                GastrOsService.Language = lang.Text;
                ViewControl widget = GastrOsService.GenerateView(arch.Text);
                Control viewAsControl = widget.View as Control;

                Form f = new Form();
                f.Size = new Size(800, 700);
                f.AutoScrollMinSize = viewAsControl.Size;
                f.Controls.Add(viewAsControl);
                f.ShowDialog(this);
                f.Dispose();

                //LockEditForm f = new LockEditForm(new Button(), true, ButtonsConfig.EditSave);
                //Form f = new Form();
                //f.Show(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.knowl = new System.Windows.Forms.TextBox();
            this.opt = new System.Windows.Forms.TextBox();
            this.arch = new System.Windows.Forms.TextBox();
            this.lang = new System.Windows.Forms.TextBox();
            this.generate = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Knowledge path";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Operational template";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Archetype id";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 89);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Language";
            // 
            // knowl
            // 
            this.knowl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.knowl.Location = new System.Drawing.Point(127, 6);
            this.knowl.Name = "knowl";
            this.knowl.Size = new System.Drawing.Size(193, 20);
            this.knowl.TabIndex = 2;
            // 
            // opt
            // 
            this.opt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.opt.Location = new System.Drawing.Point(127, 32);
            this.opt.Name = "opt";
            this.opt.Size = new System.Drawing.Size(193, 20);
            this.opt.TabIndex = 2;
            // 
            // arch
            // 
            this.arch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.arch.Location = new System.Drawing.Point(127, 58);
            this.arch.Name = "arch";
            this.arch.Size = new System.Drawing.Size(193, 20);
            this.arch.TabIndex = 2;
            // 
            // lang
            // 
            this.lang.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lang.Location = new System.Drawing.Point(127, 84);
            this.lang.Name = "lang";
            this.lang.Size = new System.Drawing.Size(193, 20);
            this.lang.TabIndex = 2;
            // 
            // generate
            // 
            this.generate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.generate.Location = new System.Drawing.Point(245, 122);
            this.generate.Name = "generate";
            this.generate.Size = new System.Drawing.Size(75, 23);
            this.generate.TabIndex = 3;
            this.generate.Text = "Generate";
            this.generate.UseVisualStyleBackColor = true;
            // 
            // GuiTestFormlet
            // 
            this.ClientSize = new System.Drawing.Size(332, 157);
            this.Controls.Add(this.generate);
            this.Controls.Add(this.lang);
            this.Controls.Add(this.arch);
            this.Controls.Add(this.opt);
            this.Controls.Add(this.knowl);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "GuiTestFormlet";
            this.Text = "GUI generation test";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
