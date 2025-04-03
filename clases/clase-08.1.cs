var a = 5;
var b = 10;
var c = 8;
var d = 2;

// if(condición) SentenciaSI [ else SentenciaSINO ]
if(a > b) { // Else no es necesario
    Console.WriteLine("a es mayor que b");
}

if(a > b){ // If completo
    Console.WriteLine(a);
} else {
    Console.WriteLine(b);
}
// max = a > b ? a : b; // Operador ternario

if(a > b){ // If anidados
    if(a > c){
        Console.WriteLine(a);
    } else {
        Console.WriteLine(c);
    }
} else {
    if(b > c){
        Console.WriteLine(b);
    } else {
        Console.WriteLine(c);
    }
}
// max = a > b ? (a > c ? a : c) : (b > c ? b : c); // Operador ternario anidado

var mayor = a;

if(b > mayor){ mayor = b; }
if(c > mayor){ mayor = c; }
if(d > mayor){ mayor = d; }

Console.WriteLine(mayor);
mayor = a;
mayor = b > mayor ? b : mayor;
mayor = c > mayor ? c : mayor;
mayor = d > mayor ? d : mayor;

// WHILE

// while(condición) { Sentencia }
var i = 0;
while(i < 10){
    Console.WriteLine(i);
    i++;
}

i = 0;
while(true){
    Console.WriteLine(i);
    i++;
    if(i == 10){
        break;
    }
}

i = 0;
while(i < 10){
    i++;
    if(i == 5){
        continue;
    }
    Console.WriteLine(i);
}

i = 0;
do {
    Console.WriteLine(i);
    i++;
} while(i < 10);

// FOR
// for(inicialización; condición; incremento) { Sentencia }
for(var j = 0; j < 10; j++){
    Console.WriteLine(j);
}

for(var j = 0; j < 10; j++){
    if(j == 5){
        continue;
    }
    Console.WriteLine(j);
}
for(var j = 0; j < 10; j++){
    if(j == 5){
        break;
    }
    Console.WriteLine(j);
}

i = 0;
for(;;){
    Console.WriteLine(i);
    i++;
    if(i == 10){
        break;
    }
}

for(int i=0, j = 10; i < 10; i++, j--){
    Console.WriteLine(i);
    Console.WriteLine(j);
}

// foreach
// foreach(tipo variable in colección) { Sentencia }
var lista = new List<int>{1,2,3,4,5};
foreach(var item in lista){
    Console.WriteLine(item);
}
foreach(var item in lista){
    if(item == 3){
        continue;
    }
    Console.WriteLine(item);
}
foreach(var item in lista){
    if(item == 3){
        break;
    }
    Console.WriteLine(item);
}

int[] li = [1, 2, 3, 4];
foreach(var l in li){
    Console.WriteLine(l);
}
