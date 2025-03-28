using static System.Console;

class Persona {
    public int Id { get; private set; }
    public string Nombre { get; private set; }
    private int _edad;
    public int Edad {
        get { return _edad; }
        set { if (value > _edad) _edad = value; }
    }
    
    public Persona(string nombre, int edad) {
        Id = GenerarId();
        Nombre = nombre;
        Edad = edad;
    }
    public void CumplirAño() {
        Edad = Edad + 1;
    }
    public override string ToString() {
        return $"{Nombre} tiene {Edad} años";
    }
    
    // Gestiona el ID de las personas 
    static int proximoId = 1000;
    static int GenerarId() {
        return proximoId++;
    }
}

// Una agenda es una colección de personas.
class Agenda {
    int cantidad;
    Persona[] personas;

    public Agenda(int maximo = 3) {
        cantidad = 0;
        personas = new Persona[maximo];
    }

    public void Agregar(Persona p) {
        if (cantidad == personas.Length - 1) {
            AumentarCapacidad(2);
        }
        personas[cantidad++] = p;
    }

    private void AumentarCapacidad(int incremento = 2) {
        Array.Resize(ref personas, personas.Length + incremento);
        // var copia = new Persona[personas.Length + incremento];
        // for (var i = 0; i < cantidad; i++) {
        //     copia[i] = personas[i];
        // }
        // personas = copia;
    }

    public void Borrar(int id) {
        for (var i = 0; i < cantidad; i++) {
            if (personas[i].Id == id) { // Ubica a la persona
                // Desplaza las personas que están después
                Array.Copy(personas, i + 1, personas, i, cantidad - i - 1);
                // for (var j = i; j < cantidad - 1; j++) { 
                //     personas[j] = personas[j + 1];
                // }
                cantidad--;
                break;
            }
        }
    }
    
    public void Mostrar() {
        WriteLine($"\nLista de personas ({cantidad} de {personas.Length})");
        for (var i = 0; i < cantidad; i++) {
            WriteLine($" {i + 1}. {personas[i],-30} #{personas[i].Id}");
        }
    }
}

// Ejemplo de uso 
Clear();
var a = new Agenda();
a.Agregar(new Persona("Juan", 30));
a.Mostrar();

a.Agregar(new Persona("Maria", 25));
a.Agregar(new Persona("Pedro", 40));
a.Mostrar();

a.Agregar(new Persona("Ana", 20));
a.Mostrar();
a.Agregar(new Persona("Luis", 35));
a.Mostrar();
