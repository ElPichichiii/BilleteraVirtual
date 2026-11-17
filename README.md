# 💳 Billetera Virtual (Trabajo MDP II)

Sistema de gestión de billetera virtual desarrollado en C# y .NET, implementando un Modelo de Dominio rico (*Rich Domain Model*), principios SOLID y el patrón de diseño **Template Method**.

---

## 📐 Arquitectura y Diagramas

Para explorar la estructura completa de clases, los flujos de secuencia de las operaciones y la justificación de las decisiones de diseño, haz clic en el siguiente enlace:

[Ver Modelo de Dominio](./docs/modelo.md)

---

## 🚀 Funcionalidades Principales

* **Registro de Clientes**: Alta de usuarios con validación de datos y unicidad de DNI.
* **Cargas de Saldo**: Acreditación segura de dinero en la billetera.
* **Transferencias**: Envío de fondos entre cuentas (por Alias o CVU) validando límites máximos y saldo disponible.
* **Pago en Cuotas**: Operación de pago financiado con validación de deudas pendientes y débito de saldo.
* **Historial de Movimientos**: Trazabilidad completa de operaciones ordenadas cronológicamente.
* **Agenda de Destinatarios**: Gestión de contactos frecuentes por cliente.

---

## 🛠️ Tecnologías y Ejecución

* **Framework**: .NET 9.0 (C#)
* **Testing**: xUnit (**66 pruebas unitarias automatizadas**, con casos normales, borde y de error)

Para ejecutar la suite de pruebas desde la raíz del proyecto:
```bash
dotnet test

## 🤖 Notas sobre el desarrollo
Este proyecto ha sido desarrollado como parte del trabajo práctico de la materia (MDP II). Durante el proceso de diseño y documentación se utilizaron herramientas de Inteligencia Artificial como asistente de apoyo para la estructuración de patrones de diseño, redacción de documentación y buenas prácticas en .NET.
