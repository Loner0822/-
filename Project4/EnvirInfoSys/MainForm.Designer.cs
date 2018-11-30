namespace EnvirInfoSys
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.编辑ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.删除ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.添加指向位置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.删除指向位置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.修改指向位置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.contextMenuStrip3 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.服务器IP设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.边界线属性设置ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.管辖设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.button1 = new System.Windows.Forms.Button();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.数据备份ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.数据恢复ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.数据同步ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.mapHelper1 = new MapHelper.MapHelper();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.修改箭头样式ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.contextMenuStrip3.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.编辑ToolStripMenuItem,
            this.删除ToolStripMenuItem,
            this.添加指向位置ToolStripMenuItem,
            this.删除指向位置ToolStripMenuItem,
            this.修改指向位置ToolStripMenuItem,
            this.修改箭头样式ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(189, 194);
            // 
            // 编辑ToolStripMenuItem
            // 
            this.编辑ToolStripMenuItem.Name = "编辑ToolStripMenuItem";
            this.编辑ToolStripMenuItem.Size = new System.Drawing.Size(188, 28);
            this.编辑ToolStripMenuItem.Text = "属性编辑";
            this.编辑ToolStripMenuItem.Click += new System.EventHandler(this.编辑ToolStripMenuItem_Click);
            // 
            // 删除ToolStripMenuItem
            // 
            this.删除ToolStripMenuItem.Name = "删除ToolStripMenuItem";
            this.删除ToolStripMenuItem.Size = new System.Drawing.Size(188, 28);
            this.删除ToolStripMenuItem.Text = "删除当前标注";
            this.删除ToolStripMenuItem.Click += new System.EventHandler(this.删除ToolStripMenuItem_Click);
            // 
            // 添加指向位置ToolStripMenuItem
            // 
            this.添加指向位置ToolStripMenuItem.Name = "添加指向位置ToolStripMenuItem";
            this.添加指向位置ToolStripMenuItem.Size = new System.Drawing.Size(188, 28);
            this.添加指向位置ToolStripMenuItem.Text = "添加指向位置";
            this.添加指向位置ToolStripMenuItem.Click += new System.EventHandler(this.添加指向位置ToolStripMenuItem_Click);
            // 
            // 删除指向位置ToolStripMenuItem
            // 
            this.删除指向位置ToolStripMenuItem.Name = "删除指向位置ToolStripMenuItem";
            this.删除指向位置ToolStripMenuItem.Size = new System.Drawing.Size(188, 28);
            this.删除指向位置ToolStripMenuItem.Text = "删除指向位置";
            this.删除指向位置ToolStripMenuItem.Click += new System.EventHandler(this.删除指向位置ToolStripMenuItem_Click);
            // 
            // 修改指向位置ToolStripMenuItem
            // 
            this.修改指向位置ToolStripMenuItem.Name = "修改指向位置ToolStripMenuItem";
            this.修改指向位置ToolStripMenuItem.Size = new System.Drawing.Size(188, 28);
            this.修改指向位置ToolStripMenuItem.Text = "修改指向位置";
            this.修改指向位置ToolStripMenuItem.Click += new System.EventHandler(this.修改指向位置ToolStripMenuItem_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1210, 35);
            this.panel1.TabIndex = 1;
            // 
            // button3
            // 
            this.button3.Dock = System.Windows.Forms.DockStyle.Left;
            this.button3.FlatAppearance.BorderSize = 3;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button3.Location = new System.Drawing.Point(200, 0);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 35);
            this.button3.TabIndex = 7;
            this.button3.TabStop = false;
            this.button3.Text = "退出";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.ContextMenuStrip = this.contextMenuStrip3;
            this.button2.Dock = System.Windows.Forms.DockStyle.Left;
            this.button2.FlatAppearance.BorderSize = 3;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button2.Location = new System.Drawing.Point(100, 0);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(100, 35);
            this.button2.TabIndex = 6;
            this.button2.Text = "系统设置";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // contextMenuStrip3
            // 
            this.contextMenuStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.服务器IP设置ToolStripMenuItem,
            this.边界线属性设置ToolStripMenuItem1,
            this.管辖设置ToolStripMenuItem});
            this.contextMenuStrip3.Name = "contextMenuStrip3";
            this.contextMenuStrip3.Size = new System.Drawing.Size(207, 88);
            // 
            // 服务器IP设置ToolStripMenuItem
            // 
            this.服务器IP设置ToolStripMenuItem.Name = "服务器IP设置ToolStripMenuItem";
            this.服务器IP设置ToolStripMenuItem.Size = new System.Drawing.Size(206, 28);
            this.服务器IP设置ToolStripMenuItem.Text = "服务器IP设置";
            this.服务器IP设置ToolStripMenuItem.Click += new System.EventHandler(this.IP设置ToolStripMenuItem_Click);
            // 
            // 边界线属性设置ToolStripMenuItem1
            // 
            this.边界线属性设置ToolStripMenuItem1.Name = "边界线属性设置ToolStripMenuItem1";
            this.边界线属性设置ToolStripMenuItem1.Size = new System.Drawing.Size(206, 28);
            this.边界线属性设置ToolStripMenuItem1.Text = "边界线属性设置";
            this.边界线属性设置ToolStripMenuItem1.Click += new System.EventHandler(this.边界线属性设置ToolStripMenuItem_Click);
            // 
            // 管辖设置ToolStripMenuItem
            // 
            this.管辖设置ToolStripMenuItem.Name = "管辖设置ToolStripMenuItem";
            this.管辖设置ToolStripMenuItem.Size = new System.Drawing.Size(206, 28);
            this.管辖设置ToolStripMenuItem.Text = "管辖分类设置";
            this.管辖设置ToolStripMenuItem.Click += new System.EventHandler(this.管辖设置ToolStripMenuItem_Click);
            // 
            // button1
            // 
            this.button1.ContextMenuStrip = this.contextMenuStrip2;
            this.button1.Dock = System.Windows.Forms.DockStyle.Left;
            this.button1.FlatAppearance.BorderSize = 3;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Location = new System.Drawing.Point(0, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 35);
            this.button1.TabIndex = 5;
            this.button1.Text = "数据管理";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.数据备份ToolStripMenuItem1,
            this.数据恢复ToolStripMenuItem1,
            this.数据同步ToolStripMenuItem1});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(153, 88);
            // 
            // 数据备份ToolStripMenuItem1
            // 
            this.数据备份ToolStripMenuItem1.Name = "数据备份ToolStripMenuItem1";
            this.数据备份ToolStripMenuItem1.Size = new System.Drawing.Size(152, 28);
            this.数据备份ToolStripMenuItem1.Text = "数据备份";
            this.数据备份ToolStripMenuItem1.Click += new System.EventHandler(this.数据备份ToolStripMenuItem_Click);
            // 
            // 数据恢复ToolStripMenuItem1
            // 
            this.数据恢复ToolStripMenuItem1.Name = "数据恢复ToolStripMenuItem1";
            this.数据恢复ToolStripMenuItem1.Size = new System.Drawing.Size(152, 28);
            this.数据恢复ToolStripMenuItem1.Text = "数据恢复";
            this.数据恢复ToolStripMenuItem1.Click += new System.EventHandler(this.数据恢复ToolStripMenuItem_Click);
            // 
            // 数据同步ToolStripMenuItem1
            // 
            this.数据同步ToolStripMenuItem1.Name = "数据同步ToolStripMenuItem1";
            this.数据同步ToolStripMenuItem1.Size = new System.Drawing.Size(152, 28);
            this.数据同步ToolStripMenuItem1.Text = "数据同步";
            this.数据同步ToolStripMenuItem1.Click += new System.EventHandler(this.数据同步ToolStripMenuItem_Click);
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 35);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1210, 35);
            this.panel2.TabIndex = 2;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.groupBox3);
            this.panel3.Controls.Add(this.groupBox2);
            this.panel3.Controls.Add(this.groupBox1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 70);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1210, 599);
            this.panel3.TabIndex = 6;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.mapHelper1);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(277, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(777, 599);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "地图显示";
            // 
            // mapHelper1
            // 
            this.mapHelper1.BackColor = System.Drawing.Color.Black;
            this.mapHelper1.centerlat = 0D;
            this.mapHelper1.centerlng = 0D;
            this.mapHelper1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapHelper1.iconspath = null;
            this.mapHelper1.Location = new System.Drawing.Point(3, 24);
            this.mapHelper1.maparr = null;
            this.mapHelper1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.mapHelper1.Name = "mapHelper1";
            this.mapHelper1.roadmappath = null;
            this.mapHelper1.satellitemappath = null;
            this.mapHelper1.Size = new System.Drawing.Size(771, 572);
            this.mapHelper1.TabIndex = 0;
            this.mapHelper1.webpath = null;
            this.mapHelper1.AddMarkerFinished += new MapHelper.MapHelper.DlAddMarkerFinished(this.mapHelper1_AddMarkerFinished);
            this.mapHelper1.ModifyMarkerFinished += new MapHelper.MapHelper.DlModifyMarkerFinished(this.mapHelper1_ModifyMarkerFinished);
            this.mapHelper1.MarkerDragBegin += new MapHelper.MapHelper.DlMarkerDragBegin(this.mapHelper1_MarkerDragBegin);
            this.mapHelper1.MarkerDragEnd += new MapHelper.MapHelper.DlMarkerDragEnd(this.mapHelper1_MarkerDragEnd);
            this.mapHelper1.MapMouseup += new MapHelper.MapHelper.DlMapMouseup(this.mapHelper1_MapMouseup);
            this.mapHelper1.MapDblClick += new MapHelper.MapHelper.DlMapDblClick(this.mapHelper1_MapDblClick);
            this.mapHelper1.RemoveMarkerFinished += new MapHelper.MapHelper.DlRemoveMarkerFinished(this.mapHelper1_RemoveMarkerFinished);
            this.mapHelper1.MarkerRightClick += new MapHelper.MapHelper.DlMarkerRightClick(this.mapHelper1_MarkerRightClick);
            this.mapHelper1.IconSelected += new MapHelper.MapHelper.DlIconSelected(this.mapHelper1_IconSelected);
            this.mapHelper1.MapTypeChanged += new MapHelper.MapHelper.DlMapTypeChanged(this.mapHelper1_MapTypeChanged);
            this.mapHelper1.MapMouseWheel += new MapHelper.MapHelper.DlMouseWheel(this.mapHelper1_MapMouseWheel);
            this.mapHelper1.PointerDone += new MapHelper.MapHelper.DlPointerDone(this.mapHelper1_PointerDone);
            this.mapHelper1.MapMouseMove += new MapHelper.MapHelper.DlMapMouseMove(this.mapHelper1_MapMouseMove);
            this.mapHelper1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mapHelper1_MouseDown);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.groupBox4);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Right;
            this.groupBox2.Location = new System.Drawing.Point(1054, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(156, 599);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "地图设置";
            this.groupBox2.Visible = false;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.radioButton2);
            this.groupBox4.Controls.Add(this.radioButton1);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox4.Location = new System.Drawing.Point(3, 24);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(150, 76);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "放大缩小";
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Dock = System.Windows.Forms.DockStyle.Top;
            this.radioButton2.Location = new System.Drawing.Point(3, 46);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(144, 22);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "双击缩小";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Dock = System.Windows.Forms.DockStyle.Top;
            this.radioButton1.Location = new System.Drawing.Point(3, 24);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(144, 22);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "双击放大";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.treeView1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(277, 599);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "管理级别";
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(3, 24);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(271, 572);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // 修改箭头样式ToolStripMenuItem
            // 
            this.修改箭头样式ToolStripMenuItem.Name = "修改箭头样式ToolStripMenuItem";
            this.修改箭头样式ToolStripMenuItem.Size = new System.Drawing.Size(188, 28);
            this.修改箭头样式ToolStripMenuItem.Text = "修改箭头样式";
            this.修改箭头样式ToolStripMenuItem.Click += new System.EventHandler(this.修改箭头样式ToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1210, 669);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "环境信息化系统";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.contextMenuStrip3.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 删除ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 编辑ToolStripMenuItem;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripMenuItem 添加指向位置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 删除指向位置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 修改指向位置ToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.GroupBox groupBox3;
        private MapHelper.MapHelper mapHelper1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem 数据备份ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 数据恢复ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 数据同步ToolStripMenuItem1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip3;
        private System.Windows.Forms.ToolStripMenuItem 服务器IP设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 边界线属性设置ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 管辖设置ToolStripMenuItem;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolStripMenuItem 修改箭头样式ToolStripMenuItem;

    }
}

