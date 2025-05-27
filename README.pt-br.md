[![Integração Contínua](https://github.com/giovanoh/library-api/actions/workflows/ci.yml/badge.svg)](https://github.com/giovanoh/library-api/actions/workflows/ci.yml)
[![Deploy Continuo](https://github.com/giovanoh/library-api/actions/workflows/cd.yml/badge.svg)](https://github.com/giovanoh/library-api/actions/workflows/cd.yml)
[![codecov](https://codecov.io/gh/giovanoh/library-api/branch/main/graph/badge.svg)](https://app.codecov.io/gh/giovanoh/library-api)

---

# library-api

> :globe_with_meridians: Leia em outros idiomas: [English](README.md)

Protótipo de uma Web API .NET 8 para um sistema de biblioteca

## Sobre

Este projeto demonstra como construir uma API RESTful usando **.NET 8**, seguindo boas práticas modernas de arquitetura, testes e manutenibilidade.  
Simula um sistema simples de biblioteca com autores e livros, e foi projetado para fins de aprendizado e demonstração.

Além das operações CRUD tradicionais, o projeto conta com um fluxo de checkout orientado a eventos (EDA) usando RabbitMQ e MassTransit, permitindo processamento assíncrono de pedidos e desacoplamento entre serviços.

## Sumário

- [Funcionalidades & Boas Práticas](#funcionalidades--boas-práticas)
- [Primeiros Passos](#primeiros-passos)
  - [Pré-requisitos](#pré-requisitos)
  - [Instalando dependências](#instalando-dependências)
  - [Executando a API](#executando-a-api)
  - [Documentação da API](#documentação-da-api)
- [Testes Automatizados](#testes-automatizados)
- [Testando a API Manualmente](#testando-a-api-manualmente)
- [Gerando relatório de cobertura de código](#gerando-relatório-de-cobertura-de-código)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Integração Contínua e Deploy (CI/CD)](#integração-contínua-e-deploy-cicd)
- [Observabilidade com Jaeger, Prometheus, Loki e Grafana](#observabilidade-com-jaeger-prometheus-loki-e-grafana)
  - [Subir todos os serviços de observabilidade](#subir-todos-os-serviços-de-observabilidade)
  - [Parar os serviços](#parar-os-serviços)
  - [Observabilidade - Principais pacotes utilizados](#observabilidade---principais-pacotes-utilizados)
  - [Resumo dos containers](#resumo-dos-containers)
- [EDA: Mensageria e Arquitetura Orientada a Eventos - Principais pacotes utilizados](#eda-mensageria-e-arquitetura-orientada-a-eventos---principais-pacotes-utilizados)
- [Visão Geral da Arquitetura](#visão-geral-da-arquitetura)
- [Exemplo: Fluxo de Checkout por Eventos](#exemplo-fluxo-de-checkout-por-eventos)
- [Observabilidade: Tracing & Métricas](#observabilidade-tracing--métricas)
- [Licença](#licença)

## Funcionalidades & Boas Práticas

- **.NET 8** com recursos modernos de C#
- **Web API tradicional com Controllers** para clara separação de responsabilidades e escalabilidade
- **Estrutura orientada ao domínio** (Domain, Infrastructure, DTOs, etc.)
- **Injeção de dependência** para serviços e repositórios, tornando o código mais testável e manutenível
- **Banco de dados em memória** usando Entity Framework Core para fácil configuração e testes
- **AutoMapper** para mapeamento entre modelos de domínio e DTOs
- **Atributos de validação customizados** e uso de validação nativa
- **Tratamento de erros consistente e centralizado** com respostas RFC-compliant `ApiProblemDetails`
- **Testes unitários e de integração** (xUnit, Moq, FluentAssertions), com mocks para isolar dependências
- **Documentação automática da API** com Swagger/OpenAPI
- **Arquitetura Orientada a Eventos (EDA)** usando RabbitMQ e MassTransit para comunicação assíncrona entre serviços
- **Fluxo de checkout baseado em mensagens**
- **Separação de responsabilidades** e princípios SOLID
- **Configuração baseada em ambiente** via `appsettings.json`
- **URLs minúsculas** e opções de serialização JSON para APIs mais limpas
- **Modelo de Maturidade de Richardson**: Esta API atinge o **Nível 2** do Modelo de Maturidade de Richardson, ou seja, utiliza recursos distintos e os verbos HTTP adequados (GET, POST, PUT, DELETE) para cada operação. Ainda não implementa HATEOAS (Nível 3), que incluiria links hipermídia nas respostas para guiar o cliente de forma dinâmica. Veja mais em [Modelo de Maturidade de Richardson](https://martinfowler.com/articles/richardsonMaturityModel.html).
- **Padrão de Resposta Unificado**: Todas as respostas de sucesso seguem o formato `ApiResponse<T>`, garantindo consistência e previsibilidade para quem consome a API. Todas as respostas de erro seguem o padrão [RFC 7807](https://datatracker.ietf.org/doc/html/rfc7807) utilizando o objeto `ApiProblemDetails`, facilitando o tratamento de erros de forma padronizada e interoperável.

## Primeiros Passos

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Instalando dependências

Antes de rodar a API pela primeira vez, restaure as dependências do projeto:

```sh
dotnet restore
```

### 1. Rodar apenas a API (Autores/Livros)
Para desenvolvimento local rápido ou testes unitários de autores e livros:
```sh
dotnet run --project src/Library.API/Library.API.csproj
```
A API estará disponível em https://localhost:5001.

> **Nota:** Os endpoints de pedidos/checkout exigem o RabbitMQ e o stack completo em execução.

### 2. Rodar o Stack Completo (Recomendado para Pedidos/Checkout)
Para testar o fluxo orientado a eventos, mensageria e observabilidade:
```sh
docker compose up --build
```
Isso irá iniciar a API, o worker Checkout, o RabbitMQ e todas as ferramentas de observabilidade.

### Documentação da API

A documentação da API está disponível via Swagger UI em:
`https://localhost:5001/swagger`

### Testes Automatizados

O projeto inclui cobertura abrangente de testes. Testes unitários e de integração estão localizados no diretório `tests/`.

#### Tipos de Testes
- **Testes Unitários**: Validação de componentes e métodos individuais de forma isolada
- **Testes de Integração**: Verificação de interações entre diferentes componentes da aplicação

#### Executando Testes
Para executar todos os testes, use o comando:
```sh
dotnet test
```

### Testando a API Manualmente

#### Importante: Testes Manuais vs Testes Automatizados

:warning: **Nota**: Esta seção descreve testes manuais para exploração e desenvolvimento. 
Estes testes são diferentes dos **testes unitários e de integração automatizados** localizados no diretório `tests/`, 
que são executados com `dotnet test`.

#### Métodos de Teste

Existem duas formas principais de realizar testes manuais na API:

##### 1. Swagger UI (Recomendado para Desenvolvimento Interativo)

Após iniciar a aplicação, acesse o Swagger UI em `https://localhost:5001/swagger`. 
Esta interface interativa permite:
- Exploração de endpoints disponíveis
- Teste direto de requisições no navegador
- Visualização de modelos de dados
- Envio de requisições com parâmetros personalizados

##### 2. Requisições de Exemplo em Arquivo `.http`

O arquivo `src/Library.API/Library.API.http` contém requisições de exemplo prontas para uso. 
Útil para testes manuais rápidos e documentação de fluxos.

Ferramentas recomendadas:
- Extensão REST Client no VS Code (como "REST Client" de Huachao Mao)
- Postman ou Insomnia

Exemplo de fluxo de testes manuais no arquivo `.http`:
- Criar um autor
- Listar autores
- Atualizar autor
- Criar um livro para o autor
- Listar livros
- Atualizar livro
- Remover livro e autor
- Criar um pedido de livro (checkout)
- Consultar o andamento do pedido de livro

### Gerando relatório de cobertura de código

Os comandos abaixo vão gerar um relatório de cobertura de código no diretório `coveragereport` na raiz do projeto. Você pode visualizar os resultados abrindo o arquivo `index.html` nesse diretório com seu navegador.

```sh
dotnet test --collect:"XPlat Code Coverage"
reportgenerator -reports:"tests/**/TestResults/**/coverage.cobertura.xml" -targetdir:coveragereport -reporttypes:Html
```

### Estrutura do Projeto

```
src/Library.API/                    # Projeto principal da API
  Controllers/                      # Endpoints da API
  Domain/                           # Modelos de domínio, repositórios, serviços
  DTOs/                             # Objetos de Transferência de Dados
  Extensions/                       # Métodos de extensão e helpers
  Infrastructure/                   # Contexto EF Core, repositórios, serviços, middlewares
  Mapping/                          # Perfis do AutoMapper
  Validation/                       # Atributos de validação customizados
src/Library.Events/                 # Contratos de eventos (mensagens compartilhadas entre serviços)
src/Library.Checkout/               # Serviço worker para processamento de eventos de pedidos
tests/Library.API.Tests/            # Testes unitários
tests/Library.API.IntegrationTests/ # Testes de integração
observability/                      # Configurações de observabilidade (dashboards Grafana, Prometheus, Loki, provisionamento)
```

### Integração Contínua e Deploy (CI/CD)

#### Visão Geral do Workflow

O projeto implementa um pipeline de Integração Contínua e Deploy (CI/CD) abrangente e automatizado usando GitHub Actions:

##### Integração Contínua (CI)
- **Gatilhos**: 
  - Pushes para branch `main`
  - Pushes para branches `docs/*`, `feature/*`, `refactor/*` e `test/*`
  - Pull requests para branch `main`

- **Processo de Build e Teste**:
  - Configura .NET 8 SDK
  - Restaura dependências do projeto
  - Compila o projeto em configuração de Release
  - Executa suítes abrangentes de testes:
    - Testes Unitários
    - Testes de Integração
  - Gera relatórios de cobertura de código
  - Envia relatórios de cobertura para Codecov para acompanhamento

##### Deploy Contínuo (CD)
- **Gatilho**: Conclusão bem-sucedida do workflow de Integração Contínua na branch `main`

- **Publicação de Imagem Docker**:
  - Constrói imagem Docker para a Library API
  - Publica imagem no Docker Hub
  - Gera múltiplas tags:
    - Tags específicas de branch
    - Tags de versionamento semântico
    - Tag `latest`

#### Benefícios Principais
- Testes automatizados para cada mudança de código
- Processo de build e deploy consistente
- Feedback imediato sobre qualidade do código
- Geração automática de imagem Docker
- Rastreamento de cobertura de código

### Observabilidade com Jaeger, Prometheus, Loki e Grafana

Para rodar a API junto com Jaeger, Prometheus, Loki, Promtail, Grafana e o Otel Collector, utilize o arquivo `docker-compose.yml`:

#### Subir todos os serviços de observabilidade

```sh
docker compose up --build
```

- Acesse a API em: http://localhost:5000 ou https://localhost:5001
- Acesse a interface web do Jaeger (traces) em: http://localhost:16686
- Acesse a interface web do Prometheus (métricas) em: http://localhost:9090
- Acesse a interface web do Grafana (dashboards, logs, métricas, traces) em: http://localhost:3000 (usuário/senha padrão: admin/admin)
- Acesse as métricas da API diretamente em: http://localhost:5000/metrics
- Acesse a API do Loki (logs) em: http://localhost:3100 (usado pelo Grafana/Promtail)
- Acesse o status do Promtail (coletor de logs) em: http://localhost:9080 (opcional)

#### Parar os serviços

```sh
docker compose down
```

#### Resumo dos containers

- **otel-collector**: Recebe traces, métricas e logs dos serviços e encaminha para o Jaeger, Prometheus, Loki e outras ferramentas de observabilidade.
- **rabbitmq**: Message broker utilizado para comunicação orientada a eventos entre os serviços (painel de administração em http://localhost:15672, usuário/senha padrão: guest/guest).
- **library-api**: API .NET principal, expõe endpoints REST, métricas e logs.
- **jaeger**: Coletor e visualizador de traces distribuídos (OpenTelemetry/Jaeger).
- **prometheus**: Coletor e banco de dados de métricas, faz scraping do endpoint /metrics da API.
- **loki**: Backend de armazenamento e indexação de logs estruturados.
- **promtail**: Coletor de logs, lê logs dos containers e envia para o Loki.
- **grafana**: Visualização centralizada de métricas, logs e traces (dashboards, queries, alertas).

#### Observabilidade - Principais pacotes utilizados

- `OpenTelemetry.Exporter.OpenTelemetryProtocol` (para Jaeger via OTLP)
- `OpenTelemetry.Exporter.Prometheus.AspNetCore` (para Prometheus)
- `OpenTelemetry.Extensions.Hosting`
- `OpenTelemetry.Instrumentation.AspNetCore`
- `OpenTelemetry.Instrumentation.Http`
- `OpenTelemetry.Instrumentation.MassTransit` (para tracing distribuído de mensageria)
- `Serilog.AspNetCore` e `Serilog.Enrichers.Span` (logs estruturados)

### Visão Geral da Arquitetura

O sistema é baseado em Arquitetura Orientada a Eventos (EDA) usando RabbitMQ e MassTransit. O fluxo principal é:

O fluxo orientado a eventos depende do RabbitMQ como broker central de mensagens, rodando como container no stack.

```
[Cliente] ---> [Library.API] --(OrderPlacedEvent)--> [RabbitMQ] --(OrderPlacedEvent)--> [Library.Checkout]
    ^             |                                                    |
    |             |<--(Eventos de status: PaymentConfirmed,            |
    |             |    OrderProcessing, OrderShipped, OrderDelivered,  |
    |             |    OrderCompleted, PaymentFailed)                  |
    |             |                                                    |
    |<-------------------(Atualização de status via API)---------------|
```

- **Library.API**: Expõe endpoints REST, publica eventos no RabbitMQ e atualiza status do pedido conforme eventos.
- **Library.Checkout**: Serviço worker que consome eventos do RabbitMQ, processa lógica de negócio (pagamento, envio) e emite novos eventos.
- **Library.Events**: Projeto compartilhado com contratos de eventos/mensagens.

### EDA: Mensageria e Arquitetura Orientada a Eventos - Principais pacotes utilizados

- `MassTransit` (biblioteca principal para mensageria distribuída)
- `MassTransit.RabbitMQ` (transporte RabbitMQ para o MassTransit)
- `OpenTelemetry.Instrumentation.MassTransit` (tracing distribuído para mensageria)

### Exemplo: Fluxo de Checkout por Eventos

Antes de criar um pedido de livro, é necessário:

1. Criar um Autor
2. Criar um Livro (usando o authorId da resposta anterior)

Depois disso, você pode:

- Criar um Pedido de Livro (usando o bookId da resposta anterior)
- A API irá publicar um OrderPlacedEvent no RabbitMQ
- O serviço Checkout processará o evento e emitirá os seguintes eventos: PaymentConfirmed, PaymentFailed, OrderProcessing, OrderShipped, OrderDelivered e OrderCompleted.
- Você pode consultar o status do pedido pela API

### Observabilidade: Tracing & Métricas

- **Jaeger**: http://localhost:16686 — visualize traces distribuídos de cada evento do pedido.
- **Grafana**: http://localhost:3000 — dashboards de métricas e traces.
- **Prometheus**: http://localhost:9090 — métricas brutas.

### Licença

MIT
