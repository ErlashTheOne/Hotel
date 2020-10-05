using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading;

namespace Hotel
{
    class Program
    {
        static string connectionString = ConfigurationManager.ConnectionStrings["HOTEL"].ConnectionString;
        static SqlConnection connection = new SqlConnection(connectionString);

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
            bool incorrectAnswer;
            bool stay = true;

            while (stay)
            {
                do
                {
                    incorrectAnswer = false;
                    string MenuAnswer = Program.Menu();
                    Console.Clear();

                    switch (MenuAnswer)
                    {
                        case "1":
                            Register();
                            break;

                        case "2":
                            EditClient();
                            break;

                        case "3":
                            CheckIn();
                            break;

                        case "4":
                            CheckOut();
                            break;

                        case "5":
                            SeeRooms();
                            break;

                        case "6":
                            stay = false;
                            Console.WriteLine("\n\tGracias por usar nuestros servícios. Que pase un buen día.");
                            Thread.Sleep(2000);
                            break;

                        default:
                            incorrectAnswer = true;
                            Console.WriteLine("\tRespuesta incorrecta. Pruebe otra vez.");
                            Thread.Sleep(2000);
                            Console.Clear();
                            break;
                    }

                } while (incorrectAnswer == true);

            }
            Environment.Exit(0);

        }


