# Smart Queue API

ASP.NET Core 8, Entity Framework Core 8 və SQL Server ilə hazırlanmış sadə növbə idarəetmə REST API-si.

## Layihə Haqqında

Bu layihə backend taskı kimi hazırlanmışdır. Məqsəd böyük bir sistem qurmaq deyil — sadə, işlək və səliqəli bir API yaratmaqdır. API kiçik bir xidmət mərkəzini simulyasiya edir: müştərilər növbəyə daxil olur, növbəti sırada çağırılır və tək-tək xidmət alır.

API aşağıdakı əsas əməliyyatları dəstəkləyir:

- Yeni müştərini növbəyə əlavə etmək
- Bütün gözləyən müştəriləri göstərmək
- Müştərini id-yə görə göstərmək (növbədəki yeri ilə birlikdə)
- Növbəti gözləyən müştərini çağırmaq
- Müştərini növbədən silmək

Müştərilər status dövriyyəsi ilə işləyir:

```text
Waiting -> Serving -> Completed
```

## Texnologiyalar

| Texnologiya | Təyinat |
| --- | --- |
| .NET 8 | Runtime və SDK |
| ASP.NET Core Web API | HTTP API qatı |
| C# | Proqramlaşdırma dili |
| Entity Framework Core 8 | ORM və miqrasiyalar |
| SQL Server | Verilənlər bazası |
| Swagger / OpenAPI | API sənədləşdirmə və test UI |

## Layihə Strukturu

```text
SmartQueue/
├── SmartQueue.API/                 # Controller-lər, middleware-lər, Program.cs
├── SmartQueue.Application/         # DTO-lar, interfeyslər, xüsusi exception-lar
│   ├── DTOs/
│   ├── Exceptions/
│   └── Interfaces/
├── SmartQueue.Domain/              # Entity-lər, enum-lar
│   ├── Entities/
│   └── Enums/
├── SmartQueue.Persistence/          # EF Core, repository-lər, service implementasiyaları
│   ├── Configurations/
│   ├── Context/
│   ├── Implementations/
│   ├── Migrations/
│   └── ServiceRegistration.cs
├── SmartQueue.sln
├── .gitignore
├── .gitattributes
└── README.md
```

## API Endpoint-lər

Base URL:

```text
https://localhost:{port}/api/queue
```

| Metod | Endpoint | Təsvir |
| --- | --- | --- |
| GET | `/api/queue` | Bütün gözləyən müştəriləri qaytarır |
| GET | `/api/queue/{id}` | Müştərini id-yə görə və növbədəki yerini qaytarır |
| POST | `/api/queue` | Yeni müştəri əlavə edir |
| POST | `/api/queue/next` | Növbəti müştərini çağırır |
| DELETE | `/api/queue/{id}` | Müştərini növbədən silir |

## Nümunə Sorğu və Cavablar

### 1) Müştəri əlavə et — POST /api/queue

Sorğu:

```json
{
  "name": "Ali"
}
```

Cavab 201 Created:

```json
{
  "id": 1,
  "name": "Ali",
  "createdAt": "2026-09-18T10:00:00Z",
  "status": "Waiting",
  "position": 1
}
```

### 2) Gözləyən müştəriləri göstər — GET /api/queue

Cavab 200 OK:

```json
[
  { "id": 1, "name": "Ali", "createdAt": "2026-09-18T10:00:00Z" },
  { "id": 2, "name": "Veli", "createdAt": "2026-09-18T10:01:00Z" }
]
```

### 3) Müştərini id-yə görə göstər — GET /api/queue/{id}

Cavab 200 OK (gözləyir):

```json
{
  "id": 1,
  "name": "Ali",
  "status": "Waiting",
  "position": 2
}
```

Cavab 200 OK (artıq xidmət olunur):

```json
{
  "id": 1,
  "name": "Ali",
  "status": "Serving",
  "position": null
}
```

404 Not Found:

```json
{ "message": "Customer with id 1 not found" }
```

### 4) Növbəti müştərini çağır — POST /api/queue/next

Cavab 200 OK:

```json
{
  "id": 1,
  "name": "Ali",
  "createdAt": "2026-09-18T10:00:00Z",
  "status": "Serving"
}
```

