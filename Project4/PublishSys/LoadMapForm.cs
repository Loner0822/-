﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PublishSys
{
    public partial class LoadMapForm : Form
    {
        public LoadMapForm()
        {
            InitializeComponent();
        }

        private IniOperator inip = null;
        private string[] folds = null;
        private string WorkPath = AppDomain.CurrentDomain.BaseDirectory;//当前exe根目录

        private void LoadMapForm_Shown(object sender, EventArgs e)
        {
            string tmp = WorkPath + "Publish\\parameter.ini";
            if (!System.IO.File.Exists(tmp))
            {
                MessageBox.Show("缺少参数文件，窗口将关闭");
                this.Close();
                return;
            }
            inip = new IniOperator(tmp);
            textBox2.Text = inip.ReadString("mapproperties", "centerlng", "");
            textBox3.Text = inip.ReadString("mapproperties", "centerlat", "");
            button2.Enabled = false;
            panel6.Visible = false;
        }

        private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            e.DrawDefault = true; //用默认颜色，只需要在TreeView失去焦点时选中节点仍然突显  
            return;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            List<Dictionary<string, object>> lst = new List<Dictionary<string, object>>();//标注list，从数据库获取
            mapHelper1.ShowMap(int.Parse(e.Node.Text), true, lst);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox2.Text.Trim().Equals("")) {
                MessageBox.Show("请输入经度");
                textBox2.Focus();
                return;
            }

            if (textBox3.Text.Trim().Equals(""))
            {
                MessageBox.Show("请输入纬度");
                textBox3.Focus();
                return;
            }

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) {
                //获取地图文件夹的上级目录
                //允许用户选择地图文件夹的总目录、其下的：roadmap、satellite或者satellite_en子目录
                string tmp = folderBrowserDialog1.SelectedPath;
                int k = tmp.LastIndexOf("roadmap");
                if (k < 0)
                {
                    k = tmp.LastIndexOf("satellite");
                }
                if (k < 0)
                {
                    k = tmp.LastIndexOf("satellite_en");
                }
                if (k > 0)
                {
                    tmp = tmp.Substring(0, k);
                }
                textBox1.Text = tmp;

                //清除TreeView中的控件
                treeView1.Nodes.Clear();

                //检查地图子文件夹是否存在
                string gpath = textBox1.Text + "\\satellite_en";
                if (!Directory.Exists(gpath))
                {
                    MessageBox.Show("请下载混合图(无偏移)，窗口将关闭");
                    this.Close();
                    return;
                }
                gpath = textBox1.Text + "\\roadmap";
                if (!Directory.Exists(gpath))
                {
                    MessageBox.Show("请下载街道图，窗口将关闭");
                    this.Close();
                    return;
                }

                button2.Enabled = true;
                folds = Directory.GetDirectories(gpath);

                //获取纯文件夹名
                for (int i = 0; i < folds.Length; i++)
                {
                    k = folds[i].LastIndexOf("\\");
                    folds[i] = folds[i].Substring(k + 1);
                }

                //按文件序号排序
                for (int i = 0; i < folds.Length - 1; i++)
                {
                    int ki = int.Parse(folds[i]);
                    for (int j = i + 1; j < folds.Length; j++)
                    {
                        int kj = int.Parse(folds[j]);
                        if (kj < ki)
                        {
                            string a = folds[i];
                            folds[i] = folds[j];
                            folds[j] = a;
                            ki = kj;
                        }
                    }
                }

                // 添加进TreeView
                this.treeView1.HideSelection = false;
                this.treeView1.DrawMode = TreeViewDrawMode.OwnerDrawText;
                this.treeView1.DrawNode += new DrawTreeNodeEventHandler(treeView1_DrawNode);
                for (int i = 0; i < folds.Length; ++i) {
                    TreeNode pNode = new TreeNode();
                    pNode.Text = folds[i];
                    //pNode.Tag = int.Parse(folds[i]);
                    treeView1.Nodes.Add(pNode);
                }

                // 初始化MapHelper
                mapHelper1.centerlat = double.Parse(textBox3.Text); //30.067;//必须设置的属性,不能为空
                mapHelper1.centerlng = double.Parse(textBox2.Text); //118.5784; //必须设置的属性,不能为空
                mapHelper1.webpath = WorkPath + "Publish\\googlemap"; //必须设置的属性,不能为空
                mapHelper1.roadmappath = tmp + "\\roadmap"; //必须设置的属性,不能为空
                mapHelper1.satellitemappath = tmp + "\\satellite_en"; //必须设置的属性,不能为空
                mapHelper1.iconspath = WorkPath + "Publish\\PNGICONFOLDER"; //必须设置的属性,不能为空
                mapHelper1.maparr = folds; //必须设置的属性,不能为空
                treeView1.SelectedNode = treeView1.Nodes[0];                
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string lat = textBox3.Text.Trim();
            string lng = textBox2.Text.Trim();
            if (lat.Equals(""))
            {
                MessageBox.Show("请输入纬度");
                textBox2.Focus();
                return;
            }
            if (lng.Equals(""))
            {
                MessageBox.Show("请输入经度");
                textBox3.Focus();
                return;
            }
            inip = new IniOperator(WorkPath + "Publish\\parameter.ini");
            inip.WriteString("mapproperties", "centerlat", lat);
            inip.WriteString("mapproperties", "centerlng", lng);
            TreeNode pNode = treeView1.SelectedNode;
            int MapLevel = int.Parse(pNode.Text);
            int MinLevel = inip.ReadInteger("mapproperties", "minlevel", 99);
            int MaxLevel = inip.ReadInteger("mapproperties", "maxlevel", 0);
            inip.WriteInteger("mapproperties", "minlevel", Math.Min(MinLevel, MapLevel));
            inip.WriteInteger("mapproperties", "maxlevel", Math.Max(MaxLevel, MapLevel));
            inip.WriteInteger("mapproperties", "currlevel", MapLevel);

            string rootPath = textBox1.Text.Trim();
            string mapPath = WorkPath + "Publish\\googlemap\\map";
            string starPath = WorkPath + "Publish\\googlemap\\satellite";

            panel6.Visible = true;

            while (Directory.Exists(mapPath))
            {
                Tools.DeleteFolder(mapPath, panel6);
                Thread.Sleep(30);
            }
            while (Directory.Exists(starPath))
            {
                Tools.DeleteFolder(starPath, panel6);
                Thread.Sleep(30);
            }
            Thread.Sleep(100);

            if (!Directory.Exists(mapPath))
            {
                Directory.CreateDirectory(mapPath);
            }

            if (!Directory.Exists(starPath))
            {
                Directory.CreateDirectory(starPath);
            }

            panel6.Visible = true;
            List<string> mpLst = new List<string>();
            mpLst.Add(MapLevel.ToString());
            string smapPath = rootPath + "\\roadmap";
            for (int i = 0; i < mpLst.Count; i++)
            {
                string destPath = mapPath + "\\" + mpLst[i];
                if (!Directory.Exists(destPath))
                {
                    Directory.CreateDirectory(destPath);
                }
                string sourPath = smapPath + "\\" + mpLst[i];
                Tools.CopyDirectory(sourPath, destPath, panel6, 0);
            }

            smapPath = rootPath + "\\satellite_en";
            for (int i = 0; i < mpLst.Count; i++)
            {
                string destPath = starPath + "\\" + mpLst[i];
                if (!Directory.Exists(destPath))
                {
                    Directory.CreateDirectory(destPath);
                }
                string sourPath = smapPath + "\\" + mpLst[i];
                Tools.CopyDirectory(sourPath, destPath, panel6, 0);
            }
            panel6.Visible = false;
            MessageBox.Show("导入成功!");
            this.DialogResult = DialogResult.OK;
        }
    }
}
