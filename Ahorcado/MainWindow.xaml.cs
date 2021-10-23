using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Ahorcado
{
    public partial class MainWindow : Window
    {
        private int countError = 0;
        private const int MAX_ERROR = 6;
        private bool seHaRendido = false;
        private bool haGanado = false;
        private bool haPerdido = false;

        public MainWindow()
        {
            InitializeComponent();
            GenerarTeclado();
            GenerarEspaciosLetrasAdivinar(GenerarPalabra());
        }

        /*
         ****************************
         **GENERADORES DE CONTENIDO**
         ****************************
         */

        //Funcion que devuelve una cadena aleatoria
        private string GenerarPalabra()
        {
            string[] poema = new string[]{
                "Es hielo abrasador, es fuego helado,",
                "es herida que duele y no se siente,",
                "es un soñado bien, un mal presente,",
                "es un breve descanso muy cansado.",

                "Es un descuido que nos da cuidado,",
                "un cobarde con nombre de valiente,",
                "un andar solitario entre la gente,",
                "un amar solamente ser amado.",

                "Es una libertad encarcelada,",
                "que dura hasta el postrero paroxismo",
                "enfermedad que crece si es curada.",

                "Este es el niño Amor, este es su abismo.",
                "¡Mirad cual amistad tendra con nada",
                "el que en todo es contrario de si mismo!"
            };

            Random rm = new Random();
            int numAleatorio = rm.Next(0, poema.Length);

            string palabra = poema[numAleatorio];

            return palabra;
        }

        //Funcion que genera el teclado y lo agrega al UniformGrid
        private void GenerarTeclado()
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

                _ = TecladoUniformGrid.Children.Add(btn);

                //Añadimos Ñ al teclado despues de la N
                if (i == 'N')
                {
                    TextBlock caja1 = new TextBlock();
                    caja1.Text = "Ñ";

                    Viewbox vBox1 = new Viewbox();
                    vBox1.Child = caja1;

                    Button btn1 = new Button();
                    btn1.Tag = "Ñ";
                    btn1.Content = vBox1;
                    btn1.Click += Button_Click;

                    _ = TecladoUniformGrid.Children.Add(btn1);
                }
            }
        }

        //Funcion que sustituye las letras por guiones bajos '_'
        private void GenerarEspaciosLetrasAdivinar(string cadena)
        {
            for (int i = 0; i < cadena.Length; i++)
            {
                if (cadena[i] != ' ' && cadena[i] != '.' && cadena[i] != ','
                    && cadena[i] != '!' && cadena[i] != '¡'
                    && cadena[i] != '?' && cadena[i] != '¿')
                {
                    generarLetraAdivinar(cadena[i], '_');
                }
                else
                {
                    generarLetraAdivinar(cadena[i], cadena[i]);
                }
            }
        }

        //Funcion que diseña un letra de la cadena a adivinar y la inserta
        private void generarLetraAdivinar(char letra, char caracterEspecial)
        {
            TextBlock caja = new TextBlock();
            caja.Text = caracterEspecial.ToString();
            caja.Tag = letra.ToString();

            Viewbox vBox = new Viewbox();
            vBox.Child = caja;
            vBox.Margin = new Thickness(1);

            _ = LetrasAdivinadasStackPanel.Children.Add(vBox);
        }


        /*
         ************************
         **FUNCIONES DE CONTROL**
         ************************
         */


        //Funcion que busca la letra dentro de la cadena a adivinar
        private void MostrarLetrasAdivinadas(string letra)
        {
            bool encontrado = false;
            haGanado = true;

            //Busca coincidencia
            foreach (Viewbox vBox in LetrasAdivinadasStackPanel.Children)
            {
                TextBlock tb = (TextBlock)vBox.Child;

                string tagChar = (string)tb.Tag;

                if (tagChar.ToLower().Equals(letra.ToLower()))
                {
                    tb.Text = (string)tb.Tag;
                    encontrado = true;
                }

                if (tb.Text.Equals("_"))
                {
                    haGanado = false;
                }
            }

            //Actualiza contador de Errores y la imagen
            if (!encontrado)
            {
                countError++;
                ActualizarImagen();

                if (countError >= MAX_ERROR)
                {
                    haPerdido = true;
                }
            }
        }

        //Funcion que Actualiza Imagen del Ahorcado
        private void ActualizarImagen()
        {
            string urlImg = @"img/" + Convert.ToString(countError + 4) + ".jpg";
            BitmapImage bmi = new BitmapImage();
            bmi.BeginInit();
            bmi.UriSource = new Uri(urlImg, UriKind.Relative);
            bmi.EndInit();

            AhorcadoImage.Source = bmi;
        }

        //Funcion que deshabilita botones (teclas del teclado virtual de App)
        private void DeschabilitarBotonesPulsados(string letraPulsada)
        {
            if (letraPulsada.Equals("todos"))
            {
                foreach (Button boton in TecladoUniformGrid.Children)
                {
                    boton.IsEnabled = false;
                }
            }
            else
            {
                foreach (Button boton in TecladoUniformGrid.Children)
                {
                    if (boton.Tag.ToString().ToLower().Equals(letraPulsada.ToLower()))
                    {
                        boton.IsEnabled = false;
                    }
                }
            }
        }

        //Funcion de finalizar partida si se cumple alguna condicion
        private void RevisarEstadoPartida()
        {
            //Se ha rendido
            if (seHaRendido)
            {
                //Muestra el texto que tenia que adivinar
                foreach (Viewbox vBox in LetrasAdivinadasStackPanel.Children)
                {
                    TextBlock tb = (TextBlock)vBox.Child;
                    tb.Text = (string)tb.Tag;
                }
                _ = LetrasAdivinadasStackPanel.Style = (Style)Resources["FondoRojo"];
                DeschabilitarBotonesPulsados("todos");
                _ = MessageBox.Show("Partida finalizada.", "Te has rendido", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            //Ha ganado
            else if (haGanado)
            {
                DeschabilitarBotonesPulsados("todos");
                _ = MessageBox.Show("¡Enhorabuena! \nHa adivinado la frase.", "¡Victoria!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            //Ha perdido
            else if (haPerdido)
            {
                DeschabilitarBotonesPulsados("todos");
                _ = MessageBox.Show("Ha agotado todos los intentos.", "Has perdido", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NuevaPartida()
        {
            _ = LetrasAdivinadasStackPanel.Style = (Style)Resources["FondoBlanco"];
            //Habila botones
            foreach (Button boton in TecladoUniformGrid.Children)
            {
                boton.IsEnabled = true;
            }

            //Borra la frase generada
            LetrasAdivinadasStackPanel.Children.Clear();

            //Generar nueva frase 
            GenerarEspaciosLetrasAdivinar(GenerarPalabra());

            //Retesea contador de intentos y variables
            countError = 0;
            seHaRendido = false;
            haGanado = false;
            haPerdido = false;

            //Actualiza la imagen del ahorcado
            ActualizarImagen();
        }


        /*
         ****************************
         **CONTROLADORES DE EVENTOS**
         ****************************
         */


        //Evento de Clicar sobre el boton (tecla virtual) de la App
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string letra = (sender as Button).Tag.ToString();

            MostrarLetrasAdivinadas(letra);

            (sender as Button).IsEnabled = false;

            RevisarEstadoPartida();
        }

        //Evento de Pulsacion de tecla del teclado real
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (!seHaRendido && !haGanado)
                {
                    seHaRendido = true;
                    RevisarEstadoPartida();
                }
            }
            else if (e.Key == Key.Insert)
            {
                NuevaPartida();
            }
            else if (!haGanado && !seHaRendido && !haPerdido)
            {
                MostrarLetrasAdivinadas(e.Key.ToString());

                DeschabilitarBotonesPulsados(e.Key.ToString());

                RevisarEstadoPartida();
            }
        }

        //Evento de botones Nueva Partida y Rendirse
        private void Button_Click_Options(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Tag.ToString().Equals("new"))
            {
                NuevaPartida();
            }
            if ((sender as Button).Tag.ToString().Equals("surrender"))
            {
                if (!seHaRendido && !haGanado)
                {
                    countError = 0;
                    seHaRendido = true;
                    RevisarEstadoPartida();
                }
            }
        }
    }
}
