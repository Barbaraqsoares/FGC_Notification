# FGC_Notification
Serviço responsável por enviar notificações aos usuários.

## Estrutura e funcionamento da API

A aplicação é um serviço .NET que processa eventos de outros serviços pelo
RabbitMQ. O MassTransit encaminha cada evento ao consumidor correspondente, e o
serviço de notificações registra a mensagem no log da aplicação. Atualmente, o
envio de e-mails não está integrado a um provedor externo.

```text
src/FGC_Notification/
├── Consumers/       # Consumidores dos eventos recebidos pelo RabbitMQ
├── Contracts/       # Formato dos eventos trocados entre serviços
├── Extensions/      # Registro de dependências, MassTransit e Serilog
├── Services/        # Lógica de notificação e sua interface
└── Program.cs       # Configuração e inicialização da aplicação
teste/               # Testes automatizados da aplicação
k8s/                 # Manifests Kubernetes da API e do RabbitMQ
```

Fluxos implementados:

- `UserCreatedEvent`: `UserCreatedConsumer` solicita uma notificação de boas-vindas
  com nome, usuário e e-mail.
- `PaymentProcessedEvent`: `PaymentProcessedConsumer` solicita uma confirmação
  de compra somente quando o status é `Approved`.
- `NotificationService`: registra os dados da notificação usando o `ILogger`;
  não envia e-mails de fato neste momento.

O RabbitMQ é configurado pelas opções `RabbitMq:Host`,
`RabbitMq:VirtualHost`, `RabbitMq:Username` e `RabbitMq:Password`, que podem ser
definidas por configuração ou variáveis de ambiente. A aplicação também
configura o Swagger para documentação da API.

## Testes

Execute os testes automatizados pela solução com:

```powershell
dotnet test FGC_Notification.sln
```

Para validar a cobertura de linhas e branches:

```powershell
dotnet test FGC_Notification.sln --collect:"XPlat Code Coverage"
```

O relatório Cobertura é salvo em `teste/TestResults/`.

## Kubernetes

Os manifests em [`k8s/`](./k8s/) criam a API, o RabbitMQ e seus Services. A API
é exposta internamente na porta 80 do Service e recebe tráfego na porta 8080 do
container.

Antes de aplicar os manifests:

1. Crie a imagem `fgc-notification:latest` com `docker build -t fgc-notification:latest .`
   e disponibilize-a ao cluster (por exemplo, carregando-a no cluster local ou
   publicando-a em um registry e atualizando a imagem em
   `k8s/deployment.yaml`).
2. Substitua `REPLACE_ME` em `k8s/secret.yaml` por credenciais próprias do
   RabbitMQ. O mesmo Secret é usado pela aplicação e pelo broker.
3. Aplique os manifests: `kubectl apply -k k8s/`.

Verifique a inicialização com `kubectl get deployments,pods,services`.

O Secret versionado contém apenas valores de exemplo. Não grave credenciais
reais no repositório; Secrets do Kubernetes também devem ser protegidos com
controle de acesso e criptografia em repouso no cluster.
