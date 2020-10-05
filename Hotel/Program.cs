using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading;

namespace Hotel
{
    class Program
    {
        static string connectionString = ConfigurationManager.ConnectionStrings["HOTEL"].ConnectionString;
        static SqlConnection conexion = new SqlConnection(connectionString);

        public static string[] queries = {
            "SELECT DNI FROM Clientes WHERE DNI=",  //0
            "INSERT INTO Clientes (DNI, Nombre, Apellido) values ", //1
            "UPDATE Clientes set ", //2
            "SELECT * FROM Habitaciones WHERE Estado='disponible'", //3
            "SELECT * FROM Habitaciones WHERE Estado='disponible' and CodHabitacion=", //4
            "UPDATE Habitaciones set Estado = ", //5
            "INSERT INTO Reservas (DNICliente, CodHabitacion, FechaCheckIn) values ", //6
            "SELECT * FROM Habitaciones WHERE Estado='reservado' and CodHabitacion=", //7
            "UPDATE Reservas set FechaCheckOut = ", //8
            "SELECT * FROM Habitaciones WHERE Estado='reservado'", //9
            "SELECT * FROM Habitaciones", //10
            "SELECT Nombre FROM Clientes WHERE DNI=", //11
            "SELECT  H.CodHabitacion, H.Estado, C.Nombre FROM Habitaciones h FULL OUTER JOIN Reservas r ON r.CodHabitacion=h.CodHabitacion and r.FechaCheckOut is null FULL OUTER JOIN Clientes c ON c.DNI=r.DNICliente where r.FechaCheckOut is null and h.CodHabitacion is not null", //12
            "SELECT  H.CodHabitacion, H.Estado, C.Nombre FROM Habitaciones h FULL OUTER JOIN Reservas r ON r.CodHabitacion=h.CodHabitacion and r.FechaCheckOut is null FULL OUTER JOIN Clientes c ON c.DNI=r.DNICliente where r.FechaCheckOut is null and h.CodHabitacion is not null and H.Estado = 'reservado'" //13
        };


        static void Main(string[] args)
        {
            bool resIncorrecta;
            bool stay = true;

            while (stay)
            {
                do
                {
                    resIncorrecta = false;
                    int RespMenu = Menu();
                    Console.Clear();

                    switch (RespMenu)
                    {
                        case 1:
                            Registro();
                            break;

                        case 2:
                            EditarCliente();
                            break;

                        case 3:
                            CheckIn();
                            break;

                        case 4:
                            CheckOut();
                            break;

                        case 5:
                            VerHabitaciones();
                            break;

                        case 6:
                            stay = false;
                            Console.WriteLine("\n\tGracias por usar nuestros servícios. Que pase un buen día.");
                            Thread.Sleep(2000);
                            break;

                        default:
                            resIncorrecta = true;
                            Console.WriteLine("\tRespuesta incorrecta. Pruebe otra vez.");
                            Thread.Sleep(2000);
                            Console.Clear();
                            break;
                    }

                } while (resIncorrecta == true);

            }
            Environment.Exit(0);

        }


        //Menu
        static int Menu()
        {
            Console.WriteLine("\n\tHotel 'El Hotel' ****");
            Console.Write("" +
                "\n\t1.- Registro de cliente" +
                "\n\t2.- Editar cliente" +
                "\n\t3.- CheckIn" +
                "\n\t4.- CheckOut" +
                "\n\t5.- Ver habitaciones" +
                "\n\t6.- Salir" +
                "\n\n\t-->");

            int RespMenu = Convert.ToInt32(Console.ReadLine());
            return RespMenu;
        }


        //Registro
        static void Registro()
        {
            string dni = string.Empty;
            do
            {
                Console.Write("\n\tIntroduce tu DNI: ");
                dni = Console.ReadLine();
                if (dni.Length != 9)
                {
                    Console.WriteLine("\tEl formato del DNI es incorrecto");
                    Thread.Sleep(2000);
                    Console.Clear();
                }
            } while (dni.Length != 9);
            if (ComprobarCliente(dni))
            {
                Console.WriteLine("\tEl usuario ya estaba registrado");
                Thread.Sleep(2000);
                Console.Clear();
            }
            else
            {
                Console.Write("\n\tIntroduce tu Nombre: ");
                string nombre = Console.ReadLine();
                Console.Write("\n\tIntroduce tu Apellido: ");
                string apellido = Console.ReadLine();
                InsertarCliente(dni, nombre, apellido);
                Console.Clear();
                Console.WriteLine($"\tUsuario registrado: {nombre} {apellido} con DNI {dni}");
                Thread.Sleep(3000);
                Console.Clear();
            }
        }

