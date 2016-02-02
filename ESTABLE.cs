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
			
            MenuItem mi_file =this.Menu.MenuItems.Add("F.資料表");
			///用戶自訂-查詢資料表內容
			mi_file.MenuItems.Add("資料表", ShowTables);
			mi_file.MenuItems.Add("資料表RW+", ShowTablesUpdate);
			///End 用戶自訂
			MenuItem msys = this.Menu.MenuItems.Add("W.窗口");
		        msys.MenuItems.Add(new MenuItem("排列圖標", this.mnuIcons_click));
                        msys.MenuItems.Add(new MenuItem("層層疊疊", this.mnuCascade_click));
                        msys.MenuItems.Add(new MenuItem("水平鋪平", this.mnuTileHorizontal_click));
                        msys.MenuItems.Add(new MenuItem("垂直鋪平", this.mnuTileVertical_click));
                        msys.MenuItems.Add(new MenuItem("關閉所有子窗口", this.CloseAllSubForm_click));
			//ESData.SetDB(3,"eschool");
		}
		///功能描述
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
		///End 功能描述		
				
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
