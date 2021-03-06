using forgotten_construction_set.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace forgotten_construction_set
{
	public class TranslationDialog : Form
	{
		private navigation nav;

		private NavigationTranslation navTranslation;

		private Thread statsThread;

		private IContainer components;

		private Button btnExport;

		private Button btnImport;

		private GroupBox gbStep2;

		private GroupBox gbStep1;

		private CheckBox cbExportDialogue;

		private GroupBox gbStats;

		private Label lblTranslatedSource;

		private Label lblWordsTotal;

		private Label lblCompletePercentage;

		private Label lblWordsExtra;

		private Label lblTranslatedWords;

		private ToolTip toolTip;

		private CheckBox cbExportChangesOnly;

		private Button btnAutoTranslate;

		public TranslationDialog(navigation nav, NavigationTranslation navTranslation)
		{
			this.nav = nav;
			this.navTranslation = navTranslation;
			this.InitializeComponent();
			base.CenterToScreen();
			this.Calculate();
		}

		private void btnAutoTranslate_Click(object sender, EventArgs e)
		{
			System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
			int num = TranslationManager.CopyTranslationsFromExistingLines();
			System.Windows.Forms.Cursor.Current = Cursors.Default;
			if (num <= 0)
			{
				MessageBox.Show("没有匹配的搜索结果", "翻译", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				return;
			}
			MessageBox.Show(string.Concat("增加了 ", num, " 个额外对话行的翻译"), "翻译", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			this.navTranslation.Refresh();
			this.Calculate();
		}

		private void btnExport_Click(object sender, EventArgs e)
		{
			base.Enabled = false;
			(new Thread(() => {
				string str = TranslationManager.ExportTranslationFiles(this.nav, this.cbExportDialogue.Checked, this.cbExportChangesOnly.Checked);
				base.BeginInvoke(new MethodInvoker(() => {
					if (!str.Contains(":"))
					{
						MessageBox.Show(str, "导出失败", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					}
					else if (!this.cbExportDialogue.Checked)
					{
						MessageBox.Show(string.Format("已导出基本游戏数据到 {0}.", str), "导出完成", MessageBoxButtons.OK);
					}
					else
					{
						MessageBox.Show(string.Format("已导出基本游戏数据与对话到 {0}.", str), "导出完成", MessageBoxButtons.OK);
					}
					this.Enabled = true;
				}));
			})
			{
				IsBackground = true
			}).Start();
		}

		private void btnImport_Click(object sender, EventArgs e)
		{
			if (TranslationManager.TranslationMode)
			{
				base.Enabled = false;
				(new Thread(() => {
					int num;
					bool flag = TranslationManager.ImportTranslationFiles(this.nav, out num);
					base.BeginInvoke(new MethodInvoker(() => {
						if (!flag)
						{
							MessageBox.Show("导入错误. 请再次尝试.", "导入错误", MessageBoxButtons.OK);
						}
						else
						{
							MessageBox.Show(string.Concat("已导入对话行: ", num.ToString()), "导入成功", MessageBoxButtons.OK);
							if (num > 0)
							{
								this.nav.HasChanges = true;
								this.navTranslation.UpdateList("");
							}
						}
						this.Enabled = true;
					}));
				})
				{
					IsBackground = true
				}).Start();
			}
		}

		public void Calculate()
		{
			this.gbStats.Text = "对话统计(计算...)";
			this.statsThread = new Thread(() => {
				float single;
				int num;
				int num1;
				int num2;
				TranslationManager.GetStats(out single, out num, out num1, out num2);
				single = (float)((int)Math.Round((double)single * 1000)) / 1000f;
				int length = 0;
				char[] chrArray = new char[] { ' ', '\n' };
				Dictionary<string, byte> strs = new Dictionary<string, byte>();
				foreach (GameData.Item value in this.nav.ou.gameData.items.Values)
				{
					if (value.type != itemType.DIALOGUE_LINE)
					{
						continue;
					}
					foreach (KeyValuePair<string, object> keyValuePair in value)
					{
						if (value.getState(keyValuePair.Key) == GameData.State.ORIGINAL || !keyValuePair.Key.StartsWith("text"))
						{
							continue;
						}
						string str = keyValuePair.Value as string;
						if (strs.ContainsKey(str))
						{
							continue;
						}
						strs.Add(str, 0);
						length += (int)str.Split(chrArray, StringSplitOptions.RemoveEmptyEntries).Length;
					}
				}
				StreamWriter streamWriter = new StreamWriter(string.Concat(TranslationManager.ActiveTranslation.path, "/translated.txt"), false, Encoding.UTF8);
				foreach (string key in strs.Keys)
				{
					streamWriter.WriteLine(key);
				}
				streamWriter.Flush();
				streamWriter.Close();
				base.BeginInvoke(new MethodInvoker(() => {
					this.lblCompletePercentage.Text = string.Concat(single.ToString(), "%");
					this.lblTranslatedSource.Text = num.ToString();
					this.lblWordsTotal.Text = num1.ToString();
					this.lblWordsExtra.Text = num2.ToString();
					this.lblTranslatedWords.Text = length.ToString();
					this.gbStats.Text = "对话统计";
				}));
			});
			this.statsThread.Start();
		}

		private void cbExportDialogue_CheckedChanged(object sender, EventArgs e)
		{
			this.cbExportChangesOnly.Enabled = this.cbExportDialogue.Checked;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.btnImport = new Button();
			this.btnExport = new Button();
			this.gbStep2 = new GroupBox();
			this.gbStep1 = new GroupBox();
			this.cbExportChangesOnly = new CheckBox();
			this.cbExportDialogue = new CheckBox();
			this.gbStats = new GroupBox();
			this.lblTranslatedWords = new Label();
			this.lblWordsExtra = new Label();
			this.lblTranslatedSource = new Label();
			this.lblWordsTotal = new Label();
			this.lblCompletePercentage = new Label();
			this.toolTip = new ToolTip(this.components);
			this.btnAutoTranslate = new Button();
			Label label = new Label();
			Label font = new Label();
			Label point = new Label();
			Label size = new Label();
			Label label1 = new Label();
			this.gbStep2.SuspendLayout();
			this.gbStep1.SuspendLayout();
			this.gbStats.SuspendLayout();
			base.SuspendLayout();
			label.AutoSize = true;
			label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			label.Location = new Point(6, 21);
			label.Name = "label1";
			label.Size = new System.Drawing.Size(63, 13);
			label.TabIndex = 1;
			label.Text = "完成:";
			font.AutoSize = true;
			font.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			font.Location = new Point(6, 40);
			font.Name = "label2";
			font.Size = new System.Drawing.Size(150, 13);
			font.TabIndex = 1;
			font.Text = "已翻译的源词:";
			this.toolTip.SetToolTip(font, "您已翻译的语句中的单词数");
			point.AutoSize = true;
			point.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			point.Location = new Point(6, 59);
			point.Name = "label3";
			point.Size = new System.Drawing.Size(119, 13);
			point.TabIndex = 1;
			point.Text = "总共的原词数:";
			this.toolTip.SetToolTip(point, "要翻译的总词数");
			size.AutoSize = true;
			size.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			size.Location = new Point(6, 95);
			size.Name = "label5";
			size.Size = new System.Drawing.Size(77, 13);
			size.TabIndex = 3;
			size.Text = "额外的词数:";
			this.toolTip.SetToolTip(size, "你写的不再使用的单词");
			label1.AutoSize = true;
			label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			label1.Location = new Point(6, 78);
			label1.Name = "label4";
			label1.Size = new System.Drawing.Size(111, 13);
			label1.TabIndex = 4;
			label1.Text = "已翻译的词数:";
			this.toolTip.SetToolTip(label1, "你写的总字数");
			this.btnImport.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.btnImport.Image = Resources.Import_grey_16x;
			this.btnImport.ImageAlign = ContentAlignment.MiddleRight;
			this.btnImport.Location = new Point(6, 19);
			this.btnImport.Name = "btnImport";
			this.btnImport.Size = new System.Drawing.Size(200, 51);
			this.btnImport.TabIndex = 0;
			this.btnImport.Text = "导入";
			this.btnImport.TextImageRelation = TextImageRelation.ImageBeforeText;
			this.toolTip.SetToolTip(this.btnImport, "导入gamedata.po以及任何在对话子目录中的对话翻译");
			this.btnImport.UseVisualStyleBackColor = true;
			this.btnImport.Click += new EventHandler(this.btnImport_Click);
			this.btnExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.btnExport.Image = Resources.VSO_Export_16x;
			this.btnExport.ImageAlign = ContentAlignment.MiddleRight;
			this.btnExport.Location = new Point(6, 19);
			this.btnExport.Name = "btnExport";
			this.btnExport.Size = new System.Drawing.Size(200, 51);
			this.btnExport.TabIndex = 0;
			this.btnExport.Text = "导出";
			this.btnExport.TextImageRelation = TextImageRelation.ImageBeforeText;
			this.toolTip.SetToolTip(this.btnExport, "导出gamedata pot文件");
			this.btnExport.UseVisualStyleBackColor = true;
			this.btnExport.Click += new EventHandler(this.btnExport_Click);
			this.gbStep2.Controls.Add(this.btnImport);
			this.gbStep2.Location = new Point(12, 117);
			this.gbStep2.Name = "gbStep2";
			this.gbStep2.Size = new System.Drawing.Size(212, 76);
			this.gbStep2.TabIndex = 1;
			this.gbStep2.TabStop = false;
			this.gbStep2.Text = "导入翻译文件";
			this.gbStep1.Controls.Add(this.cbExportChangesOnly);
			this.gbStep1.Controls.Add(this.cbExportDialogue);
			this.gbStep1.Controls.Add(this.btnExport);
			this.gbStep1.Location = new Point(12, 12);
			this.gbStep1.Name = "gbStep1";
			this.gbStep1.Size = new System.Drawing.Size(212, 99);
			this.gbStep1.TabIndex = 0;
			this.gbStep1.TabStop = false;
			this.gbStep1.Text = "导出游戏数据字符串";
			this.cbExportChangesOnly.AutoSize = true;
			this.cbExportChangesOnly.Enabled = false;
			this.cbExportChangesOnly.Location = new Point(114, 76);
			this.cbExportChangesOnly.Name = "cbExportChangesOnly";
			this.cbExportChangesOnly.Size = new System.Drawing.Size(92, 17);
			this.cbExportChangesOnly.TabIndex = 2;
			this.cbExportChangesOnly.Text = "仅限更改";
			this.toolTip.SetToolTip(this.cbExportChangesOnly, "仅导出尚未完全翻译的对话的对话文件");
			this.cbExportChangesOnly.UseVisualStyleBackColor = true;
			this.cbExportDialogue.AutoSize = true;
			this.cbExportDialogue.Location = new Point(6, 76);
			this.cbExportDialogue.Name = "cbExportDialogue";
			this.cbExportDialogue.Size = new System.Drawing.Size(99, 17);
			this.cbExportDialogue.TabIndex = 1;
			this.cbExportDialogue.Text = "导出对话";
			this.toolTip.SetToolTip(this.cbExportDialogue, "将对话包导出为pot文件");
			this.cbExportDialogue.UseVisualStyleBackColor = true;
			this.cbExportDialogue.CheckedChanged += new EventHandler(this.cbExportDialogue_CheckedChanged);
			this.gbStats.Controls.Add(this.lblTranslatedWords);
			this.gbStats.Controls.Add(label1);
			this.gbStats.Controls.Add(size);
			this.gbStats.Controls.Add(this.lblWordsExtra);
			this.gbStats.Controls.Add(point);
			this.gbStats.Controls.Add(font);
			this.gbStats.Controls.Add(label);
			this.gbStats.Controls.Add(this.lblTranslatedSource);
			this.gbStats.Controls.Add(this.lblWordsTotal);
			this.gbStats.Controls.Add(this.lblCompletePercentage);
			this.gbStats.Location = new Point(12, 199);
			this.gbStats.Name = "gbStats";
			this.gbStats.Size = new System.Drawing.Size(212, 128);
			this.gbStats.TabIndex = 2;
			this.gbStats.TabStop = false;
			this.gbStats.Text = "统计";
			this.lblTranslatedWords.Location = new Point(145, 78);
			this.lblTranslatedWords.Name = "lblTranslatedWords";
			this.lblTranslatedWords.Size = new System.Drawing.Size(61, 13);
			this.lblTranslatedWords.TabIndex = 5;
			this.lblTranslatedWords.Text = "-";
			this.lblTranslatedWords.TextAlign = ContentAlignment.TopRight;
			this.lblWordsExtra.Location = new Point(145, 95);
			this.lblWordsExtra.Name = "lblWordsExtra";
			this.lblWordsExtra.Size = new System.Drawing.Size(61, 13);
			this.lblWordsExtra.TabIndex = 2;
			this.lblWordsExtra.Text = "-";
			this.lblWordsExtra.TextAlign = ContentAlignment.TopRight;
			this.lblTranslatedSource.Location = new Point(145, 40);
			this.lblTranslatedSource.Name = "lblTranslatedSource";
			this.lblTranslatedSource.Size = new System.Drawing.Size(61, 13);
			this.lblTranslatedSource.TabIndex = 0;
			this.lblTranslatedSource.Text = "-";
			this.lblTranslatedSource.TextAlign = ContentAlignment.TopRight;
			this.lblWordsTotal.Location = new Point(145, 59);
			this.lblWordsTotal.Name = "lblWordsTotal";
			this.lblWordsTotal.Size = new System.Drawing.Size(61, 13);
			this.lblWordsTotal.TabIndex = 0;
			this.lblWordsTotal.Text = "-";
			this.lblWordsTotal.TextAlign = ContentAlignment.TopRight;
			this.lblCompletePercentage.Location = new Point(145, 21);
			this.lblCompletePercentage.Name = "lblCompletePercentage";
			this.lblCompletePercentage.Size = new System.Drawing.Size(61, 13);
			this.lblCompletePercentage.TabIndex = 0;
			this.lblCompletePercentage.Text = "-%";
			this.lblCompletePercentage.TextAlign = ContentAlignment.TopRight;
			this.btnAutoTranslate.Location = new Point(126, 334);
			this.btnAutoTranslate.Name = "btnAutoTranslate";
			this.btnAutoTranslate.Size = new System.Drawing.Size(95, 23);
			this.btnAutoTranslate.TabIndex = 3;
			this.btnAutoTranslate.Text = "自动翻译";
			this.toolTip.SetToolTip(this.btnAutoTranslate, "从现有翻译中复制翻译为新语句");
			this.btnAutoTranslate.UseVisualStyleBackColor = true;
			this.btnAutoTranslate.Click += new EventHandler(this.btnAutoTranslate_Click);
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(236, 369);
			base.Controls.Add(this.btnAutoTranslate);
			base.Controls.Add(this.gbStats);
			base.Controls.Add(this.gbStep1);
			base.Controls.Add(this.gbStep2);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "TranslationDialog";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			this.Text = "翻译";
			base.FormClosing += new FormClosingEventHandler(this.TranslationDialog_FormClosing);
			this.gbStep2.ResumeLayout(false);
			this.gbStep1.ResumeLayout(false);
			this.gbStep1.PerformLayout();
			this.gbStats.ResumeLayout(false);
			this.gbStats.PerformLayout();
			base.ResumeLayout(false);
		}

		protected override bool ProcessDialogKey(Keys keyData)
		{
			if (Control.ModifierKeys == Keys.None && keyData == Keys.Escape)
			{
				base.Close();
				return true;
			}
			return base.ProcessDialogKey(keyData);
		}

		private void TranslationDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (this.statsThread.IsAlive)
			{
				this.statsThread.Abort();
			}
		}
	}
}