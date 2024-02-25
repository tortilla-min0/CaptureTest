using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace CaptureTest.CaptureTarget
{
    public class CaptureTargetForm : Form
    {
        public bool ForceInvisible{get;set;}
        public CaptureTargetForm( bool forceInvisible = false )
        {
            this.ClientSize = new Size( 1280, 720 );
            // this.Height = 720;
            this.FormBorderStyle = FormBorderStyle.None;

            this.ForceInvisible = forceInvisible;

            InitControl();
        }

        public void InitControl()
        {
            Controls.Clear();
            var pnl = new Panel();
            pnl.Width = this.Width;
            pnl.Height = this.Height;
            pnl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            pnl.AutoScroll = true;
            pnl.Dock = DockStyle.Fill;
            const int width = 20;
            const int height = 30;
            const int margin_x = 2;
            const int margin_y = 2;
            var n = 0;
            var visible = true;
            for(var y = 0; (y + height) < 720 ; y+=(height + margin_y))
            {
                var pnl2 = new Panel();
                pnl2.Size = new Size(1280,height);
                pnl2.Location = new Point(0,y);
                for(var x = 0; (x + width) < 1280 ;x+=(width + margin_x) )
                {
                    n++;
                    var content = new TextBox();
                    content.Width = width;
                    content.Height = height;
                    content.Location = new Point(x,0);
                    content.Text = string.Format("{0} : {1},{2}",n,x,y);
                    pnl2.Controls.Add(content);
                }
                pnl2.Visible = (visible=!visible);
                pnl.Controls.Add(pnl2);
            }
            this.Controls.Add(pnl);
        }

        override protected void SetVisibleCore(bool value)
        {
            if( ForceInvisible )
            {
                value = false;
                if (!this.IsHandleCreated) {
                    CreateHandle();
                }
            }
            base.SetVisibleCore(value);
        }

        async public Task<Bitmap> CaptureAsync( bool aVisible, CreateType aType = CreateType.CreateControl )
        {
            if( aVisible )
            {
                Visible = true;
            }
            else
            {
                if( aType == CreateType.CreateControl )
                {
                    this.CreateControlsan();
                }
                else if( aType == CreateType.CreateHandle )
                {
                    this.GetAncestors().ToList().ForEach(c => {c.CreateHandlesan();});
                }
            }

            PaintEventHandler handler = null;
            Bitmap ret = null;

            ret = new Bitmap( Width, Height );
            this.DrawToBitmap(ret,this.ClientRectangle);

            return ret;
        }

    }

    public static class ControlRecursive
    {
        public static IEnumerable<Control> GetAncestors(this Control c )
        {
            yield return c;
            foreach( Control ch in c.Controls )
            {
                foreach( var g in ch.GetAncestors() )
                {
                    yield return g;
                }
            }
        }

        static MethodInfo s_methodInfoCreateHandle;
        public static void CreateHandlesan( this Control c )
        {
            if( c.IsHandleCreated )
            {
                return;
            }
            if( s_methodInfoCreateHandle == null )
            {
                s_methodInfoCreateHandle = typeof(Control).GetMethod("CreateHandle", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance );
            }
            s_methodInfoCreateHandle.Invoke(c,null);
            return;
        }

        static MethodInfo s_methodInfoCreateControl;
        public static void CreateControlsan( this Control c )
        {
            // 自身のHandleが作成されていても、新規に子コントロールを追加した場合には、
            // CreateControlを掛ける必要がある模様
            //
            // if( c.IsHandleCreated )
            // {
            //     return;
            // }
            if( s_methodInfoCreateControl == null )
            {
                s_methodInfoCreateControl = typeof(Control).GetMethod( "CreateControl", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance );
            }
            s_methodInfoCreateControl.Invoke(c,new object[]{true});
            return;
        }

    }
    public enum CreateType
    {
        CreateHandle,
        CreateControl,
    }
}
