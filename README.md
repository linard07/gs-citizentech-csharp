# CitizenTech Alerts API

Backend em **C# / .NET 8** da CitizenTech: recebe medições de clima, decide quando existe um **evento extremo** (chuva forte, alagamento, calor extremo ou frio extremo), cria o **alerta** e indica à população os **pontos de segurança** mais próximos e compatíveis com aquele tipo de evento.

> Projeto da Global Solution (FIAP), disciplina **C# Software Development**.
> Equipe: Guilherme Linard (RM 555768), Lucas Vasquez (RM 555159) e Ali Andrea (RM 558052) — Turma 3ESPZ.

## O problema e a solução

Eventos extremos atingem a cidade de São Paulo com frequência, e o cidadão não sabe para onde ir. A CitizenTech:

1. **Detecta** o evento a partir de uma medição (por exemplo, a chuva diária ou a previsão do modelo de Machine Learning do mesmo projeto, que usa o limiar de **30 mm/dia**).
2. **Alerta**, classificando a severidade (Atenção, Alerta ou Emergência).
3. **Direciona** as pessoas para pontos de segurança: áreas refrigeradas, abrigos elevados contra alagamento, locais com água a preço reduzido, áreas governamentais.
4. **Acompanha** a ocupação dos pontos e gera estatísticas.

> **Importante:** esses pontos de segurança ainda não existem. Os 6 pontos cadastrados na inicialização são **fictícios** (marcados com `isFictitious: true` e "(fictício)" no nome), só para demonstrar o funcionamento.

## Tecnologias

- .NET 8, ASP.NET Core Web API (controllers)
- Entity Framework Core 8 com SQLite (arquivo `citizentech.db`, criado sozinho)
- Swagger (Swashbuckle) para a documentação dos endpoints
- xUnit + EF Core InMemory para os testes unitários

## Como executar

Pré-requisito: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (`dotnet --version` deve mostrar 8.x).

```bash
git clone <URL-DO-REPOSITORIO>
cd CitizenTech.Alerts

dotnet restore
dotnet run --project src/CitizenTech.Api
```

A API sobe em **http://localhost:5080** e abre o **Swagger** em `/swagger`. Na primeira execução o banco SQLite é criado e os pontos de segurança fictícios são cadastrados.

### Testes

```bash
dotnet test
```

### Migrations (opcional)

Por padrão o banco é criado com `EnsureCreated`. Para usar migrations do EF Core:

```bash
dotnet tool install --global dotnet-ef
rm -f src/CitizenTech.Api/citizentech.db
dotnet ef migrations add InitialCreate --project src/CitizenTech.Api
```

Se houver migrations, a API as aplica sozinha na inicialização.

## Endpoints

A documentação interativa completa está no Swagger (`/swagger`). Exemplos prontos em [`requests.http`](requests.http).

### Alertas

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/alerts` | Lista alertas (filtros `type`, `status`; paginação `page`, `pageSize`) |
| GET | `/api/alerts/{id}` | Busca um alerta |
| POST | `/api/alerts` | Avalia uma medição; se passar do limiar, cria o alerta (201), senão responde 200 sem criar |
| POST | `/api/alerts/{id}/close` | Encerra um alerta ativo |
| GET | `/api/alerts/{id}/safe-zones` | Pontos de segurança mais próximos, compatíveis e com vagas |
| POST | `/api/alerts/{id}/check-ins` | Registra a entrada de pessoas em um ponto de segurança |
| GET | `/api/alerts/statistics` | Alertas por tipo e total de pessoas abrigadas |

### Pontos de segurança

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/safe-zones` | Lista os pontos (`onlyActive=true` por padrão) |
| GET | `/api/safe-zones/{id}` | Busca um ponto |
| POST | `/api/safe-zones` | Cadastra um ponto |
| PUT | `/api/safe-zones/{id}` | Atualiza um ponto |
| DELETE | `/api/safe-zones/{id}` | Desativa o ponto (exclusão lógica) |
| POST | `/api/safe-zones/{id}/check-out` | Registra a saída de pessoas |

### Exemplo: criar um alerta

`POST /api/alerts`

```json
{
  "type": "ChuvaForte",
  "measuredValue": 55,
  "region": "Sé",
  "latitude": -23.5503,
  "longitude": -46.6339,
  "source": "Modelo ML",
  "predictedProbability": 0.82
}
```

Resposta `201 Created`: o alerta criado com severidade `Alerta`.

