using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Paranoya
{
    public partial class Form1 : Form
    {
        List<String> lstDebug = new List<String>();
        public static double limiarCanto;
        public static int nroMaxCantos;
        public static double toleranciaCor;
        public static int tamJanelaCor;
        public static int nroMaioresCantos;

        //Função executada quando o programa é inicializado. 
        //Define os valores default para os parametros.
        public Form1()
        {
            InitializeComponent();
            this.IsMdiContainer = true;

            limiarCanto = 0;           //Default 0.
            toleranciaCor = 25;        //10% de 255 = 25
            tamJanelaCor = 3;          //41 Para resultado apenas com cor.
            nroMaxCantos = 500;        //Acima de 1000 = problemas de desempenho;
            nroMaioresCantos = 1;
        }
        //Pega apenas o nome do arquivo. Retirando o C://Arquivos de programas...
        private String pegaNome(String arquivo)
        {
            String[] saida = arquivo.Split('\\');
            return saida[saida.Length-1];            
        }
        //Método responsável por abrir as imagens selecionadas na caixa de dialogo.
        //Para cada imagem, é aberto um form filho.
        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Abrir arquivo.
            openFileDialog1.Multiselect = true;
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                // Le os arquivos selecionados
                foreach (String arquivo in openFileDialog1.FileNames)
                {
                    Image img = Image.FromFile(arquivo);
                    this.criaFormFilho(img, this.pegaNome(arquivo));
                }
            }
        }
        //Botão ajuda do menu.
        private void AjudaFocoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Interface para processamento de imagens desenvolvido por Nikolas Moya - 2012. Clique no botão 'Panoramizar' para processar todas as imagens marcadas com o checkbox.");
        }
        //Converte uma cor em nivel de cinza.
        private int pixel(Color c)
        {
            int alfa, red, green, blue;
            alfa = c.A;
            red = c.R;
            green = c.G;
            blue = c.B;
            return (int)(red + green + blue) / 3;
        }


        //Abre nova imagem. Recebe uma imagem e um título para a janela.
        private void criaFormFilho (Image img, String titulo)
        {
            imagem novaImage = new imagem();
            novaImage.MdiParent = this;
            novaImage.pictureBox1.Image = img;
            novaImage.Text = titulo;
            novaImage.processar.Checked = true;
            novaImage.Show();
        }
        

        //Abre janela de debug. Recebe a string a ser exibida.
        private void Debug(String txt)
        {
            debug novoForm = new debug();
            novoForm.info.Text = txt;
            novoForm.Text = "";
            novoForm.Text = "Debug";
            novoForm.Show();
        }
        //Recebe uma lista de strings e formata com quebra de linha cada elemento da lista
        private void Debug(List<String> txt)
        {
            debug novoForm = new debug();
            novoForm.Text = "";
            foreach(String linha in txt)
                novoForm.info.Text += linha + Environment.NewLine;
            novoForm.Text = "Debug";
            novoForm.Show();
        }


        //Detecção de cantos: Função auxiliar para a detecção de cantos de Forstner.
        void ATrA(int[] gx, int[] gy, int[,] ATA)
        {
            int i, j;
            i = 0;

            for (j = 0; j < 9; j++)
            {
                ATA[i, i] = (gx[j] * gx[j]) + ATA[i, i];
                ATA[i, i + 1] = (gx[j] * gy[j]) + ATA[i, i + 1];
                ATA[i + 1, i] = (gx[j] * gy[j]) + ATA[i + 1, i];
                ATA[i + 1, i + 1] = (gy[j] * gy[j]) + ATA[i + 1, i + 1];
            }
        }
        //Algoritmo de detecção de cantos de Forstner.
        private List<Ponto> deteccaoCantosForstner(Image imag)
        {
            List<Ponto> lstSaida = new List<Ponto>();
            List<Canto> lstTemp = new List<Canto>();
            Bitmap img = new System.Drawing.Bitmap(imag);

            int col, lin, i, j;
            int[,] ATA = new int[2, 2];
            double[,] m = new double[2, 2];
            double[,] ATAI = new double[2, 2];
            double Forstner, e, trace, determinante;

            //Colocar as matrizes do tamanho da imagem:
            float[,] media = new float[img.Width, img.Height];
            int[,] lpGx = new int[img.Width, img.Height];
            int[,] lpGy = new int[img.Width, img.Height];
            int[] Gx = new int[9];
            int[] Gy = new int[9];

            int somaPixelGx, somaPixelGy;
            somaPixelGx = somaPixelGy = 0;

            for (lin = 1; lin < img.Width - 1; lin++)
            {
                for (col = 1; col < img.Height - 1; col++)
                {
                    //soma Gx
                    somaPixelGx += this.pixel(img.GetPixel(lin - 1, col - 1)) * -1;
                    somaPixelGx += this.pixel(img.GetPixel(lin, col - 1)) * -1;
                    somaPixelGx += this.pixel(img.GetPixel(lin + 1, col - 1)) * -1;

                    somaPixelGx += this.pixel(img.GetPixel(lin - 1, col + 1)) * +1;
                    somaPixelGx += this.pixel(img.GetPixel(lin, col + 1)) * +1;
                    somaPixelGx += this.pixel(img.GetPixel(lin + 1, col + 1)) * +1;

                    //soma Gy
                    somaPixelGy += this.pixel(img.GetPixel(lin - 1, col - 1)) * +1;
                    somaPixelGy += this.pixel(img.GetPixel(lin + 1, col - 1)) * -1;

                    somaPixelGy += this.pixel(img.GetPixel(lin - 1, col)) * +1;
                    somaPixelGy += this.pixel(img.GetPixel(lin + 1, col)) * -1;

                    somaPixelGy += this.pixel(img.GetPixel(lin - 1, col + 1)) * +1;
                    somaPixelGy += this.pixel(img.GetPixel(lin + 1, col + 1)) * -1;

                    //carrega valores nas matrizes e limpa os acumuladores
                    //foi  tirado tudo que era multiplicado por zero para economizar processamento
                    lpGx[lin, col] = somaPixelGx;
                    lpGy[lin, col] = somaPixelGy;
                    somaPixelGx = somaPixelGy = 0;
                }
            }

            //Carega os valores de uma janela 3x3 de Gx e Gy
            for (lin = 1; lin < img.Width - 1; lin++)
            {
                for (col = 1; col < img.Height - 1; col++)
                {
                    Gx[0] = lpGx[lin - 1, col - 1];
                    Gx[1] = lpGx[lin, col - 1];
                    Gx[2] = lpGx[lin + 1, col + 1];
                    Gx[3] = lpGx[lin - 1, col];
                    Gx[4] = lpGx[lin, col];
                    Gx[5] = lpGx[lin + 1, col];
                    Gx[6] = lpGx[lin - 1, col + 1];
                    Gx[7] = lpGx[lin, col + 1];
                    Gx[8] = lpGx[lin + 1, col + 1];

                    Gy[0] = lpGy[lin - 1, col - 1];
                    Gy[1] = lpGy[lin, col - 1];
                    Gy[2] = lpGy[lin + 1, col + 1];
                    Gy[3] = lpGy[lin - 1, col];
                    Gy[4] = lpGy[lin, col];
                    Gy[5] = lpGy[lin + 1, col];
                    Gy[6] = lpGy[lin - 1, col + 1];
                    Gy[7] = lpGy[lin, col + 1];
                    Gy[8] = lpGy[lin + 1, col + 1];

                    ATrA(Gx, Gy, ATA);
                    determinante = (ATA[0, 0] * ATA[1, 1]) - (ATA[0, 1] * ATA[1, 0]);
                    for (int n = 0; n < 2; n++)
                    {
                        for (int o = 0; o < 2; o++)
                        {
                            ATAI[n, o] = ATA[n, o];
                        }
                    }

                    if (determinante != 0) //determinante tem que ser diferente de 0, senão não tem inversa
                    {
                        for (i = 0; i < 2; i++)
                        {
                            for (j = 0; j < 2; j++)
                            {
                                m[j, i] = Math.Pow(-1.0, ((i + 1) + (j + 1))) * (ATAI[1 - i, 1 - j]); // matriz dos cofatores
                            }
                        }

                        for (int k = 0; k < 2; k++)
                        {
                            for (int l = 0; l < 2; l++)
                            {
                                ATAI[k, l] = (1.0 / determinante) * m[k, l];//Matriz inversa
                            }
                        }

                    }

                    if (determinante != 0) // só calcula se o determinate for diferente de 0
                    {
                        e = 0.00001;
                        trace = ATAI[0, 0] + ATAI[1, 1];
                        Forstner = (1 / (trace + e));
                    }
                    else
                    {
                        Forstner = 0; //caso for determinate for 0, não faz nada na imagem
                    }
                    media[lin, col] = (float)Forstner;


                    for (i = 0; i < 2; i++)
                    {
                        for (j = 0; j < 2; j++)
                        {
                            ATA[i, j] = 0;
                        }
                    }
                }
            }
            for (lin = 1; lin < img.Width; lin++)
            {
                for (col = 1; col < img.Height; col++)
                {
                    if (media[lin, col] < limiarCanto)
                    //if (media[lin, col] < 0)
                    {   
                        lstTemp.Add(new Canto(new Ponto(lin, col), media[lin,col]));
                    }

                    //img.SetPixel(lin, col, Color.White);
                }
            }
            lstTemp.Sort();
            //Pega os N cantos definidos pela janela de parametros, ou o nro de cantos que vieram, se for menos que o definido.
            for (i = 0; i < lstTemp.Count && i < nroMaxCantos; i++) 
            {
                lstSaida.Add(lstTemp[i].p1);
                //lstDebug.Add(lstTemp[i].toTexto());
            }
            //lstDebug.Add("Nro Cantos: " + lstSaida.Count.ToString());
            //this.Debug(lstDebug);
            return lstSaida;
        }

        //Dada uma imagem, um pixel incial e um tamanho de janela, faça uma operação
        private double calculaJanela(Bitmap img1, Ponto i, int tamJanela, String operacao)
        {
            int lin, col = 0;
            double saida = -1;
            try
            {
                for (lin = i.X - (tamJanela / 2); lin <= i.X + (tamJanela / 2); lin++)
                {
                    for (col = i.Y - (tamJanela / 2); col <= i.Y + (tamJanela / 2); col++)
                    {
                        if (operacao.Equals("quadrado"))
                        {
                            saida += this.pixel(img1.GetPixel(lin, col)) * this.pixel(img1.GetPixel(lin, col));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return -1;
            }
            return saida;
        }
        //Recebe 2 imagens, dois pontos, um tamanho de janela, e faz a operação indicada. Os pontos viram os centros das janelas.
        private double calculaJanela(Bitmap img1, Bitmap img2, Ponto i, Ponto j, int tamJanela, String operacao/*soma, subtracao, multiplicacao*/)
        {
            int lin, col = 0;
            int[] vet1 = new int[tamJanela * tamJanela];
            int[] vet2 = new int[tamJanela * tamJanela];
            int cont = 0;
            double saida = -1;
            try
            {
                for (lin = i.X - (tamJanela / 2); lin <= i.X + (tamJanela / 2); lin++)
                {
                    for (col = i.Y - (tamJanela / 2); col <= i.Y + (tamJanela / 2); col++)
                    {
                        vet1[cont] = this.pixel(img1.GetPixel(lin, col));
                        cont++;
                    }
                }
            }
            catch (Exception e)
            {
                return -1;
            }
            cont = 0;
            try
            {
                for (lin = j.X - (tamJanela / 2); lin <= j.X + (tamJanela / 2); lin++)
                {
                    for (col = j.Y - (tamJanela / 2); col <= j.Y + (tamJanela / 2); col++)
                    {
                        vet2[cont] = this.pixel(img2.GetPixel(lin, col));
                        cont++;
                    }
                }
            }
            catch (Exception e)
            {
                return -1;
            }
            for (int w = 0; w < (tamJanela * tamJanela); w++)
            {
                if (operacao.Equals("soma"))
                {
                    saida += vet1[w] + vet2[w];
                }
                if (operacao.Equals("subtracao"))
                {
                    saida += vet1[w] - vet2[w];
                }
                if (operacao.Equals("multiplicacao"))
                {
                    saida += vet1[w] * vet2[w];
                }
                if (operacao.Equals("subtracao ao quadrado"))
                {
                    saida += (vet1[w] - vet2[w]) * (vet1[w] - vet2[w]);
                }
            }
            return saida;
        }

        //Calculo do W, para o algoritmo de casamento de pontos.
        private double calculaW(Bitmap img1, Bitmap img2, Ponto i, Ponto j)
        {
            double c = 0.00001;
            //double c = 10;
            double resultado = 0;
            resultado = 1 / ( 1 + c * calculaJanela(img1, img2, i, j, 11, "subtracao ao quadrado"));
            //lstDebug.Add(resultado.ToString());
            return (double) Math.Abs(resultado);
        }
        //Calcula a distância entre 2 pixels. Dada uma tolerância configurada na janela de parâmetros
        private bool distanciaCor(Bitmap img1, Bitmap img2, Ponto p1, Ponto p2)
        {
            Color c1 = img1.GetPixel(p1.X, p1.Y);
            Color c2 = img2.GetPixel(p2.X, p2.Y);
            double tolerancia = 255 * 0.10;
            if (Math.Abs((c1.R - c2.R)) > tolerancia)
                return false;
            if (Math.Abs((c1.G - c2.G)) > tolerancia)
                return false;
            if (Math.Abs((c1.B - c2.B)) > tolerancia)
                return false;
            return true;
        }
        private bool distanciaCor(Color c1, Color c2)
        {
            //double tolerancia = 255 * 0.01;
            double tolerancia = toleranciaCor;
            if (Math.Abs((c1.R - c2.R)) > tolerancia)
                return false;
            if (Math.Abs((c1.G - c2.G)) > tolerancia)
                return false;
            if (Math.Abs((c1.B - c2.B)) > tolerancia)
                return false;
            return true;
        }
        //Dada duas janelas, calcula pixel a pixel, se a distancia entre cor deles é inferior a tolerância.
        //Votação Unanime. Ou seja, todos os pixels da janela devem estar abaixo da tolerância.
        private bool calculaJanelaCor(Bitmap img1, Bitmap img2, Ponto i, Ponto j, int tamJanela)
        {
            int lin, col = 0;
            Color[] vet1 = new Color[tamJanela * tamJanela];
            Color[] vet2 = new Color[tamJanela * tamJanela];
            int cont = 0;
            double saida = -1;
            try
            {
                for (lin = i.X - (tamJanela / 2); lin <= i.X + (tamJanela / 2); lin++)
                {
                    for (col = i.Y - (tamJanela / 2); col <= i.Y + (tamJanela / 2); col++)
                    {
                        vet1[cont] = img1.GetPixel(lin, col);
                        cont++;
                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }
            cont = 0;
            try
            {
                for (lin = j.X - (tamJanela / 2); lin <= j.X + (tamJanela / 2); lin++)
                {
                    for (col = j.Y - (tamJanela / 2); col <= j.Y + (tamJanela / 2); col++)
                    {
                        vet2[cont] = img2.GetPixel(lin, col);
                        cont++;
                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }
            int contProxima = 0;
            for (int w = 0; w < (tamJanela * tamJanela); w++)
            {
                bool proxima = distanciaCor(vet1[w], vet2[w]);

                if (!proxima)
                    return false;
            }
            return true;

        }

        //Principal função. Recebe 2 listas de cantos, retorna uma lista de casamento. Os cantos da lista 1, com os cantos da lista 2.
        private List<Conexao> casamentoDoutorado(Bitmap img1, Bitmap img2, List<Ponto> lstCantos1, List<Ponto> lstCantos2)
        {
            //Variáveis
            List<List<double>> lstProbabilidades = new List<List<double>>();
            List<List<double>> lstDistancias = new List<List<double>>();
            List<Conexao> lstConexoes = new List<Conexao>();
            int i, j, k, w;
            
            int NROITERACOES = 10;
            double alfa = 0.3;
            double beta = 3;

            double maior = 0;
            int ponto = 0;

            //Processamento
            for (j = 0; j < lstCantos1.Count; j++)   //Para todos os cantos da imagem 1:
            {
                //Resetar variaveis:
                double denominador = 0;
                double maiorWd = 0;
                List<double> lstProbTemp = new List<double>();
                List<double> lstDistTemp = new List<double>();
                Ponto p1 = new Ponto(lstCantos1[j].X, lstCantos1[j].Y);     //Fixa o primeiro ponto.
                //Processamento:
                //Pega o maior WD e faz a soma dos Wd's:
                for (w = 0; w < lstCantos2.Count; w++)  //Soma de todos os d'. Ou seja, wd() para todos os cantos da imagem 2.
                {
                    //Restar variaveis:
                    Ponto p3 = new Ponto(lstCantos2[w].X, lstCantos2[w].Y);
                    double wdlinha = calculaW(img1, img2, p1, p3);

                    //Processamento:
                    if (wdlinha > maiorWd)
                        maiorWd = wdlinha;
                    denominador += wdlinha;
                }
                //Para todos os cantos da lista 2
                for (k = 0; k < lstCantos2.Count; k++)   
                {
                    //Resetar variaveis:
                    Ponto p2 = new Ponto(lstCantos2[k].X, lstCantos2[k].Y);


                    //Calcula o vetor de deslocamento entre p1 e p2.
                    double absoluto = Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y);
                    lstDistTemp.Add((double)absoluto);

                    if (calculaJanelaCor(img1, img2, p1, p2, tamJanelaCor))   //Se as cores forem próximas:
                    //if (distanciaCor(img1.GetPixel(p1.X, p1.Y), img2.GetPixel(p2.X, p2.Y)))
                    {
                        double numerador = calculaW(img1, img2, p1, p2);
                        double pd = 0;

                        //Calculo da probabiliade de p1 com a soma de todos os wd dos cantos da imagem 2, calculado antes deste for:
                        pd = Math.Abs((double)(numerador / denominador) * maiorWd);
                        lstProbTemp.Add((double)pd);
                    }
                    else //Se as cores não forem próximas, coloca probabilidade 0.
                    {
                        lstProbTemp.Add(0);
                    }

                    
                }
                lstProbabilidades.Add(lstProbTemp);
                lstDistancias.Add(lstDistTemp);
            }
            //Final da inicialização
            /*  
             * 
             *  Resultado Inicialização:
             *  lstProbabilidades[0][0] ==> Probabilidade do primeiro canto da imagem1, se casar com o primeiro canto da imagem 2.
             *  lstProbabilidades[4][9] ==> Probabilidade do quarto canto da imagem1, se casar com o nono canto da imagem 2.
             *  [...]
             *  
             *  Matriz de deslocamento
             *  lstDistancias[0][0] ==> Vetor de deslocamento entre o primeiro canto da imagem 1, com o primeiro canto da imagem 2.
             *  lstDistancias[4][9] ==> Vetor de deslocamento entre o quarto canto da imagem 1, com o nono canto da imagem 2.
             *  [...]
             *  
             */
            //Atualização das probabilidades:
            for (w = 0; w < NROITERACOES; w++)      //Condição de parada
            {
                for (int linha = 0; linha < lstCantos1.Count; linha++)  //Para cada canto da imagem 1.
                {
                    List<double> ListaProbPixel1 = new List<double>();
                    List<double> ListaDistanciaPixel1 = new List<double>();
                    ListaProbPixel1 = lstProbabilidades[linha];
                    ListaDistanciaPixel1 = lstDistancias[linha];

                    for (i = 0; i < ListaProbPixel1.Count; i++)
                    {
                        if (ListaProbPixel1[i] > 0.05)
                        {
                            for (int vizinho = 0; vizinho < lstCantos1.Count; vizinho++)    //Para cada vizinho do canto 1
                            {
                                List<double> linhaProbVizinho = new List<double>();
                                List<double> linhaDistanciaVizinho = new List<double>();

                                linhaProbVizinho = lstProbabilidades[vizinho];
                                linhaDistanciaVizinho = lstProbabilidades[vizinho];

                                double somaProb = 0;
                                double somaTodasProbabilidades = 0;
                                for (int coluna = 0; coluna < linhaDistanciaVizinho.Count; coluna++)  //Para cada canto da imagem 2
                                {
                                    if (Math.Abs(ListaDistanciaPixel1[i] - linhaDistanciaVizinho[coluna]) < 1)  //Se o vizinho e o canto da segunda imagem, forem próximos.
                                    {
                                        somaProb += linhaProbVizinho[coluna];
                                    }
                                    somaTodasProbabilidades += linhaProbVizinho[coluna];
                                }
                                ListaProbPixel1[i] = (ListaProbPixel1[i] * (alfa + beta * somaProb)) / somaTodasProbabilidades; //Incrementa a probabilidade do canto inicial.
                            }
                        }
                    }
                    lstProbabilidades[linha] = ListaProbPixel1;
                }
            }

            //Para cada canto da imagem 1, pega o casamento com um canto da imagem 2 com maior probabilidade e define como sendo o casamento.
            //Pegar a maior e fazer o casamento
            for (i = 0; i < lstCantos1.Count; i++)
            {
                int entrou = 0;
                maior = lstProbabilidades[i][0];
                for (j = 0; j < lstCantos2.Count; j++)
                {
                    if (lstProbabilidades[i][j] > maior && lstProbabilidades[i][j] > 0.007)
                    {
                        entrou = 1;
                        maior = lstProbabilidades[i][j];
                        ponto = j;
                    }
                }
                if (entrou == 1)
                {
                    lstDebug.Add(lstCantos1[i].texto() + " casa com " + lstCantos2[ponto].texto() + " Probabilidade: " + (maior*10000).ToString());
                    lstConexoes.Add(new Conexao(lstCantos1[i], lstCantos2[ponto], maior));
                }
            }
fim:
            //this.Debug(lstDebug);
            //Retorna a lista de conexões.
            return lstConexoes;
        }






        //Simplismente coloca a imagem 2 ao término da imagem 1.
        private Bitmap concatenaImagem (Bitmap img1, Bitmap img2)
        {
            Bitmap imgSaida = new Bitmap(img1.Width + img2.Width, img1.Height);
            //Debug(imgSaida.Width.ToString()+ imgSaida.Height.ToString());
            int lin, col = 0;
            //Copia a primeira imagem para a imagem maior.
            for (lin = 0; lin < img1.Width; lin++)
            {
                for (col = 0; col < img1.Height; col++)
                {
                    imgSaida.SetPixel(lin, col, img1.GetPixel(lin, col));
                }
            }
            lin = 0;
            col = 0;
            //Copia a segunda imagem para a imagem maior.
            for (lin = 0; lin < img2.Width; lin++)
            {
                for (col = 0; col < img2.Height; col++)
                {
                    imgSaida.SetPixel(lin + img1.Width, col, img2.GetPixel(lin, col));
                }
            }
            return imgSaida;
        }
       
        //Coloca a imagem 2 sob a imagem 1, dada uma posição de sobreposição
        private Bitmap sobrepoeImagem(Bitmap img1, Bitmap img2, int posYnaImg1)
        {
            Bitmap imgSaida = new Bitmap(posYnaImg1 + img2.Width, img1.Height);
            int lin, col = 0;
            //Copia a primeira imagem para a imagem maior.
            for (lin = 0; lin < posYnaImg1; lin++)
            {
                for (col = 0; col < img1.Height; col++)
                {
                    imgSaida.SetPixel(lin, col, img1.GetPixel(lin, col));
                }
            }
            lin = 0;
            col = 0;
            //Copia a segunda imagem para a imagem maior.
            for (lin = 0; lin < img2.Width; lin++)
            {
                for (col = 0; col < img2.Height; col++)
                {
                    imgSaida.SetPixel(lin + posYnaImg1, col, img2.GetPixel(lin, col));
                }
            }
            return imgSaida;
        }
        //Cria uma imagem com a soma de todas as imagens a serem panaromizadas e inicializa com a primeira imagem.
        private Bitmap initPanoramica(Bitmap img1, int comprimentoPanoramica, int alturaPanoramica)
        {
            Bitmap imgSaida = new Bitmap(comprimentoPanoramica, alturaPanoramica);
            int lin, col = 0;
            //Copia a primeira imagem para a imagem maior.
            for (lin = 0; lin < img1.Width; lin++)
            {
                for (col = 0; col < img1.Height; col++)
                {
                    imgSaida.SetPixel(lin, col, img1.GetPixel(lin, col));
                }
            }
            return imgSaida;
        }
        /**Botão da principal funcionalidade do software.
         * Para todas as janelas abertas, apenas as janelas que estiverem marcadas para serem processadas,
         * os cantos são detectados. Se mais um de uma janela for processada, além de apenas detectar os cantos,
         * é feita um cálculo de similaridade entre os cantos detectados. 
         * O casamento retorna uma lista de conexões. As melhores conexões determinam a posição que uma imagem deve
         * sobrepor a outra.
         * Se a opção de debug estiver marcada, uma nova imagem é gerada com os casamentos. E os melhores casamentos
         * ficam destacados em amarelo.
         * **/
        private void panoramizarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<List<Ponto>> listaCantos = new List<List<Ponto>>();
            List<imagem> lstForms = new List<imagem>();
            int i = 0;
            int flag = 0;
            int alturaComum = 0;
            foreach (imagem filho in this.MdiChildren)       //Para cada form aberto
            {
                if (filho.processar.Checked)    //Se estiver marcado para processar.
                {
                    if (flag == 0)
                    {
                        flag = 1;
                        alturaComum = filho.pictureBox1.Image.Height;
                    }
                    if (alturaComum != filho.pictureBox1.Image.Height)
                    {
                        MessageBox.Show("As imagens tem altura distintas");
                        return;
                    }
                    //Deteccao cantos Fortstner
                    List<Ponto> cantos = this.deteccaoCantosForstner(filho.pictureBox1.Image);
                    Bitmap saida = new Bitmap(filho.pictureBox1.Image);
                    foreach (Ponto ponto in cantos)
                    {
                        saida.SetPixel(ponto.X, ponto.Y, Color.White);
                    }
                    if (this.checkBox1.Checked)
                        this.criaFormFilho(saida, "Forstner: " + filho.Text); //Desenha a detecção de cantos das imagens
                    listaCantos.Add(cantos);        //Lista contendo uma lista de cantos detectados para cada imagem.
                    lstForms.Add(filho);            //Uma lista de imagens abertas e marcadas para serem processadas.
                }
            }

            Application.DoEvents();
            if (lstForms.Count >= 2)        //Se tem 2 ou mais fotos abertas, além de Forstner, execute a similaridade.
            {
                //Lembrando que as imagens devem ter a mesma altura. 
                //E serão concatenadas na ordem em que foram abertas.
                Bitmap imgPanoramica = new Bitmap(lstForms[0].pictureBox1.Image);
                List<Conexao> lstConexoes = new List<Conexao>();
                List<List<Conexao>> lst_de_lst_deMaiores = new List<List<Conexao>>();
                List<Conexao> lstMaiores = new List<Conexao>();
                List<List<Conexao>> matConexao = new List<List<Conexao>>();
                int NROMAIORES = nroMaioresCantos;
                Conexao maiorCox = null;

                List<Int32> lstSobreposicao = new List<Int32>();
                int somaPosX = 0;
                for (i = 0; i < lstForms.Count-1 ; i++)   //Para cada imagem:   (A primeira com a segunda, a segunda com a terceira, [...])
                {
                    //Pega a imagem dos forms
                    Bitmap img1 = new System.Drawing.Bitmap(lstForms[i].pictureBox1.Image);
                    Bitmap img2 = new System.Drawing.Bitmap(lstForms[i+1].pictureBox1.Image);

                    List<Ponto> lstCantos1 = new List<Ponto>();
                    lstCantos1 = listaCantos[i];

                    List<Ponto> lstCantos2 = new List<Ponto>();
                    lstCantos2 = listaCantos[i+1];

                    lstConexoes = this.casamentoDoutorado(img1, img2, lstCantos1, lstCantos2);
                    if (lstConexoes.Count == 0)
                    {
                        MessageBox.Show("Não foi possível casar estas imagens");
                        return;
                    }
                    
                    matConexao.Add(lstConexoes);

                    lstMaiores = new List<Conexao>();
                    for (int j = 0; j < (lstConexoes.Count); j += 1)
                        lstMaiores.Add(lstConexoes[j]);

                    lstMaiores.Sort(delegate(Conexao c1, Conexao c2) { return c1.ratio.CompareTo(c2.ratio); });
                    int posX = Math.Abs(lstMaiores[lstMaiores.Count - 1].p1.X - lstMaiores[lstMaiores.Count - 1].p2.X);
                    lst_de_lst_deMaiores.Add(lstMaiores);

                    int tempSoma = 0;
                    int tempCont = 0;
                    for (int j = lstMaiores.Count - 1; j>=0 && (j > (lstMaiores.Count - 1)-NROMAIORES) ; j--)
                    {
                        tempSoma += Math.Abs(lstMaiores[j].p1.X - lstMaiores[j].p2.X);
                        tempCont += 1;
                    }
                    if (tempCont == NROMAIORES)
                        posX = tempSoma / NROMAIORES;
                    else
                        posX = tempSoma / tempCont;
                    somaPosX += posX;
                    lstSobreposicao.Add(somaPosX);
                }

                int comprimentoPanoramica = somaPosX + lstForms[lstForms.Count - 1].pictureBox1.Image.Width;
                int alturaPanoramica = lstForms[0].pictureBox1.Image.Height;
                Bitmap primeira = new System.Drawing.Bitmap(lstForms[0].pictureBox1.Image);
                imgPanoramica = initPanoramica(primeira, comprimentoPanoramica, alturaPanoramica);
                for (i = 1; i <= lstForms.Count - 1; i++)   
                {
                    Bitmap img1 = new System.Drawing.Bitmap(lstForms[i].pictureBox1.Image);
                    imgPanoramica = this.sobrepoeImagem(imgPanoramica, img1, lstSobreposicao[i-1]);
                }
                if (this.checkBox1.Checked)
                {
                    for (i = 0; i < lstForms.Count - 1; i++)
                    {
                        Bitmap img1 = new System.Drawing.Bitmap(lstForms[i].pictureBox1.Image);
                        Bitmap img2 = new System.Drawing.Bitmap(lstForms[i + 1].pictureBox1.Image);
                        Bitmap imgCasamento = new System.Drawing.Bitmap(img1.Width + img2.Width, img1.Height);
                        imgCasamento = this.concatenaImagem(img1, img2);

                        int comprimentoComum = img1.Width;
                        Graphics artist = Graphics.FromImage(imgCasamento);
                        Pen Caneta;
                        maiorCox = new Conexao(matConexao[i][0].p1, matConexao[i][0].p2, matConexao[i][0].ratio);
                        for (int j = 0; j < matConexao[i].Count; j++)
                        {
                            if (matConexao[i][j].ratio > maiorCox.ratio)
                            {
                                maiorCox = matConexao[i][j];
                            }
                            Caneta = new Pen(Color.Magenta, 1);
                            artist.DrawLine(Caneta
                                , matConexao[i][j].p1.X
                                , matConexao[i][j].p1.Y
                                , matConexao[i][j].p2.X + comprimentoComum
                                , matConexao[i][j].p2.Y);
                        }
                        lstMaiores = lst_de_lst_deMaiores[i];
                        for (int j = lstMaiores.Count - 1; j >= 0 && (j > (lstMaiores.Count - 1) - NROMAIORES); j--)
                        {
                            maiorCox = lstMaiores[j];
                            Caneta = new Pen(Color.Gold, 3);
                            artist.DrawLine(Caneta
                                    , maiorCox.p1.X
                                    , maiorCox.p1.Y
                                    , maiorCox.p2.X + comprimentoComum
                                    , maiorCox.p2.Y);
                        }
                        
                        criaFormFilho(imgCasamento, "Casamento " + i.ToString() + " com " + (i + 1).ToString());
                    }
                }
                criaFormFilho(imgPanoramica, "Imagens Panoramicas");
            }
        }

        //Evento do botão 'Parametros'
        private void parametrosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            parametros config = new parametros();
            config.limiarCantos.Text = limiarCanto.ToString();
            config.maxCantos.Text = nroMaxCantos.ToString();
            config.toleranciaCor.Text = toleranciaCor.ToString();
            config.janelaCor.Text = tamJanelaCor.ToString();
            config.nroMaioresCantos.Text = nroMaioresCantos.ToString();

            config.Text = "Parametros";
            config.Show();
        }

        //Função para salvar a imagem selecionada
        private void salvarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.saveFileDialog1.Filter = "JPEG Image|*.jpg|Bitmap Image|*.bmp";
            this.saveFileDialog1.Title = "Salve a imagem panoramica";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                imagem ativo = (imagem) this.ActiveMdiChild;
                if (ativo != null)
                {
                    ativo.pictureBox1.Image.Save(this.saveFileDialog1.FileName);
                }
            }
        }

    }
    //Classe que define um ponto.
    public class Ponto
    {
        // Field
        public int X;
        public int Y;

        // Constructor
        public Ponto(int x, int y)
        {
            X = x;
            Y = y;
        }
        // Method
        public int getX()
        {
            return this.X;
        }
        public int getY()
        {
            return this.Y;
        }
        public String toString()
        {
            return this.X.ToString() + ", " + this.Y.ToString();
        }
        public String texto()
        {
            return this.X.ToString() + ", " + this.Y.ToString();
        }
    }
    public class Canto : IComparable<Canto>
    {
        public Ponto p1;
        public double ratio;

        public Canto(Ponto ponto1, double taxa)
        {
            p1 = ponto1;
            ratio = taxa;
        }
        public String toTexto()
        {
            return this.p1.texto() + " com tx. Forstner de: " + this.ratio.ToString();
        }
        public int CompareTo(Canto other)
        {
            return this.ratio.CompareTo(other.ratio);
        }
    }
    //Classe que define uma conexão entre 2 pontoes e uma taxa para esta conexao.
    public class Conexao
    {
        // Field
        public Ponto p1;
        public Ponto p2;
        public double ratio;
        // Constructor
        public Conexao(Ponto ponto1, Ponto ponto2, double taxa)
        {
            p1 = ponto1;
            p2 = ponto2;
            ratio = taxa;
        }
        // Method
        public Ponto getP1()
        {
            return this.p1;
        }
        public Ponto getP2()
        {
            return this.p2;
        }
        public double getRatio()
        {
            return this.ratio;
        }
        public String toString()
        {
            return this.p1.texto()+ " casa com " + this.p2.texto() + " com " + this.ratio.ToString() + " de certeza"; 
        }
    }
}

