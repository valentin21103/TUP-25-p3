# Ejercicio: Sistema Bancario con Operaciones y Historial Personalizado

## Objetivo:
Implementar un sistema bancario en C# utilizando programación orientada a objetos que permita gestionar clientes, cuentas y operaciones bancarias. El sistema deberá registrar de forma individual cada operación (depósito, extracción, pago y transferencia) en clases separadas y mantener un historial global en el banco, así como un historial personal en cada cliente.

## Requerimientos:
1.	Clientes y Cuentas:
    *	El banco debe poder gestionar múltiples clientes.
    *	Cada cliente tiene un nombre y puede poseer varias cuentas.
    *	Cada cuenta cuenta con un número (formato XXXXX) y un saldo.
    *	Se deben definir tres tipos de cuentas:
    *	Oro: Acumula un 5% de crédito sobre los pagos realizados.
    *	Plata: Acumula un 3% de crédito.
    *	Bronce: Acumula un 1% de crédito.
2.	Operaciones Bancarias:
    *	El sistema debe soportar las siguientes operaciones:
        *	Depositar: Incrementa el saldo de la cuenta.
        *	Extraer: Disminuye el saldo de la cuenta si hay fondos suficientes.
        *	Pagar: Disminuye el saldo de la cuenta y acumula crédito según el porcentaje definido para el tipo de cuenta.
        *	Transferir: Permite mover fondos de una cuenta a otra.
    *	Cada operación se debe implementar en su propia clase derivada de una clase abstracta base llamada, por ejemplo, Operacion.
    *	Cada operación debe registrar la fecha, el monto y las cuentas involucradas (para transferencias se deben registrar ambas: origen y destino).
3.	Registro y Reportes:
    *	El banco debe mantener un registro global de todas las operaciones realizadas.
    *	Además, cada cliente debe mantener un historial (lista) de las operaciones realizadas en cualquiera de sus cuentas.
    *	Al finalizar las operaciones, el sistema deberá generar un informe completo que muestre:
    *	El detalle global de las operaciones (tipo, monto, fecha y cuentas involucradas).
    *	El estado final de cada cuenta (saldo y crédito acumulado).
    *	El historial individual de operaciones para cada cliente.
4.	Ejemplo de Uso:
    *	Se deberá crear un escenario de prueba en el que:
    *	Se tengan 2 clientes, cada uno con 2 cuentas (utilizando distintos tipos: Oro, Plata y Bronce).
    *	Se ejecuten 10 operaciones por cada cliente, asegurándose de incluir al menos una operación de cada tipo.
    *	Al finalizar, se imprima el informe global y el reporte detallado por cliente.

## Consideraciones Adicionales:
*	Utiliza principios de programación orientada a objetos (herencia, encapsulamiento y polimorfismo).
*	Asegúrate de manejar adecuadamente los posibles errores (por ejemplo, fondos insuficientes para extracción o pago).
*	Diseña el sistema de manera modular y con clases separadas para facilitar futuras ampliaciones o modificaciones.

## Entrega:
Desarrolla y documenta el código en C#. El programa debe compilar y ejecutarse mostrando en consola el informe global de operaciones y el historial individual de cada cliente, junto con el estado final de sus cuentas.

> NOTA: 
Para hacer el trabajo se debe pasar a la rama 'Main', hacer un refresh