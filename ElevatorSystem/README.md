# Elevator System - Multithreaded Implementation

Este projeto implementa um sistema de elevador multithread com thread safety, seguindo os requisitos especificados no README principal.

## Arquitetura

### Componentes Principais

1. **Elevator** - Classe que representa um elevador individual
   - Gerencia estados: IDLE, MOVING_UP, MOVING_DOWN, DOOR_OPEN
   - Implementa thread safety usando locks e ConcurrentQueue
   - Processa requisições de forma FIFO

2. **ElevatorController** - Controlador que gerencia operações do elevador
   - Thread dedicado para processamento de requisições
   - Interface assíncrona para requisições
   - Monitoramento de performance

3. **API Controller** - Endpoints REST para interação com o sistema
   - POST /api/elevator/request - Solicitar elevador
   - GET /api/elevator/status - Status do elevador
   - GET /api/elevator/requests - Requisições ativas
   - POST /api/elevator/load-test - Teste de carga

## Thread Safety

### Implementações de Thread Safety

1. **Locks e Mutexes**
   - `_lockObject` para operações críticas
   - Locks em mudanças de estado do elevador
   - Locks em operações de fila

2. **Coleções Thread-Safe**
   - `ConcurrentQueue<ElevatorRequest>` para fila de requisições
   - `ConcurrentDictionary<int, ElevatorRequest>` para requisições ativas
   - `HashSet<int>` protegido por lock para andares alvo

3. **Operações Atômicas**
   - Mudanças de estado do elevador são atômicas
   - Adição/remoção de requisições são thread-safe
   - Processamento de requisições em thread dedicado

## Performance

### Requisitos Atendidos

- ✅ **100+ requisições concorrentes** - Testado com 150 requisições simultâneas
- ✅ **Tempo de resposta < 100ms** - Implementado com monitoramento
- ✅ **Uso de memória razoável** - Coleções otimizadas e cleanup automático

### Otimizações

1. **Thread Dedicado** - Processamento contínuo sem bloqueio
2. **Cleanup Automático** - Remoção de requisições completadas
3. **Monitoramento de Performance** - Logs de tempo de resposta
4. **Coleções Otimizadas** - Uso de estruturas thread-safe eficientes

## Testes

### Testes Unitários (xUnit)

- **ElevatorTests** - Testes da classe Elevator
- **ElevatorControllerTests** - Testes do controlador com foco em thread safety
- **IntegrationTests** - Testes de integração da API

### Cobertura de Testes

- ✅ Validação de entrada (andares inválidos)
- ✅ Thread safety (requisições concorrentes)
- ✅ Performance (tempo de resposta < 100ms)
- ✅ Carga (100+ requisições simultâneas)
- ✅ Estados do elevador
- ✅ Tratamento de erros

## Como Executar

### Executar a API

```bash
cd ElevatorSystem.API
dotnet run
```

A API estará disponível em: https://localhost:7001

### Executar Testes

```bash
cd ElevatorSystem.Tests
dotnet test
```

### Swagger UI

Acesse: https://localhost:7001/swagger

## Endpoints da API

### Solicitar Elevador
```http
POST /api/elevator/request
Content-Type: application/json

{
  "floor": 5,
  "direction": "UP",
  "destinationFloor": 8
}
```

### Status do Elevador
```http
GET /api/elevator/status
```

### Requisições Ativas
```http
GET /api/elevator/requests
```

### Teste de Carga
```http
POST /api/elevator/load-test
Content-Type: application/json

{
  "numberOfRequests": 100
}
```

## Logs e Monitoramento

O sistema inclui logging detalhado para:
- Requisições de elevador
- Mudanças de estado
- Performance (tempo de resposta)
- Erros e exceções
- Thread safety violations

## Tratamento de Erros

- Validação de andares (1-10)
- Timeouts para elevadores travados
- Exceções para operações concorrentes
- Graceful degradation sob carga

## Próximos Passos

1. Implementar algoritmo de escalonamento mais sofisticado
2. Adicionar múltiplos elevadores
3. Implementar métricas de performance em tempo real
4. Adicionar persistência de dados
5. Implementar interface web em tempo real
