using static System.Console;

// Implementación de un juego de "Tic Tac Toe" (Tateti) en C#
// para mostrar como usar una matriz de 3x3

// Fichas
var Vacio = "  ";
var X     = "❌";
var O     = "⭕️";

// Tablero
string[,] tablero = new [,] {
    { Vacio, Vacio, Vacio },
    { Vacio, Vacio, Vacio },
    { Vacio, Vacio, Vacio }
};

// Mostrar el tablero
void MostrarTablero(string mensaje = ""){
    Clear();
    WriteLine($"\n{mensaje}\n\n");
    for(int f = 0; f < 3; f++){
        for(int c = 0; c < 3; c++){
            Write(tablero[f, c]);
            if (c < 2) Write(" | ");
        }
        WriteLine();
        if (f < 2) WriteLine("---+----+---");
    }
    WriteLine();
}

// Leer un valor entero
int Leer(string mensaje){
    Write(mensaje);
    return int.Parse(ReadLine());
}

// Muestra un mensaje y espera una tecla
void Mensaje(string mensaje){
    Write($"\n{mensaje}...");
    ReadKey();
}

// Verifica si el jugador gano
bool Gano(string jugador){
    // Verifica que el jugador ocupe una fila, columna o diagonal
    return 
        (tablero[0, 0] == jugador && tablero[1, 1] == jugador && tablero[2, 2] == jugador) || // Diagonal principal
        (tablero[0, 2] == jugador && tablero[1, 1] == jugador && tablero[2, 0] == jugador) || // Diagonal secundaria
        (tablero[0, 0] == jugador && tablero[0, 1] == jugador && tablero[0, 2] == jugador) || // Fila 1
        (tablero[1, 0] == jugador && tablero[1, 1] == jugador && tablero[1, 2] == jugador) || // Fila 2
        (tablero[2, 0] == jugador && tablero[2, 1] == jugador && tablero[2, 2] == jugador) || // Fila 3
        (tablero[0, 0] == jugador && tablero[1, 0] == jugador && tablero[2, 0] == jugador) || // Columna 1
        (tablero[0, 1] == jugador && tablero[1, 1] == jugador && tablero[2, 1] == jugador) || // Columna 2
        (tablero[0, 2] == jugador && tablero[1, 2] == jugador && tablero[2, 2] == jugador);   // Columna 3
}

// Verifica si hay empate
bool Empate(){
    for(int f = 0; f < 3; f++){
        for(int c = 0; c < 3; c++){
            if(tablero[f, c] == Vacio) return false;
        }
    }
    return true;
}

// Juego principal

string jugador = X;
while(true){
    MostrarTablero($"Turno de {jugador}");
    int f = Leer("Fila    (1-3): ");
    int c = Leer("Columna (1-3): ");

    if(f < 1 || f > 3 || c < 1 || c > 3){
        Mensaje("Fila o columna no válida");
        continue;
    }

    if(tablero[f-1, c-1] != Vacio){
        Mensaje("Posición ocupada");
        continue;
    }

    tablero[f-1, c-1] = jugador;

    if(Gano(jugador)){
        MostrarTablero($"🎉 Gano el jugador {jugador}");
        break;
    } else if(Empate()){
        MostrarTablero("🤷🏻‍♂️ Empate");
        break;
    }
    jugador = (jugador == X) ? O : X; // Cambia de jugador
}