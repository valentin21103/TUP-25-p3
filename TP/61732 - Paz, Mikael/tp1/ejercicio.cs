using System;       // Para usar la consola  (Console)
using System.IO;    // Para leer archivos    (File)

// Ayuda: 
//   Console.Clear() : Borra la pantalla
//   Console.Write(texto) : Escribe texto sin salto de línea
//   Console.WriteLine(texto) : Escribe texto con salto de línea
//   Console.ReadLine() : Lee una línea de texto
//   Console.ReadKey() : Lee una tecla presionada

// File.ReadLines(origen) : Lee todas las líneas de un archivo y devuelve una lista de strings
// File.WriteLines(destino, lineas) : Escribe una lista de líneas en un archivo

// Escribir la solucion al TP1 en este archivo. 
int rIndicePorId(int id)
{
    for (int i = 0; i < cantidadContactos; i++)
    {
        if (agenda[i].Id == id)
        {
            return i; // Retorna el índice si encuentra el ID
        }
    }
    return -1; // Retorna -1 si no encuentra el ID
}

static void LeerContactosDeArchivo()
{
    if (!File.Exists("agenda.csv"))
    {
        Console.WriteLine("El archivo 'agenda.csv' no existe.");
        return;
    }

    try
    {
        string[] lineas = File.ReadAllLines("agenda.csv");

        foreach (string linea in lineas)
        {
            // Lógica para procesar cada línea
            // Ejemplo: Divide la línea en base al carácter ","
            string[] datos = linea.Split(',');
            if (datos.Length >= 2) // Verifica que tenga al menos dos datos
            {
                int id = int.Parse(datos[0]); // Convierte el primer dato a entero (ID)
                string nombre = datos[1]; // Segundo dato como nombre

                Console.WriteLine($"ID: {id}, Nombre: {nombre}"); // Ejemplo de salida
            }
            else
            {
                Console.WriteLine("Línea mal formateada: " + linea);
            }
        }
    }
    catch (IOException e)
    {
        Console.WriteLine($"Error al leer el archivo: {e.Message}");
    }
    catch (FormatException e)
    {
        Console.WriteLine($"Error de formato al procesar los datos: {e.Message}");
    }
}
