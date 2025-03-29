using System;
using System.IO;

class Program
{
    struct Contacto
    {
        public int Id ;
        public string Nombre;
        public string Telefono;
        public string Email;

        public Contacto(int id, string nombre, string telefono, string email)
        {
            Id = id;
            Nombre = nombre ?? string.Empty;
            Telefono = telefono ?? string.Empty;
            Email = email ?? string.Empty;
        }
    }

    static Contacto[] contactos = new Contacto[100];
    static int totalContactos = 0;

    static void Main()
    {
        CargarContactos();
        while (true)
        {
            Console.Clear();
            Console.WriteLine("===== AGENDA DE CONTACTOS =====");
            Console.WriteLine("1) Agregar contacto\n2) Modificar contacto\n3) Borrar contacto\n4) Listar contactos\n5) Buscar contacto\n0) Salir");
            Console.Write("Seleccione opción: ");
            string? opcion = Console.ReadLine();
            switch (opcion)
            {
                case "1": Agregar(); break;
                case "2": Modificar(); break;
                case "3": Borrar(); break;
                case "4": Listar(); break;
                case "5": Buscar(); break;
                case "0": Salir(); return;
                default: Console.WriteLine("Opción no válida."); break;
            }
        }
    }

    static void Agregar()
    {
        if (totalContactos >= contactos.Length)
        {
            Console.WriteLine("Agenda llena.");
            return;
        }
        Console.Write("Nombre: "); string? nombre = Console.ReadLine() ?? string.Empty;
        Console.Write("Teléfono: "); string? telefono = Console.ReadLine() ?? string.Empty;
        Console.Write("Email: "); string? email = Console.ReadLine() ?? string.Empty;
        contactos[totalContactos] = new Contacto(totalContactos + 1, nombre, telefono, email);
        totalContactos++;
        Console.WriteLine("Contacto agregado."); Console.ReadKey();
    }

    static void Modificar()
    {
        Console.Write("ID a modificar: ");
        if (!int.TryParse(Console.ReadLine(), out int id) || id < 1 || id > totalContactos)
        {
            Console.WriteLine("ID no válido.");
            return;
        }
        id--;
        Contacto c = contactos[id];
        Console.WriteLine($"Datos actuales: {c.Nombre}, {c.Telefono}, {c.Email}");
        Console.Write("Nuevo Nombre (dejar vacío para no cambiar): "); string? nombre = Console.ReadLine();
        Console.Write("Nuevo Teléfono (dejar vacío para no cambiar): "); string? telefono = Console.ReadLine();
        Console.Write("Nuevo Email (dejar vacío para no cambiar): "); string? email = Console.ReadLine();
        
        contactos[id] = new Contacto(c.Id, nombre ?? c.Nombre, telefono ?? c.Telefono, email ?? c.Email);
        Console.WriteLine("Contacto modificado."); Console.ReadKey();
    }

    static void Borrar()
    {
        Console.Write("ID a borrar: ");
        if (!int.TryParse(Console.ReadLine(), out int id) || id < 1 || id > totalContactos)
        {
            Console.WriteLine("ID no válido.");
            return;
        }
        id--;
        for (int i = id; i < totalContactos - 1; i++)
            contactos[i] = contactos[i + 1];
        totalContactos--;
        Console.WriteLine("Contacto borrado."); Console.ReadKey();
    }

    static void Listar()
    {
        Console.WriteLine("ID  NOMBRE            TELÉFONO   EMAIL");
        for (int i = 0; i < totalContactos; i++)
            Console.WriteLine($"{contactos[i].Id,-3} {contactos[i].Nombre,-16} {contactos[i].Telefono,-10} {contactos[i].Email}");
        Console.ReadKey();
    }

    static void Buscar()
    {
        Console.Write("Término de búsqueda: ");
        string? term = Console.ReadLine()?.ToLower();
        if (string.IsNullOrEmpty(term)) return;
        bool encontrado = false;
        Console.WriteLine("ID  NOMBRE            TELÉFONO   EMAIL");
        for (int i = 0; i < totalContactos; i++)
        {
            if (contactos[i].Nombre.ToLower().Contains(term) || contactos[i].Telefono.Contains(term) || contactos[i].Email.ToLower().Contains(term))
            {
                Console.WriteLine($"{contactos[i].Id,-3} {contactos[i].Nombre,-16} {contactos[i].Telefono,-10} {contactos[i].Email}");
                encontrado = true;
            }
        }
        if (!encontrado) Console.WriteLine("No se encontraron resultados.");
        Console.ReadKey();
    }

    static void CargarContactos()
    {
        if (File.Exists("agenda.csv"))
        {
            foreach (var line in File.ReadAllLines("agenda.csv"))
            {
                var data = line.Split(',');
                if (data.Length == 4 && int.TryParse(data[0], out int id))
                {
                    contactos[totalContactos++] = new Contacto(id, data[1], data[2], data[3]);
                }
            }
        }
    }

    static void Salir()
    {
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < totalContactos; i++)
            sb.AppendLine($"{contactos[i].Id},{contactos[i].Nombre},{contactos[i].Telefono},{contactos[i].Email}");
        File.WriteAllText("agenda.csv", sb.ToString());
        Console.WriteLine("Saliendo...");
        Console.ReadKey();
    }
}