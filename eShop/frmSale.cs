using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace eShop
{
    public partial class frmSale : Form
    {
        SqlConnection conn;
        SqlCommand com;
        SqlDataReader dr;
        DataTable dt;
        DataTable dtProduct;
        int delRow = -1;
        private DataTable GetAllProduct()
        {
            string sql = "SELECT * FROM Product";
            Connect();
            com = new SqlCommand(sql, conn);
            dr = com.ExecuteReader();
            dtProduct = new DataTable();
            if (dr.HasRows)
            {
                
                dtProduct.Load(dr);
                
            }
            else
            {
                dtProduct = null;
            }
            dr.Close();
            CloseConnect();
            return dtProduct;
        }
        public frmSale()
        {
            InitializeComponent();
        }
        private void Connect()
        {
            conn = new SqlConnection(Config.strConn);
            conn.Open();
        }
        private void CloseConnect()
        {
            conn.Close();
        }

        private string GenAutoSaleID()
        {
            string sql = "SELECT MAX(SaleID) As SaleID FROM Sale";
            string maxid="", prefix="INV-",dbYear="",nowYear="";
            int oldid=0, newid=0;

            Connect();
            com = new SqlCommand(sql, conn);
            dr = com.ExecuteReader();
            nowYear = (DateTime.Today.Year + 543).ToString().Substring(2, 2);
            if (dr.HasRows)
            {
                dt = new DataTable();
                dt.Load(dr);
                maxid = dt.Rows[0]["SaleID"].ToString();
               // prefix = maxid.Substring(0, 4);
                dbYear = maxid.Substring(4, 2);
                
                if (dbYear == nowYear)
                {
                    oldid = Convert.ToInt32(maxid.Substring(6, 5));
                    newid = oldid + 1;
                }
                else
                {
                    newid = 1;
                }
            }
            else
            {
                newid = 1;
            }
            dr.Close();
            CloseConnect();
            return   prefix + nowYear + newid.ToString ("00000");

        }

        private void frmSale_Load(object sender, EventArgs e)
        {
            txtSaleID.Text = GenAutoSaleID();

            cboProductID.DataSource = GetAllProduct();
            cboProductID.DisplayMember = "ProductID";
            cboProductID.ValueMember = "ProductID";


        }
        private double CalTotal(double price, int qty)
        {
            double total = price * qty;
            return total ;
        }
        private void cboProductID_SelectedIndexChanged(object sender, EventArgs e)
        {
            


        }

        private void cboProductID_SelectedValueChanged(object sender, EventArgs e)
        {
            
            string pid = "" ;
            pid = cboProductID.SelectedValue.ToString ();
            DataRow[] drp;
            if (pid != "System.Data.DataRowView")
            {
                drp = dtProduct.Select("ProductID=" + pid);
                txtProductName.Text = drp[0]["ProductName"].ToString();
                txtUnitPrice.Text = drp[0]["UnitPrice"].ToString();
                lblUnitInStock.Text = drp[0]["UnitInStock"].ToString();
                nudQty.Maximum = Convert.ToInt32(lblUnitInStock.Text);
                nudQty.Value = 1;
                nudQty_ValueChanged(sender, e);
            }

        }

        private void nudQty_ValueChanged(object sender, EventArgs e)
        {
            double price = Convert.ToDouble (txtUnitPrice .Text);
            int qty = Convert.ToInt32 ( nudQty.Value);
            txtTotal.Text = CalTotal(price, qty).ToString("0.00");
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            int newrow;
            newrow = dataGridView1.Rows.Add();
            dataGridView1.Rows[newrow].Cells[0].Value = cboProductID.SelectedValue;
            dataGridView1.Rows[newrow].Cells[1].Value = txtProductName.Text;
            dataGridView1.Rows[newrow].Cells[2].Value = txtUnitPrice.Text;
            dataGridView1.Rows[newrow].Cells[3].Value = nudQty.Value;
            dataGridView1.Rows[newrow].Cells[4].Value = txtTotal.Text;
            lblNetTotal.Text = CalNetTotal().ToString("0.00");

        }
        private double CalNetTotal()
        {
            double nettotal = 0;
            double cnt = dataGridView1.Rows.Count;
            
            for (int i = 0; i < cnt - 1; i++)
            {
              nettotal += Convert.ToDouble (dataGridView1.Rows[i].Cells[4].Value);
            }
            
            return nettotal;
        }

        private void btnRemoveRow_Click(object sender, EventArgs e)
        {
            if (delRow > -1 && delRow < dataGridView1.Rows.Count -1)
            {
                dataGridView1.Rows.RemoveAt(delRow);
                delRow = -1;
                lblNetTotal.Text = CalNetTotal().ToString("0.00");
            }
            else
            {
                MessageBox.Show("กรุณาเลือกแถวที่ต้องลบด้วย");
            }
       } 

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            delRow = e.RowIndex;
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            lblNetTotal.Text = CalNetTotal().ToString("0.00");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string sqlSale = "";
            sqlSale = "INSERT INTO Sale";
            sqlSale += "(SaleID,CustomerName,SaleDate,NetTotal,EmployeeName) ";
            sqlSale += "VALUES(@sid,@cn,@sd,@net,@en)";
            Connect();
            com = new SqlCommand(sqlSale, conn);
            com.Parameters.Clear();
            com.Parameters.AddWithValue("@sid", txtSaleID.Text);
            com.Parameters.AddWithValue("@cn", txtCustomerName.Text);
            com.Parameters.AddWithValue("@sd", dtpSaleDate.Value);
            com.Parameters.AddWithValue("@net", lblNetTotal.Text);
            com.Parameters.AddWithValue("@en", Config.EmplyeeName );
            com.ExecuteNonQuery();
            SaveSaleDetail();
            MessageBox.Show("บันทึกข้ัอมูลเรียบร้อยแล้ว");
            CloseConnect();
            //txtSaleID.Text = GenAutoSaleID();
            frmSale_Load(sender, e);
            ClearAll();

        }
        private void SaveSaleDetail()
        {
            string sql = "";
            string sid = txtSaleID.Text;
            string pid = "";
            int qty = 0;
            int newstock, currstock;
            double price = 0;
            double total = 0;
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                pid = dataGridView1.Rows[i].Cells[0].Value.ToString();
                price = Convert.ToDouble ( dataGridView1.Rows[i].Cells[2].Value);
                qty = Convert.ToInt32(dataGridView1.Rows[i].Cells[3].Value);
                
                total = Convert.ToDouble(dataGridView1.Rows[i].Cells[4].Value);
                sql = "INSERT INTO SaleDetail";
                sql += "(SaleID,ProductID,SalePrice,Quantity,Total) ";
                sql += "VALUES(@sid,@pid,@price,@qty,@total)";
                com = new SqlCommand(sql, conn);
                com.Parameters.Clear();
                com.Parameters.AddWithValue("@sid", sid);
                com.Parameters.AddWithValue("@pid", pid);
                com.Parameters.AddWithValue("@price", price);
                com.Parameters.AddWithValue("@qty", qty);
                com.Parameters.AddWithValue("@total", total);
                com.ExecuteNonQuery();
                currstock = GetCurrentStock(pid);
                newstock = currstock - qty;
                UpdateNewStock(pid, newstock);
 



            }

        }
        private void UpdateNewStock(string pid,int newstock)
        {
            string sql = "UPDATE Product SET UnitInStock=@newstock ";
            sql += "WHERE ProductID=@pid";
            com = new SqlCommand(sql, conn);
            com.Parameters.Clear();
            com.Parameters.AddWithValue("@newstock", newstock);
            com.Parameters.AddWithValue("@pid", pid);
            com.ExecuteNonQuery();

        }
        private int GetCurrentStock(string pid)
        {
            string sql = "SELECT UnitInStock FROM Product ";
            sql += "WHERE ProductID=@pid";
            com = new SqlCommand(sql, conn);
            com.Parameters.Clear();
            com.Parameters.AddWithValue("@pid", pid);
            dr = com.ExecuteReader();
            if (dr.HasRows)
            {
                dt = new DataTable();
                dt.Load(dr);
                dr.Close();
                return Convert.ToInt32(dt.Rows[0]["UnitInStock"]);
            }
            else
            {
                dr.Close();
                return -1;
            }
            
        }
        private void ClearAll()
        {
            txtCustomerName.Clear();
            cboProductID.SelectedIndex = 0;
            nudQty.Value = 1;
            btnRemoveAll_Click (btnRemoveAll,EventArgs.Empty);
        }
    }
}
