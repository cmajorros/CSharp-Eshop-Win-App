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
    public partial class frmLogin : Form
    {
        SqlConnection conn;
        SqlCommand com;
        SqlDataReader dr;
        DataTable dt;

        public frmLogin()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            bool pass;
            string usr;
            string pwd;
            usr = txtUserName.Text;
            pwd = txtPassword.Text;
            pass = Login(usr, pwd);
            if (pass == true)
            {
                this.Hide();
                frmMain fm = new frmMain();
                fm.Show();
            }
            else
            {
                MessageBox.Show("Invalid Username and Pasword");
                txtUserName.Focus();
                txtUserName.Clear();
                txtPassword.Clear();
            }
        }
        private bool Login(String usr, String pwd)
        {
            string sql = "SELECT EmployeeName FROM Employee ";
            sql += "WHERE UserName=@usr AND Password=@pwd";
            conn = new SqlConnection(Config.strConn);
            conn.Open();
            if (conn.State == ConnectionState.Open)
            {
                com = new SqlCommand(sql, conn);
                com.Parameters.Clear();
                com.Parameters.AddWithValue("@usr", usr);
                com.Parameters.AddWithValue("@pwd", pwd);
                dr = com.ExecuteReader();
                if (dr.HasRows)
                {
                    dt = new DataTable();
                    dt.Load(dr);
                    Config.EmplyeeName = dt.Rows[0]["EmployeeName"].ToString();
                    dr.Close();
                    conn.Close();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLogin_MouseHover(object sender, EventArgs e)
        {
            btnLogin.BackgroundImageLayout = ImageLayout.Zoom;
        }

        private void btnLogin_MouseLeave(object sender, EventArgs e)
        {
            btnLogin.BackgroundImageLayout = ImageLayout.Center;
        }
    }
}
