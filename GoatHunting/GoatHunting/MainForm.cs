using GoatHunting;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GoatHunting
{
    public class MainForm : Form
    {
        private Button startGameButton;
        private Button highScoreButton;
        private Button exitButton;
        private Label titleLabel;

        public MainForm()
        {
            InitializeForm();
            InitializeControls();
        }

        private void InitializeForm()
        {
            this.Text = "Main Menu";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            // Set background color or image
            //this.BackgroundImage = Image.FromFile("background.jpg"); // Replace with the actual image path
            //this.BackgroundImageLayout = ImageLayout.Stretch;
        }

        private void InitializeControls()
        {
            titleLabel = new Label
            {
                Text = "Welcome to the Goat Hunting Game",
                Font = new Font("Arial", 24, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                Size = new Size(400, 50),
                Location = new Point((this.ClientSize.Width - 400) / 2, 20)
            };
            this.Controls.Add(titleLabel);

            // Start Game Button
            startGameButton = CreateStyledButton("Start Game", 120);
            startGameButton.Click += StartGameButton_Click;
            this.Controls.Add(startGameButton);

            // High Score Button
            highScoreButton = CreateStyledButton("High Score", 180);
            highScoreButton.Click += HighScoreButton_Click;
            this.Controls.Add(highScoreButton);

            // Exit Button
            exitButton = CreateStyledButton("Exit", 240);
            exitButton.Click += ExitButton_Click;
            this.Controls.Add(exitButton);
        }

        private Button CreateStyledButton(string text, int topPosition)
        {
            return new Button
            {
                Text = text,
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.DarkSlateBlue,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(200, 50),
                Location = new Point((this.ClientSize.Width - 200) / 2, topPosition)
            };
        }

        private void StartGameButton_Click(object sender, EventArgs e)
        {
            GameForm gameForm = new GameForm();
            gameForm.FormClosed += (s, args) => this.Show(); // Show MainForm when GameForm is closed
            gameForm.Show();
            this.Hide();
        }

        private void HighScoreButton_Click(object sender, EventArgs e)
        {
            HighScoreForm highScoreForm = new HighScoreForm(GameForm.HighestKillCount);
            highScoreForm.ShowDialog();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
