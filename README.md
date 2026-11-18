# Oasis — API de Proteção contra Calor Extremo

## Sobre o projeto

O **Oasis** é uma solução CitizenTech desenvolvida para a Global Solution 2026.2 da FIAP.

O projeto busca apoiar cidadãos durante períodos de calor extremo, oferecendo informações sobre pontos de proteção climática, como locais com sombra, hidratação gratuita e comércio de água.

A solução também permite localizar pontos de proteção próximos a uma coordenada geográfica e consultar alertas demonstrativos de calor com orientações preventivas.

Esta API representa o backend desenvolvido para a disciplina de **C# Software Development**.

## Ficha CitizenTech

| Campo | Descrição |
|---|---|
| Nome da solução | Oasis |
| Cidade | São Paulo — SP |
| Área-piloto | Região da Praça da República |
| Público beneficiado | Pedestres, idosos, trabalhadores ao ar livre e cidadãos expostos ao calor |
| Problema | Dificuldade de encontrar locais de proteção e hidratação durante períodos de calor extremo |
| Solução | API para consultar pontos de proteção, buscar locais próximos e obter orientações sobre calor |
| Indicadores propostos | Quantidade de pontos cadastrados, pontos próximos encontrados e consultas de alertas |
| ODS relacionados | ODS 3 — Saúde e Bem-Estar; ODS 11 — Cidades e Comunidades Sustentáveis; ODS 13 — Ação Climática |

Os indicadores apresentados são propostas de acompanhamento do impacto, não resultados reais medidos.

## Tecnologias

- C# e .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- LINQ
- Swagger UI / OpenAPI
- xUnit
- Programação assíncrona com Async/Await

## Arquitetura

O projeto está organizado em três projetos:

- **Oasis.Api:** controllers, serviços, repositórios, acesso ao banco e configuração da API.
- **Oasis.Domain:** entidades de domínio e interfaces.
- **Oasis.Tests:** testes automatizados.

A arquitetura utiliza orientação a objetos, interfaces, injeção de dependência, Repository Pattern e programação assíncrona.

### Estrutura principal

```text
Oasis-CSharp/
├── Oasis.Api/
│   ├── Controllers/
│   │   ├── ProtectionPointsController.cs
│   │   └── HeatAlertsController.cs
│   ├── Data/
│   │   ├── OasisDbContext.cs
│   │   └── DbSeeder.cs
│   ├── Migrations/
│   ├── Repositories/
│   │   └── ProtectionPointRepository.cs
│   ├── Services/
│   │   ├── GeoDistanceService.cs
│   │   └── HeatAlertService.cs
│   └── Program.cs
├── Oasis.Domain/
│   ├── Interfaces/
│   │   └── IProtectionPointRepository.cs
│   └── Models/
│       ├── ProtectionPoint.cs
│       └── HeatAlert.cs
├── Oasis.Tests/
│   ├── ProtectionPointTests.cs
│   ├── HeatAlertServiceTests.cs
│   └── GeoDistanceServiceTests.cs
└── README.md
```

## Funcionalidades

### Gerenciamento de pontos de proteção

- Cadastrar pontos de proteção climática.
- Consultar todos os pontos cadastrados.
- Consultar um ponto pelo ID.
- Atualizar informações de um ponto.
- Excluir um ponto.
- Validar os dados recebidos pela API.
- Persistir registros em banco SQLite.

### Busca por proximidade

A API permite encontrar pontos ativos próximos à localização informada pelo cidadão.

A busca utiliza latitude, longitude e raio em quilômetros.

A distância geográfica aproximada é calculada utilizando a fórmula de Haversine.

Os resultados são ordenados do ponto mais próximo ao mais distante.

**Observação:** a distância calculada não representa necessariamente a distância de caminhada por ruas e calçadas.

### Alertas de calor

A API disponibiliza um serviço demonstrativo de classificação de calor com quatro níveis:

| Temperatura | Classificação |
|---|---|
| Abaixo de 30°C | LOW |
| De 30°C até menos de 35°C | MODERATE |
| De 35°C até menos de 40°C | HIGH |
| A partir de 40°C | EXTREME |

Cada resposta inclui a temperatura analisada, o nível de risco demonstrativo, uma mensagem e recomendações preventivas.

**Importante:** os limites são regras acadêmicas de demonstração e não correspondem a alertas meteorológicos oficiais. O serviço não consulta dados climáticos em tempo real nesta versão.

## Tipos de proteção

| Código | Tipo |
|---|---|
| 1 | Shade (sombra) |
| 2 | FreeWater (água gratuita) |
| 3 | Commerce (comércio de água) |

## Executando o projeto

### Pré-requisitos

- .NET SDK 10
- Git
- Visual Studio Code ou editor compatível

### Comandos

Clone o repositório, após sua publicação no GitHub:

```bash
git clone URL_DO_REPOSITORIO
cd Oasis-CSharp
```

Caso a pasta clonada tenha outro nome, utilize o nome correspondente no comando `cd`.

Restaure as dependências:

```bash
dotnet restore
```