        static bool ComprobarCliente(string dni)
        {
            conexion.Open();
            string query = queries[0] + $"'{dni}'";
            SqlCommand comando = new SqlCommand(query, conexion);
            SqlDataReader clientes = comando.ExecuteReader();
            if (clientes.Read())
            {
                conexion.Close();
                return true;
            }
            else
            {
                conexion.Close();
                return false;
            }
        }

        static void InsertarCliente(string dni, string nombre, string apellido)
        {
            conexion.Open();

            string query = queries[1] + $"('{dni}', '{nombre}', '{apellido}')";
            SqlCommand comando = new SqlCommand(query, conexion);
            comando.ExecuteNonQuery();

            conexion.Close();
        }


        //Editar Cliente
        static void EditarCliente()
        {
            string dni = string.Empty;
            bool invalidDni = false;

            do
            {
                do
                {
                    Console.Write("\n\tIntroduce el DNI del usuario que quieres editar: (Salir para salir) ");

                    dni = Console.ReadLine();

                    if (dni.Length != 9 && dni.ToLower() != "salir")
                    {
                        Console.WriteLine("\tEl formato del DNI es incorrecto");
                        Thread.Sleep(2000);
                        Console.Clear();
                    }
                } while (dni.Length != 9 && dni.ToLower() != "salir");

                if (dni.ToLower() != "salir")
                {
                    if (ComprobarCliente(dni))
                    {
                        ActualizarClienteConDni(dni);
                    }
                    else
                    {
                        invalidDni = true;
                        Console.WriteLine("\tEl DNI no pertenece a nadie.");
                        Thread.Sleep(2000);
                        Console.Clear();
                    }
                }
                else
                {
                    Console.Clear();
                }

            } while (invalidDni == true);
        }

        static void ActualizarClienteConDni(string dni)
        {

            Console.Write("\n\tIntroduce el nombre: ");
            string nombre = Console.ReadLine();

            Console.Write("\n\tIntroduce el apellido: ");
            string apellido = Console.ReadLine();

            conexion.Open();

            string query = queries[2] + $"Nombre = '{nombre}', Apellido = '{apellido}' WHERE Dni = '{dni}'";
            SqlCommand comando = new SqlCommand(query, conexion);
            comando.ExecuteNonQuery();

            conexion.Close();
            Console.Clear();
            Console.WriteLine($"\tUsuario actualizado: {nombre} {apellido} con DNI {dni}");
            Thread.Sleep(3000);
            Console.Clear();
        }


        //CheckIn
        static void CheckIn()
        {
            string dni = string.Empty;
            do
            {
                Console.Write("\n\tIntroduce el DNI del usuario para hacer el check in: (Salir para salir) ");

                dni = Console.ReadLine();

                if (dni.Length != 9 && dni.ToLower() != "salir")
                {
                    Console.WriteLine("\tEl formato del DNI es incorrecto");
                    Thread.Sleep(2000);
                    Console.Clear();
                }
            } while (dni.Length != 9 && dni.ToLower() != "salir");

            if (dni.ToLower() != "salir")
            {
                if (ComprobarCliente(dni))
                {
                    bool habitacionInvalida;
                    do
                    {
                        habitacionInvalida = false;
                        PintarHabitacionesDisponibles();
                        Console.Write("\n\t¿Que habitación deseas? ");
                        string resHabitaciones = Console.ReadLine();

                        if (ComprobarHabitacion(resHabitaciones))
                        {
                            ReservarHabitacion(dni, resHabitaciones);
                        }
                        else
                        {
                            habitacionInvalida = true;
                            Console.Clear();
                            Console.WriteLine("\n\tEl número de habitación introducido no es válido.");
                            Thread.Sleep(3000);
                            Console.Clear();
                        }
                    } while (habitacionInvalida);
                }
                else
                {
                    Console.WriteLine("\tEl DNI no pertenece a nadie, no puedes hacer una reserva hasta haberse registrado.");
                    Thread.Sleep(3000);
                    Console.Clear();
                }
            }
            else
            {
                Console.Clear();
            }
        }

