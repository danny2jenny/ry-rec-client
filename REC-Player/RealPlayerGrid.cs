using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ry.rec
{
    /// <summary>
    /// 实时播放的Grid 
    /// </summary>
    public class RealPlayerGrid : TableLayoutPanel
    {
        public RealPlayerGrid(PlayerManager mgr, int row, int column)
        {
            this.RowCount = row;
            this.ColumnCount = column;

            // 设置行属性
            float rowHeight = 100 / row;
            float columnWidth = 100 / column;
            for (int i = 0; i < row; i++)
            {
                RowStyles.Add(new RowStyle(SizeType.Percent, rowHeight));
            }
            for (int i = 0; i < column; i++)
            {
                ColumnStyles.Add(new ColumnStyle(SizeType.Percent, columnWidth));
            }

            // 添加播放器
            for (int r = 0; r < RowCount; r++)
            {
                for (int c = 0; c < ColumnCount; c++)
                {
                    FormRealPlayer player = new FormRealPlayer(mgr);
                    player.Dock = DockStyle.Fill;
                    player.FormBorderStyle = FormBorderStyle.None;
                    player.TopLevel = false;
                    this.Controls.Add(player, c, r);
                    player.setGrid(this, c, r);
                    player.Show();
                }
            }
        }

        // 停止所有的播放
        public void stopAll()
        {
            foreach (FormRealPlayer player in this.Controls)
            {
                player.stop();
            }
        }

        // 所有的Form不选择
        public void unSelectAll()
        {
            foreach (FormRealPlayer player in this.Controls)
            {
                player.formSelect(false);
            }
        }

        // 得到一个播放器
        public FormRealPlayer getPlayer()
        {
            // 如果有空余的播放器
            foreach (FormRealPlayer pl in this.Controls)
            {
                if (!pl.isPlaying)
                {
                    return pl;
                }
            }

            // 如果有选中的播放器
            foreach (FormRealPlayer pl in this.Controls)
            {
                if (pl.isSelected)
                {
                    return pl;
                }
            }

            // 直接得到第一个播放器
            if (this.Controls.Count > 0)
            {
                FormRealPlayer player = (FormRealPlayer)this.GetControlFromPosition(0, 0);
                return player;
            }

            return null;

        }
    }
}
