# FGC_Notification
Serviço responsável por enviar notificações aos usuários.

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
