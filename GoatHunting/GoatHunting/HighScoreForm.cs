using System;
using System.Drawing;
using System.Windows.Forms;

namespace GoatHunting
{
    public class HighScoreForm : Form
    {
        private Label titleLabel;
        private ListBox highScoreList;
        private Button backButton;
        private int highestKillCount;

        public HighScoreForm(int highestKillCount)
        {
            //this.highestKillCount = highestKillCount;
            if(highestKillCount < HighScoreManager.LoadHighScore()) this.highestKillCount = HighScoreManager.LoadHighScore();
            InitializeForm();
            InitializeControls();
        }

        private void InitializeForm()
        {
            this.Text = "High Scores";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            // Set background color or image
            this.BackColor = Color.MidnightBlue;
        }

        private void InitializeControls()
        {
            // Title Label
            titleLabel = new Label
            {
                Text = "High Scores",
                Font = new Font("Arial", 24, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                Size = new Size(400, 50),
                Location = new Point((this.ClientSize.Width - 400) / 2, 20)
            };
            this.Controls.Add(titleLabel);

            // High Score List
            highScoreList = new ListBox
            {
                Font = new Font("Arial", 14),
                ForeColor = Color.White,
                BackColor = Color.DarkSlateBlue,
                Size = new Size(400, 200),
                Location = new Point((this.ClientSize.Width - 400) / 2, 90),
                BorderStyle = BorderStyle.None
            };

            // Display the highest kill count
            highScoreList.Items.Add($"Highest Kill Count: {this.highestKillCount}");

            this.Controls.Add(highScoreList);

            // Back Button
            backButton = new Button
            {
                Text = "Back",
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.DarkSlateBlue,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 40),
                Location = new Point((this.ClientSize.Width - 120) / 2, 310)
            };
            backButton.Click += BackButton_Click;
            this.Controls.Add(backButton);
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