/*
 *EXEMPLOS DE CÓDIGOS:
 *Converter de Image para bitmap:
 *                    Bitmap carregada = new System.Drawing.Bitmap(filho.pictureBox1.Image);
 * 
 * Desenhar linhas sobre uma imagem:
 * Graphics artist = Graphics.FromImage(imgSaida);
 * artist.DrawLine(new Pen(Color.Black, 1)
                            , lstConexoes[i + 1].X
                            , lstConexoes[i + 1].Y
                            , lstConexoes[i + 2].X+300
                            , lstConexoes[i + 2].Y);
 * artist.Dispose();
 * Criar imagem bitmap do zero

                    Bitmap img = new System.Drawing.Bitmap(800, 600, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    //Converte de Image to bitmap;
 
                    for (int x = 0; x < img.Width; x++)
                    {
                        for (int y = 0; y < img.Height; y++)
                        {
                            if (x % 2 == 0)
                                img.SetPixel(x, y, Color.Black);
                            else
                                img.SetPixel(x, y, Color.White);
                        }
                    }
                    this.criaFormFilho(img, "RGB 24 bits");
 * 
 *             /*for (j = 0; j < lstCantos1.Count; j++)   //Pega um pixel da lista 1
            {
                x = lstCantos1[j].X;
                y = lstCantos1[j].Y;
                pixel1 = this.pixel(img1.GetPixel(x, y));
                maior = -1;
                k = 0;
                //while (k < lstCantos2.Count)
                for (k = 0; k < lstCantos2.Count; k++)    //Para todos os pixels da lista 2
                {
                    x = lstCantos2[k].X;
                    y = lstCantos2[k].Y;
                    pixel2 = this.pixel(img2.GetPixel(x, y));
                    similaridade = (double)((pixel1 * pixel2) * 1.0 / Math.Sqrt(Math.Pow(pixel1, 2) * Math.Pow(pixel2, 2))) * 1.0;
                    if (similaridade > maior)
                    {
                        maior = similaridade;
                        posPixel = lstCantos2[k].ToString();
                    }
                    similaridade = 0;
                }
                lstDebug.Add(lstCantos1[j] + " casa com " + posPixel + " Similaridade: " + maior.ToString());
            }
 * 
 *  //Similaridade entre os cantos detectados.
        private List<Conexao> similaridadeC1(Bitmap img1, Bitmap img2, List<Ponto> lstCantos1, List<Ponto> lstCantos2)
        {
            List<String> lstDebug = new List<String>();
            List<Conexao> lstConexoes = new List<Conexao>();

            Ponto maiorPonto = new Ponto(-1, -1);

            double maior = -1;
            double similaridade = 0;
            String posPixel = "";
            double somaProduto, quadradoI, quadradoJ = 0;

            lstDebug.Add("Tam lista 1: " + lstCantos1.Count);
            lstDebug.Add("Tam lista 2: " + lstCantos2.Count);
            lstDebug.Add("1ro pixel lista 1: " + lstCantos1[0].texto());
            lstDebug.Add("1ro pixel lista 2: " + lstCantos2[0].texto());

            int j, k;

            for (j = 0; j < lstCantos1.Count; j++)   //Pega um pixel da lista 1
            {
                Ponto p1 = new Ponto(lstCantos1[j].X, lstCantos1[j].Y);     //Fixa o primeiro ponto.
                maior = -1;
                for (k = 0; k < lstCantos2.Count; k++)    //Para todos os pixels da lista 2
                {
                    Ponto p2 = new Ponto(lstCantos2[k].X, lstCantos2[k].Y);
                    somaProduto = calculaJanela(img1, img2, p1, p2, 5, "multiplicacao");
                    quadradoI = calculaJanela(img1, p1, 5, "quadrado");
                    quadradoJ = calculaJanela(img2, p2, 5, "quadrado");
                    similaridade = (double)(somaProduto * 1.0) / Math.Sqrt((quadradoI * quadradoJ)) * 1.0;
                    if (similaridade > maior)
                    {
                        maior = similaridade;
                        maiorPonto = new Ponto(lstCantos2[k].X, lstCantos2[k].Y);
                    }
                }
                lstDebug.Add(lstCantos1[j].texto() + " casa com " + maiorPonto.texto() + " Similaridade: " + maior.ToString());
                lstConexoes.Add(new Conexao(p1, maiorPonto, maior));
            }
            this.Debug(lstDebug);
            return lstConexoes;
        }
 * 
 * 
 * VETOR DE DESLOCAMENTO
 *             /*for (i = 0; i < lstCantos1.Count; i++)  //Para todos os cantos da imagem 1:
            {
                List<double> lstDistTemp = new List<double>();
                Ponto p1 = new Ponto(lstCantos1[i].X, lstCantos1[i].Y); //Pega o primeiro canto

                for (j = 0; j < lstCantos2.Count; j++)
                {
                    Ponto p2 = new Ponto(lstCantos2[j].X, lstCantos2[j].Y);

                    double absoluto = Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y);
                    lstDistTemp.Add((double)absoluto);
                }
                lstDistancias.Add(lstDistTemp);
            }
 * 
 * for (w = 0; w < NROITERACOES; w++)
            {
                for (int cantoAtualImg2 = 0; cantoAtualImg2 < lstCantos2.Count; cantoAtualImg2++)
                {
                    for (i = 0; i < lstCantos1.Count; i++)  //Para um canto i da imagem 1
                    {
                        for (j = 0; j < lstCantos1.Count; j++)  //Para todos os vizinhos j do canto i da imagem 1.
                        {
                            double somaProbVizinho = 0;
                            for (k = 0; k < lstCantos2.Count; k++)  //Para um unico canto k da imagem 2.
                            {
                                double diferenca = 0;
                                diferenca = Math.Abs(lstDistancias[i][cantoAtualImg2] - lstDistancias[j][k]);  //A diferença absoluta entre o canto i da img1, com o canto j da img1, em relação ao canto k da img2.
                                if (diferenca < 1)
                                {
                                    somaProbVizinho += lstProbabilidades[j][k]; //Soma a probabilidade de que o vizinho de p1, casaria com o ponto 2, na probabilidade de p1 casar com o ponto 2.
                                }
                            }
                            //A probabilidade k+1 para o canto 1 é igual a probabilidade K vezes a soma probabilidade dos meus vizinho que tem um deslocamento muito proximo à imagem 2.
                            //lstDebug.Add("ProbAntes: " + lstProbabilidades[i][cantoAtualImg2].ToString());
                            lstProbabilidades[i][cantoAtualImg2] = (lstProbabilidades[i][cantoAtualImg2] + somaProbVizinho) / 10000000;
                            //lstDebug.Add("ProbDepois: " + lstProbabilidades[i][cantoAtualImg2].ToString());
                        }
                    }
                }
            }
 * 
 * 
 * 
        /*private List<Conexao> casamentoCor(Bitmap img1, Bitmap img2, List<Ponto> lstCantos1, List<Ponto> lstCantos2)
        {
            //Variáveis
            List<Conexao> lstConexoes = new List<Conexao>();
            int j, k;
            int divisor = 8;

            //Processamento
            for (j = 0; j < lstCantos1.Count / divisor; j++)   //Para todos os cantos da imagem 1:
            {
                Ponto p1 = new Ponto(lstCantos1[j].X, lstCantos1[j].Y);     //Fixa o primeiro ponto.

                for (k = 0; k < lstCantos2.Count / divisor; k++) //Para todos os cantos da imagem 2
                {
                    //Resetar variaveis:
                    Ponto p2 = new Ponto(lstCantos2[k].X, lstCantos2[k].Y);
                    if (calculaJanelaCor(img1, img2, p1, p2, 3))   //Se as cores forem próximas:
                    {
                        lstConexoes.Add(new Conexao(lstCantos1[j], lstCantos2[k], 10));
                        break;
                    }
                }

            }


            return lstConexoes;
        }
        */
 