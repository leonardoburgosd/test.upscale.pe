

## Flujos

### Flujo de registro de usuario
``` mermaid
flowchart TD
    A[Usuario llena los datos] --> B[Se genera código aleatorio]
    B --> C[Se envía correo con el código]
    C --> D[Usuario abre link con el código]
    D --> E[Validación del código]
    E -->|Correcto| F[Mostrar mensaje de bienvenida]
    E -->|Incorrecto| G[Mostrar error]
```
### Flujo de autenticacion
``` mermaid
flowchart TD
    A[Ingresa al Login] --> B[Selecciona opción DNI o CE]
    B --> C[Ingresa número]
    C --> D[Ingresa contraseña]
    D --> E{¿Contraseña correcta?}

    E -->|Sí| F[Redirigir a página Profile]

    E -->|No| G[Mostrar error]
    G --> H[Aumentar contador CVF]
    H --> I{¿Intentos >= 5?}

    I -->|No| D

    I -->|Sí| J[Bloquear cuenta]
    J --> K[Enviar correo de cuenta bloqueada]
    K --> L[Redirigir a vista cuenta bloqueada]
```

### Flujo de sesion activa
``` mermaid
flowchart TD
    A[Autenticación correcta] --> B[Redirigir a vista Perfil]
    B --> C[Sin interacción por 20 minutos]
    C --> D[Mostrar modal con contador regresivo]

    D --> E{¿Usuario presiona el botón?}

    E -->|Sí| F[Mantener sesión activa]
    F --> B

    E -->|No| G[Redirigir automáticamente al Login]
    G --> H[Mostrar modal: sesión cerrada por inactividad]
```
## Tecnologías usadas

- ASP .NET Core 10
- Bootstrap
- Visual Studio
- Razor
- SQLServer
- Entity Framework Core
- Kiro (Agente de IA)
- ChatGpt (compactar redacción de documentación)
- SonarQube


## Descripción de implementación de interfaces 
1. Bootstrap como framework base para el diseño

2. Si se necesita ajustes en colores y tipografias no existentes se asignará en custom-styles.css sin sobreescribir comportamiento crítico.

3. Usa la guía oficial como paleta de colores (https://guias.servicios.gob.pe/creacion-servicios-digitales/estilos/colores) y la tipografía (https://guias.servicios.gob.pe/creacion-servicios-digitales/estilos/tipografia)

## Modelo de datos

### 📊 Tabla: Users

| Campo                | Tipo SQL Server     | Requerido | Tamaño | Default    | Descripción |
|---------------------|--------------------|----------|--------|-----------|------------|
| Id                  | BIGINT             | Sí       | -      | Identity  | Identificador único autogenerado |
| Names               | NVARCHAR           | Sí       | 255    | -         | Nombres del usuario |
| FathersSurname      | NVARCHAR           | Sí       | 100    | -         | Apellido paterno |
| MothersSurname      | NVARCHAR           | Sí       | 100    | -         | Apellido materno |
| DocumentType        | NVARCHAR           | Sí       | 3      | -         | Tipo de documento ('DNI', 'CE') |
| DocumentNumber      | INT                | Sí       | -      | -         | Número de documento (8-9 dígitos) |
| DateOfBirth         | DATETIME           | Sí       | -      | -         | Fecha de nacimiento |
| Nationality         | NVARCHAR           | Sí       | 50     | 'Peruana' | Nacionalidad del usuario |
| Gender              | NVARCHAR           | Sí       | 1      | -         | Género ('M', 'F') |
| MainEmail           | NVARCHAR           | Sí       | 255    | -         | Email principal (único) |
| SecondaryEmail      | NVARCHAR           | No       | 255    | NULL      | Email secundario |
| MainPhone           | NVARCHAR           | Sí       | 11     | -         | Teléfono principal |
| SecondaryPhone      | NVARCHAR           | No       | 11     | NULL      | Teléfono secundario |
| ContractType        | NVARCHAR           | Sí       | 100    | -         | Tipo de contrato |
| HiringDate          | DATETIME           | Sí       | -      | -         | Fecha de contratación |
| UserPasswordEncrypt | VARBINARY          | Sí       | MAX    | -         | Contraseña encriptada |
| UserSalt            | VARBINARY          | Sí       | MAX    | -         | Salt para encriptación |
| CVF                 | INT                | Sí       | -      | -         | Contador de intentos fallidos (0-5) |
| ValidationCode      | NVARCHAR           | Sí       | 12     | -         | Código de validación |
| CreatedAt           | DATETIME           | Sí       | -      | UTCNOW    | Fecha de creación |
| UpdatedAt           | DATETIME           | No       | -      | NULL      | Fecha de actualización |

---

### 🔑 Índices

| Campo           | Tipo   | Descripción |
|-----------------|--------|------------|
| MainEmail       | UNIQUE | Email único por usuario |
| DocumentNumber  | UNIQUE | Documento único |
| MainPhone       | INDEX  | Optimización de búsqueda |