using static System.Console;
using System.Text;
using System.Globalization;

// Funcion auxiliar para facilitar la lectura de datos
string Leer(string mensaje){
    Write(mensaje);
    return ReadLine();
}

// Esta es la implementacion de una pila unica 
// Usa variables `globales` para almacenar el estado interno
void demoPila(){
    // Simula una pila de nombres mediante funciones y un array.

    int cantidad = 0;
    string[] pila = new string[100];

    void Push(string valor){
        pila[cantidad++] = valor;
    }

    string Pop(){
        return pila[--cantidad];
    }

    bool IsEmpty(){
        return cantidad == 0;
    }

    Clear();
    while(true){
        var valor = Leer("Nombre: ");
        if(valor=="") break;
        Push(valor);
    }

    while(!IsEmpty()){
        WriteLine($" - {Pop()}");
    }
}

// Defino un nuevo tipo de dato llamado Stack
// me permite tener multiples copias o instancias de la pila
// Stack es una estructura de datos que permite almacenar elementos
// en un orden específico, siguiendo el principio LIFO (Last In, First Out).
struct Stack {
    int cantidad = 0;
    string[] elementos = new string[100];

    public Stack() { 
        // Inicializamos explícitamente los campos
        cantidad = 0;
        elementos = new string[100];
    }

    public void Push(string valor) {
        elementos[cantidad++] = valor;
    }

    public string Pop() {
        return elementos[--cantidad];
    }

    public bool IsEmpty() {
        return cantidad == 0;
    }
}

// Demo de uso de la pila (Stack)
void demoStack(){
    var s = new Stack();  // Crea una nueva instancia de Stack
    Clear();
    WriteLine("Ingrese nombres (deje vacío para terminar):");
    while (true) {
        var nombre = Leer("Nombre :");
        if (nombre == "") break;
        s.Push(nombre);
    }
    WriteLine("\nNombres en orden inverso:");
    while (!s.IsEmpty()) {
        WriteLine($" - {s.Pop()}");
    }
}

// Aca se define una pila de enteros 
// Es identica a las anterior pero con int en lugar de string
struct StackInt {
    int cantidad = 0;
    int[] elementos = new int[100];

    public StackInt() { 
        cantidad = 0;
        elementos = new int[100];
    }

    public void Push(int valor) {
        elementos[cantidad++] = valor;
    }

    public int Pop() {
        return elementos[--cantidad];
    }

    public bool IsEmpty() {
        return cantidad == 0;
    }
}

// Se puede definir una pila de dobles
// Es identica a las anterior pero con double en lugar de string o int
struct StackDouble {
    int cantidad = 0;
    double[] elementos = new double[100];

    public StackDouble() {
        cantidad = 0;
        elementos = new double[100];
    }

    public void Push(double valor) {
        elementos[cantidad++] = valor;
    }

    public double Pop() {
        return elementos[--cantidad];
    }

    public bool IsEmpty() {
        return cantidad == 0;
    }
}

/// TIPO GENERICO ///
// Podemos generalizar la pila para cualquier tipo de dato
// Stack<T> es una pila del tipo generico T 
// Para usarlos reemplazamos T por el tipo deseado
struct Stack<T> {
    public int cantidad; // Se cambia a público
    T[] elementos;

    public Stack() {
        cantidad = 0;
        elementos = new T[100];
    }
    
    public Stack(int capacity) : this() {
        elementos = new T[capacity];
    }

    public void Push(T valor) {
        elementos[cantidad++] = valor;
    }

    public T Pop() {
        return elementos[--cantidad];
    }

    public bool IsEmpty() {
        return cantidad == 0;
    }
}

void demoMultipleStacks() {
    WriteLine("\nEjemplo de Stack con enteros");
    var si = new Stack<int>();
    si.Push(1);
    si.Push(2);
    while(!si.IsEmpty()) WriteLine($" - {si.Pop()}");

    WriteLine("\nEjemplo de Stack con dobles");
    var sd = new Stack<double>();
    sd.Push(1.1);
    sd.Push(2.2);
    while(!sd.IsEmpty()) WriteLine($" - {sd.Pop()}");

    WriteLine("\nEjemplo de Stack con cadenas");
    var ss = new Stack<string>(); //Exactamente igual a Stack
    ss.Push("Juan");
    ss.Push("Pedro");
    while(!ss.IsEmpty()) WriteLine($" - {ss.Pop()}");
}
