namespace WinFormsApp4
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Text = "Form1";
        }

        #endregion

    }
    public class MainForm : Form
    {
        private Label lblMatrixA, lblVectorB, lblInitialX, lblEpsilon, lblSolution;
        private TextBox txtMatrixA, txtVectorB, txtInitialX, txtEpsilon, txtResults, txtSolution;
        private Button btnSolve;

        public MainForm()
        {
            this.Text = "Розв'язання методом ітерацій";
            this.ClientSize = new System.Drawing.Size(350, 600);
            InitializeControls();
        }

        private void InitializeControls()
        {
            lblMatrixA = new Label
            {
                Text = "Матриця A:",
                Location = new Point(12, 15),
                AutoSize = true
            };
            this.Controls.Add(lblMatrixA);

            txtMatrixA = new TextBox
            {
                Location = new Point(12, 35),
                Size = new Size(300, 80),
                Multiline = true
            };
            this.Controls.Add(txtMatrixA);

            lblVectorB = new Label
            {
                Text = "Вектор b:",
                Location = new Point(12, 125),
                AutoSize = true
            };
            this.Controls.Add(lblVectorB);

            txtVectorB = new TextBox
            {
                Location = new Point(12, 145),
                Size = new Size(300, 23)
            };
            this.Controls.Add(txtVectorB);

            lblInitialX = new Label
            {
                Text = "Початкове наближення:",
                Location = new Point(12, 180),
                AutoSize = true
            };
            this.Controls.Add(lblInitialX);

            txtInitialX = new TextBox
            {
                Location = new Point(12, 200),
                Size = new Size(300, 23)
            };
            this.Controls.Add(txtInitialX);

            lblEpsilon = new Label
            {
                Text = "Точність:",
                Location = new Point(12, 235),
                AutoSize = true
            };
            this.Controls.Add(lblEpsilon);

            txtEpsilon = new TextBox
            {
                Location = new Point(12, 255),
                Size = new Size(300, 23)
            };
            this.Controls.Add(txtEpsilon);

            btnSolve = new Button
            {
                Text = "Обчислити",
                Location = new Point(12, 290),
                Size = new Size(300, 30)
            };
            btnSolve.Click += BtnSolve_Click;
            this.Controls.Add(btnSolve);

            txtResults = new TextBox
            {
                Location = new Point(12, 340),
                Size = new Size(300, 150),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true
            };
            this.Controls.Add(txtResults);

            lblSolution = new Label
            {
                Text = "Розв'язок:",
                Location = new Point(12, 500),
                AutoSize = true
            };
            this.Controls.Add(lblSolution);

            txtSolution = new TextBox
            {
                Location = new Point(12, 520),
                Size = new Size(300, 23),
                ReadOnly = true
            };
            this.Controls.Add(txtSolution);
        }

        // обробник кнопки 'обчислити'
        private void BtnSolve_Click(object sender, EventArgs e)
        {
            try
            {
                double[,] A = ParseMatrix(txtMatrixA.Text);
                double[] b = ParseVector(txtVectorB.Text);
                double[] x0 = ParseVector(txtInitialX.Text);
                double epsilon = double.Parse(txtEpsilon.Text);

                int n = A.GetLength(0);
                if (A.GetLength(1) != n || b.Length != n || x0.Length != n)
                {
                    throw new ArgumentException("Розмірності матриці або векторів не збігаються.");
                }

                (double[] solution, string iterationResults) = SolveByIteration(A, b, x0, epsilon);

                txtResults.Text = iterationResults;
                txtSolution.Text = string.Join(", ", solution);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // метод множення
        private static double[,] MatrixMultiply(double[,] A, double[,] B)
        {
            int n = A.GetLength(0);
            int m = A.GetLength(1);
            int p = B.GetLength(1);
            double[,] result = new double[n, p];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < p; j++)
                {
                    for (int k = 0; k < m; k++)
                    {
                        result[i, j] += A[i, k] * B[k, j];
                    }
                }
            }
            return result;
        }

        // Функція для транспонування матриці
        private static double[,] TransposeMatrix(double[,] A)
        {
            int n = A.GetLength(0);
            int m = A.GetLength(1);
            double[,] result = new double[m, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    result[j, i] = A[i, j];
                }
            }
            return result;
        }

        // Метод простої ітерації для розв'язку системи лінійних рівнянь
        private static (double[], string) SolveByIteration(double[,] A, double[] b, double[] x0, double epsilon, int maxIterations = 1000)
        {
            int n = A.GetLength(0);
            double[,] AT = TransposeMatrix(A);  // Транспонована матриця A
            double[,] ATA = MatrixMultiply(AT, A);  // A^T * A
            double[] ATb = new double[n];  // A^T * b

            // Обчислення A^T * b
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    ATb[i] += AT[i, j] * b[j];
                }
            }

            // Нормування системи: A^T * A * x = A^T * b
            double[] x = new double[n];
            double[] xPrev = new double[n];
            Array.Copy(x0, xPrev, n);  // Копіюємо початкове значення в xPrev

            double maxDifference;
            int iteration = 0;
            string resultsLog = "Ітерації:\r\n";

            // Ітераційний процес
            do
            {
                iteration++;
                maxDifference = 0.0;

                // Обчислення наступної ітерації за формулою x^n = C * x^(n-1) + d
                for (int i = 0; i < n; i++)
                {
                    x[i] = ATb[i];
                    for (int j = 0; j < n; j++)
                    {
                        if (i != j)
                        {
                            x[i] -= ATA[i, j] * xPrev[j];
                        }
                    }
                    // Ділимо на головний елемент A^T * A
                    if (ATA[i, i] != 0)
                    {
                        x[i] /= ATA[i, i];
                    }
                    else
                    {
                        throw new InvalidOperationException($"Не можна поділити на нуль у A^T * A[{i},{i}].");
                    }

                    // Обчислюємо максимальну різницю між поточною та попередньою ітерацією
                    maxDifference = Math.Max(maxDifference, Math.Abs(x[i] - xPrev[i]));
                }

                // Лог ітерацій
                resultsLog += $"Ітерація {iteration}: x = [{string.Join(", ", x)}]\r\n";

                // Оновлюємо значення xPrev для наступної ітерації
                Array.Copy(x, xPrev, n);

                // Перевірка на максимальну кількість ітерацій
                if (iteration >= maxIterations)
                {
                    resultsLog += "Досягнута максимальна кількість ітерацій. Програма припиняє роботу.\r\n";
                    break;
                }

            } while (maxDifference > epsilon);  // Перевірка на точність

            if (maxDifference <= epsilon)
            {
                resultsLog += $"Збіжність досягнута на ітерації {iteration}.";
            }

            return (x, resultsLog);
        }

        // метод для парсингу матриці
        private double[,] ParseMatrix(string input)
        {
            var rows = input.Trim().Split('\n');
            int n = rows.Length;
            var matrix = new double[n, n];

            for (int i = 0; i < n; i++)
            {
                var elements = rows[i].Trim().Split(' ');
                for (int j = 0; j < n; j++)
                {
                    matrix[i, j] = double.Parse(elements[j]);
                }
            }

            return matrix;
        }

        // метод для парсингу вектора
        private double[] ParseVector(string input)
        {
            var elements = input.Trim().Split(' ');
            double[] vector = new double[elements.Length];
            for (int i = 0; i < elements.Length; i++)
            {
                vector[i] = double.Parse(elements[i]);
            }
            return vector;
        }
    }
}