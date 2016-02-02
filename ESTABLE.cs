using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ES_DataLayer;
using ES_FORMS;

namespace Main
{
    static class Program
    {
        /// <summary>
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
    partial class MainForm :Form
    {
        /// <summary>
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
	        this.Menu = new MainMenu();
			
            MenuItem mi_file =this.Menu.MenuItems.Add("F.��ƪ�");
			///�Τ�ۭq-�d�߸�ƪ��e
			mi_file.MenuItems.Add("��ƪ�", ShowTables);
			mi_file.MenuItems.Add("��ƪ�RW+", ShowTablesUpdate);
			///End �Τ�ۭq
			MenuItem msys = this.Menu.MenuItems.Add("W.���f");
		        msys.MenuItems.Add(new MenuItem("�ƦC�ϼ�", this.mnuIcons_click));
                        msys.MenuItems.Add(new MenuItem("�h�h�|�|", this.mnuCascade_click));
                        msys.MenuItems.Add(new MenuItem("�����Q��", this.mnuTileHorizontal_click));
                        msys.MenuItems.Add(new MenuItem("�����Q��", this.mnuTileVertical_click));
                        msys.MenuItems.Add(new MenuItem("�����Ҧ��l���f", this.CloseAllSubForm_click));
			//ESData.SetDB(3,"eschool");
		}
		///�\��y�z
		private void sql(object sender, EventArgs e)
		{
			Inputsql inp=new Inputsql();
			inp.MdiParent=this;
			inp.Show();
		}
		private void ShowTables(object sender,EventArgs e)
		{
			Pub.midform=this;
			esreport.ShowTable("Show Tables;");
		}
		private void ShowTablesUpdate(object sender,EventArgs e)
		{
			Pub.midform=this;
			ES_FORMS.ListBoxFilterForm lbf=new ES_FORMS.ListBoxFilterForm();
			System.Data.Common.DbDataReader dr=ESData.GetInst.Reader_ShowTables();
			while(dr.Read())
			{
				lbf.lb.Items.Add(dr[0].ToString());
			}
			dr.Close();
			dr.Dispose();
			if(lbf.ShowDialog()==DialogResult.OK)
			{
				//esreport.ShowTableWithUpdate("Select * from "+lbf.lb.SelectedItem.ToString());
				esreport.ShowTableWithImportxls("Select * from "+lbf.lb.SelectedItem.ToString());
			}
			
		}
		///End �\��y�z		
				
    }
	class Inputsql:Form
	{
		Button btn=new Button();
		RichTextBox tb=new RichTextBox();
		public Inputsql()
		{
 			 btn.Dock = DockStyle.Bottom;
            tb.Dock = DockStyle.Fill;
	    btn.Text="Query";
  //          Rename_btn.Click += ren_click;
            Controls.Add(tb);
            Controls.Add(btn);

			btn.Click+=click;
		}
		private void click(object sender, EventArgs e)
		{
			esreport.ShowTableWithImportxls(tb.Text);
		}
	}
}
