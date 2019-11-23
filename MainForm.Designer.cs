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
      this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
      this.colourBox = new System.Windows.Forms.Label();
      this.opacityLabel = new System.Windows.Forms.Label();
      this.opacityInput = new System.Windows.Forms.NumericUpDown();
      ((System.ComponentModel.ISupportInitialize)(this.xInput)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.yInput)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.zInput)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.opacityInput)).BeginInit();
      this.SuspendLayout();
      // 
      // textInput
      // 
      this.textInput.Location = new System.Drawing.Point(67, 12);
      this.textInput.Name = "textInput";
      this.textInput.Size = new System.Drawing.Size(120, 20);
      this.textInput.TabIndex = 0;
      // 
      // xInput
      // 
      this.xInput.Location = new System.Drawing.Point(67, 38);
      this.xInput.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
      this.xInput.Name = "xInput";
      this.xInput.Size = new System.Drawing.Size(120, 20);
      this.xInput.TabIndex = 1;
      this.xInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      this.xInput.Value = new decimal(new int[] {
            40,
            0,
            0,
            0});
      // 
      // yInput
      // 
      this.yInput.Location = new System.Drawing.Point(67, 64);
      this.yInput.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
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
      this.zInput.Location = new System.Drawing.Point(67, 88);
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
      this.xLabel.Location = new System.Drawing.Point(11, 40);
      this.xLabel.Name = "xLabel";
      this.xLabel.Size = new System.Drawing.Size(14, 13);
      this.xLabel.TabIndex = 4;
      this.xLabel.Text = "X";
      // 
      // yLabel
      // 
      this.yLabel.AutoSize = true;
      this.yLabel.Location = new System.Drawing.Point(11, 66);
      this.yLabel.Name = "yLabel";
      this.yLabel.Size = new System.Drawing.Size(14, 13);
      this.yLabel.TabIndex = 5;
      this.yLabel.Text = "Y";
      // 
      // zLabel
      // 
      this.zLabel.AutoSize = true;
      this.zLabel.Location = new System.Drawing.Point(11, 90);
      this.zLabel.Name = "zLabel";
      this.zLabel.Size = new System.Drawing.Size(14, 13);
      this.zLabel.TabIndex = 6;
      this.zLabel.Text = "Z";
      // 
      // injectButton
      // 
      this.injectButton.Location = new System.Drawing.Point(14, 165);
      this.injectButton.Name = "injectButton";
      this.injectButton.Size = new System.Drawing.Size(173, 23);
      this.injectButton.TabIndex = 8;
      this.injectButton.Text = "Inject";
      this.injectButton.UseVisualStyleBackColor = true;
      this.injectButton.Click += new System.EventHandler(this.InjectButton_Click);
      // 
      // textLabel
      // 
      this.textLabel.AutoSize = true;
      this.textLabel.Location = new System.Drawing.Point(11, 15);
      this.textLabel.Name = "textLabel";
      this.textLabel.Size = new System.Drawing.Size(28, 13);
      this.textLabel.TabIndex = 9;
      this.textLabel.Text = "Text";
      // 
      // colourLabel
      // 
      this.colourLabel.AutoSize = true;
      this.colourLabel.Location = new System.Drawing.Point(11, 114);
      this.colourLabel.Name = "colourLabel";
      this.colourLabel.Size = new System.Drawing.Size(37, 13);
      this.colourLabel.TabIndex = 10;
      this.colourLabel.Text = "Colour";
      // 
      // colourBox
      // 
      this.colourBox.BackColor = System.Drawing.Color.White;
      this.colourBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.colourBox.Cursor = System.Windows.Forms.Cursors.Hand;
      this.colourBox.Location = new System.Drawing.Point(67, 114);
      this.colourBox.Name = "colourBox";
      this.colourBox.Size = new System.Drawing.Size(120, 20);
      this.colourBox.TabIndex = 13;
      this.colourBox.Text = "FFFFFF";
      this.colourBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      this.colourBox.Click += new System.EventHandler(this.ColourBox_Click);
      // 
      // opacityLabel
      // 
      this.opacityLabel.AutoSize = true;
      this.opacityLabel.Location = new System.Drawing.Point(11, 141);
      this.opacityLabel.Name = "opacityLabel";
      this.opacityLabel.Size = new System.Drawing.Size(43, 13);
      this.opacityLabel.TabIndex = 15;
      this.opacityLabel.Text = "Opacity";
      // 
      // opacityInput
      // 
      this.opacityInput.Hexadecimal = true;
      this.opacityInput.Location = new System.Drawing.Point(67, 139);
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
      this.opacityInput.ValueChanged += new System.EventHandler(this.OpacityInput_ValueChanged);
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(199, 198);
      this.Controls.Add(this.opacityLabel);
      this.Controls.Add(this.opacityInput);
      this.Controls.Add(this.colourBox);
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
      this.Name = "MainForm";
      this.Text = "Talos Text Tool";
      ((System.ComponentModel.ISupportInitialize)(this.xInput)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.yInput)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.zInput)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.opacityInput)).EndInit();
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
    private System.ComponentModel.BackgroundWorker backgroundWorker1;
    private System.Windows.Forms.Label colourBox;
    private System.Windows.Forms.Label opacityLabel;
    private System.Windows.Forms.NumericUpDown opacityInput;
  }
}