        //Menu
        static string Menu()
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
            return Console.ReadLine();
        }


        //Registro
        static void Register()
        {
            string dni = string.Empty;
            do
            {
                Console.Write("\n\tIntroduce tu DNI: ");
                dni = Console.ReadLine();
                if (dni.Length != 9)
                {
                    Console.Clear();
                    Console.WriteLine("\n\tEl formato del DNI es incorrecto");
                    Thread.Sleep(2000);
                    Console.Clear();
                }
            } while (dni.Length != 9);
            if (CheckClient(dni))
            {
                Console.Clear();
                Console.WriteLine("\n\tEl usuario ya estaba registrado");
                Thread.Sleep(2000);
                Console.Clear();
            }
            else
            {
                Console.Write("\n\tIntroduce tu Nombre: ");
                string name = Console.ReadLine();
                Console.Write("\n\tIntroduce tu Apellido: ");
                string surname = Console.ReadLine();
                InsertClient(dni, name, surname);
                Console.Clear();
                Console.WriteLine($"\n\tUsuario registrado: {name} {surname} con DNI {dni}");
                Thread.Sleep(3000);
                Console.Clear();
            }
        }

        static bool CheckClient(string dni)
        {
            connection.Open();
            string query = queries[0] + $"'{dni}'";
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader client = command.ExecuteReader();
            if (client.Read())
            {
                connection.Close();
                return true;
            }
            else
            {
                connection.Close();
                return false;
            }
        }

        static void InsertClient(string dni, string name, string surname)
        {
            connection.Open();

            string query = queries[1] + $"('{dni}', '{name}', '{surname}')";
            SqlCommand command = new SqlCommand(query, connection);
            command.ExecuteNonQuery();

            connection.Close();
        }


        //Editar Cliente
        static void EditClient()
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
                        Console.Clear();
                        Console.WriteLine("\n\tEl formato del DNI es incorrecto");
                        Thread.Sleep(2000);
                        Console.Clear();
                    }
                } while (dni.Length != 9 && dni.ToLower() != "salir");

                if (dni.ToLower() != "salir")
                {
                    if (CheckClient(dni))
                    {
                        UpdateClientWithDni(dni);
                    }
                    else
                    {
                        invalidDni = true;
                        Console.Clear();
                        Console.WriteLine("\n\tEl DNI no pertenece a nadie.");
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

        static void UpdateClientWithDni(string dni)
        {

            Console.Write("\n\tIntroduce el nombre: ");
            string name = Console.ReadLine();

            Console.Write("\n\tIntroduce el apellido: ");
            string surname = Console.ReadLine();

            connection.Open();

            string query = queries[2] + $"Nombre = '{name}', Apellido = '{surname}' WHERE Dni = '{dni}'";
            SqlCommand command = new SqlCommand(query, connection);
            command.ExecuteNonQuery();

            connection.Close();
            Console.Clear();
            Console.WriteLine($"\n\tUsuario actualizado: {name} {surname} con DNI {dni}");
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
                if (CheckClient(dni))
                {
                    bool invalidRoom;
                    do
                    {
                        invalidRoom = false;
                        ShowAvailableRooms();
                        Console.Write("\n\t¿Que habitación deseas? ");
                        string roomsAnswer = Console.ReadLine();

                        if (CheckRooms(roomsAnswer))
                        {
                            ReserveRoom(dni, roomsAnswer);
                        }
                        else
                        {
                            invalidRoom = true;
                            Console.Clear();
                            Console.WriteLine("\n\tEl número de habitación introducido no es válido.");
                            Thread.Sleep(3000);
                            Console.Clear();
                        }
                    } while (invalidRoom);
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

        static void ShowAvailableRooms()
        {
            Console.Clear();
            Console.WriteLine("\n\tHabitaciones disponibles:");
            connection.Open();
            SqlCommand command = new SqlCommand(queries[3], connection);
            SqlDataReader client = command.ExecuteReader();
            while (client.Read())
            {
                Console.WriteLine($"\t{ client["CodHabitacion"]} {client["Estado"]} ");
            }
            connection.Close();
        }

        static bool CheckRooms(string roomAnswer)
        {
            connection.Open();
            string query = queries[4] + $"'{roomAnswer}'";
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader client = command.ExecuteReader();
            if (client.Read())
            {
                connection.Close();
                return true;
            }
            else
            {
                connection.Close();
                return false;
            }
        }

        static void ReserveRoom(string dni, string roomsAnswer)
        {
            connection.Open();

            string query = queries[5] + $"'reservado' WHERE CodHabitacion = '{roomsAnswer}'";
            SqlCommand command = new SqlCommand(query, connection);
            command.ExecuteNonQuery();

            connection.Close();

            DateTime checkInTime = DateTime.Now;

            connection.Open();

            string query2 = queries[6] + $"('{dni}', '{roomsAnswer}', '{checkInTime.ToString()}')";
            SqlCommand command2 = new SqlCommand(query2, connection);
            command2.ExecuteNonQuery();

            connection.Close();

            Console.Clear();
            Console.WriteLine($"\n\tReserva efectuada de la habitacion {roomsAnswer}, para el cliente {dni}, del día {checkInTime.ToString()}");
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
                if (CheckClient(dni))
                {
                    ShowOccupiedRooms();
                    Console.Write("\n\t¿De que habitación deseas hacer el check out? ");
                    string roomsAnswer = Console.ReadLine();

                    if (CheckFullRooms(roomsAnswer))
                    {
                        EmptyRoom(dni, roomsAnswer);
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

        private static void ShowOccupiedRooms()
        {
            Console.Clear();
            Console.WriteLine("\n\tHabitaciones ocupadas:");
            connection.Open();
            SqlCommand command = new SqlCommand(queries[9], connection);
            SqlDataReader client = command.ExecuteReader();
            while (client.Read())
            {
                Console.WriteLine($"\t{ client["CodHabitacion"]} {client["Estado"]} ");
            }
            connection.Close();
        }

        private static bool CheckFullRooms(string roomsAnswer)
        {
            connection.Open();
            string query = queries[7] + $"'{roomsAnswer}'";
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader client = command.ExecuteReader();
            if (client.Read())
            {
                connection.Close();
                return true;
            }
            else
            {
                connection.Close();
                return false;
            }
        }

        private static void EmptyRoom(string dni, string roomsAnswer)
        {
            connection.Open();

            string query = queries[5] + $"'disponible' WHERE CodHabitacion = '{roomsAnswer}'";
            SqlCommand command = new SqlCommand(query, connection);
            command.ExecuteNonQuery();

            connection.Close();

            DateTime checkOutTime = DateTime.Now;

            connection.Open();

            string query2 = queries[8] + $"'{checkOutTime.ToString()}' WHERE CodHabitacion = '{roomsAnswer}'";
            SqlCommand command2 = new SqlCommand(query2, connection);
            command2.ExecuteNonQuery();

            connection.Close();

            Console.Clear();
            Console.WriteLine($"\n\tCheck out de la habitacion {roomsAnswer}, para el cliente {dni}, el día {checkOutTime.ToString()}.");
            Thread.Sleep(3000);
            Console.Clear();
        }


        //Ver habitaciones
        static void SeeRooms()
        {
            bool incorrectAnswer;
            bool stay = true;

            while (stay)
            {
                do
                {
                    incorrectAnswer = false;
                    string menu2Answer = Menu2();
                    Console.Clear();

                    switch (menu2Answer)
                    {
                        case "1":
                            ShowAllRooms();
                            break;

                        case "2":
                            ShowAllOccupiedRooms();
                            break;

                        case "3":
                            ShowAllEmptyRooms();
                            break;

                        case "4":
                            stay = false;
                            Console.Clear();
                            break;

                        default:
                            incorrectAnswer = true;
                            Console.WriteLine("\n\tRespuesta incorrecta. Pruebe otra vez.");
                            Thread.Sleep(2000);
                            Console.Clear();
                            break;
                    }

                } while (incorrectAnswer == true);
            }
        }


        //Menu 2
        static string Menu2()
        {
            Console.Write("" +
                "\n\t1.- Ver listado de todas las habitaciones" +
                "\n\t2.- Ver listado de habitaciones ocupadas" +
                "\n\t3.- Ver listado de habitaciones vacías" +
                "\n\t4.- Salir" +
                "\n\n\t-->");

            return Console.ReadLine();
        }

        private static void ShowAllRooms()
        {
            Console.Clear();
            Console.WriteLine("\n\tListado de todas las habitaciones con nombre de su huésped o vacía: \n");
            connection.Open();
            SqlCommand command = new SqlCommand(queries[12], connection);
            SqlDataReader client = command.ExecuteReader();
            while (client.Read())

            {
                string name;
                if (client["Nombre"].ToString() != "")
                {
                    name = $" {client["Nombre"].ToString()}";
                }
                else
                {
                    name = string.Empty;
                }
                Console.WriteLine($"\t{client["CodHabitacion"]} {client["Estado"]}{name}");
            }
            connection.Close();
            Console.ReadLine();
            Console.Clear();
        }

        private static void ShowAllOccupiedRooms()
        {
            Console.Clear();
            Console.WriteLine("\n\tVer listado de habitaciones ocupadas con el nombre de su huésped: \n");
            connection.Open();
            SqlCommand command = new SqlCommand(queries[13], connection);
            SqlDataReader client = command.ExecuteReader();
            while (client.Read())

            {
                string name;
                if (client["Nombre"].ToString() != "")
                {
                    name = $" {client["Nombre"].ToString()}";
                }
                else
                {
                    name = string.Empty;
                }
                Console.WriteLine($"\t{client["CodHabitacion"]} {client["Estado"]}{name}");
            }
            connection.Close();
            Console.ReadLine();
            Console.Clear();
        }

        private static void ShowAllEmptyRooms()
        {
            Console.Clear();
            Console.WriteLine("\n\tListado de habitaciones vacías: \n");
            connection.Open();
            SqlCommand command = new SqlCommand(queries[3], connection);
            SqlDataReader client = command.ExecuteReader();
            while (client.Read())
            {
                Console.WriteLine($"\t{ client["CodHabitacion"]} {client["Estado"]} ");
            }
            connection.Close();
            Console.ReadLine();
            Console.Clear();

        }
    }
}
