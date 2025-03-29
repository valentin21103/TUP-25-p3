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

// Escribir la solucion al TP1 en este archivo. (Borre el ejemplo de abajo)

Console.WriteLine("---AGENDA DE CONTACTOS---");
Console.WriteLine("1. Agregar Contacto");
Console.WriteLine("2. Borrar Contacto");
Console.WriteLine("3. Modificar Contacto");
Console.WriteLine("4. Listar Contactos");
Console.WriteLine("5. Buscar Contacto");
Console.WriteLine("0. Salir");
Console.Write("Seleccione una opcion: ");
Console.ReadKey();
Console.Clear();