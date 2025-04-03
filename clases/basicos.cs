// if (<condition>) <code>
// if (<condition>) <code> else <code>

int a = 10, b = 20, c = 30, d = 5, e = 15;

int max = a;
if(b > max)
    max = b;
if(c > max)
    max = c;
if(d > max)
    max = d;
if(e > max)
    max = e;
WriteLine(max);

var tipo = 1;
var vocal = "";

switch(tipo){
    case 1:
        vocal = "a";
        break;
    case 2:
        vocal = "e";
        break;
    case 3:
        vocal = "i";
        break;
    case 4:
        vocal = "o";
        break;
    case 5:
        vocal = "u";
        break;
}

vocal = "desconocida";

vocal = tipo == 1 ? "a" : tipo == 2 ? "e" : tipo == 3 ? "i" : tipo == 4 ? "o" : tipo == 5 ? "u" : "desconocida";


vocal = tipo switch {
    1 => "a",
    2 => "e",
    3 => "i",
    4 => "o",
    5 => "u",
    _ => "desconocida"
};



var i = 0;          // Inicialización
while(i < 10){      // Condición
    if(i == 2) continue;
    Console.WriteLine(i);
    if(i == 7){
        break;
    }
    i++;            // Incremento
}

for(var j = 0; j < 10; j++){
    if(j == 2) continue;
    Console.WriteLine(j);
    if(j == 7){
        break;
    }
}