using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
namespace eShop
{
    public partial class frmProduct : Form
    {
        int selIndex = -1;
        SqlConnection con;
        SqlCommand com;
        SqlDataReader dr;
        DataTable dt; 
        public frmProduct()
        {
            InitializeComponent();
        }
        private void Connect()
        {
            con = new SqlConnection(Config.strConn);
            con.Open();

        }
        private void CloseConnect()
        {
            con.Close();
        }
        private void QueryProductByID(int proID)
        {
            string sql = "SELECT * FROM Product WHERE ProductID=@proID";
            Connect();
            com = new SqlCommand(sql, con);
            com.Parameters.Clear();
            com.Parameters.AddWithValue("@proID", proID);
            dr = com.ExecuteReader();
            if (dr.HasRows)
            {
                dt = new DataTable();
                dt.Load(dr);
                dgvProList.DataSource = dt;
            }
            else
            {
                dgvProList.DataSource = null;
            }
            dr.Close();
            CloseConnect();

        }
        private void Search(string proName)
        {
            string sql = "SELECT * FROM Product WHERE ProductName LIKE @proName";
            Connect();
            com = new SqlCommand(sql, con);
            com.Parameters.Clear();
            com.Parameters.Add("@proName", SqlDbType.NVarChar).Value = proName + "%";
            dr = com.ExecuteReader();
            if (dr.HasRows)
            {
                dt = new DataTable();
                dt.Load(dr);
                lstSearch.DataSource = dt;
                lstSearch.DisplayMember = "ProductName";
                lstSearch.ValueMember = "ProductID";
                lstSearch.Visible = true;
            }
            else
            {
                lstSearch.Visible = false;
            }
            dr.Close();
            CloseConnect();

        }
        private void Delete(int proID)
        {
            string sql = "DELETE FROM Product WHERE ProductID=@proID";
            Connect();
            com = new SqlCommand(sql, con);
            com.Parameters.Clear();
            com.Parameters.AddWithValue("@proID", proID);
            com.ExecuteNonQuery();
            MessageBox.Show("ลบข้อมูลเรียบร้อยแล้ว");
            dgvProList.DataSource = null;


        }
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            Search(txtSearch.Text);
        }

        private void lstSearch_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lstSearch_Click(object sender, EventArgs e)
        {
            QueryProductByID( Convert.ToInt32(lstSearch.SelectedValue));
            lstSearch.Visible = false;
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            int proID = 0;

            if (selIndex > -1 && selIndex < dgvProList.Rows.Count - 1)
            {
                proID = Convert.ToInt32(dgvProList.Rows[selIndex].Cells[0].Value);
                if (MessageBox.Show("ต้องการลบสินค้ารหัส " + proID + " นี้หรือไม่", "ลบ",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Delete (proID);
                }
            }
        }

        private void dgvProList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selIndex = e.RowIndex;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (selIndex > -1 && selIndex < dgvProList.Rows.Count - 1)
            {
                groupBox3.Visible = true;
                txtProductID.Text = dgvProList.Rows[selIndex].Cells[0].Value.ToString();
                txtProductName.Text = dgvProList.Rows[selIndex].Cells[1].Value.ToString();
                txtPrice.Text = dgvProList.Rows[selIndex].Cells[2].Value.ToString();
                txtStock.Text =  dgvProList.Rows[selIndex].Cells[3].Value.ToString();
                if (dgvProList.Rows[selIndex].Cells[4].Value == DBNull.Value)
                {
                    picProduct.Image = picProduct.ErrorImage; 
                }
                else
                {
                    byte[] pic = { };
                    pic = (byte[])dgvProList.Rows[selIndex].Cells[4].Value;
                    MemoryStream ms = new MemoryStream(pic);
                    picProduct.Image = Image.FromStream(ms);

                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtProductID.Text != "")
            {
                Update();

            }
            else
            { 
            //Insert();
            }
        }

        private void lblBrowse_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "Image File (*.jpg;*.png;*.gif)|*.jpg;*.png;*.gif";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                picProduct.Image = Image.FromFile(openFileDialog1.FileName);
            }
        }
        private byte[] ConvertBinaryToByte(string fileName)
        {
            byte[] pic = {};
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            pic = br.ReadBytes((int)fs.Length);
            return pic;

        }
        private void Update()
        {
            string sql = "UPDATE Product SET ";
            sql += "ProductName =@pname, ";
            sql += "UnitPrice =@price, ";
            sql += "UnitInStock =@stock ";
            if (openFileDialog1.FileName != "")
            {
                sql += ",ProductImage =@pic ";
            }
                sql += "WHERE ProductID =@pid";
                byte[] pic = { };
                Connect();
                com = new SqlCommand(sql, con);
                com.Parameters.Clear();
                com.Parameters.AddWithValue("@pname", txtProductName.Text);
                com.Parameters.AddWithValue("@price", txtPrice.Text);
                com.Parameters.AddWithValue("@stock", txtStock.Text);
                if (openFileDialog1.FileName != "")
                {
                    pic = ConvertBinaryToByte(openFileDialog1.FileName);
                    com.Parameters.AddWithValue("@pic", pic);
                }
                com.Parameters.AddWithValue("@pid", txtProductID.Text);
                com.ExecuteNonQuery();
                MessageBox.Show("บันทึกข้อมูลเรียบร้อยแล้ว");
                CloseConnect();
                groupBox3.Visible = false;
                groupBox2.Visible = true; 

        }
    }
}
