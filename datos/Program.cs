using System.Collections;
using System.Text.RegularExpressions;
using TUP;

class Program {

    static string MostrarMenu()
    {
        Console.Clear();
        Consola.Escribir("=== MENU DE OPCIONES ===", ConsoleColor.Cyan);
        Consola.Escribir("1. Listar alumnos por comisión");
        Consola.Escribir("2. Exportar datos de alumnos");
        Consola.Escribir("3. Crear carpetas");
        Consola.Escribir("4. Copiar trabajo práctico");
        Consola.Escribir("5. Contar líneas efectivas del TP1");
        Consola.Escribir("6. Listar Ausentes");
        Consola.Escribir("0. Salir");

        return Consola.ElegirOpcion("\nElija una opción (0-6): ", "0123456");
    }

    static void Main(string[] args) {
        var clase = Clase.Cargar("./alumnos.md");
        var practicoActual = 1;

        while (true) {
            string opcion = MostrarMenu();
            if (opcion == "0") return;

            switch (opcion) {
                case "1":
                    Console.Clear();
                    Consola.Escribir("=== Listar alumnos por comisión ===", ConsoleColor.Cyan);
                    clase.ListarAlumnosPorComision();
                    break;
                case "2":
                    Consola.Escribir("=== Exportar datos de alumnos ===", ConsoleColor.Cyan);
                    clase.ExportarDatos();
                    break;
                case "3":
                    Consola.Escribir("=== Crear carpetas ===", ConsoleColor.Cyan);
                    clase.CrearCarpetas();
                    break;
                case "4":
                    Consola.Escribir("=== Copiar trabajo práctico ===", ConsoleColor.Cyan);
                    string tp = Consola.LeerCadena("Ingrese el nombre del trabajo práctico a copiar (ej: tp1): ", new[] { "tp1", "tp2", "tp3" });
                    bool forzar = Consola.Confirmar("¿Forzar copia incluso si ya existe?");
                    
                    clase.CopiarTrabajoPractico(tp, forzar);
                    break;
                case "5":
                    Consola.Escribir($"=== Verificar Presentacion Trabajo Practico ===", ConsoleColor.Cyan);
                    Clase.VerificaPresentacionDeTrabajoPractico(practicoActual);
                    break;
                case "6":
                    Consola.Escribir("=== Listar Trabajo practicos no Presentados ===", ConsoleColor.Cyan);
                    clase.ListarAusentes(practicoActual);
                    break;
            }

            Consola.EsperarTecla();
        }
    }
}