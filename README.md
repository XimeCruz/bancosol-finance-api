# BancoSol Finance API

API REST de gestión financiera personal desarrollada para la prueba técnica Backend de BancoSol. Permite registrar y consultar ingresos en BOB/USD, consultar el tipo de cambio USD/BOB en HexaRate y obtener balances consolidados por período.

## Tecnologías

- .NET 10 y ASP.NET Core 10
- Entity Framework Core 10 con PostgreSQL mediante Npgsql
- OpenAPI 3.1 y Scalar (interfaz disponible en `/swagger`)
- `HttpClientFactory` y resiliencia estándar para HexaRate
- xUnit, FluentAssertions y NSubstitute
- Docker y GitHub Actions

## Arquitectura

La solución emplea Clean Architecture ligera:

- `Domain`: entidad `Income`, moneda y reglas invariantes.
- `Application`: casos de uso, puertos y cálculo puro del balance.
- `Infrastructure`: EF Core, repositorio y cliente HexaRate.
- `Api`: HTTP, validación de entrada y manejo global de errores.

Se eligió un monolito modular porque el alcance no justifica la complejidad operativa de microservicios. Las abstracciones permiten sustituir persistencia o proveedor de tipo de cambio. Si el dominio creciera, el módulo de tasas podría extraerse y publicar actualizaciones mediante un bus de eventos.

## Ejecutar localmente

Requisitos: SDK de .NET 10.

La aplicación requiere una base PostgreSQL llamada `bancosol_finance`. La cadena
de conexión se guarda con User Secrets para evitar publicar contraseñas:

```powershell
dotnet user-secrets init --project src/BancoSol.Finance.Api
dotnet user-secrets set "ConnectionStrings:FinanceDatabase" "Host=localhost;Port=5432;Database=bancosol_finance;Username=postgres;Password=TU_PASSWORD" --project src/BancoSol.Finance.Api
```

```bash
dotnet restore BancoSol.Finance.slnx
dotnet run --project src/BancoSol.Finance.Api
```

Abrir:

- API: `http://localhost:5080`
- documentación interactiva: `http://localhost:5080/swagger`
- documentación interactiva alternativa: `http://localhost:5080/api-docs`
- OpenAPI JSON: `http://localhost:5080/openapi/v1.json`
- salud: `http://localhost:5080/health`

La tabla `incomes` se crea automáticamente al iniciar por primera vez.

## Ejecutar con Docker

```bash
docker compose up --build
```

Docker Compose inicia la API y PostgreSQL. Swagger queda disponible en
`http://localhost:8080/swagger`; el volumen `postgres-data` conserva la base.

## Endpoints

| Método | Ruta | Resultado |
| --- | --- | --- |
| `POST` | `/api/v1/incomes` | Registra un ingreso y responde `201` con `Location` |
| `GET` | `/api/v1/incomes` | Devuelve el historial completo |
| `GET` | `/api/v1/incomes/{id}` | Devuelve un ingreso; `400` si el ID es inválido y `404` si no está registrado |
| `GET` | `/api/v1/exchange-rates/USD/BOB` | Devuelve la tasa vigente de HexaRate |
| `GET` | `/api/v1/balances?from=...&to=...&currency=BOB` | Balance consolidado del período |
| `GET` | `/health` | Estado de la API y base de datos |

También se incluye `BancoSol.Finance.http` con peticiones listas para ejecutar desde IDEs compatibles.

### Ejemplo de registro

```json
{
  "amount": 5000,
  "description": "Sueldo diciembre",
  "receivedDate": "2025-12-01",
  "source": "Sueldo",
  "currency": "BOB"
}
```

Los importes usan `decimal`; en persistencia tienen precisión `(18,2)`. Solo se admiten `BOB` y `USD`, sin distinguir mayúsculas y minúsculas.

## Documentación interactiva

La especificación OpenAPI 3.1 se genera desde metadatos y comentarios XML del
código. Scalar la presenta visualmente en `/swagger` y `/api-docs`, desde donde
un consumidor puede ejecutar cada petición directamente.

Cada operación documenta:

- método y URL;
- propósito y comportamiento;
- parámetros y formato esperado;
- ejemplos de solicitudes y respuestas;
- códigos `200`, `201`, `400`, `404` y `503` aplicables;
- esquemas de ingresos, balances, tipos de cambio y errores `ProblemDetails`.

Un GUID válido que no existe devuelve `404` con el mensaje “El ingreso con ID
'...' no está registrado”. Un identificador sin formato GUID devuelve `400`,
porque la solicitud en sí es inválida.

## Pruebas

```bash
dotnet test BancoSol.Finance.slnx -c Release
```

Las pruebas de integración usan una base PostgreSQL separada. Antes de ejecutarlas:

```powershell
$env:ConnectionStrings__FinanceTestDatabase = "Host=localhost;Port=5432;Database=bancosol_finance_tests;Username=postgres;Password=TU_PASSWORD"
```

Se cubren los criterios críticos:

- rechazo de EUR sin persistir el ingreso;
- conversión y suma de BOB + USD hacia BOB;
- conversión y suma de BOB + USD hacia USD;
- redondeo monetario;
- registro HTTP válido, encabezado `Location` y consulta posterior;
- respuesta HTTP `400` para EUR.
- respuesta HTTP `404` con mensaje para un GUID no registrado;
- respuesta HTTP `400` para un identificador mal formado.

## Decisiones y supuestos

### Ambigüedad matemática del enunciado

El caso completo del PDF calcula correctamente `3000 BOB + 100 USD + 2000 BOB = 5692 BOB` con tasa `6.92`. Sin embargo, la sección de pruebas omite los `2000 BOB` y conserva el resultado `5692`. La operación correcta para `3000 BOB + 100 USD` es `3692 BOB`. La prueba automatizada conserva los tres ingresos del ejemplo completo y espera `5692`; la lógica no introduce una excepción para reproducir el error tipográfico.

### Tipo de cambio

El balance usa la tasa vigente al momento de generar el reporte, como solicita el enunciado. La respuesta expone la tasa empleada para trazabilidad. Ante timeout, respuesta HTTP fallida o contenido inválido de HexaRate se responde `503` mediante `ProblemDetails`; se aplican timeout y reintentos transitorios.

### Fechas y redondeo

- El rango es inclusivo: `from <= ReceivedDate <= to`.
- `from > to` produce `400`.
- El total se redondea a dos decimales con `MidpointRounding.AwayFromZero`.
- El historial se ordena por fecha de recepción y creación, descendente.

### Identidad de usuario

La prueba habla de “mis ingresos”, pero no define autenticación ni clientes. Por ello el alcance mantiene un único historial global. En producción se incorporaría OpenID Connect/JWT y `CustomerId`, derivado exclusivamente del usuario autenticado, para impedir acceso horizontal (OWASP Broken Access Control).

## Seguridad y operación

- Validación por DTO, dominio y casos de uso.
- EF Core parametriza consultas y reduce riesgo de inyección SQL.
- Errores RFC 9457/`ProblemDetails`; no se exponen stack traces.
- `traceId` en cada error para correlación.
- Ejecución Docker como usuario no privilegiado.
- Configuración externa por variables de entorno; no se versionan secretos.
- Endpoint de salud y logs estructurados sin datos sensibles.

Para producción se recomienda TLS en el proxy/hosting, autenticación OIDC, autorización por recurso, rate limiting, almacenamiento administrado (PostgreSQL/RDS), migraciones controladas y telemetría centralizada.

## CI/CD y despliegue

`.github/workflows/ci.yml` restaura, compila con advertencias tratadas como errores, ejecuta las pruebas con cobertura y construye la imagen Docker en cada push/PR.

Para Render, Railway, Azure o AWS se puede desplegar directamente el `Dockerfile`, exponer el puerto `8080` y configurar una instancia PostgreSQL administrada. Configuración mínima:

```text
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__FinanceDatabase=Host=SERVIDOR;Port=5432;Database=bancosol_finance;Username=USUARIO;Password=SECRETO
HexaRate__BaseUrl=https://hexarate.paikama.co/
```

La documentación se habilita deliberadamente en todos los entornos porque el enunciado exige acceso público a `/swagger`.