Compile:

```bash
dotnet build
```

Execute os testes:

```bash
dotnet test
```

Inicie a API:

```bash
dotnet run --project Oasis.Api
```

O banco SQLite será criado ou atualizado automaticamente pelas migrations.

Quando a tabela estiver vazia, serão cadastrados dez pontos fictícios de demonstração.

## Documentação da API

Com a aplicação em execução no ambiente Development:

**Swagger UI:**

http://localhost:5176/swagger

**OpenAPI:**

http://localhost:5176/openapi/v1.json

A porta pode variar conforme a configuração do ambiente.

## Endpoints

| Método | Endpoint | Descrição |
|---|---|---|
| GET | `/api/protection-points` | Lista todos os pontos |
| GET | `/api/protection-points/nearby` | Busca pontos próximos |
| GET | `/api/protection-points/{id}` | Consulta um ponto pelo ID |
| POST | `/api/protection-points` | Cadastra um ponto |
| PUT | `/api/protection-points/{id}` | Atualiza um ponto |
| DELETE | `/api/protection-points/{id}` | Exclui um ponto |
| GET | `/api/heat-alerts/analyze` | Analisa uma temperatura |

### Exemplo de cadastro

**POST `/api/protection-points`**

```json
{
  "name": "Oasis Demo - Água",
  "description": "Ponto fictício para demonstração.",
  "address": "Região da Praça da República, São Paulo - SP",
  "latitude": -23.5432,
  "longitude": -46.6427,
  "type": 2,
  "isActive": true
}
```

### Exemplo de busca por proximidade

**GET**

```text
/api/protection-points/nearby?latitude=-23.5432&longitude=-46.6427&radiusKm=1
```

A resposta contém os pontos ativos encontrados no raio informado, incluindo o campo `distanceKm`.

### Exemplo de alerta de calor

**GET**

```text
/api/heat-alerts/analyze?temperatureC=35
```

**Resposta de exemplo:**

```json
{
  "temperatureC": 35,
  "riskLevel": "HIGH",
  "message": "Alerta demonstrativo de calor intenso.",
  "recommendations": [
    "Reduza a exposição direta ao sol.",
    "Procure ambientes frescos e ventilados.",
    "Mantenha a hidratação.",
    "Dê atenção especial a pessoas vulneráveis."
  ],
  "isSimulation": true
}
```

## Banco de dados

O projeto utiliza SQLite e Entity Framework Core.

A entidade `ProtectionPoint` possui:

- Id
- Name
- Description
- Address
- Latitude
- Longitude
- Type
- IsActive
- CreatedAt

O acesso aos dados utiliza LINQ e métodos assíncronos.

As migrations estão versionadas no projeto.

## Testes automatizados

Os testes utilizam xUnit.

Para executar:

```bash
dotnet test
```

**Última execução registrada:**

- Total de testes: 19
- Aprovados: 19
- Falhas: 0
- Ignorados: 0

Os testes abrangem propriedades e tipos da entidade `ProtectionPoint`, classificação dos alertas de calor e cálculos de distância geográfica.

O CRUD, a busca por proximidade e o endpoint de alertas também foram validados manualmente por requisições HTTP.

## Dados de demonstração

O projeto inclui dez pontos fictícios próximos à Praça da República, em São Paulo.

Esses registros são exclusivamente demonstrativos.

As coordenadas não representam serviços reais verificados e não devem ser utilizadas para orientação de cidadãos em situações de emergência.

## Integração com a solução Oasis

A API disponibiliza endpoints REST para consumo pelo aplicativo Mobile.

A arquitetura proposta também prevê integração com um serviço meteorológico no módulo Oasis-SOA, para obter informações como temperatura, umidade e possibilidade de chuva.

Essa integração meteorológica é uma evolução planejada e não está implementada nesta API.

## Contexto acadêmico

**Instituição:** FIAP

**Projeto:** Global Solution 2026.2

**Tema:** CitizenTech

**Disciplina:** C# Software Development

## Integrantes

- Guilherme Linard — RM 555768
- Ali Andrea — RM 558052
- Lucas Vasquez — RM 555159

## Evidências

As capturas de tela da API estão disponíveis na pasta `docs/evidencias/`.

| Evidência | Arquivo |
|---|---|
| Documentação Swagger com 7 endpoints | [7 endpoints.JPG](docs/evidencias/7%20endpoints.JPG) |
| Execução de 19 testes automatizados | [19 testes.JPG](docs/evidencias/19%20testes.JPG) |
| Endpoint de análise de temperatura | [get api temperatura.JPG](docs/evidencias/get%20api%20temperatura.JPG) |
| Busca por proximidade geográfica | [get nearby.JPG](docs/evidencias/get%20nearby.JPG) |
| Consulta dos pontos de proteção | [get protection-points.JPG](docs/evidencias/get%20protection-points.JPG) |

Todas as evidências foram capturadas durante a execução local da aplicação.
## Repositório

**GitHub:** adicionar o endereço definitivo do repositório Oasis-CSharp após a publicação.