### Limiares de severidade

| Tipo | Unidade | Atenção | Alerta | Emergência |
|---|---|---|---|---|
| `ChuvaForte` | mm/dia | ≥ 30 | ≥ 50 | ≥ 80 |
| `Alagamento` | cm de água | ≥ 10 | ≥ 30 | ≥ 60 |
| `CalorExtremo` | °C (máxima) | ≥ 35 | ≥ 38 | ≥ 40 |
| `FrioExtremo` | °C (mínima) | ≤ 8 | ≤ 5 | ≤ 2 |

Abaixo do limiar de Atenção, nenhum alerta é criado.

### Compatibilidade entre alerta e ponto de segurança

| Tipo de alerta | Serviço exigido do ponto |
|---|---|
| `ChuvaForte`, `Alagamento` | `FloodSafeShelter` |
| `CalorExtremo` | `AirConditioning` |
| `FrioExtremo` | `Heating` |

### Códigos de erro

Os erros usam `ProblemDetails`: **400** (validação do corpo ou dos parâmetros), **404** (recurso inexistente) e **422** (regra de negócio violada, como ponto lotado ou alerta já encerrado).

## Arquitetura

```
src/CitizenTech.Api/
├── Domain/            Entidades e regras de negócio (sem dependência de framework)
├── Application/       Interfaces, serviços, DTOs e mapeamentos
├── Infrastructure/    EF Core (DbContext, configurações, repositórios, seed)
├── Controllers/       Endpoints REST
├── Middleware/        Tratamento global de exceções
└── Program.cs         Composição da aplicação e injeção de dependência
tests/CitizenTech.Tests/   Testes unitários (xUnit)
docs/EVIDENCIAS.md         Evidências de funcionamento
```

## Onde cada conteúdo da disciplina aparece

**POO (3,0)**
- **Herança e classe abstrata:** `Entity` é a base de `SafeZone`, `WeatherAlert` e `ShelterCheckIn`.
- **Encapsulamento:** setters privados e regras dentro das entidades (`SafeZone.CheckIn`, `WeatherAlert.Close`, `WeatherAlert.RegisterCheckIn`); a coleção de check-ins é exposta como somente leitura.
- **Polimorfismo e interfaces:** `IRiskEvaluator` com quatro implementações (`RainRiskEvaluator`, `FloodRiskEvaluator`, `HeatRiskEvaluator`, `ColdRiskEvaluator`) que herdam de `ThresholdRiskEvaluator` (padrão Strategy).
- **Exceções de domínio:** `DomainException` e `NotFoundException`.

**Web API (2,5)**
- Controllers REST com verbos e status HTTP corretos (200, 201, 204, 400, 404, 422), `CreatedAtAction`, validação com DataAnnotations e `ProblemDetails`.
- Swagger com comentários XML e tipos de resposta (`ProducesResponseType`).
- Tratamento global de erros (`IExceptionHandler`).

**EF Core + LINQ (1,5)**
- `DbContext`, configurações Fluent API (`IEntityTypeConfiguration`), relacionamentos 1:N, conversão de enums, índices e *seed* de dados.
- Padrão Repository e Unit of Work (`IUnitOfWork` implementado pelo `AppDbContext`).
- LINQ: filtros e paginação (`Where`, `OrderBy`, `Skip`, `Take`), projeção com subconsulta (`Select` + `Sum`) e agrupamento (`GroupBy`) nas estatísticas, e ordenação por distância (Haversine) nos pontos de segurança.

**Async/Await (1,0)**
- Todos os métodos de serviço, repositório e controller são assíncronos, com `CancellationToken` propagado até o EF Core.

**Qualidade do código (2,0)**
- Camadas separadas, injeção de dependência (`Program.cs`), interfaces para tudo que é testado, nomes claros, validações no domínio, logs estruturados (`ILogger`) e testes unitários (`dotnet test`).

## Evidências de funcionamento

Veja [`docs/EVIDENCIAS.md`](docs/EVIDENCIAS.md).

## Limitações e próximos passos

- Os pontos de segurança são **fictícios**; a integração com órgãos reais ainda não existe.
- O alerta é criado a partir de uma medição enviada à API. Os próximos passos são ligar o modelo de ML (projeto de IA & ML) para enviar a previsão automaticamente, e notificar a população (push/SMS).
- Controle de concorrência na ocupação dos pontos (por exemplo, `RowVersion`) e autenticação nos endpoints de escrita.
