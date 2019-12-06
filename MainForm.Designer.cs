namespace TalosTextTool {
  partial class MainForm {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
      this.textInput = new System.Windows.Forms.TextBox();
      this.colourDialog = new System.Windows.Forms.ColorDialog();
      this.xInput = new System.Windows.Forms.NumericUpDown();
      this.yInput = new System.Windows.Forms.NumericUpDown();
      this.zInput = new System.Windows.Forms.NumericUpDown();
      this.xLabel = new System.Windows.Forms.Label();
      this.yLabel = new System.Windows.Forms.Label();
      this.zLabel = new System.Windows.Forms.Label();
      this.injectButton = new System.Windows.Forms.Button();
      this.textLabel = new System.Windows.Forms.Label();
      this.colourLabel = new System.Windows.Forms.Label();
      this.colourInput = new System.Windows.Forms.Label();
      this.opacityLabel = new System.Windows.Forms.Label();
      this.opacityInput = new System.Windows.Forms.NumericUpDown();
      this.opacityLabelBox = new System.Windows.Forms.Label();
      this.opacityInputBox = new System.Windows.Forms.NumericUpDown();
      this.colourInputBox = new System.Windows.Forms.Label();
      this.colourLabelBox = new System.Windows.Forms.Label();
      this.zLabelBox = new System.Windows.Forms.Label();
      this.yLabelBox = new System.Windows.Forms.Label();
      this.xLabelBox = new System.Windows.Forms.Label();
      this.zInputBox = new System.Windows.Forms.NumericUpDown();
      this.yInputBox = new System.Windows.Forms.NumericUpDown();
      this.x2InputBox = new System.Windows.Forms.NumericUpDown();
      this.xInputBox = new System.Windows.Forms.NumericUpDown();
      this.z2InputBox = new System.Windows.Forms.NumericUpDown();
      this.y2InputBox = new System.Windows.Forms.NumericUpDown();
      this.titleText = new System.Windows.Forms.Label();
      this.titleBox = new System.Windows.Forms.Label();
      this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.autoInject = new System.Windows.Forms.ToolStripMenuItem();
      ((System.ComponentModel.ISupportInitialize)(this.xInput)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.yInput)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.zInput)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.opacityInput)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.opacityInputBox)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.zInputBox)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.yInputBox)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.x2InputBox)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.xInputBox)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.z2InputBox)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.y2InputBox)).BeginInit();
      this.contextMenuStrip.SuspendLayout();
      this.SuspendLayout();
      // 
      // textInput
      // 
      this.textInput.Location = new System.Drawing.Point(70, 160);
      this.textInput.MaxLength = 256;
      this.textInput.Name = "textInput";
      this.textInput.Size = new System.Drawing.Size(120, 20);
      this.textInput.TabIndex = 0;
      this.textInput.Text = "Custom String";
      // 
      // xInput
      // 
      this.xInput.Location = new System.Drawing.Point(70, 35);
      this.xInput.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
      this.xInput.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
      this.xInput.Name = "xInput";
      this.xInput.Size = new System.Drawing.Size(120, 20);
      this.xInput.TabIndex = 1;
      this.xInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.xInput.Value = new decimal(new int[] {
            1120,
            0,
            0,
            0});
      // 
      // yInput
      // 
      this.yInput.Location = new System.Drawing.Point(70, 60);
      this.yInput.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
      this.yInput.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
      this.yInput.Name = "yInput";
      this.yInput.Size = new System.Drawing.Size(120, 20);
      this.yInput.TabIndex = 2;
      this.yInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.yInput.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
      // 
      // zInput
      // 
      this.zInput.Location = new System.Drawing.Point(70, 85);
      this.zInput.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
      this.zInput.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
      this.zInput.Name = "zInput";
      this.zInput.Size = new System.Drawing.Size(120, 20);
      this.zInput.TabIndex = 3;
      this.zInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // xLabel
      // 
      this.xLabel.AutoSize = true;
      this.xLabel.Location = new System.Drawing.Point(10, 37);
      this.xLabel.Name = "xLabel";
      this.xLabel.Size = new System.Drawing.Size(14, 13);
      this.xLabel.TabIndex = 4;
      this.xLabel.Text = "X";
      // 
      // yLabel
      // 
      this.yLabel.AutoSize = true;
      this.yLabel.Location = new System.Drawing.Point(10, 62);
      this.yLabel.Name = "yLabel";
      this.yLabel.Size = new System.Drawing.Size(14, 13);
      this.yLabel.TabIndex = 5;
      this.yLabel.Text = "Y";
      // 
      // zLabel
      // 
      this.zLabel.AutoSize = true;
      this.zLabel.Location = new System.Drawing.Point(10, 87);
      this.zLabel.Name = "zLabel";
      this.zLabel.Size = new System.Drawing.Size(14, 13);
      this.zLabel.TabIndex = 6;
      this.zLabel.Text = "Z";
      // 
      // injectButton
      // 
      this.injectButton.Location = new System.Drawing.Point(205, 159);
      this.injectButton.Name = "injectButton";
      this.injectButton.Size = new System.Drawing.Size(180, 22);
      this.injectButton.TabIndex = 8;
      this.injectButton.Text = "Inject";
      this.injectButton.UseVisualStyleBackColor = true;
      this.injectButton.Click += new System.EventHandler(this.InjectButton_Click);
      // 
      // textLabel
      // 
      this.textLabel.AutoSize = true;
      this.textLabel.Location = new System.Drawing.Point(10, 162);
      this.textLabel.Name = "textLabel";
      this.textLabel.Size = new System.Drawing.Size(28, 13);
      this.textLabel.TabIndex = 9;
      this.textLabel.Text = "Text";
      // 
      // colourLabel
      // 
      this.colourLabel.AutoSize = true;
      this.colourLabel.Location = new System.Drawing.Point(10, 112);
      this.colourLabel.Name = "colourLabel";
      this.colourLabel.Size = new System.Drawing.Size(37, 13);
      this.colourLabel.TabIndex = 10;
      this.colourLabel.Text = "Colour";
      // 
      // colourInput
      // 
      this.colourInput.BackColor = System.Drawing.Color.White;
      this.colourInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.colourInput.Cursor = System.Windows.Forms.Cursors.Hand;
      this.colourInput.Location = new System.Drawing.Point(70, 110);
      this.colourInput.Name = "colourInput";
      this.colourInput.Size = new System.Drawing.Size(120, 20);
      this.colourInput.TabIndex = 13;
      this.colourInput.Text = "FFFFFF";
      this.colourInput.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      this.colourInput.Click += new System.EventHandler(this.ColourInput_Click);
      // 
      // opacityLabel
      // 
      this.opacityLabel.AutoSize = true;
      this.opacityLabel.Location = new System.Drawing.Point(10, 137);
      this.opacityLabel.Name = "opacityLabel";
      this.opacityLabel.Size = new System.Drawing.Size(43, 13);
      this.opacityLabel.TabIndex = 15;
      this.opacityLabel.Text = "Opacity";
      // 
      // opacityInput
      // 
      this.opacityInput.Hexadecimal = true;
      this.opacityInput.Location = new System.Drawing.Point(70, 135);
      this.opacityInput.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
      this.opacityInput.Name = "opacityInput";
      this.opacityInput.Size = new System.Drawing.Size(120, 20);
      this.opacityInput.TabIndex = 14;
      this.opacityInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.opacityInput.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
      // 
      // opacityLabelBox
      // 
      this.opacityLabelBox.AutoSize = true;
      this.opacityLabelBox.Location = new System.Drawing.Point(205, 137);
      this.opacityLabelBox.Name = "opacityLabelBox";
      this.opacityLabelBox.Size = new System.Drawing.Size(43, 13);
      this.opacityLabelBox.TabIndex = 25;
      this.opacityLabelBox.Text = "Opacity";
      // 
      // opacityInputBox
      // 
      this.opacityInputBox.Hexadecimal = true;
      this.opacityInputBox.Location = new System.Drawing.Point(265, 135);
      this.opacityInputBox.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
      this.opacityInputBox.Name = "opacityInputBox";
      this.opacityInputBox.Size = new System.Drawing.Size(120, 20);
      this.opacityInputBox.TabIndex = 24;
      this.opacityInputBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.opacityInputBox.Value = new decimal(new int[] {
            176,
            0,
            0,
            0});
      // 
      // colourInputBox
      // 
      this.colourInputBox.BackColor = System.Drawing.Color.Black;
      this.colourInputBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.colourInputBox.Cursor = System.Windows.Forms.Cursors.Hand;
      this.colourInputBox.ForeColor = System.Drawing.Color.White;
      this.colourInputBox.Location = new System.Drawing.Point(265, 110);
      this.colourInputBox.Name = "colourInputBox";
      this.colourInputBox.Size = new System.Drawing.Size(120, 20);
      this.colourInputBox.TabIndex = 23;
      this.colourInputBox.Text = "000000";
      this.colourInputBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      this.colourInputBox.Click += new System.EventHandler(this.ColourInputBox_Click);
      // 
      // colourLabelBox
      // 
      this.colourLabelBox.AutoSize = true;
      this.colourLabelBox.Location = new System.Drawing.Point(205, 112);
      this.colourLabelBox.Name = "colourLabelBox";
      this.colourLabelBox.Size = new System.Drawing.Size(37, 13);
      this.colourLabelBox.TabIndex = 22;
      this.colourLabelBox.Text = "Colour";
      // 
      // zLabelBox
      // 
      this.zLabelBox.AutoSize = true;
      this.zLabelBox.Location = new System.Drawing.Point(205, 87);
      this.zLabelBox.Name = "zLabelBox";
      this.zLabelBox.Size = new System.Drawing.Size(14, 13);
      this.zLabelBox.TabIndex = 21;
      this.zLabelBox.Text = "Z";
      // 
      // yLabelBox
      // 
      this.yLabelBox.AutoSize = true;
      this.yLabelBox.Location = new System.Drawing.Point(205, 62);
      this.yLabelBox.Name = "yLabelBox";
      this.yLabelBox.Size = new System.Drawing.Size(14, 13);
      this.yLabelBox.TabIndex = 20;
      this.yLabelBox.Text = "Y";
      // 
      // xLabelBox
      // 
      this.xLabelBox.AutoSize = true;
      this.xLabelBox.Location = new System.Drawing.Point(205, 37);
      this.xLabelBox.Name = "xLabelBox";
      this.xLabelBox.Size = new System.Drawing.Size(14, 13);
      this.xLabelBox.TabIndex = 19;
      this.xLabelBox.Text = "X";
      // 
      // zInputBox
      // 
      this.zInputBox.Location = new System.Drawing.Point(265, 85);
      this.zInputBox.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
      this.zInputBox.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
      this.zInputBox.Name = "zInputBox";
      this.zInputBox.Size = new System.Drawing.Size(57, 20);
      this.zInputBox.TabIndex = 18;
      this.zInputBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // yInputBox
      // 
      this.yInputBox.Location = new System.Drawing.Point(265, 60);
      this.yInputBox.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
      this.yInputBox.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
      this.yInputBox.Name = "yInputBox";
      this.yInputBox.Size = new System.Drawing.Size(57, 20);
      this.yInputBox.TabIndex = 17;
      this.yInputBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.yInputBox.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
      // 
      // x2InputBox
      // 
      this.x2InputBox.Location = new System.Drawing.Point(328, 35);
      this.x2InputBox.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
      this.x2InputBox.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
      this.x2InputBox.Name = "x2InputBox";
      this.x2InputBox.Size = new System.Drawing.Size(57, 20);
      this.x2InputBox.TabIndex = 26;
      this.x2InputBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.x2InputBox.Value = new decimal(new int[] {
            1260,
            0,
            0,
            0});
      // 
      // xInputBox
      // 
      this.xInputBox.Location = new System.Drawing.Point(265, 35);
      this.xInputBox.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
      this.xInputBox.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
      this.xInputBox.Name = "xInputBox";
      this.xInputBox.Size = new System.Drawing.Size(57, 20);
      this.xInputBox.TabIndex = 27;
      this.xInputBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.xInputBox.Value = new decimal(new int[] {
            1100,
            0,
            0,
            0});
      // 
      // z2InputBox
      // 
      this.z2InputBox.Location = new System.Drawing.Point(328, 85);
      this.z2InputBox.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
      this.z2InputBox.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
      this.z2InputBox.Name = "z2InputBox";
      this.z2InputBox.Size = new System.Drawing.Size(57, 20);
      this.z2InputBox.TabIndex = 29;
      this.z2InputBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // y2InputBox
      // 
      this.y2InputBox.Location = new System.Drawing.Point(328, 60);
      this.y2InputBox.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
      this.y2InputBox.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
      this.y2InputBox.Name = "y2InputBox";
      this.y2InputBox.Size = new System.Drawing.Size(57, 20);
      this.y2InputBox.TabIndex = 28;
      this.y2InputBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.y2InputBox.Value = new decimal(new int[] {
            65,
            0,
            0,
            0});
      // 
      // titleText
      // 
      this.titleText.AutoSize = true;
      this.titleText.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.titleText.Location = new System.Drawing.Point(80, 8);
      this.titleText.Name = "titleText";
      this.titleText.Size = new System.Drawing.Size(39, 20);
      this.titleText.TabIndex = 30;
      this.titleText.Text = "Text";
      // 
      // titleBox
      // 
      this.titleBox.AutoSize = true;
      this.titleBox.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.titleBox.Location = new System.Drawing.Point(248, 8);
      this.titleBox.Name = "titleBox";
      this.titleBox.Size = new System.Drawing.Size(93, 20);
      this.titleBox.TabIndex = 31;
      this.titleBox.Text = "Background";
      // 
      // contextMenuStrip
      // 
      this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autoInject});
      this.contextMenuStrip.Name = "contextMenuStrip";
      this.contextMenuStrip.Size = new System.Drawing.Size(181, 48);
      // 
      // autoInject
      // 
      this.autoInject.CheckOnClick = true;
      this.autoInject.Name = "autoInject";
      this.autoInject.Size = new System.Drawing.Size(180, 22);
      this.autoInject.Text = "Auto Inject";
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(395, 189);
      this.ContextMenuStrip = this.contextMenuStrip;
      this.Controls.Add(this.titleBox);
      this.Controls.Add(this.titleText);
      this.Controls.Add(this.z2InputBox);
      this.Controls.Add(this.y2InputBox);
      this.Controls.Add(this.xInputBox);
      this.Controls.Add(this.x2InputBox);
      this.Controls.Add(this.opacityLabelBox);
      this.Controls.Add(this.opacityInputBox);
      this.Controls.Add(this.colourInputBox);
      this.Controls.Add(this.colourLabelBox);
      this.Controls.Add(this.zLabelBox);
      this.Controls.Add(this.yLabelBox);
      this.Controls.Add(this.xLabelBox);
      this.Controls.Add(this.zInputBox);
      this.Controls.Add(this.yInputBox);
      this.Controls.Add(this.opacityLabel);
      this.Controls.Add(this.opacityInput);
      this.Controls.Add(this.colourInput);
      this.Controls.Add(this.colourLabel);
      this.Controls.Add(this.textLabel);
      this.Controls.Add(this.injectButton);
      this.Controls.Add(this.zLabel);
      this.Controls.Add(this.yLabel);
      this.Controls.Add(this.xLabel);
      this.Controls.Add(this.zInput);
      this.Controls.Add(this.yInput);
      this.Controls.Add(this.xInput);
      this.Controls.Add(this.textInput);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.KeyPreview = true;
      this.Name = "MainForm";
      this.Text = "Talos Text Tool";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
      this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
      ((System.ComponentModel.ISupportInitialize)(this.xInput)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.yInput)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.zInput)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.opacityInput)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.opacityInputBox)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.zInputBox)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.yInputBox)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.x2InputBox)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.xInputBox)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.z2InputBox)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.y2InputBox)).EndInit();
      this.contextMenuStrip.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox textInput;
    private System.Windows.Forms.ColorDialog colourDialog;
    private System.Windows.Forms.NumericUpDown xInput;
    private System.Windows.Forms.NumericUpDown yInput;
    private System.Windows.Forms.NumericUpDown zInput;
    private System.Windows.Forms.Label xLabel;
    private System.Windows.Forms.Label yLabel;
    private System.Windows.Forms.Label zLabel;
    private System.Windows.Forms.Button injectButton;
    private System.Windows.Forms.Label textLabel;
    private System.Windows.Forms.Label colourLabel;
    private System.Windows.Forms.Label colourInput;
    private System.Windows.Forms.Label opacityLabel;
    private System.Windows.Forms.NumericUpDown opacityInput;
    private System.Windows.Forms.Label opacityLabelBox;
    private System.Windows.Forms.NumericUpDown opacityInputBox;
    private System.Windows.Forms.Label colourInputBox;
    private System.Windows.Forms.Label colourLabelBox;
    private System.Windows.Forms.Label zLabelBox;
    private System.Windows.Forms.Label yLabelBox;
    private System.Windows.Forms.Label xLabelBox;
    private System.Windows.Forms.NumericUpDown zInputBox;
    private System.Windows.Forms.NumericUpDown yInputBox;
    private System.Windows.Forms.NumericUpDown x2InputBox;
    private System.Windows.Forms.NumericUpDown xInputBox;
    private System.Windows.Forms.NumericUpDown z2InputBox;
    private System.Windows.Forms.NumericUpDown y2InputBox;
    private System.Windows.Forms.Label titleText;
    private System.Windows.Forms.Label titleBox;
    private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
    private System.Windows.Forms.ToolStripMenuItem autoInject;
  }
}

