using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EncryptionToVB6;
using Newtonsoft.Json;

namespace Sgm.OC.Login
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }


        string UrlLocal
        {
            get
            {
                return string.Format(ConfigurationSettings.AppSettings["urlTemplate"].ToString(),
                    ConfigurationSettings.AppSettings["schema"].ToString(),
                    ConfigurationSettings.AppSettings["urlLocal"].ToString(),
                    ConfigurationSettings.AppSettings["portLocal"].ToString()
                    );
            }
        }

        string UrlVPN
        {
            get
            {
                return string.Format(ConfigurationSettings.AppSettings["urlTemplate"].ToString(),
                      ConfigurationSettings.AppSettings["schema"].ToString(),
                      ConfigurationSettings.AppSettings["urlVPN"].ToString(),
                      ConfigurationSettings.AppSettings["portVPN"].ToString()
                      );
            }
        }

        string RadioButtonMode
        {
            get
            {
                return ConfigurationSettings.AppSettings["radioButtonDefault"].ToString();
            }
        }

        private void btnGenerarToken_Click(object sender, EventArgs e)
        {
            try
            {
                GenerateToken();
            }
            catch
            {

            }
        }

        //token = "{'login':'" + GblUser + "','sucursal':" + Gbl_SucursalUsua + "}"

        private class UserInfo
        {
            public string Login { get; set; }
            public string Sucursal { get; set; }
        }

        private void GenerateToken()
        {

            if (rbtEncriptacionAes.Checked)
            {

                EncryptedPasswordManager encryptedPasswordManager = new EncryptedPasswordManager();
                string token = encryptedPasswordManager.Encryptation(txtUser.Text, int.Parse(txtSucursal.Text));

                txtToken.Text = token;

                txtUrlVPN.Text = string.Empty;
                txtUrlLocal.Text = string.Empty;

                txtUrlVPN.Text = UrlVPN + token;
                txtUrlLocal.Text = UrlLocal + token;
            }
            else if (rbtJsonInfo.Checked)
            {
                string token = JsonConvert.SerializeObject(new UserInfo()
                {
                    Login = txtUser.Text,
                    Sucursal = txtSucursal.Text
                });

                txtToken.Text = token;

                txtUrlVPN.Text = string.Empty;
                txtUrlLocal.Text = string.Empty;

                txtUrlVPN.Text = UrlVPN + token;
                txtUrlLocal.Text = UrlLocal + token;
            }

        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(txtToken.Text);
            }
            catch
            {
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //copy
            Clipboard.SetText(txtUrlVPN.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //open
            System.Diagnostics.Process.Start(txtUrlVPN.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //copy
            Clipboard.SetText(txtUrlLocal.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //open
            System.Diagnostics.Process.Start(txtUrlLocal.Text);
        }

        private void txtUrlVPN_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

            txtUrlVPN.Text = UrlVPN;
            txtUrlLocal.Text = UrlLocal;

            switch (RadioButtonMode)
            {
                default:
                case "aes": rbtEncriptacionAes.Checked = true; rbtJsonInfo.Checked = false; break;
                case "json": rbtEncriptacionAes.Checked = false; rbtJsonInfo.Checked = true; break;
            }
        }

    }
}
