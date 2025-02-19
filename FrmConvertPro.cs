using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using FFMpegCore;
using NReco.VideoConverter;
//using NReco.VideoInfo;

namespace Converter
{
    public partial class FrmConvertPro : KryptonForm
    {
        public FrmConvertPro()
        {
            InitializeComponent();
        }
        private void AbrirFormEnPanel(Form form)
        {
            // Fecha qualquer formulário já aberto dentro do painel
            if (this.pnlContainer.Controls.Count > 0)
            {
                Control controleAtual = this.pnlContainer.Controls[0];

                if (controleAtual is Form formAberto) // Verifica se é um Form antes de converter
                {
                    formAberto.Close(); // Fecha o formulário atual antes de abrir outro
                }
            }

            pnlContainer.Controls.Clear(); // Remove qualquer controle que ainda esteja no painel

            form.TopLevel = false;
            form.Dock = DockStyle.Fill;
            form.FormClosed += Form_FormClosed; // Garante que ao fechar, verificamos se ainda há formulários abertos
            this.pnlContainer.Controls.Add(form);
            this.pnlContainer.Tag = form;
            form.Show();
        }
        private void Form_FormClosed(object sender, FormClosedEventArgs e)
        {
        }
        // Atualiza a lblJanelaAberta quando um formulário é fechado
       
        private void btnFechar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnConvertMp_Click(object sender, EventArgs e)
        {
            AbrirFormEnPanel(new FrmConverterMpQuatro());            
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            AbrirFormEnPanel(new FrmDowload());           
        }

        private void btnConverterMpTreis_Click(object sender, EventArgs e)
        {
            AbrirFormEnPanel(new FrmCompressMpTreis());           
        }
    }
}
