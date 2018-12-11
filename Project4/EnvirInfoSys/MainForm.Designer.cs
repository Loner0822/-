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
            this.修改箭头样式ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel3 = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.mapHelper1 = new MapHelper.MapHelper();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.数据管理ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.数据备份ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.数据恢复ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.数据同步ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.下载单位注册数据ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.系统设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.服务器IP设置ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.边界线属性设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.管辖分类设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.图符对应设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.图符管理设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.密码管理ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.退出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.设置当前点为中心点ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip3 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.导入当前单位边界线ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.contextMenuStrip1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.contextMenuStrip3.SuspendLayout();
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
            this.contextMenuStrip1.Size = new System.Drawing.Size(189, 172);
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
            // 修改箭头样式ToolStripMenuItem
            // 
            this.修改箭头样式ToolStripMenuItem.Name = "修改箭头样式ToolStripMenuItem";
            this.修改箭头样式ToolStripMenuItem.Size = new System.Drawing.Size(188, 28);
            this.修改箭头样式ToolStripMenuItem.Text = "修改箭头样式";
            this.修改箭头样式ToolStripMenuItem.Click += new System.EventHandler(this.修改箭头样式ToolStripMenuItem_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.groupBox3);
            this.panel3.Controls.Add(this.groupBox2);
            this.panel3.Controls.Add(this.groupBox1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 32);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1210, 637);
            this.panel3.TabIndex = 6;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.mapHelper1);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(277, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(777, 637);
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
            this.mapHelper1.Size = new System.Drawing.Size(771, 610);
            this.mapHelper1.TabIndex = 0;
            this.mapHelper1.webpath = null;
            this.mapHelper1.AddMarkerFinished += new MapHelper.MapHelper.DlAddMarkerFinished(this.mapHelper1_AddMarkerFinished);
            this.mapHelper1.ModifyMarkerFinished += new MapHelper.MapHelper.DlModifyMarkerFinished(this.mapHelper1_ModifyMarkerFinished);
            this.mapHelper1.MarkerDragBegin += new MapHelper.MapHelper.DlMarkerDragBegin(this.mapHelper1_MarkerDragBegin);
            this.mapHelper1.MarkerDragEnd += new MapHelper.MapHelper.DlMarkerDragEnd(this.mapHelper1_MarkerDragEnd);
            this.mapHelper1.MapMouseup += new MapHelper.MapHelper.DlMapMouseup(this.mapHelper1_MapMouseup);
            this.mapHelper1.MapRightClick += new MapHelper.MapHelper.DlMapRightClick(this.mapHelper1_MapRightClick);
            this.mapHelper1.MapDblClick += new MapHelper.MapHelper.DlMapDblClick(this.mapHelper1_MapDblClick);
            this.mapHelper1.RemoveMarkerFinished += new MapHelper.MapHelper.DlRemoveMarkerFinished(this.mapHelper1_RemoveMarkerFinished);
            this.mapHelper1.MarkerRightClick += new MapHelper.MapHelper.DlMarkerRightClick(this.mapHelper1_MarkerRightClick);
            this.mapHelper1.IconSelected += new MapHelper.MapHelper.DlIconSelected(this.mapHelper1_IconSelected);
            this.mapHelper1.MapTypeChanged += new MapHelper.MapHelper.DlMapTypeChanged(this.mapHelper1_MapTypeChanged);
            this.mapHelper1.MapMouseWheel += new MapHelper.MapHelper.DlMouseWheel(this.mapHelper1_MapMouseWheel);
            this.mapHelper1.PointerDone += new MapHelper.MapHelper.DlPointerDone(this.mapHelper1_PointerDone);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.groupBox4);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Right;
            this.groupBox2.Location = new System.Drawing.Point(1054, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(156, 637);
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
            this.groupBox1.Size = new System.Drawing.Size(277, 637);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "组织结构";
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(3, 24);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(271, 610);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseDown);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.数据管理ToolStripMenuItem,
            this.系统设置ToolStripMenuItem,
            this.退出ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1210, 32);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 数据管理ToolStripMenuItem
            // 
            this.数据管理ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.数据备份ToolStripMenuItem,
            this.数据恢复ToolStripMenuItem,
            this.数据同步ToolStripMenuItem,
            this.下载单位注册数据ToolStripMenuItem});
            this.数据管理ToolStripMenuItem.Name = "数据管理ToolStripMenuItem";
            this.数据管理ToolStripMenuItem.Size = new System.Drawing.Size(124, 28);
            this.数据管理ToolStripMenuItem.Text = "数据管理(&M)";
            // 
            // 数据备份ToolStripMenuItem
            // 
            this.数据备份ToolStripMenuItem.Name = "数据备份ToolStripMenuItem";
            this.数据备份ToolStripMenuItem.Size = new System.Drawing.Size(251, 28);
            this.数据备份ToolStripMenuItem.Text = "数据备份(&B)";
            this.数据备份ToolStripMenuItem.Click += new System.EventHandler(this.数据备份ToolStripMenuItem_Click);
            // 
            // 数据恢复ToolStripMenuItem
            // 
            this.数据恢复ToolStripMenuItem.Name = "数据恢复ToolStripMenuItem";
            this.数据恢复ToolStripMenuItem.Size = new System.Drawing.Size(251, 28);
            this.数据恢复ToolStripMenuItem.Text = "数据恢复(&R)";
            this.数据恢复ToolStripMenuItem.Click += new System.EventHandler(this.数据恢复ToolStripMenuItem_Click);
            // 
            // 数据同步ToolStripMenuItem
            // 
            this.数据同步ToolStripMenuItem.Name = "数据同步ToolStripMenuItem";
            this.数据同步ToolStripMenuItem.Size = new System.Drawing.Size(251, 28);
            this.数据同步ToolStripMenuItem.Text = "数据同步(&U)";
            this.数据同步ToolStripMenuItem.Click += new System.EventHandler(this.数据同步ToolStripMenuItem_Click);
            // 
            // 下载单位注册数据ToolStripMenuItem
            // 
            this.下载单位注册数据ToolStripMenuItem.Name = "下载单位注册数据ToolStripMenuItem";
            this.下载单位注册数据ToolStripMenuItem.Size = new System.Drawing.Size(251, 28);
            this.下载单位注册数据ToolStripMenuItem.Text = "下载单位注册数据(&O)";
            this.下载单位注册数据ToolStripMenuItem.Click += new System.EventHandler(this.下载单位注册数据ToolStripMenuItem_Click);
            // 
            // 系统设置ToolStripMenuItem
            // 
            this.系统设置ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.服务器IP设置ToolStripMenuItem1,
            this.边界线属性设置ToolStripMenuItem,
            this.管辖分类设置ToolStripMenuItem,
            this.图符对应设置ToolStripMenuItem,
            this.图符管理设置ToolStripMenuItem,
            this.密码管理ToolStripMenuItem});
            this.系统设置ToolStripMenuItem.Name = "系统设置ToolStripMenuItem";
            this.系统设置ToolStripMenuItem.Overflow = System.Windows.Forms.ToolStripItemOverflow.AsNeeded;
            this.系统设置ToolStripMenuItem.Size = new System.Drawing.Size(116, 28);
            this.系统设置ToolStripMenuItem.Text = "系统设置(&S)";
            // 
            // 服务器IP设置ToolStripMenuItem1
            // 
            this.服务器IP设置ToolStripMenuItem1.Name = "服务器IP设置ToolStripMenuItem1";
            this.服务器IP设置ToolStripMenuItem1.Size = new System.Drawing.Size(229, 28);
            this.服务器IP设置ToolStripMenuItem1.Text = "服务器IP设置(&S)";
            this.服务器IP设置ToolStripMenuItem1.Click += new System.EventHandler(this.IP设置ToolStripMenuItem_Click);
            // 
            // 边界线属性设置ToolStripMenuItem
            // 
            this.边界线属性设置ToolStripMenuItem.Name = "边界线属性设置ToolStripMenuItem";
            this.边界线属性设置ToolStripMenuItem.Size = new System.Drawing.Size(229, 28);
            this.边界线属性设置ToolStripMenuItem.Text = "边界线属性设置(&B)";
            this.边界线属性设置ToolStripMenuItem.Click += new System.EventHandler(this.边界线属性设置ToolStripMenuItem_Click);
            // 
            // 管辖分类设置ToolStripMenuItem
            // 
            this.管辖分类设置ToolStripMenuItem.Name = "管辖分类设置ToolStripMenuItem";
            this.管辖分类设置ToolStripMenuItem.Size = new System.Drawing.Size(229, 28);
            this.管辖分类设置ToolStripMenuItem.Text = "管辖分类设置(&M)";
            this.管辖分类设置ToolStripMenuItem.Click += new System.EventHandler(this.管辖分类设置ToolStripMenuItem_Click);
            // 
            // 图符对应设置ToolStripMenuItem
            // 
            this.图符对应设置ToolStripMenuItem.Name = "图符对应设置ToolStripMenuItem";
            this.图符对应设置ToolStripMenuItem.Size = new System.Drawing.Size(229, 28);
            this.图符对应设置ToolStripMenuItem.Text = "图符对应设置(&C)";
            this.图符对应设置ToolStripMenuItem.Click += new System.EventHandler(this.图符对应设置ToolStripMenuItem_Click);
            // 
            // 图符管理设置ToolStripMenuItem
            // 
            this.图符管理设置ToolStripMenuItem.Name = "图符管理设置ToolStripMenuItem";
            this.图符管理设置ToolStripMenuItem.Size = new System.Drawing.Size(229, 28);
            this.图符管理设置ToolStripMenuItem.Text = "图符扩展设置(&I)";
            this.图符管理设置ToolStripMenuItem.Click += new System.EventHandler(this.图符管理设置ToolStripMenuItem_Click);
            // 
            // 密码管理ToolStripMenuItem
            // 
            this.密码管理ToolStripMenuItem.Name = "密码管理ToolStripMenuItem";
            this.密码管理ToolStripMenuItem.Size = new System.Drawing.Size(229, 28);
            this.密码管理ToolStripMenuItem.Text = "密码管理(&P)";
            this.密码管理ToolStripMenuItem.Click += new System.EventHandler(this.密码管理ToolStripMenuItem_Click);
            // 
            // 退出ToolStripMenuItem
            // 
            this.退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
            this.退出ToolStripMenuItem.Size = new System.Drawing.Size(85, 28);
            this.退出ToolStripMenuItem.Text = "退出(&Q)";
            this.退出ToolStripMenuItem.Click += new System.EventHandler(this.退出ToolStripMenuItem_Click);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.设置当前点为中心点ToolStripMenuItem});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(297, 32);
            // 
            // 设置当前点为中心点ToolStripMenuItem
            // 
            this.设置当前点为中心点ToolStripMenuItem.Name = "设置当前点为中心点ToolStripMenuItem";
            this.设置当前点为中心点ToolStripMenuItem.Size = new System.Drawing.Size(296, 28);
            this.设置当前点为中心点ToolStripMenuItem.Text = "设置当前单位的地图中心点";
            this.设置当前点为中心点ToolStripMenuItem.Click += new System.EventHandler(this.设置当前点为中心点ToolStripMenuItem_Click);
            // 
            // contextMenuStrip3
            // 
            this.contextMenuStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.导入当前单位边界线ToolStripMenuItem});
            this.contextMenuStrip3.Name = "contextMenuStrip3";
            this.contextMenuStrip3.Size = new System.Drawing.Size(243, 32);
            // 
            // 导入当前单位边界线ToolStripMenuItem
            // 
            this.导入当前单位边界线ToolStripMenuItem.Name = "导入当前单位边界线ToolStripMenuItem";
            this.导入当前单位边界线ToolStripMenuItem.Size = new System.Drawing.Size(242, 28);
            this.导入当前单位边界线ToolStripMenuItem.Text = "导入当前单位边界线";
            this.导入当前单位边界线ToolStripMenuItem.Click += new System.EventHandler(this.导入当前单位边界线ToolStripMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1210, 669);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "环境信息化系统";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.contextMenuStrip2.ResumeLayout(false);
            this.contextMenuStrip3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 删除ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 编辑ToolStripMenuItem;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripMenuItem 添加指向位置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 删除指向位置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 修改指向位置ToolStripMenuItem;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.GroupBox groupBox3;
        private MapHelper.MapHelper mapHelper1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ToolStripMenuItem 修改箭头样式ToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 数据管理ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 数据备份ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 数据恢复ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 数据同步ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 系统设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 服务器IP设置ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 退出ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 边界线属性设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 管辖分类设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 图符对应设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 图符管理设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 密码管理ToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem 设置当前点为中心点ToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip3;
        private System.Windows.Forms.ToolStripMenuItem 导入当前单位边界线ToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem 下载单位注册数据ToolStripMenuItem;

    }
}

