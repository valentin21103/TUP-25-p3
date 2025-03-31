using System.Collections;
using System.Text.RegularExpressions;
using TUP;

class Program {
    static string MostrarMenu() {
        Console.Clear();
        Consola.Escribir("=== MENU DE OPCIONES ===", ConsoleColor.Cyan);
        Consola.Escribir("1. Listar alumnos");
        Consola.Escribir("2. Exportar datos");
        Consola.Escribir("3. Normalizar Carpetas");
        Consola.Escribir("4. Copiar trabajo práctico");
        Consola.Escribir("5. Verificar Presentacion Trabajo Practico");
        Consola.Escribir("6. Listar Trabajo practicos no Presentados");
        Consola.Escribir("0. Salir");

        return Consola.ElegirOpcion("\nElija una opción (0-6): ", "0123456");
    }

    static void Main(string[] args) {
        var clase = Clase.Cargar();
        var practico = 1;

        while (true) {
            string opcion = MostrarMenu();
            if (opcion == "0") return;

            switch (opcion) {
                case "1":
                    Console.Clear();
                    Consola.Escribir("=== Listar alumnos ===", ConsoleColor.Cyan);
                    clase.ListarAlumnos();
                    break;
                case "2":
                    Consola.Escribir("=== Exportar datos ===", ConsoleColor.Cyan);
                    clase.ExportarDatos();
                    break;
                case "3":
                    Consola.Escribir("=== Normalizar Carpetas ===", ConsoleColor.Cyan);
                    clase.CrearCarpetas();
                    break;
                case "4":
                    Consola.Escribir("=== Copiar trabajo práctico ===", ConsoleColor.Cyan);
                    string tp = Consola.LeerCadena("Ingrese el nombre del trabajo práctico a copiar (ej: tp1): ", new[] { "1", "2", "3" });
                    bool forzar = Consola.Confirmar("¿Forzar copia incluso si ya existe?");

                    clase.CopiarPractico(int.Parse(tp), forzar);
                    break;
                case "5":
                    Consola.Escribir($"=== Verificar Presentacion Trabajo Practico ===", ConsoleColor.Cyan);
                    clase.CrearCarpetas(); // Normaliza el nombre de las carpetas 
                    clase.VerificaPresentacionPractico(practico);
                    clase = Clase.Cargar(); // Recarga la lista de alumnos
                    break;
                case "6":

                    Consola.Escribir("=== Listar Trabajo practicos no Presentados ===", ConsoleColor.Cyan);
                    clase.ListarAusentes(practico);
                    break;
            }
            Consola.EsperarTecla();
        }
    }
}