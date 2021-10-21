﻿using System;
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
         **************************
         *GENERADORES AUTOMIATICOS*
         **************************
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
                    AgregarEnye();
                }
            }
        }

        //Funcion que añade la Ñ al teclado del UniformGrid
        private void AgregarEnye()
        {
            TextBlock caja = new TextBlock();
            caja.Text = "Ñ";

            Viewbox vBox = new Viewbox();
            vBox.Child = caja;

            Button btn = new Button();
            btn.Tag = "Ñ";
            btn.Content = vBox;
            btn.Click += Button_Click;

            _ = TecladoUniformGrid.Children.Add(btn);
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
                    TextBlock caja = new TextBlock();
                    caja.Text = Convert.ToString("_");
                    caja.Tag = cadena[i].ToString();

                    Viewbox vBox = new Viewbox();
                    vBox.Child = caja;
                    vBox.Margin = new Thickness(1);

                    _ = LetrasAdivinadasStackPanel.Children.Add(vBox);
                }
                else if (cadena[i] == ' ')
                {
                    TextBlock caja = new TextBlock();
                    caja.Text = Convert.ToString(" ");
                    caja.Tag = " ";

                    Viewbox vBox = new Viewbox();
                    vBox.Child = caja;

                    _ = LetrasAdivinadasStackPanel.Children.Add(vBox);
                }
            }

        }


        /*
         **********************
         *FUNCIONES DE CONTROL*
         **********************
         */


        //Funcion que busca la letra dentro de la cadena a adivinar
        private void MostrarLetrasAdivinadas(char letra)
        {
            bool encontrado = false;
            haGanado = true;

            //Busca coincidencia
            foreach (Viewbox vBox in LetrasAdivinadasStackPanel.Children)
            {
                TextBlock tb = (TextBlock)vBox.Child;

                string tagChar = (string)tb.Tag;

                if (Convert.ToChar(tagChar.ToLower()) == letra)
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

        //Funcion que deshabilita botones pulsados
        private void DeschabilitarBotonesPulsados(string letraBotonPulsado)
        {
            if (letraBotonPulsado.Equals("todos"))
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
                    if (boton.Tag.ToString().ToLower().Equals(letraBotonPulsado.ToLower()))
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

        /*
         *********
         *EVENTOS*
         *********
         */

        //Evento de Clicar sobre el boton (tecla) de la App
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            char caracter = (sender as Button).Tag.ToString().ToLower()[0];

            MostrarLetrasAdivinadas(caracter);

            (sender as Button).IsEnabled = false;

            RevisarEstadoPartida();
        }

        //Evento de pulsacion de tecla del teclado real
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            MostrarLetrasAdivinadas(e.Key.ToString().ToLower()[0]);

            DeschabilitarBotonesPulsados(e.Key.ToString());

            //Antes de comprobar el estado de partida, se comprueba
            //que no este finalizada. Asi se evita repetir los avisos.
            if (!haGanado && !seHaRendido && !haPerdido)
            {
                RevisarEstadoPartida();
            }
        }

        private void NuevaPartidaButton_Click(object sender, RoutedEventArgs e)
        {
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

        private void RendirseButton_Click(object sender, RoutedEventArgs e)
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
