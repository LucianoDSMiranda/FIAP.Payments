# FIAP.Payments

Servico de pagamentos desenvolvido em ASP.NET Core 8 com MassTransit e RabbitMQ. A aplicacao consome eventos de pedido realizado, simula o processamento do pagamento e publica eventos com o resultado da transacao.

## Objetivo

Este servico recebe um `OrderPlacedEvent`, decide aleatoriamente se o pagamento foi aprovado ou rejeitado e publica:

- `PaymentProcessedEvent`
- `PurchaseCreatedIntegrationEvent`

## Stack

- .NET 8
- ASP.NET Core Minimal API
- MassTransit
- RabbitMQ

## Fluxo da aplicacao

1. O servico escuta a fila `order-placed-queue`.
2. Ao receber um `OrderPlacedEvent`, o consumer processa o pagamento.
3. O status final pode ser `Approved` ou `Rejected`.
4. O resultado e publicado em dois eventos para consumo por outros servicos.

## Eventos

### Consumido

`OrderPlacedEvent`

```json
{
  "userId": "guid",
  "gameId": "guid",
  "price": 0,
  "email": "user@example.com"
}
```

### Publicados

`PaymentProcessedEvent`

```json
{
  "userId": "guid",
  "gameId": "guid",
  "price": 0,
  "status": "Approved"
}
```

`PurchaseCreatedIntegrationEvent`

```json
{
  "userId": "guid",
  "gameId": "guid",
  "email": "user@example.com",
  "price": 0,
  "status": "Approved"
}
```

## Configuracao

O RabbitMQ esta configurado no codigo com os valores padrao locais:

- Host: `localhost`
- Username: `guest`
- Password: `guest`
- Fila de consumo: `order-placed-queue`

As entidades de mensagem configuradas sao:

- `OrderPlacedEvent`
- `PaymentProcessedEvent`

## Pre-requisitos

- SDK do .NET 8
- RabbitMQ em execucao localmente

## Como executar

1. Suba o RabbitMQ localmente.
2. No diretorio do projeto, execute:

```bash
dotnet restore
dotnet run
```

Por padrao, em desenvolvimento, a aplicacao fica disponivel em:

- `http://localhost:5118`
- `https://localhost:7298`

## Endpoints HTTP

- `GET /` - retorna uma mensagem simples indicando que a API esta ativa
- `GET /health` - endpoint de health check

## Estrutura do projeto

```text
Consumers/
  OrderPlacedEventConsumer.cs
Messages/
  OrderPlacedEvent.cs
  PaymentProcessedEvent.cs
  PurchaseCreatedIntegrationEvent.cs
  Enums/
    PaymentStatus.cs
Program.cs
```