        static void PintarHabitacionesDisponibles()
        {
            Console.Clear();
            Console.WriteLine("\n\tHabitaciones disponibles:");
            conexion.Open();
            SqlCommand comando = new SqlCommand(queries[3], conexion);
            SqlDataReader clientes = comando.ExecuteReader();
            while (clientes.Read())
            {
                Console.WriteLine($"\t{ clientes["CodHabitacion"]} {clientes["Estado"]} ");
            }
            conexion.Close();
        }

        static bool ComprobarHabitacion(string resHabitaciones)
        {
            conexion.Open();
            string query = queries[4] + $"'{resHabitaciones}'";
            SqlCommand comando = new SqlCommand(query, conexion);
            SqlDataReader clientes = comando.ExecuteReader();
            if (clientes.Read())
            {
                conexion.Close();
                return true;
            }
            else
            {
                conexion.Close();
                return false;
            }
        }

        static void ReservarHabitacion(string dni, string resHabitaciones)
        {
            conexion.Open();

            string query = queries[5] + $"'reservado' WHERE CodHabitacion = '{resHabitaciones}'";
            SqlCommand comando = new SqlCommand(query, conexion);
            comando.ExecuteNonQuery();

            conexion.Close();

            DateTime checkInTime = DateTime.Now;

            conexion.Open();

            string query2 = queries[6] + $"('{dni}', '{resHabitaciones}', '{checkInTime.ToString()}')";
            SqlCommand comando2 = new SqlCommand(query2, conexion);
            comando2.ExecuteNonQuery();

            conexion.Close();

            Console.Clear();
            Console.WriteLine($"\n\tReserva efectuada de la habitacion {resHabitaciones}, para el cliente {dni}, del día {checkInTime.ToString()}");
            Thread.Sleep(3000);
            Console.Clear();

        }


        //CheckOut
        static void CheckOut()
        {
            string dni = string.Empty;
            do
            {
                Console.Write("\n\tIntroduce el DNI del usuario para hacer el check out: (Salir para salir) ");

                dni = Console.ReadLine();

                if (dni.Length != 9 && dni.ToLower() != "salir")
                {
                    Console.WriteLine("\tEl formato del DNI es incorrecto");
                    Thread.Sleep(2000);
                    Console.Clear();
                }
            } while (dni.Length != 9 && dni.ToLower() != "salir");

            if (dni.ToLower() != "salir")
            {
                if (ComprobarCliente(dni))
                {
                    PintarHabitacionesOcupadas();
                    Console.Write("\n\t¿De que habitación deseas hacer el check out? ");
                    string resHabitaciones = Console.ReadLine();

                    if (ComprobarHabitacionLlena(resHabitaciones))
                    {
                        VaciarHabitacion(dni, resHabitaciones);
                    }
                    else
                    {
                        Console.WriteLine("\tEl número de habitación estaba disponible o el número introducido no es válido.");
                        Thread.Sleep(3000);
                        Console.Clear();
                    }
                }
                else
                {
                    Console.WriteLine("\tEl DNI no pertenece a nadie, no puedes hacer el check out sin estar registrado.");
                    Thread.Sleep(3000);
                    Console.Clear();
                }
            }
            else
            {
                Console.Clear();
            }
        }

        private static void PintarHabitacionesOcupadas()
        {
            Console.Clear();
            Console.WriteLine("\n\tHabitaciones ocupadas:");
            conexion.Open();
            SqlCommand comando = new SqlCommand(queries[9], conexion);
            SqlDataReader clientes = comando.ExecuteReader();
            while (clientes.Read())
            {
                Console.WriteLine($"\t{ clientes["CodHabitacion"]} {clientes["Estado"]} ");
            }
            conexion.Close();
        }

        private static bool ComprobarHabitacionLlena(string resHabitaciones)
        {
            conexion.Open();
            string query = queries[7] + $"'{resHabitaciones}'";
            SqlCommand comando = new SqlCommand(query, conexion);
            SqlDataReader clientes = comando.ExecuteReader();
            if (clientes.Read())
            {
                conexion.Close();
                return true;
            }
            else
            {
                conexion.Close();
                return false;
            }
        }

