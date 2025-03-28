using System;
using System.IO;

struct Contacto
{
    public int Id;
    public string Nombre;
    public string Telefono;
    public string Email;
}

class Program
{
    const int MAX_CONTACTOS = 100;
    static Contacto[] agenda = new Contacto[MAX_CONTACTOS];
    static int contadorContactos = 0;
    static string archivoCSV = "agenda.csv";

    static void Main()
    {
        CargarContactos();

        while (true)
        {
            Console.WriteLine("\n--- AGENDA DE CONTACTOS ---");
            Console.WriteLine("1. Agregar contacto");
            Console.WriteLine("2. Modificar contacto");
            Console.WriteLine("3. Borrar contacto");
            Console.WriteLine("4. Listar contactos");
            Console.WriteLine("5. Buscar contacto");
            Console.WriteLine("6. Salir");
            Console.Write("Seleccione una opción: ");
            
            string opcion = Console.ReadLine();
            Console.Clear();

            switch (opcion)
            {
                case "1":
                    AgregarContacto();
                    break;
                case "2":
                    ModificarContacto();
                    break;
                case "3":
                    BorrarContacto();
                    break;
                case "4":
                    ListarContactos();
                    break;
                case "5":
                    BuscarContacto();
                    break;
                case "6":
                    GuardarContactos();
                    return;
                default:
                    Console.WriteLine("Opción no válida.");
                    break;
            }
        }
    }

    static void AgregarContacto()
    {
        if (contadorContactos >= MAX_CONTACTOS)
        {
            Console.WriteLine("Agenda llena.");
            return;
        }

        Contacto nuevo;
        nuevo.Id = (contadorContactos == 0) ? 1 : agenda[contadorContactos - 1].Id + 1;

        Console.Write("Nombre: ");
        nuevo.Nombre = Console.ReadLine();
        Console.Write("Teléfono: ");
        nuevo.Telefono = Console.ReadLine();
        Console.Write("Email: ");
        nuevo.Email = Console.ReadLine();

        agenda[contadorContactos] = nuevo;
        contadorContactos++;
        Console.WriteLine("Contacto agregado.");
    }

    static void ModificarContacto()
    {
        Console.Write("Ingrese el ID del contacto a modificar: ");
        int id = int.Parse(Console.ReadLine());

        for (int i = 0; i < contadorContactos; i++)
        {
            if (agenda[i].Id == id)
            {
                Console.Write($"Nuevo nombre ({agenda[i].Nombre}): ");
                string nombre = Console.ReadLine();
                if (!string.IsNullOrEmpty(nombre)) agenda[i].Nombre = nombre;

                Console.Write($"Nuevo teléfono ({agenda[i].Telefono}): ");
                string telefono = Console.ReadLine();
                if (!string.IsNullOrEmpty(telefono)) agenda[i].Telefono = telefono;

                Console.Write($"Nuevo email ({agenda[i].Email}): ");
                string email = Console.ReadLine();
                if (!string.IsNullOrEmpty(email)) agenda[i].Email = email;

                Console.WriteLine("Contacto modificado.");
                return;
            }
        }
        Console.WriteLine("Contacto no encontrado.");
    }

    static void BorrarContacto()
    {
        Console.Write("Ingrese el ID del contacto a borrar: ");
        int id = int.Parse(Console.ReadLine());

        for (int i = 0; i < contadorContactos; i++)
        {
            if (agenda[i].Id == id)
            {
                for (int j = i; j < contadorContactos - 1; j++)
                {
                    agenda[j] = agenda[j + 1];
                }
                contadorContactos--;
                Console.WriteLine("Contacto eliminado.");
                return;
            }
        }
        Console.WriteLine("Contacto no encontrado.");
    }

    static void ListarContactos()
    {
        if (contadorContactos == 0)
        {
            Console.WriteLine("No hay contactos.");
            return;
        }

        Console.WriteLine("ID  | Nombre            | Teléfono       | Email");
        Console.WriteLine("-----------------------------------------------");

        for (int i = 0; i < contadorContactos; i++)
        {
            Console.WriteLine($"{agenda[i].Id,-3} | {agenda[i].Nombre,-16} | {agenda[i].Telefono,-14} | {agenda[i].Email}");
        }
    }

    static void BuscarContacto()
    {
        Console.Write("Ingrese término de búsqueda: ");
        string termino = Console.ReadLine().ToLower();
        bool encontrado = false;

        for (int i = 0; i < contadorContactos; i++)
        {
            if (agenda[i].Nombre.ToLower().Contains(termino) ||
                agenda[i].Telefono.ToLower().Contains(termino) ||
                agenda[i].Email.ToLower().Contains(termino))
            {
                Console.WriteLine($"{agenda[i].Id} | {agenda[i].Nombre} | {agenda[i].Telefono} | {agenda[i].Email}");
                encontrado = true;
            }
        }

        if (!encontrado) Console.WriteLine("No se encontraron coincidencias.");
    }

    static void GuardarContactos()
    {
        using (StreamWriter sw = new StreamWriter(archivoCSV))
        {
            for (int i = 0; i < contadorContactos; i++)
            {
                sw.WriteLine($"{agenda[i].Id},{agenda[i].Nombre},{agenda[i].Telefono},{agenda[i].Email}");
            }
        }
        Console.WriteLine("Datos guardados.");
    }

    static void CargarContactos()
    {
        if (!File.Exists(archivoCSV)) return;

        using (StreamReader sr = new StreamReader(archivoCSV))
        {
            string linea;
            while ((linea = sr.ReadLine()) != null)
            {
                string[] datos = linea.Split(',');
                if (contadorContactos < MAX_CONTACTOS)
                {
                    agenda[contadorContactos].Id = int.Parse(datos[0]);
                    agenda[contadorContactos].Nombre = datos[1];
                    agenda[contadorContactos].Telefono = datos[2];
                    agenda[contadorContactos].Email = datos[3];
                    contadorContactos++;
                }
            }
        }
    }
}

