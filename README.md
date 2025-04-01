# TUP - Programación 3

## Ejercicio Práctico 2 - Sistema Bancario

> *Entrega: Antes de las 23:59 del próximo sábado 5 de abril*

### Objetivo
Implementar un sistema bancario en C# utilizando programación orientada a objetos. El sistema gestionará clientes, cuentas y operaciones bancarias, registrando cada transacción (depósito, extracción, pago y transferencia) en clases separadas, y manteniendo un historial global en el banco, así como un historial individual para cada cliente.

### Requerimientos

#### 1. Banco
- Gestionar múltiples clientes.
- Registrar las operaciones utilizando el número de cuenta.
- Permitir listar el detalle de todas las operaciones.

#### 2. Clientes y Cuentas
- Cada cliente tiene un nombre y puede poseer varias cuentas.
- Cada cuenta se identifica con un número (formato `XXXXX`) y tiene un saldo.
- Se deben definir tres tipos de cuentas:
  - **Oro:** Acumula un 5% de crédito sobre los pagos realizados.
  - **Plata:** Acumula un 3% de crédito.
  - **Bronce:** Acumula un 1% de crédito.

#### 3. Operaciones Bancarias
El sistema debe soportar las siguientes operaciones:
- **Depositar:** Incrementa el saldo de la cuenta.
- **Extraer:** Disminuye el saldo de la cuenta si hay fondos suficientes.
- **Transferir:** Mover fondos de una cuenta a otra.
- **Pagar:** Disminuir el saldo de la cuenta y acumular crédito según el porcentaje del tipo de cuenta.

Cada operación se debe implementar en su propia clase derivada de una clase abstracta base (por ejemplo, `Operacion`), la cual registrará la fecha, el monto y las cuentas involucradas (en el caso de transferencias, tanto la cuenta de origen como la de destino).

#### 4. Registro y Reportes
- Mantener un registro global de todas las operaciones realizadas.
- Cada cliente debe tener un historial (lista) de las operaciones realizadas en sus cuentas.
- Al finalizar, el sistema generará un informe que incluya:
  - Detalle global de las operaciones (tipo, monto, fecha y cuentas involucradas).
  - Estado final de cada cuenta (saldo y crédito acumulado).
  - Historial individual de operaciones para cada cliente.

#### 5. Ejemplo de Uso
Se debe crear un escenario de prueba que incluya:
- 2 clientes, cada uno con 2 cuentas (utilizando distintos tipos: Oro, Plata y Bronce).
- Al menos 10 operaciones por cliente, asegurándose de que se ejecute al menos una operación de cada tipo.
- Al finalizar, imprimir el informe global y el reporte detallado por cliente.

### Consideraciones Adicionales
- Utilizar principios de programación orientada a objetos (herencia, encapsulamiento y polimorfismo).
- Manejar adecuadamente los posibles errores (por ejemplo, fondos insuficientes para extracción o pago).

### Entrega
El programa debe implementarse completamente en 'ejercicio.cs' y debe funcionar sin crear un proyecto `dotnet script ./ejercicio.cs`
> **Nota:** Para realizar el trabajo, se debe trabajar en la rama `Main` y hacer un refresh y luego `pull`.