        private static void VaciarHabitacion(string dni, string resHabitaciones)
        {
            conexion.Open();

            string query = queries[5] + $"'disponible' WHERE CodHabitacion = '{resHabitaciones}'";
            SqlCommand comando = new SqlCommand(query, conexion);
            comando.ExecuteNonQuery();

            conexion.Close();

            DateTime checkOutTime = DateTime.Now;

            conexion.Open();

            string query2 = queries[8] + $"'{checkOutTime.ToString()}' WHERE CodHabitacion = '{resHabitaciones}'";
            SqlCommand comando2 = new SqlCommand(query2, conexion);
            comando2.ExecuteNonQuery();

            conexion.Close();

            Console.Clear();
            Console.WriteLine($"\n\tCheck out de la habitacion {resHabitaciones}, para el cliente {dni}, el día {checkOutTime.ToString()}.");
            Thread.Sleep(3000);
            Console.Clear();
        }


        //Ver habitaciones
        static void VerHabitaciones()
        {
            bool resIncorrecta;
            bool stay = true;

            while (stay)
            {
                do
                {
                    resIncorrecta = false;
                    int RespMenu = Menu2();
                    Console.Clear();

                    switch (RespMenu)
                    {
                        case 1:
                            ListadoTodasLasHabitaciones();
                            break;

                        case 2:
                            ListadoHabitacionesOcupadas();
                            break;

                        case 3:
                            ListadoHabitacionesDisponibles();
                            break;

                        case 4:
                            stay = false;
                            Console.Clear();
                            break;

                        default:
                            resIncorrecta = true;
                            Console.WriteLine("\tRespuesta incorrecta. Pruebe otra vez.");
                            Thread.Sleep(2000);
                            Console.Clear();
                            break;
                    }

                } while (resIncorrecta == true);
            }
        }


        //Menu 2
        static int Menu2()
        {
            Console.Write("" +
                "\n\t1.- Ver listado de todas las habitaciones" +
                "\n\t2.- Ver listado de habitaciones ocupadas" +
                "\n\t3.- Ver listado de habitaciones vacías" +
                "\n\t4.- Salir" +
                "\n\n\t-->");

            int RespMenu2 = Convert.ToInt32(Console.ReadLine());
            return RespMenu2;
        }

        private static void ListadoTodasLasHabitaciones()
        {
            Console.Clear();
            Console.WriteLine("\n\tListado de todas las habitaciones con nombre de su huésped o vacía: \n");
            conexion.Open();
            SqlCommand comando = new SqlCommand(queries[12], conexion);
            SqlDataReader clientes = comando.ExecuteReader();
            while (clientes.Read())

            {
                string nombre;
                if (clientes["Nombre"].ToString() != "")
                {
                    nombre = $" {clientes["Nombre"].ToString()}";
                }
                else
                {
                    nombre = string.Empty;
                }
                Console.WriteLine($"\t{clientes["CodHabitacion"]} {clientes["Estado"]}{nombre}");
            }
            conexion.Close();
            Console.ReadLine();
            Console.Clear();
        }

        private static void ListadoHabitacionesOcupadas()
        {
            Console.Clear();
            Console.WriteLine("\n\tVer listado de habitaciones ocupadas con el nombre de su huésped: \n");
            conexion.Open();
            SqlCommand comando = new SqlCommand(queries[13], conexion);
            SqlDataReader clientes = comando.ExecuteReader();
            while (clientes.Read())

            {
                string nombre;
                if (clientes["Nombre"].ToString() != "")
                {
                    nombre = $" {clientes["Nombre"].ToString()}";
                }
                else
                {
                    nombre = string.Empty;
                }
                Console.WriteLine($"\t{clientes["CodHabitacion"]} {clientes["Estado"]}{nombre}");
            }
            conexion.Close();
            Console.ReadLine();
            Console.Clear();
        }

        private static void ListadoHabitacionesDisponibles()
        {
            Console.Clear();
            Console.WriteLine("\n\tListado de habitaciones vacías: \n");
            conexion.Open();
            SqlCommand comando = new SqlCommand(queries[3], conexion);
            SqlDataReader clientes = comando.ExecuteReader();
            while (clientes.Read())
            {
                Console.WriteLine($"\t{ clientes["CodHabitacion"]} {clientes["Estado"]} ");
            }
            conexion.Close();
            Console.ReadLine();
            Console.Clear();

        }
    }
}
