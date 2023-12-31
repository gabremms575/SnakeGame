﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Media;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace SnakeGame
{
    public partial class Form1 : Form
    {

        private Rectangle Snake;
        private Rectangle Tail;
        static int snakeSpeed = 1;
        bool upArrowDown = false;
        bool downArrowDown = false;
        bool leftDown = false;
        bool rightDown = false;
        string direction = "Right"; // Default direction
        static int width = 20;
        static int height = 15;
        static int score = 0;
         static int delay = 150;
        static bool gameOver = false;
        static Random random = new Random();
        static int foodX, foodY;
        static int headX, headY;
        static List<Point> tailSegments = new List<Point>();
        static int tailLength = 0;
        int blockSize;
        bool gameStarted = false;
        SoundPlayer Point = new SoundPlayer();
        SoundPlayer Loss = new SoundPlayer();


        public Form1()
        {
            InitializeComponent();
            Setup();
            DoubleBuffered = true;
            Paint += Form1_Paint;
            KeyDown += Form1_KeyDown;
            gameTimer.Tick += gameTimer_Tick;
            gameTimer.Interval = delay;
            label1.Visible = true;
            label2.Visible = true;
            Point.SoundLocation = @"C:\Users\emmsg\source\repos\SnakeGame\SnakeGame\Resources\511397__pjhedman__se2-ding.wav";
            Point.Load();
            Loss.SoundLocation = @"C:\Users\emmsg\source\repos\SnakeGame\SnakeGame\Resources\382310__myfox14__game-over-arcade.wav";
            Loss.Load();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           

            gameTimer.Start();
        }

        private void Setup()
        {
            width = ClientSize.Width / 20;
            height = ClientSize.Height / 20;
            blockSize = Math.Min(ClientSize.Width / width, ClientSize.Height / height);

            headX = width / 2;
            headY = height / 2;
            GenerateFood();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;

            

            for (int x = 0; x < width; x++)
            {
                graphics.DrawLine(Pens.Black, x * blockSize, 0, x * blockSize, ClientSize.Height);
            }
            for (int y = 0; y < height; y++)
            {
                graphics.DrawLine(Pens.Black, 0, y * blockSize, ClientSize.Width, y * blockSize);
            }

            Rectangle Snake = new Rectangle(headX * blockSize, headY * blockSize, blockSize, blockSize);
            graphics.FillRectangle(Brushes.Green, Snake);

            Brush eyeBrush = Brushes.White;
            switch (direction)
            {
                case "Up":
                    graphics.FillEllipse(eyeBrush, new RectangleF((headX * blockSize) + blockSize / 4, (headY * blockSize), blockSize / 4, blockSize / 4));
                    graphics.FillEllipse(eyeBrush, new RectangleF((headX * blockSize) + blockSize * 0.5f, (headY * blockSize), blockSize / 4, blockSize / 4));
                    break;
                case "Down":
                    graphics.FillEllipse(eyeBrush, new RectangleF((headX * blockSize) + blockSize / 4, (headY * blockSize) + blockSize * 0.5f, blockSize / 4, blockSize / 4));
                    graphics.FillEllipse(eyeBrush, new RectangleF((headX * blockSize) + blockSize * 0.5f, (headY * blockSize) + blockSize * 0.5f, blockSize / 4, blockSize / 4));
                    break;
                case "Left":
                    graphics.FillEllipse(eyeBrush, new RectangleF((headX * blockSize), (headY * blockSize) + blockSize / 4, blockSize / 4, blockSize / 4));
                    graphics.FillEllipse(eyeBrush, new RectangleF((headX * blockSize), (headY * blockSize) + blockSize * 0.5f, blockSize / 4, blockSize / 4));
                    break;
                case "Right":
                    graphics.FillEllipse(eyeBrush, new RectangleF((headX * blockSize) + blockSize * 0.5f, (headY * blockSize) + blockSize / 4, blockSize / 4, blockSize / 4));
                    graphics.FillEllipse(eyeBrush, new RectangleF((headX * blockSize) + blockSize * 0.5f, (headY * blockSize) + blockSize * 0.5f, blockSize / 4, blockSize / 4));
                    break;
            }

            foreach (var tailSegment in tailSegments)
            {
                Rectangle Tail = new Rectangle(tailSegment.X * blockSize, tailSegment.Y * blockSize, blockSize, blockSize);
                graphics.FillRectangle(Brushes.DarkGreen, Tail);
            }

            Rectangle foodRect = new Rectangle(foodX * blockSize, foodY * blockSize, blockSize, blockSize);
            graphics.FillEllipse(Brushes.Red, foodRect);

            string scoreText = $"Score: {score}";
            Font scoreFont = new Font(Font.FontFamily, 10);
            graphics.DrawString(scoreText, scoreFont, Brushes.Black, new PointF(2, 22));




            if (gameOver)
            {
                string gameOverText = "Game Over!";
                SizeF gameOverSize = graphics.MeasureString(gameOverText, Font);
                float gameOverX = (ClientSize.Width - gameOverSize.Width) / 2;
                float gameOverY = (ClientSize.Height - gameOverSize.Height) / 2;
                graphics.DrawString(gameOverText, Font, Brushes.White, new PointF(gameOverX, gameOverY));
                
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space && !gameStarted)
            {
                gameStarted = true;
                label1.Visible = false;
                label2.Visible = false;
                gameTimer.Start(); // Start the game
            }


            



            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (!downArrowDown)
                    {
                        direction = "Up";
                        upArrowDown = true;
                        downArrowDown = false;
                        leftDown = false;
                        rightDown = false;
                    }
                    break;
                case Keys.Down:
                    if (!upArrowDown)
                    {
                        direction = "Down";
                        downArrowDown = true;
                        upArrowDown = false;
                        leftDown = false;
                        rightDown = false;
                    }
                    break;
                case Keys.Left:
                    if (!rightDown)
                    {
                        direction = "Left";
                        leftDown = true;
                        rightDown = false;
                        upArrowDown = false;
                        downArrowDown = false;
                    }
                    break;
                case Keys.Right:
                    if (!leftDown)
                    {
                        direction = "Right";
                        rightDown = true;
                        leftDown = false;
                        upArrowDown = false;
                        downArrowDown = false;
                    }
                    break;
            }
        }



        private void gameTimer_Tick(object sender, EventArgs e)
        {
            if (!gameStarted)
            {
                return;
            }

            Logic();
            Refresh();
        }

        private void Logic()
        {
            Point prevTail = new Point(headX, headY);

            if (upArrowDown && headY > 0)
            {
                headY -= 1;
            }
            else if (downArrowDown && headY < height - 1)
            {
                headY += 1;
            }
            else if (leftDown && headX > 0)
            {
                headX -= 1;
            }
            else if (rightDown && headX < width - 1)
            {
                headX += 1;
            }

            if (tailLength > 0)
            {
                tailSegments.Insert(0, prevTail);
                if (tailSegments.Count > tailLength)
                {
                    tailSegments.RemoveAt(tailSegments.Count - 1);
                }

                foreach (var tailSegment in tailSegments)
                {
                    if (tailSegment.X == headX && tailSegment.Y == headY)
                    {
                        GameOver();
                        return;
                    }
                }
            }

            if (headX == foodX && headY == foodY)
            {
                score += 10;
                Point.Play();
                tailLength++;
                GenerateFood();
            }

            if (headX < 0 || headX >= width || headY < 0 || headY >= height)
            {
                
                GameOver();
                return;
            }
        }

        private void GenerateFood()
        {
            bool validFood = false;
            while (!validFood)
            {
                foodX = random.Next(1, width - 1); 
                foodY = random.Next(1, height - 1); 

                validFood = true;
                if (foodX == headX && foodY == headY)
                {
                    validFood = false;
                    continue;
                }

                foreach (var tailSegment in tailSegments)
                {
                    if (foodX == tailSegment.X && foodY == tailSegment.Y)
                    {
                        validFood = false;
                        break;
                    }
                }
            }
        }

        private void GameOver()
        {
            Loss.Play();
            gameTimer.Stop();
            gameOver = true;

          

            MessageBox.Show("Game Over");
        }
    }
}