using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using NReco.VideoConverter;

namespace Converter
{
    public partial class FrmConvertPro : KryptonForm
    {
        public FrmConvertPro()
        {
            InitializeComponent();
            
        }
        private void AbrirFormEnPanel(object Form)
        {
            if (this.panelConteiner.Controls.Count > 0)
                this.panelConteiner.Controls.RemoveAt(0);
            Form fh = Form as Form;
            fh.TopLevel = false;
            fh.Dock = DockStyle.Fill;
            this.panelConteiner.Controls.Add(fh);
            this.panelConteiner.Tag = fh;
            fh.Show();
        }
        private void BtnDownload_Click(object sender, EventArgs e)
        {
            FrmDowload frmDowload = new FrmDowload();
            AbrirFormEnPanel(frmDowload);
        }

        private void btnConverterMp_Click(object sender, EventArgs e)
        {
            FrmConverter frmConverterMp = new FrmConverter();
            AbrirFormEnPanel(frmConverterMp);
        }
    }
}
