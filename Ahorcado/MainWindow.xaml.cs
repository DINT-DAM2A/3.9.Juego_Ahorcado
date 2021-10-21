using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Ahorcado
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int countError = 0;
        int maxError = 6
            ;
        public MainWindow()
        {
            InitializeComponent();
            generarTeclado();
            generarEspaciosLetrasAdivinadas();
        }

        //Funcion que genera el teclado y lo agrega al UniformGrid
        private void generarTeclado()
        {

            for (char i = 'A'; i <= 'Z'; i++)
            {
                TextBlock caja = new TextBlock();
                caja.Text = Convert.ToString(i);

                Viewbox vBox = new Viewbox();
                vBox.Child = caja;

                Button btn = new Button();
                btn.Tag = i;
                btn.Content = vBox;
                btn.Click += Button_Click;

                tecladoUniformGrid.Children.Add(btn);

                //Añadimos Ñ al teclado despues de la N
                if (i == 'N')
                {
                    agregarEnye();
                }
            }
        }

        //Funcion que añade la Ñ al teclado de la UniformGrid
        private void agregarEnye()
        {
            TextBlock caja = new TextBlock();
            caja.Text = "Ñ";

            Viewbox vBox = new Viewbox();
            vBox.Child = caja;

            Button btn = new Button();
            btn.Tag = "Ñ";
            btn.Content = vBox;

            tecladoUniformGrid.Children.Add(btn);
        }

        //Sustituye las letras por guiones bajos '_' y los agrega dentro
        //del StackPanel encapsulados dentro de TextBlock/ViewBox 
        private void generarEspaciosLetrasAdivinadas()
        {
            String palabra = "Hola que tal como estas muy bien y tu hhh";

            for (int i = 0; i < palabra.Length; i++)
            {
                if (palabra[i] != ' ')
                {
                    TextBlock caja = new TextBlock();
                    caja.Text = Convert.ToString("_");
                    caja.Tag = palabra[i].ToString();

                    Viewbox vBox = new Viewbox();
                    vBox.Child = caja;
                    vBox.Margin = new Thickness(1);

                    letrasAdivinadasStackPanel.Children.Add(vBox);
                }
                else
                {
                    TextBlock caja = new TextBlock();
                    caja.Text = Convert.ToString(" ");
                    caja.Tag = " ";

                    Viewbox vBox = new Viewbox();
                    vBox.Child = caja;

                    letrasAdivinadasStackPanel.Children.Add(vBox);
                }
            }

        }

        //Recorre los TextBlock que estan dentro de letrasAdivinadasStackPanel
        //y comprueba su Tag con la letra que se le pasa como argumento
        //En caso que coincidan, le pasa el contenido de Tag al Texto del mismo TextBlock
        private void mostrarLetrasAdivinadas(Char letra)
        {
            Boolean encontrado = false;
            List<TextBlock> listaTextBlock = new List<TextBlock>();

            //Crea la lista de los TextBlock, hijos de letrasAdivinadasStackPanel
            foreach (Viewbox vBox in letrasAdivinadasStackPanel.Children)
            {
                listaTextBlock.Add((TextBlock)vBox.Child);
            }

            //Recorre la Lista de TextBlock y comprueba si alguno coincide
            //con la letra intorducida. Si coincide, la muestra.
            foreach (TextBlock textBox in listaTextBlock)
            {
                String caracter = (String)textBox.Tag;

                if (Convert.ToChar(caracter.ToLower()) == letra)
                {
                    textBox.Text = (String)textBox.Tag;
                    encontrado = true;
                }
            }

            //Si la letra introducida no se encuentra en la frase para adivinar,
            //aumenta contador de errores y sustituye la imagen del Ahorcado
            if (!encontrado)
            {
                countError++;

                String urlImg = @"img/" + Convert.ToString(countError + 4) + ".jpg";
                BitmapImage bmi = new BitmapImage();
                bmi.BeginInit();
                bmi.UriSource = new Uri(urlImg, UriKind.Relative);
                bmi.EndInit();

                ahorcadoImage.Source = bmi;
                Console.Out.WriteLine("URL: " + ahorcadoImage.Source);
            }
        }

        //Recorre los Botones de tecladoUniformGrid y comprueba su Tag
        //con la cadena que se le pasa como argumento.
        //En caso que coincida, deshabilita el Boton de tecladoUniformGrid
        private void ocultarBotonesPulsados(String btnInput)
        {
            foreach (Button boton in tecladoUniformGrid.Children)
            {
                if (boton.Tag.ToString().ToLower().Equals(btnInput.ToLower()))
                {
                    boton.IsEnabled = false;
                }
            }
        }

        //Evento de Clicar sobre el boton (tecla) de la App
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Convierte el Tag del boton a caracter en minusculas
            char caracter = (sender as Button).Tag.ToString().ToLower()[0];
            mostrarLetrasAdivinadas(caracter);

            //Deshabilita el boton al pulsarlo
            (sender as Button).IsEnabled = false;
        }

        //Evento de pulsacion de tecla del teclado real
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            //Le pasa el primer caracter en minusculas del boton pulsado
            mostrarLetrasAdivinadas(e.Key.ToString().ToLower()[0]);

            //Le pasa nombre(valor) del boton pulsado
            ocultarBotonesPulsados(e.Key.ToString());
        }
    }
}
