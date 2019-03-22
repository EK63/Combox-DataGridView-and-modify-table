//
// Picklist
// Background:
// 1. this program is just a demo
// 2. the routine here is to be used in a larger program
//
// Purpose:
// In the larger program there are common tables and the picklist is a way for the master user to modify the common tables.
// tblPickList contains  3 fields and is not editable;
// - cboPickListName        -   Combobox display
// - cboPickListDescription -   a description for the user which is displayed in text
// - cboPickListFileName    -   table name to be opened
// 
// Logic:
// 1. user scrolls through cbo and shose table to be modified
// 2. on clicking the relevant cbo name a table is opened in the DataGridView
// 3. using the bindingNavigator1 table can be modified, edit, delete and save
// 
//
// PROBLEM:
// Everything works, upto;
// 1. If i click SAVE, the routine does not accept the selected tble data
// 2. literature state that you should be able to save the modifications with the following routine:
//      TableAdapterName.Update(DataSetName.TableName);
//      >> i cant transfer the above info from the cbo & dgv to the save routine
//
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;

namespace ComboData
{
    public partial class frmPickList : Form
    {
        //Sql Connection declared
        SqlConnection connection; // required

        //Declare connection string
        String connectionString;    // required

        //Declare general
        DataTable tblpicklistTable = new DataTable();

        public frmPickList()
        {
            InitializeComponent();
            // connect to the data with this string
            connectionString = ConfigurationManager.ConnectionStrings["ComboData.Properties.Settings.PicklistConnectionString"].ConnectionString;  // the string is found in App.config
        }

        private void FrmPickList_Load(object sender, EventArgs e)
        {
             //Define Calls
            PopulatePickList();     // required - names the sequence 
        }

        private void PopulatePickList()     // required
        {
            // by using () Sql will close automatically or else you must add connection.Close();
            using (connection = new SqlConnection(connectionString))
            //using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM tblPickList ORDER BY cboPickListName", connection))
            using (SqlDataAdapter adapter = new SqlDataAdapter("Select Id,cboPickListName,cboPickListDescription,dbPickListFileName  FROM tblPickList", connection))
            {
                //DataTable tblpicklistTable = new DataTable();   //Moved to partial class to make it general
                adapter.Fill(tblpicklistTable);

                //Insert the Default Item to DataTable.
                DataRow row = tblpicklistTable.NewRow();
                row[0] = 0;
                row[1] = "Please select";
                tblpicklistTable.Rows.InsertAt(row, 0);

                // set list box properties
                cboPickList.DisplayMember = "cboPickListName";
                cboPickList.ValueMember = "cboPickListDescription";
 
                //Hook-up ComboBox to the data
                cboPickList.DataSource = tblpicklistTable;
            }
        }

        // to display the selection
        // this is where you will open the data table
        private void cboPicklist_Select(object sender, EventArgs e)
        {
            //Original
            lblPickListValue.Text = cboPickList.Text;
            lblPickListDescription.Text = cboPickList.SelectedValue.ToString();
            //lblPickListFileName.Text = tblpicklistTable.Rows[rowIndex]["dbPickListFileName"].ToString();

            //Opening of the DataGridView
            int rowIndex = cboPickList.SelectedIndex;
            if (rowIndex >= 1)
            {
                // Display tbl name to be opened
                lblPickListFileName.Text = tblpicklistTable.Rows[rowIndex]["dbPickListFileName"].ToString();

                //Open DataGridView
                using (connection = new SqlConnection(connectionString))
                using (SqlDataAdapter adapter = new SqlDataAdapter("Select * FROM " + tblpicklistTable.Rows[rowIndex]["dbPickListFileName"].ToString(), connection))
                {
                    DataTable dgvTableToUse = new DataTable();
                    adapter.Fill(dgvTableToUse);

                    //Populate the Table
                    //dgvPickListTable  = Data Grid View  
                    //dgvTableToUse     = Data table
                    dgvPickListTable.DataSource = dgvTableToUse;

                    //BindingNavigator
                    BindingSource bindingSource = new BindingSource(dgvTableToUse, null);
                    dgvPickListTable.DataSource = bindingSource;
                    bindingNavigator1.BindingSource = bindingSource;

                    MessageBox.Show("Table to use: " + tblpicklistTable.Rows[rowIndex]["dbPickListFileName"].ToString());
                }
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SaveToolStripButton_Click(object sender, EventArgs e)
        {
            // As per Forum
            //
            // Problem is the table name is not carried over
            //

            //MessageBox.Show("Table to use: " + tblpicklistTable.Rows[rowIndex]["dbPickListFileName"].ToString()); // to test if the info is transferred to the routine

            //SqlDataAdapter adapter = new SqlDataAdapter(
            //    "SELECT Id, GenderShort, GenderLong FROM " + "dbPickListFileName", connection);
            //SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            //builder.QuotePrefix = "[";
            //builder.QuoteSuffix = "]";

            //  TableAdapterName.Update(DataSetName.TableName);
                

            MessageBox.Show("Record was saved");
        }
    }
}
