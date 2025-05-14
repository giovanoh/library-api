[![Integração Contínua](https://github.com/giovanoh/library-api/actions/workflows/ci.yml/badge.svg)](https://github.com/giovanoh/library-api/actions/workflows/ci.yml)
[![Deploy Continuo](https://github.com/giovanoh/library-api/actions/workflows/cd.yml/badge.svg)](https://github.com/giovanoh/library-api/actions/workflows/cd.yml)

---

# library-api

> :globe_with_meridians: Leia em outros idiomas: [English](README.md)

Protótipo de uma Web API .NET 8 para um sistema de biblioteca

## Sobre

Este projeto demonstra como construir uma API RESTful usando **.NET 8**, seguindo boas práticas modernas de arquitetura, testes e manutenibilidade.  
Simula um sistema simples de biblioteca com autores e livros, e foi projetado para fins de aprendizado e demonstração.

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
- **Separação de responsabilidades** e princípios SOLID
- **Configuração baseada em ambiente** via `appsettings.json`
- **URLs minúsculas** e opções de serialização JSON para APIs mais limpas

## Primeiros Passos

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Instalando dependências

Antes de rodar a API pela primeira vez, restaure as dependências do projeto:

```sh
dotnet restore
```

### Executando a API

```sh
dotnet run --project src/Library.API/Library.API.csproj
```

A API estará disponível em `https://localhost:5001` (ou conforme configurado).

### Documentação da API

Após rodar, acesse o Swagger UI em:  
`https://localhost:5001/swagger`

### Executando os testes

Os testes unitários e de integração estão localizados no diretório `tests/`.

```sh
dotnet test
```

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

#### Dica Adicional

Certifique-se de que a API esteja rodando (`dotnet run`) antes de realizar qualquer teste manual.

### Gerando relatório de cobertura de código

Os comandos abaixo vão gerar um relatório de cobertura de código no diretório `coveragereport` na raiz do projeto. Você pode visualizar os resultados abrindo o arquivo `index.html` nesse diretório com seu navegador.

```sh
dotnet test --collect:"XPlat Code Coverage"
reportgenerator -reports:"tests/**/TestResults/**/coverage.cobertura.xml" -targetdir:coveragereport -reporttypes:Html
```

### Estrutura do Projeto

```
src/Library.API/           # Projeto principal da API
  Controllers/             # Endpoints da API
  Domain/                  # Modelos de domínio, repositórios, serviços
  Infrastructure/          # Contexto EF Core, repositórios, serviços
  DTOs/                    # Objetos de Transferência de Dados
  Validation/              # Atributos de validação customizados
  Mapping/                 # Perfis do AutoMapper
tests/Library.API.Tests/   # Testes unitários
tests/Library.API.IntegrationTests/ # Testes de integração
observability/             # Configurações de observabilidade (dashboards Grafana, Prometheus, Loki, provisionamento)
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

**Nota**: O deploy requer credenciais configuradas do Docker Hub nos Secrets do GitHub.

### Observabilidade com Jaeger, Prometheus, Loki e Grafana

Para rodar a API junto com Jaeger, Prometheus, Loki, Promtail e Grafana, utilize o arquivo `docker-compose.yml`:

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

#### Observabilidade - Principais pacotes utilizados

- `OpenTelemetry.Exporter.OpenTelemetryProtocol` (para Jaeger via OTLP)
- `OpenTelemetry.Exporter.Prometheus.AspNetCore` (para Prometheus)
- `OpenTelemetry.Extensions.Hosting`
- `OpenTelemetry.Instrumentation.AspNetCore`
- `OpenTelemetry.Instrumentation.Http`
- `Serilog.AspNetCore` e `Serilog.Enrichers.Span` (logs estruturados)

#### Resumo dos containers

- **library-api**: Sua API .NET principal, expõe endpoints REST, métricas e logs.
- **jaeger**: Coletor e visualizador de traces distribuídos (OpenTelemetry/Jaeger).
- **prometheus**: Coletor e banco de dados de métricas, faz scraping do endpoint /metrics da API.
- **loki**: Backend de armazenamento e indexação de logs estruturados.
- **promtail**: Coletor de logs, lê logs dos containers e envia para o Loki.
- **grafana**: Visualização centralizada de métricas, logs e traces (dashboards, queries, alertas).

## Licença

MIT 