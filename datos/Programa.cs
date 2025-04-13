using TUP;

class Program {
    static string ElegirOpcionMenu() {
        Console.Clear();
        Consola.Escribir("=== MENÚ DE OPCIONES ===", ConsoleColor.Cyan);
        Consola.Escribir("1. Listar alumnos");
        Consola.Escribir("2. Publicar trabajo práctico");
        Consola.Escribir("3. Verificar presentación de trabajos práctico");
        Consola.Escribir("4. Faltan presentar trabajo práctico");
        Consola.Escribir("5. Verificar asistencia");
        Consola.Escribir("0. Salir");
        return Consola.ElegirOpcion("\nElija una opción (0-5): ", "012345");
    }

    static void RenameTP3Directories(string path="../TP") {
        foreach (var dir in Directory.GetDirectories(path)) {
            string folderName = Path.GetFileName(dir);
            if (folderName.Equals("TP3", StringComparison.Ordinal)) {
                string newDir = Path.Combine(Path.GetDirectoryName(dir)!, "tp3");
                Console.WriteLine($"Renombrando: {dir} -> {newDir}");
                Directory.Move(dir, newDir);
            }
        }
    }

    static void ListarLineasEfectivaPracticos(string path="../TP") {
        foreach (var dir in Directory.GetDirectories(path)) {
            string folderName = Path.GetFileName(dir);
            if (folderName.Equals("tp3", StringComparison.Ordinal)) {
                string[] lines = File.ReadAllLines(Path.Combine(dir, "efectiva.txt"));
                foreach (var line in lines) {
                    Console.WriteLine(line);
                }
            }
        }
    }


    static void Main(string[] args) {
        var clase = Clase.Cargar();
        var practico = 3;

        Consola.Escribir("=== Bienvenido al sistema de gestión de alumnos ===", ConsoleColor.Cyan);
        while (true) {
            string opcion = ElegirOpcionMenu();
            if (opcion == "0") return;
            Console.Clear();
            Asistencias.Cargar();

            switch (opcion) {
                case "1":
                    Consola.Escribir("=== Listado de alumnos ===", ConsoleColor.Cyan);
                    clase.ListarAlumnos();
                    clase.ExportarDatos();

                    break;
                case "2":
                    Consola.Escribir("=== Copiar trabajo práctico ===", ConsoleColor.Cyan);
                    string tp   = Consola.LeerCadena("Ingrese el número del trabajo práctico a copiar (ej: 1): ", new[] { "1", "2", "3" });
                    bool forzar = Consola.Confirmar("¿Forzar copia incluso si ya existe?");

                    clase.NormalizarCarpetas();
                    clase.CopiarPractico(int.Parse(tp), forzar);
                    break;
                case "3":
                    Consola.Escribir($"=== Verificar presentación de trabajo práctico ===", ConsoleColor.Cyan);
                    clase.NormalizarCarpetas();
                    clase.Reiniciar();
                    for(var p = 1; p <= practico; p++) {
                        clase.VerificaPresentacionPractico(p);
                    }
                    var asistencias = Asistencias.Cargar(false);
                    clase.CargarAsistencia(asistencias);
                    clase.Guardar();
                    clase = Clase.Cargar();
                    break;
                case "4":
                    Consola.Escribir($"=== Alumnos que no presentaron práctico {practico} ===", ConsoleColor.Cyan);
                    clase.ListarNoPresentaron(practico);
                    break;
                case "5":
                    Consola.Escribir("=== Verificar asistencia ===", ConsoleColor.Cyan);
                    Asistencias.Cargar(true);
                    break;
            }
            Consola.EsperarTecla();
        }
    }
}