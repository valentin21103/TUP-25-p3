// TP2: Sistema de Cuentas Bancarias
//

// Implementar un sistema de cuentas bancarias que permita realizar operaciones como depósitos, retiros, transferencias y pagos.

class Banco{}
class Cliente{}

abstract class Cuenta{}
    class CuentaOro: Cuenta{}
    class CuentaPlata: Cuenta{}
    class CuentaBronce: Cuenta{}

abstract class Operacion{}
    class Deposito: Operacion{}
    class Retiro: Operacion{}
    class Transferencia: Operacion{}
    class Pago: Operacion{}