404 Not Found (gözləyən müştəri yoxdur):

```json
{ "message": "No customers in the queue." }
```

409 Conflict (concurrency retry limiti aşıldı):

```json
{ "message": "Could not call the next customer." }
```

### 5) Müştərini sil — DELETE /api/queue/{id}

Cavab 204 No Content

404 Not Found:

```json
{ "message": "Customer with id 1 not found" }
```

## Verilənlər Bazası

Proyekt SQL Server istifadə edir. Connection string appsettings faylında qeyd olunur.

Fayl:

```text
SmartQueue.API/appsettings.json
```

Nümunə:

```json
{
  "ConnectionStrings": {
    "Default": "<your-local-sql-server-connection-string>"
  }
}
```

Mühüm qeyd: layihədə `Customer` entity-də `Version` sahəsi concurrency token kimi istifadə olunur. Bu, eyni anda birdən çox sorğunun `/next` endpoint-ini çağırması halında eyni müştərinin iki dəfə çağırılmasını qarşısını alır.

## Miqrasiyalar

EF Core miqrasiyalar `SmartQueue.Persistence/Migrations` qovluğunda yerləşir.

## Layihəni İşə Salmaq

### Tələblər

- .NET 8 SDK
- SQL Server çalışır vəziyyətdə olmalıdır
- EF Core CLI qurulmalıdır

EF Core CLI qurmaq:

```bash
dotnet tool install --global dotnet-ef
```

### Addımlar

```bash
cd SmartQueue
dotnet restore
dotnet ef database update --project SmartQueue.Persistence --startup-project SmartQueue.API
dotnet run --project SmartQueue.API
```

Swagger UI açılacaq:

```text
https://localhost:<port>/swagger
```

## /next Endpoint-i və Concurrency Həlli

Problem: iki müştəri eyni anda `POST /api/queue/next` çağırsa, hər ikisi eyni ilk gözləyən müştərini oxuya bilər. Sonra hər ikisi həmin müştərini `Serving` vəziyyətinə keçirə bilər. Bu da eyni müştərinin iki dəfə çağrılması ilə nəticələnir.

Həll: Optimistic Concurrency + Version token.

`Customer` entity-də int tipli `Version` sahəsi var və EF Core-da concurrency token kimi konfiqurasiya olunur:

```csharp
modelBuilder.Entity<Customer>(e =>
{
    e.Property(x => x.Version).IsConcurrencyToken();
});
```

Yeniləmə zamanı EF Core bu dəyəri `WHERE` şartinə əlavə edir:

```sql
UPDATE Customers
SET Status = 'Serving', Version = 1
WHERE Id = 5 AND Version = 0;
```

Əgər 1 sətir təsirlənirsə — uğurlu yeniləmə.
Əgər 0 sətir təsirlənirsə — başqa sorğu əvvəlcə dəyişdiyi üçün EF Core `DbUpdateConcurrencyException` atır.

Köhnə ardıcıllıqda bu vəziyyət üçün retry logic hazırlanır:

```csharp
const int maxAttempts = 3;

for (int attempt = 1; attempt <= maxAttempts; attempt++)
{
    var customer = await _repository.GetNextWaitingCustomerAsync();

    if (customer is null)
        throw new NotFoundException("No customers in the queue.");

    customer.Status = QueueStatus.Serving;

    try
    {
        await _repository.SaveChangesAsync();
        return new CallNextCustomerDto(...);
    }
    catch (DbUpdateConcurrencyException)
    {
        _repository.Detach(customer);
    }
}

throw new ConflictException("Could not call the next customer.");
```

Bu yanaşma sadə, etibarlıdır, SQL Server və digər relational DB-lərdə işləyə bilir. Pessimistic lock və ya semaphore kimi əks variantlar daha ağır və daha məhduddur.

## Qeydlər

- Bütün zaman möhürləri UTC-də saxlanılır.
- Ad unikallığı verilənlər bazası səviyyəsində qorunur.
- API-lər laylı arxitektura ilə ayrılmışdır: API, Application, Domain, Persistence.
- Proyekt minimal, lakin səliqəli və işlək bir queue management solution nümunəsidir.
