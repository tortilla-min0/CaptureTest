using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CaptureTest.CaptureTarget;

namespace CaptureTest
{
    public partial class Form1 : Form
    {
        List<Button> buttons = null;
        public Form1()
        {
            InitializeComponent();
            buttons = new List<Button>(new[]{ button1,button2,button3,button4,button5 });
        }

        CaptureTargetForm captureForm = null;
        Button lastButton = null;
        async void BtnClicked(object sender,EventArgs e)
        {
            BtnClicked( sender, e, true );
        }
        async void BtnClicked(object sender,EventArgs e, bool aInstanceCreate = true)
        {
            try{
                buttons.ForEach(btn=>{ btn.Enabled = false;});

                if( sender.Equals(button1) )
                {
                    if(aInstanceCreate){captureForm = new CaptureTargetForm(false);}
                    else{captureForm.InitControl();}
                    captureForm.Opacity = 0;
                    pictureBox1.Image = await captureForm.CaptureAsync(aVisible:true);
                }
                else if( sender.Equals(button2) )
                {
                    if(aInstanceCreate){captureForm = new CaptureTargetForm(true);}
                    else{captureForm.InitControl();}
                    pictureBox1.Image = await captureForm.CaptureAsync(aVisible:false,CreateType.CreateControl);
                }
                else if( sender.Equals(button3) )
                {
                    if(aInstanceCreate){captureForm = new CaptureTargetForm(true);}
                    else{captureForm.InitControl();}
                    pictureBox1.Image = await captureForm.CaptureAsync(aVisible:false,CreateType.CreateHandle);
                }
                else if( sender.Equals(button4) )
                {
                    pictureBox1.Image = null;
                    return;
                }
                else if( sender.Equals(button5) )
                {
                    if( lastButton == null ){ return; }
                    if( (captureForm!= null) && (captureForm.IsDisposed) ){ return; }
                    BtnClicked(lastButton,e,false);
                    return;
                }
                // captureForm.Show();
                if( sender.Equals(button2) || sender.Equals(button3) )
                {
                    captureForm.GetAncestors().ToList().ForEach(cfc=>{Console.WriteLine(String.Format("{0} : {1}" , cfc.Text,cfc.IsHandleCreated));});
                }
            }
            finally
            {
                if( !sender.Equals(button5) && !sender.Equals(button4) )
                {
                    lastButton = (Button)sender;
                    button5.Text = lastButton.Text + Environment.NewLine + "on Last Instance";
                }
                buttons.ForEach(btn=>{ btn.Enabled = true;});
                button5.Enabled = (lastButton != null);
            }
        }
    }
}
